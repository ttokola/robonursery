# pylint: disable=E1101
import sys
import socket
import struct
from ga_spawner import GASpawner
from mlagents.envs import UnityEnvironment, BrainInfo
from robo_messages.robo_message_pb2 import RoboMessage
class UniScene():

    def __init__(self,file_name, worker_id, port, ai_addr, max_step = 100):
        self.file_name = file_name
        self.worker_id = worker_id
        self.port = port
        self._socket = None
        self.totalReward = 0
        self.buffer_size = 12000
        self.max_step = max_step
        self.current_step = 0
        self.ai_addr = ai_addr
        self.env:UnityEnvironment = None
        self.brain_info = None
        
        # available env info from Unity environment
            # env.logfile_path
            # env.brains
            # env.global_done
            # env.academy_name
            # env.number_brains
            # env.number_external_brains
            # env.brain_names
            # env.external_brain_names
        self.brains = None ##currently not used for anything
        self.globalDone  = None ##currently not used for anything
        self.academyName = None ##currently not used for anything
        self.numberOfBrains = None ##currently not used for anything
        self.numberOfExternalBrains = None ##currently not used for anything
        self.brainNames = None ##currently not used for anything
        self.externalBrainNames = None ##currently not used for anything
        self.logFilePath = None ##currently not used for anything
        
        ####Available info from Brain##################
            #self.brain_name 
            #self.vector_observation_space_size
            #self.num_stacked_vector_observations 
            #self.number_visual_observations 
            #self.camera_resolutions 
            #self.vector_action_space_size 
            #self.vector_action_descriptions 
            #self.vector_action_space_type 
        ################################################
        self.brainName = None ## to be passed to external AI
        self.vectorObservationSpaceSize = None ## to be passed to external AI
        self.NumberOfStackedVectorObservations = None ## to be passed to external AI
        self.numberOfVisualObservations = None ## to be passed to external AI
        self.CameraResolutions = None ## to be passed to external AI
        self.vectorActionSpaceSize = None ## to be passed to external AI
        self.vectorActionDescriptions = None ## to be passed to external AI
        self.vectorActionSpaceType = None ## to be passed to external AI
        self.numberOfAgents = None ## to be passed to external AI

            
    def connect (self):
        try:
            # self._socket = socket.create_connection(self.ai_addr)
            self._socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self._socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            self._socket.bind(("localhost", 0))
            self._socket.connect(self.ai_addr)
            self._socket.settimeout(5)
        except ConnectionError:
            print("Connection failed!")
    
    def close_connection(self):
        """
        Send 
        """
        roboMessage= RoboMessage()
        roboMessage.header.status = 400 # TODO: Figure out a good communication scheme (Start, OK, end, error). Modify protobuf?
        self.send(roboMessage.SerializeToString())
        self._socket.shutdown(socket.SHUT_RD)
        self._socket.close()
        print("%s shut down socket" % str(self.worker_id))

    def receive(self):
        """
        """
        if(self._socket == None):
            return None
        s = self._socket.recv(self.buffer_size)
        if len(s) == 0:
            return None
        message_length = struct.unpack("I", bytearray(s[:4]))[0]
        s = s[4:]
        # Receive multipart messages
        while len(s) != message_length:
            s += self._socket.recv(self.buffer_size)
        robo_message = RoboMessage()
        robo_message.ParseFromString(s)
        return robo_message

    def send(self, message):
        if(self._socket == None):
            print("Connection not open!")
            raise AttributeError
        self._socket.send(struct.pack("I", len(message)) + message)

    def buildUnity(self):
        print("build plz")


    def startEnvironment(self,seed:int, graphics_flag:bool, tryAgainCounter:int=0) -> UnityEnvironment:
        """
        Start Unity environment

        Args:
            seed: random seed passed to UnityEnvironment
            graphics_flag: 

        """

        try:
            print("Starting Unity scene from location " + self.file_name)
            print()
            env = UnityEnvironment(
                file_name=self.file_name, 
                worker_id= self.worker_id ,
                base_port=self.port,
                seed=seed,
                no_graphics=graphics_flag              
            )
            return env
        except Exception as err:
            print("ERROR! Simulation could not be started.")
            print()
            print(err)
            raise err

    def initialize(self, seed, graphics_flag):
        self.env = self.startEnvironment(seed, graphics_flag)

        if (self.env == None):
            print("Error! Could not start Unity environment.")
            sys.exit(1)
        if (self.env.number_external_brains != 1):
            print ("Simulation aborted! The simulation environment has '" + str(self.env.number_external_brains) + "' Brains. Robonursery must have exactly one external Brain.")
            sys.exit(1)

        # save used Brain and it's info
        # these parameters should be included in the "robomessage" so
        # that ext AI can figure out what kind of observations are available
        # and also the format of the observation arrays
        # same applies also for actions sent by the AI
        sceneBrain = self.env.brains[self.env.external_brain_names[0]]
        self.brainName = sceneBrain.brain_name
        self.vectorObservationSpaceSize = sceneBrain.vector_observation_space_size
        self.NumberOfStackedVectorObservations = sceneBrain.num_stacked_vector_observations ## to be passed to external AI
        self.numberOfVisualObservations = sceneBrain.number_visual_observations ## to be passed to external AI
        self.CameraResolutions = sceneBrain.camera_resolutions ## to be passed to external AI
        self.vectorActionSpaceSize = int(sceneBrain.vector_action_space_size[0]) ## to be passed to external AI
        self.vectorActionDescriptions = sceneBrain.vector_action_descriptions ## to be passed to external AI
        self.vectorActionSpaceType = sceneBrain.vector_action_space_type ## to be passed to external AI
        
        brains = self.env.reset()
        print(brains)
        self.brainName = list(brains.keys())[0]
        self.brain_info = brains[self.brainName]
        self.connect()

    def run_step(self):
        observations = self.brain_info.vector_observations[0]
        roboMessage = self.create_message()
        message = roboMessage.SerializeToString()
        self.send(message)
        actions = self.receive()
        info = self.env.step({self.brainName: actions.agent_action.vector_actions})
        self.brain_info = info[self.brainName]
        #collect rewards from the Scene
        self.totalReward += self.brain_info.rewards[0]
        self.current_step += 1
    
    def run_steps(self):
        while(self.current_step < self.max_step):
            self.run_step()
        self.close_connection()
        self.env.close()

    def create_message(self):
        if self.brain_info == None:
            return RoboMessage()     
        roboMessage= RoboMessage()
        roboMessage.header.status = 200
        roboMessage.agent_info.vector_actions_size = self.vectorActionSpaceSize # TODO: get correct value
        for i in range(len(self.brain_info.vector_observations[0])):
            roboMessage.agent_info.stacked_vector_observation.append(self.brain_info.vector_observations[0][i])
        
        # TODO: Process visual observations
        
        roboMessage.agent_info.reward = self.brain_info.rewards[0]
        return roboMessage
