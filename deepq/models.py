import numpy as np
import tensorflow as tf
import tensorflow.contrib.layers as c_layers
from tensorflow.python.tools import freeze_graph
from unityagents import UnityEnvironmentException
#port https://github.com/openai/baselines/tree/master/baselines/deepq
#to ml-agents framework

def clipped_error(x):
  # Huber loss
  try:
    return tf.select(tf.abs(x) < 1.0, 0.5 * tf.square(x), tf.abs(x) - 0.5)
  except:
    return tf.where(tf.abs(x) < 1.0, 0.5 * tf.square(x), tf.abs(x) - 0.5)

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


class DEEPQModel(object):
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

    def create_deepq_optimizer(self, q, target_q_t, value, a_size, lr ):
        # optimizer
        with tf.variable_scope('optimizer'):
            self.target_q_t = tf.placeholder('float32', [None], name='target_q_t')
            #self.action = tf.placeholder('int64', [None], name='action')

            action_one_hot = tf.one_hot(value, a_size, 1.0, 0.0, name='action_one_hot')
            q_acted = tf.reduce_sum(q * action_one_hot, reduction_indices=1, name='q_acted')

            self.delta = self.target_q_t - q_acted

            self.global_step = tf.Variable(0, trainable=False, name='global_step', dtype=tf.int32)

            self.loss = tf.reduce_mean(clipped_error(self.delta), name='loss')
            self.learning_rate_step = tf.placeholder('int64', None, name='learning_rate_step')
            self.learning_rate_op = tf.train.exponential_decay(
                    lr,
                    self.global_step,
                    self.learning_rate_decay_step,
                    self.learning_rate_decay,
                    staircase=True)
            self.update_batch = tf.train.RMSPropOptimizer(
                self.learning_rate_op, momentum=0.95, epsilon=0.01).minimize(self.loss)
        self.increment_step = tf.assign(self.global_step, self.global_step + 1)

class ContinuousControlModel(DEEPQModel):
    def __init__(self, lr, brain, h_size, epsilon, max_step, dueling=False, layer_norm=True):
        """
        Creates Continuous Control Actor-Critic model.
        :param brain: State-space size
        :param h_size: Hidden layer size
        """
        self.s_size = brain.state_space_size
        self.a_size = brain.action_space_size

        hidden_state, hidden_visual, hidden_policy, hidden_value = None, None, None, None
        if brain.number_observations > 0:
            self.h_size, self.w_size = brain.camera_resolutions[0]['height'], brain.camera_resolutions[0]['width']
            bw = brain.camera_resolutions[0]['blackAndWhite']
            hidden_visual = self.create_visual_encoder(self.h_size, self.w_size, bw, self.h_size, 2, tf.nn.tanh)
        if brain.state_space_size > 0:
            #self.s_size = brain.state_space_size
            if brain.state_space_type == "continuous":
                hidden_state = self.create_continuous_state_encoder(self.s_size, self.h_size, 2, tf.nn.tanh)
            else:
                hidden_state = self.create_discrete_state_encoder(self.s_size, self.h_size, 2, tf.nn.tanh)

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

        #self.mu = tf.layers.dense(hidden_policy, a_size, activation=None, use_bias=False,
        #                          kernel_initializer=c_layers.variance_scaling_initializer(factor=0.1))
        #self.log_sigma_sq = tf.Variable(tf.zeros([a_size]))
        #self.sigma_sq = tf.exp(self.log_sigma_sq)

        with tf.variable_scope("action_value"):
            action_out = hidden_policy
            if layer_norm:
                action_out = c_layers.layer_norm(action_out, center=True, scale=True)
            self.action_out = tf.nn.relu(action_out)
            self.action_scores = c_layers.fully_connected(action_out, num_outputs=a_size, activation_fn=None)

        if dueling:
            with tf.variable_scope("state_value"):
                state_out = hidden_value
                if layer_norm:
                    state_out = c_layers.layer_norm(state_out, center=True, scale=True)
                state_out = tf.nn.relu(state_out)
                self.state_score = c_layers.fully_connected(state_out, num_outputs=1, activation_fn=None)
                action_scores_mean = tf.reduce_mean(self.action_scores, 1)
                action_scores_centered = self.action_scores - tf.expand_dims(action_scores_mean, 1)
            self.q_out = state_score + action_scores_centered
        else:
            self.q_out = self.action_scores

        self.epsilon = tf.placeholder(shape=[None, a_size], dtype=tf.float32, name='epsilon')

        self.output = self.mu + tf.sqrt(self.sigma_sq) * self.epsilon
        self.output = tf.identity(self.output, name='action')

        a = tf.exp(-1 * tf.pow(tf.stop_gradient(self.output) - self.mu, 2) / (2 * self.sigma_sq))
        b = 1 / tf.sqrt(2 * self.sigma_sq * np.pi)
        self.probs = a * b

        self.entropy = tf.reduce_sum(0.5 * tf.log(2 * np.pi * np.e * self.sigma_sq))

        self.value = tf.layers.dense(hidden_value, 1, activation=None, use_bias=False)

        self.old_probs = tf.placeholder(shape=[None, a_size], dtype=tf.float32, name='old_probabilities')

        self.create_deepq_optimizer(self.probs, self.old_probs, self.value, self.entropy, 0.0, epsilon, lr, max_step)


class DiscreteControlModel(DEEPQModel):
    def __init__(self, lr, brain, h_size, epsilon, beta, max_step, dueling=False, layer_norm=True):
        """
        Creates Discrete Control Actor-Critic model.
        :param brain: State-space size
        :param h_size: Hidden layer size
        """
        hidden_state, hidden_visual, hidden = None, None, None
        if brain.number_observations > 0:
            h_size, w_size = brain.camera_resolutions[0]['height'], brain.camera_resolutions[0]['width']
            bw = brain.camera_resolutions[0]['blackAndWhite']
            hidden_visual = self.create_visual_encoder(h_size, w_size, bw, h_size, 1, tf.nn.elu)[0]
        if brain.state_space_size > 0:
            s_size = brain.state_space_size
            if brain.state_space_type == "continuous":
                hidden_state = self.create_continuous_state_encoder(s_size, h_size, 1, tf.nn.elu)[0]
            else:
                hidden_state = self.create_discrete_state_encoder(s_size, h_size, 1, tf.nn.elu)[0]

        if hidden_visual is None and hidden_state is None:
            raise Exception("No valid network configuration possible. "
                            "There are no states or observations in this brain")
        elif hidden_visual is not None and hidden_state is None:
            hidden = hidden_visual
        elif hidden_visual is None and hidden_state is not None:
            hidden = hidden_state
        elif hidden_visual is not None and hidden_state is not None:
            hidden = tf.concat([hidden_visual, hidden_state], axis=1)

        a_size = brain.action_space_size

        self.batch_size = tf.placeholder(shape=None, dtype=tf.int32, name='batch_size')
        self.policy = tf.layers.dense(hidden, a_size, activation=None, use_bias=False,
                                      kernel_initializer=c_layers.variance_scaling_initializer(factor=0.1))
        self.probs = tf.nn.softmax(self.policy)
        self.action = tf.multinomial(self.policy, 1)
        self.output = tf.identity(self.action, name='action')
        self.value = tf.layers.dense(hidden, 1, activation=None, use_bias=False)

        self.entropy = -tf.reduce_sum(self.probs * tf.log(self.probs + 1e-10), axis=1)

        #self.action_holder = tf.placeholder(shape=[None], dtype=tf.int32)
        #self.selected_actions = c_layers.one_hot_encoding(self.action_holder, a_size)
        #self.old_probs = tf.placeholder(shape=[None, a_size], dtype=tf.float32, name='old_probabilities')
        #self.responsible_probs = tf.reduce_sum(self.probs * self.selected_actions, axis=1)
        #self.old_responsible_probs = tf.reduce_sum(self.old_probs * self.selected_actions, axis=1)

        

        self.create_deepq_optimizer(self.responsible_probs, self.old_responsible_probs,
                                  self.value, self.entropy, beta, epsilon, lr, max_step)

def _mlp(hiddens, inpt, num_actions, scope, reuse=False, layer_norm=False):
    with tf.variable_scope(scope, reuse=reuse):
        out = inpt
        for hidden in hiddens:
            out = c_layers.fully_connected(out, num_outputs=hidden, activation_fn=None)
            if layer_norm:
                out = c_layers.layer_norm(out, center=True, scale=True)
            out = tf.nn.relu(out)
        q_out = c_layers.fully_connected(out, num_outputs=num_actions, activation_fn=None)
        return q_out


def mlp(hiddens=[], layer_norm=False):
    """This model takes as input an observation and returns values of all actions.

    Parameters
    ----------
    hiddens: [int]
        list of sizes of hidden layers

    Returns
    -------
    q_func: function
        q_function for DQN algorithm.
    """
    return lambda *args, **kwargs: _mlp(hiddens, layer_norm=layer_norm, *args, **kwargs)


def _cnn_to_mlp(convs, hiddens, dueling, inpt, num_actions, scope, reuse=False, layer_norm=False):
    with tf.variable_scope(scope, reuse=reuse):
        out = inpt
        with tf.variable_scope("convnet"):
            for num_outputs, kernel_size, stride in convs:
                out = layers.convolution2d(out,
                                           num_outputs=num_outputs,
                                           kernel_size=kernel_size,
                                           stride=stride,
                                           activation_fn=tf.nn.relu)
        conv_out = layers.flatten(out)
        with tf.variable_scope("action_value"):
            action_out = conv_out
            for hidden in hiddens:
                action_out = layers.fully_connected(action_out, num_outputs=hidden, activation_fn=None)
                if layer_norm:
                    action_out = layers.layer_norm(action_out, center=True, scale=True)
                action_out = tf.nn.relu(action_out)
            action_scores = layers.fully_connected(action_out, num_outputs=num_actions, activation_fn=None)

        if dueling:
            with tf.variable_scope("state_value"):
                state_out = conv_out
                for hidden in hiddens:
                    state_out = layers.fully_connected(state_out, num_outputs=hidden, activation_fn=None)
                    if layer_norm:
                        state_out = layers.layer_norm(state_out, center=True, scale=True)
                    state_out = tf.nn.relu(state_out)
                state_score = layers.fully_connected(state_out, num_outputs=1, activation_fn=None)
            action_scores_mean = tf.reduce_mean(action_scores, 1)
            action_scores_centered = action_scores - tf.expand_dims(action_scores_mean, 1)
            q_out = state_score + action_scores_centered
        else:
            q_out = action_scores
        return q_out


def cnn_to_mlp(convs, hiddens, dueling=False, layer_norm=False):
    """This model takes as input an observation and returns values of all actions.

    Parameters
    ----------
    convs: [(int, int int)]
        list of convolutional layers in form of
        (num_outputs, kernel_size, stride)
    hiddens: [int]
        list of sizes of hidden layers
    dueling: bool
        if true double the output MLP to compute a baseline
        for action scores

    Returns
    -------
    q_func: function
        q_function for DQN algorithm.
    """

    return lambda *args, **kwargs: _cnn_to_mlp(convs, hiddens, dueling, layer_norm=layer_norm, *args, **kwargs)

