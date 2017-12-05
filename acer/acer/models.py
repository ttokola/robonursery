import numpy as np
import tensorflow as tf
import tensorflow.contrib.layers as c_layers
from tensorflow.python.tools import freeze_graph
from unityagents import UnityEnvironmentException


from baselines.common import set_global_seeds

from baselines.a2c.utils import batch_to_seq, seq_to_batch
from baselines.a2c.utils import Scheduler, make_path, find_trainable_variables
from baselines.a2c.utils import cat_entropy_softmax
from baselines.a2c.utils import EpisodeStats
from baselines.a2c.utils import get_by_index, check_shape, avg_norm, gradient_add, q_explained_variance
from baselines.acer.buffer import Buffer

def batch_to_seq(h, nbatch, nsteps, flat=False):
    if flat:
        h = tf.reshape(h, [nbatch, nsteps])
    else:
        h = tf.reshape(h, [nbatch, nsteps, -1])
    return [tf.squeeze(v, [1]) for v in tf.split(axis=1, num_or_size_splits=nsteps, value=h)]

def seq_to_batch(h, flat = False):
    shape = h[0].get_shape().as_list()
    if not flat:
        assert(len(shape) > 1)
        nh = h[0].get_shape()[-1].value
        return tf.reshape(tf.concat(axis=1, values=h), [-1, nh])
    else:
        return tf.reshape(tf.stack(values=h, axis=1), [-1])


# remove last step
def strip(var, nenvs, nsteps, flat = False):
    vars = batch_to_seq(var, nenvs, nsteps + 1, flat)
    return seq_to_batch(vars[:-1], flat)

def avg_norm(t):
    return tf.reduce_mean(tf.sqrt(tf.reduce_sum(tf.square(t), axis=-1)))

def q_explained_variance(qpred, q):
    _, vary = tf.nn.moments(q, axes=[0, 1])
    _, varpred = tf.nn.moments(q - qpred, axes=[0, 1])
    check_shape([vary, varpred], [[]] * 2)
    return 1.0 - (varpred / vary)


def q_retrace(R, D, q_i, v, rho_i, nenvs, nsteps, gamma):
    """
    Calculates q_retrace targets

    :param R: Rewards
    :param D: Dones
    :param q_i: Q values for actions taken
    :param v: V values
    :param rho_i: Importance weight for each action
    :return: Q_retrace values
    """
    rho_bar = batch_to_seq(tf.minimum(1.0, rho_i), nenvs, nsteps, True)  # list of len steps, shape [nenvs]
    rs = batch_to_seq(R, nenvs, nsteps, True)  # list of len steps, shape [nenvs]
    ds = batch_to_seq(D, nenvs, nsteps, True)  # list of len steps, shape [nenvs]
    q_is = batch_to_seq(q_i, nenvs, nsteps, True)
    vs = batch_to_seq(v, nenvs, nsteps + 1, True)
    v_final = vs[-1]
    qret = v_final
    qrets = []
    for i in range(nsteps - 1, -1, -1):
        check_shape([qret, ds[i], rs[i], rho_bar[i], q_is[i], vs[i]], [[nenvs]] * 6)
        qret = rs[i] + gamma * qret * (1.0 - ds[i])
        qrets.append(qret)
        qret = (rho_bar[i] * (qret - q_is[i])) + vs[i]
    qrets = qrets[::-1]
    qret = seq_to_batch(qrets, flat=True)
    return qret


def create_agent_model(env, lr=1e-4, h_size=128, 
    rmsprop_epsilon=1e-5, rmsprop_alpha=0.99, 
    epsilon=0.2, alpha=0.99, beta=1e-3, delta=1, gamma=0.99,
     max_step=5e6, trust_region=True):
    """
    Takes a Unity environment and model-specific hyper-parameters and returns the
    appropriate PPO agent model for the environment.
    :param env: a Unity environment.
    :param lr: Learning rate.
    :param h_size: Size of hidden layers/
    :param epsilon: Value for policy-divergence threshold.
    :param beta: Strength of entropy regularization.
    :return: a sub-class of PPOAgent tailored to the environment.
    :param max_step: Total number of training steps.
    """
    brain_name = env.brain_names[0]
    brain = env.brains[brain_name]
    if brain.action_space_type == "continuous":
        return ContinuousControlModel(lr, brain, h_size,
            rmsprop_epsilon, rmsprop_alpha,
            epsilon=epsilon, alpha=alpha, delta=delta, gamma=gamma,
            max_step=max_step, trust_region=trust_region)

    if brain.action_space_type == "discrete":
        #return DiscreteControlModel(lr, brain, h_size, epsilon, beta, max_step)
        pass

def save_model(sess, saver, model_path="./", steps=0):
    """
    Saves current model to checkpoint folder.
    :param sess: Current Tensorflow session.
    :param model_path: Designated model path.
    :param steps: Current number of steps in training process.
    :param saver: Tensorflow saver for session.
    """
    last_checkpoint = model_path + '/model-' + str(steps) + '.cptk'
    saver.save(sess, last_checkpoint)
    tf.train.write_graph(sess.graph_def, model_path, 'raw_graph_def.pb', as_text=False)
    print("Saved Model")


def export_graph(model_path, env_name="env", target_nodes="action"):
    """
    Exports latest saved model to .bytes format for Unity embedding.
    :param model_path: path of model checkpoints.
    :param env_name: Name of associated Learning Environment.
    :param target_nodes: Comma separated string of needed output nodes for embedded graph.
    """
    ckpt = tf.train.get_checkpoint_state(model_path)
    freeze_graph.freeze_graph(input_graph=model_path + '/raw_graph_def.pb',
                              input_binary=True,
                              input_checkpoint=ckpt.model_checkpoint_path,
                              output_node_names=target_nodes,
                              output_graph=model_path + '/' + env_name + '.bytes',
                              clear_devices=True, initializer_nodes="", input_saver="",
                              restore_op_name="save/restore_all", filename_tensor_name="save/Const:0")

class ACERModel(object):
    def create_visual_encoder(self, o_size_h, o_size_w, bw, h_size, num_streams, activation):
        """
        Builds a set of visual (CNN) encoders.
        :param o_size_h: Height observation size.
        :param o_size_w: Width observation size.
        :param bw: Whether image is greyscale {True} or color {False}.
        :param h_size: Hidden layer size.
        :param num_streams: Number of visual streams to construct.
        :param activation: What type of activation function to use for layers.
        :return: List of hidden layer tensors.
        """
        if bw:
            c_channels = 1
        else:
            c_channels = 3
        self.observation_in = tf.placeholder(shape=[None, o_size_h, o_size_w, c_channels], dtype=tf.float32,
                                             name='observation_0')
        streams = []
        for i in range(num_streams):
            self.conv1 = tf.layers.conv2d(self.observation_in, 32, kernel_size=[3, 3], strides=[2, 2],
                                          use_bias=False, activation=activation)
            self.conv2 = tf.layers.conv2d(self.conv1, 64, kernel_size=[3, 3], strides=[2, 2],
                                          use_bias=False, activation=activation)
            hidden = tf.layers.dense(c_layers.flatten(self.conv2), h_size, use_bias=False, activation=activation)
            streams.append(hidden)
        return streams

    def create_continuous_state_encoder(self, s_size, h_size, num_streams, activation):
        """
        Builds a set of hidden state encoders.
        :param s_size: state input size.
        :param h_size: Hidden layer size.
        :param num_streams: Number of state streams to construct.
        :param activation: What type of activation function to use for layers.
        :return: List of hidden layer tensors.
        """
        self.state_in = tf.placeholder(shape=[None, s_size], dtype=tf.float32, name='state')
        streams = []
        for i in range(num_streams):
            hidden_1 = tf.layers.dense(self.state_in, h_size, use_bias=False, activation=activation)
            hidden_2 = tf.layers.dense(hidden_1, h_size, use_bias=False, activation=activation)
            streams.append(hidden_2)
        return streams

    def create_discrete_state_encoder(self, s_size, h_size, num_streams, activation):
        """
        Builds a set of hidden state encoders from discrete state input.
        :param s_size: state input size (discrete).
        :param h_size: Hidden layer size.
        :param num_streams: Number of state streams to construct.
        :param activation: What type of activation function to use for layers.
        :return: List of hidden layer tensors.
        """
        self.state_in = tf.placeholder(shape=[None, 1], dtype=tf.int32, name='state')
        state_in = tf.reshape(self.state_in, [-1])
        state_onehot = c_layers.one_hot_encoding(state_in, s_size)
        streams = []
        for i in range(num_streams):
            hidden = tf.layers.dense(state_onehot, h_size, use_bias=False, activation=activation)
            streams.append(hidden)
        return streams

    def create_acer_optimizer(self, trust_region,
                probs,
                old_probs,
                loss_policy,
                entropy,
                beta,
                f_pol,
                rmsprop_epsilon,
                lr,
                nsteps,
                max_step,
                ent_coef=0.01):
        """
        Creates training-specific Tensorflow ops for PPO models.
        :param trust_region: bool: Whether to use trust region
        :param probs: Current policy probabilities
        :param old_probs: Past policy probabilities
        :param loss_policy: Current value estimate
        :param beta: Entropy regularization strength
        :param entropy: Current policy entropy
        :param epsilon: Value for policy-divergence threshold
        :param lr: Learning rate
        :param max_step: Total number of training steps.
        :param ent_coef: entropy coefficient, 0.01 in openAI implementation
        """
        if trust_region:
            g = tf.gradients(- (loss_policy - ent_coef * entropy) * nsteps*1.1 * nenvs, f) #[nenvs * nsteps, nact]
            # k = tf.gradients(KL(f_pol || f), f)
            k = - f_pol / (f + eps) #[nenvs * nsteps, nact] # Directly computed gradient of KL divergence wrt f
            k_dot_g = tf.reduce_sum(k * g, axis=-1)
            adj = tf.maximum(0.0, (tf.reduce_sum(k * g, axis=-1) - delta) / (tf.reduce_sum(tf.square(k), axis=-1) + eps)) #[nenvs * nsteps]

            # Calculate stats (before doing adjustment) for logging.
            avg_norm_k = avg_norm(k)
            avg_norm_g = avg_norm(g)
            avg_norm_k_dot_g = tf.reduce_mean(tf.abs(k_dot_g))
            avg_norm_adj = tf.reduce_mean(tf.abs(adj))

            g = g - tf.reshape(adj, [nenvs * max_step, 1]) * k
            grads_f = -g/(nenvs*nsteps) # These are turst region adjusted gradients wrt f ie statistics of policy pi
            grads_policy = tf.gradients(f, params, grads_f)
            grads_q = tf.gradients(loss_q * q_coef, params)
            grads = [gradient_add(g1, g2, param) for (g1, g2, param) in zip(grads_policy, grads_q, params)]

            avg_norm_grads_f = avg_norm(grads_f) * (nsteps * nenvs)
            norm_grads_q = tf.global_norm(grads_q)
            norm_grads_policy = tf.global_norm(grads_policy)
        else:
            grads = tf.gradients(loss, params)

        if max_grad_norm is not None:
            grads, self.norm_grads = tf.clip_by_global_norm(grads, max_grad_norm)
        self.grads = list(zip(grads, params))
        self.global_step = tf.Variable(0, trainable=False, name='global_step', dtype=tf.int32)
        self.optimizer = tf.train.RMSPropOptimizer(learning_rate=LR, decay=rprop_alpha, epsilon=rmsprop_epsilon)
        self.grads_and_vars = self.optimizer.compute_gradients(self.loss) 
        self.opt_op = optimizer.apply_gradients(self.grads)

        # so when you call _train, you first do the gradient step, then you apply ema
        with tf.control_dependencies([self.opt_op]):
            self.train = tf.group(ema_apply_op)
        self.increment_step = tf.assign(self.global_step, self.global_step + 1)

    def create_acer_loss(self, actions,
                        f, q, v, f_pol, qret, rho_i, f_i, rho,
                        q_coef, ent_coef, epsilon=1e-6,
                        a_size, nsteps, nenvs):
        # Get pi and q values for actions taken
        f_i = get_by_index(f, actiions)
        q_i = get_by_index(q, actions)

        # Compute ratios for importance truncation
        rho = f / (MU + eps)
        rho_i = get_by_index(rho, actions)

        # Calculate Q_retrace targets
        qret = q_retrace(R, D, q_i, v, rho_i, nenvs, nsteps, gamma)
        # Calculate losses
        # Entropy
        #entropy = tf.reduce_mean(cat_entropy_softmax(f))
        self.entropy = tf.reduce_sum(0.5 * tf.log(2 * np.pi * np.e * self.sigma_sq))
        # Policy Graident loss, with truncated importance sampling & bias correction
        v = strip(v, nenvs, nsteps, True)
        check_shape([qret, v, rho_i, f_i], [[nenvs * nsteps]] * 4)
        check_shape([rho, f, q], [[nenvs * nsteps, nact]] * 2)

        # Truncated importance sampling
        adv = qret - v
        logf = tf.log(f_i + epsilon)
        gain_f = logf * tf.stop_gradient(adv * tf.minimum(c, rho_i))  # [nenvs * nsteps]
        self.loss_f = -tf.reduce_mean(gain_f)

        # Bias correction for the truncation
        adv_bc = (q - tf.reshape(v, [nenvs * nsteps, 1]))  # [nenvs * nsteps, nact]
        logf_bc = tf.log(f + epsilon) # / (f_old + eps)
        check_shape([adv_bc, logf_bc], [[nenvs * nsteps, nact]]*2)
        gain_bc = tf.reduce_sum(logf_bc * tf.stop_gradient(adv_bc * tf.nn.relu(1.0 - (c / (rho + epsilon))) * f), axis = 1) #IMP: This is sum, as expectation wrt f
        self.loss_bc= -tf.reduce_mean(gain_bc)

        self.loss_policy = loss_f + loss_bc

        # Value/Q function loss, and explained variance
        check_shape([qret, q_i], [[nenvs * nsteps]]*2)
        self.ev = q_explained_variance(tf.reshape(q_i, [nenvs, nsteps]), tf.reshape(qret, [nenvs, nsteps]))
        self.loss_q = tf.reduce_mean(tf.square(tf.stop_gradient(qret) - q_i)*0.5)

        # Net loss
        check_shape([self.loss_policy, self.loss_q, self.entropy], [[]] * 3)
        self.loss = self.loss_policy + q_coef * self.loss_q - ent_coef * self.entropy

class AcerCnnPolicy(object):
    def __init__(self, hidden_policy, a_size, max_steps, reuse=False):
        with tf.variable_scope("model", reuse=reuse):
            fc = c_layers.fully_connected(hidden_policy, 512, weights_initializer=tf.initializers.orthogonal)
            pi_logits = c_layers.fully_connected(fc, a_size)
            pi = tf.nn.softmax(pi_logits)
            q = c_layers.fully_connected(fc, a_size)
            noise = tf.random_uniform(tf.shape(logits))
            self.action = tf.argmax(pi_logits - tf.log(-tf.log(noise)), 1)
        self.pi = pi
        self.q = q

class ContinuousControlModel(ACERModel):
    def __init__(self, lr, brain, h_size, 
        rmsprop_epsilon, rmsprop_alpha, 
        epsilon, alpha, delta, gamma, max_step,
        nsteps, trust_region ):
        """
        Creates Continuous Control Actor-Critic model.
        :param brain: State-space size
        :param h_size: Hidden layer size
        :param trust_region: bool: Whether to use trust region
        :param alpha: for ExponentialMovingAverage
        :param nsteps: updates performed every nsteps steps
        """
        s_size = brain.state_space_size
        a_size = brain.action_space_size



        hidden_state, hidden_visual, hidden_policy, hidden_value = None, None, None, None
        if brain.number_observations > 0:
            h_size, w_size = brain.camera_resolutions[0]['height'], brain.camera_resolutions[0]['width']
            bw = brain.camera_resolutions[0]['blackAndWhite']
            hidden_visual = self.create_visual_encoder(h_size, w_size, bw, h_size, 2, tf.nn.tanh)
        if brain.state_space_size > 0:
            s_size = brain.state_space_size
            if brain.state_space_type == "continuous":
                hidden_state = self.create_continuous_state_encoder(s_size, h_size, 2, tf.nn.tanh)
            else:
                hidden_state = self.create_discrete_state_encoder(s_size, h_size, 2, tf.nn.tanh)
        
        if hidden_visual is None and hidden_state is None:
            raise Exception("No valid network configuration possible. "
                            "There are no states or observations in this brain")
        elif hidden_visual is not None and hidden_state is None:
            hidden_policy, hidden_value = hidden_visual
        elif hidden_visual is None and hidden_state is not None:
            hidden_policy, hidden_value = hidden_state
        elif hidden_visual is not None and hidden_state is not None:
            hidden_policy = tf.concat([hidden_visual[0], hidden_state[0]], axis=1)
            hidden_value = tf.concat([hidden_visual[1], hidden_state[1]], axis=1)
        
        self.batch_size = tf.placeholder(shape=None, dtype=tf.int32, name='batch_size')
        MU = tf.placeholder(ft.float32, [batch_size, a_size])

        policy = AcerCnnPolicy
        train_model = policy(hidden_policy, a_size, max_steps )
        with tf.variable_scope("model"):
            params = tf.trainable_variables()
        print("Params {}".format(len(params)))
        for var in params:
            print(var)
        
        # create polyak averaged model
        ema = tf.train.ExponentialMovingAverage(alpha)
        ema_apply_op = ema.apply(params)

        def custom_getter(getter, *args, **kwargs):
            v = ema.average(getter(*args, **kwargs))
            print(v.name)
            return v

        with tf.variable_scope("", custom_getter=custom_getter, reuse=True):
            polyak_model = policy(sess, ob_space, ac_space, nsteps + 1, nstack, reuse=True)

        # Notation: (var) = batch variable, (var)s = seqeuence variable, (var)_i = variable index by action at step i
        v = tf.reduce_sum(train_model.pi * train_model.q, axis = -1) # shape is [nenvs * (nsteps + 1)]

        # strip off last step
        f, f_pol, q = map(lambda var: strip(var, 1, max_step), [train_model.pi, polyak_model.pi, train_model.q])

        #self.mu = tf.layers.dense(hidden_policy, a_size, activation=None, use_bias=False,
        #                    kernel_initizer=c_layers.variance_scaling_initializer(factor=0.1))
        #self.log_sigma_sq = tf.Variable(tf.zeros([a_size]))
        #self.sigma_sq = tf.exp(self.log_sigma_sq)

        #self.epsilon = tf.placeholder(shape=[None, a_size], dtype=tf.float32, name='epsilon')
        
        #self.output = self.mu + tf.sqrt(self.sigma_sq) * self.epsilon
        #self.output = tf.identity(self.output, name='action')

        #a = tf.exp(-1 * tf.pow(tf.stop_gradient(self.output) - self.mu, 2) / (2 * self.sigma_sq))
        #b = 1 / tf.sqrt(2 * self.sigma_sq * np.pi)
        #self.probs = a * b

        #self.entropy = tf.reduce_sum(0.5 * tf.log(2 * np.pi * np.e * self.sigma_sq))

        #self.value = tf.layers.dense(hidden_value, 1, activation=None, use_bias=False)

        #self.old_probs = tf.placeholder(shape=[None, a_size], dtype=tf.float32, name='old_probabilities')
        #self.log_sigma_sq = tf.Variable(tf.zeros([a_size]))
        #self.sigma_sq = tf.exp(self.log_sigma_sq)

        #self.entropy = tf.reduce_sum(0.5 * tf.log(2 * np.pi * np.e * self.sigma_sq))
        self.create_acer_loss()
        self.create_acer_optimizer(self.probs, self.old_probs, loss_policy, self.entropy, 0.0, epsilon, lr, max_step)
        
        lr = Scheduler(v=lr, nvalues=total_timesteps, schedule=lrschedule)

        # Ops/Summaries to run, and their names for logging
        run_ops = [self.train, self.loss, self.loss_q, self.entropy, self.loss_policy, self.loss_f, self.loss_bc, self.ev, self.norm_grads]
        names_ops = ['loss', 'loss_q', 'entropy', 'loss_policy', 'loss_f', 'loss_bc', 'explained_variance',
                     'norm_grads']
        if trust_region:
            run_ops = run_ops + [norm_grads_q, norm_grads_policy, avg_norm_grads_f, avg_norm_k, avg_norm_g, avg_norm_k_dot_g,
                                 avg_norm_adj]
            names_ops = names_ops + ['norm_grads_q', 'norm_grads_policy', 'avg_norm_grads_f', 'avg_norm_k', 'avg_norm_g',
                                     'avg_norm_k_dot_g', 'avg_norm_adj']

        def train(obs, actions, rewards, dones, mus, states, masks, steps):
            cur_lr = lr.value_steps(steps)
            td_map = {train_model.X: obs, polyak_model.X: obs, A: actions, R: rewards, D: dones, MU: mus, LR: cur_lr}
            if states != []:
                td_map[train_model.S] = states
                td_map[train_model.M] = masks
                td_map[polyak_model.S] = states
                td_map[polyak_model.M] = masks
            return names_ops, sess.run(run_ops, td_map)[1:]  # strip off _train

        def save(save_path):
            ps = sess.run(params)
            make_path(save_path)
            joblib.dump(ps, save_path)

        self.train = train
        self.save = save
        self.train_model = train_model
        self.step_model = step_model
        self.step = step_model.step
        self.initial_state = step_model.initial_state
        tf.global_variables_initializer().run(session=sess)