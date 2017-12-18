import numpy as np

experience_replay_keys = ['states', 'actions', 'rewards', 'next_states']


class ReplayMemory(object):
    __init__(self, deepq_model, memory_size, use_observations, use_states):
        if use_states:
            self.state = np.empty((self.memory_size, deepq_model.a_size), dtype=np.float32)
        if use_observations:
            self.obseration = np.empty(self.memory_size)
        self.action
        self.reward
        self.next_state
        self.count = 0
        self.current = 0
def discount_rewards(r, gamma=0.99, value_next=0.0):
    """
    Computes discounted sum of future rewards for use in updating value estimate.
    defined at time t as R_t=sum(gamma^(t'-t)*r_t'). t' [0,T] where T is the step at which game terminates
    The definition is the same as in PPO.
    The future rewards are assumed to be discounted by a factor of gamma per time-step.
    gamma was set to 0.99 throughout
    :param r: list of rewards.
    :param gamma:Discount factor, 0.99 in article. Trades-off immediate and future rewards
    :param value_next: t+1 value estimate for returns calculation
    """
    running_sum = value_next
    for t in reversed(range(0, r.size)):
        running_sum =  running_sum * gamma + r[t]
        discounted_r[t] = running_sum
    return discount_r

