import numpy as np
import tensorflow as tf

class Trainer(object):
    def __init__(self, deepq_model, sess, info, is_continuous, use_observations, use_states):
        """
        Responsible for collections and training the model.
        :param deepq_model: Tensorflow graph defining model.
        :sess: Tensorflow session
        :param info: Environment BrainInfo object.
        :param is_continuous: whether action-space is continuous.
        :param use_observations: Whether afent takes image observations
        """
        self.model = deepq_model
        self.sess = sess
        stats = {'cumulative_reward': [], 'episode_length': [], 'value_estimate': [],
                 'entropy': [], 'value_loss': [], 'policy_loss': [], 'learning_rate': []}

        self.training_buffer = vectorize_history(empty_local_history({}))
        self.history_dict = empty_all_history(info)