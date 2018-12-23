#  https://github.com/mymultiverse/GeneticAlgo_OpenAIGymCartPole
#  http://laid.delanover.com/activation-functions-in-deep-learning-sigmoid-relu-lrelu-prelu-rrelu-elu-softmax/

# pylint: disable=E1101
import numpy as np
# import tensorflow as tf
import time
import socket
import struct
import sys
# random needed for testing actions. Can be removed when external AI is functional
import random
sys.path.append('./')
from robo_messages.robo_message_pb2 import RoboMessage

def parseInt(val):
    try:
        i_val = int(val)
        return i_val
    except ValueError:
        print("Error! %s is not an integer." % val)
        raise

def softmax(x):
	x = np.exp(x)/np.sum(np.exp(x))
	return x

# def lreLu(x):
# 	alpha=0.2
# 	return tf.nn.relu(x)-alpha*tf.nn.relu(-x)

def sigmoid(x):
	return 1/(1+np.exp(-x))

def reLu(x):
	return np.maximum(0,x)
    
def normalize(x:np.ndarray):
    return 2*(x - np.min(x))/255-1

class RollerAI:
    """
    Placeholder "AI" for testing robonursery

    Attributes:
        port_number (int): Port number used for listening to new connections
        buffer_size: (int): Size of receive buffer.
        socket (:obj:`Socket`): Socket reserved for accepting incoming connections
        connection (:obj:`Socket`): Socket for the current active connection
        timeout (int): timeout for communication sockets in seconds

    """
    def __init__(self, port, gene_path ):
        """
        Roller AI constructor

        Args:
            port(int): port number used for listening to incoming connections

        """
        with open(gene_path, mode='rb') as gene_file:
            gene_bytes = gene_file.read()
            self.genes = np.frombuffer(gene_bytes, dtype='uint8')      
        self.W1 = normalize(self.genes[0:80]).reshape((8,10))
        self.B1 = normalize(self.genes[80:90])
        self.W2 = normalize(self.genes[90:130]).reshape((10,4))
        self.B2 = normalize(self.genes[130:134])
        self.W3 = normalize(self.genes[134:142]).reshape((4,2))
        self.B3 = normalize(self.genes[142:144])
        self.input = np.zeros((8,))
        self.output = np.zeros((2,))

        # Set up socket
        self.port_number : int = port
        self.buffer_size = 12000
        self.socket = None
        self.connection = None
        self.timeout = 500
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.socket.bind(("localhost", port))
        self.socket.listen()

    def feed_forward(self):
        self.layer1 = np.tanh(np.dot(self.input, self.W1) + self.B1.T )
        self.layer2 = np.tanh(np.dot(self.layer1, self.W2) + self.B2.T )
        self.output = np.tanh(np.dot(self.layer2, self.W3) + self.B3.T )

    def open_connection(self):
        """
        Open socket and listen for connections. 
        """
        try:
            self.connection,addr = self.socket.accept()
            self.connection.settimeout(self.timeout)
        except TimeoutError:
            print("Connection timed out!\n")
            self.close_connection()
        
    def receive(self):
        """
        """
        if(self.socket == None or self.connection == None):
            return None
        s = self.connection.recv(self.buffer_size)
        if len(s) == 0:
            return None
        message_length = struct.unpack("I", bytearray(s[:4]))[0]
        s = s[4:]
        # Receive multipart messages
        while len(s) != message_length:
            s += self.connection.recv(self.buffer_size)
        robo_message = RoboMessage()
        robo_message.ParseFromString(s)
        return robo_message

    def send(self, message):
        self.connection.send(struct.pack("I", len(message)) + message)

    def close_connection(self):
        if self.socket != None:
            self.socket.close()
        if self.connection != None:
            self.connection.close()

def main():
    if(len(sys.argv) != 3):
        print("Incorrect number of arguments!")
    port = parseInt(sys.argv[2])
    gene_path = sys.argv[1]
    try:
        roller_ai = RollerAI(port, gene_path)
        while(True):

            roller_ai.open_connection()
            observations = roller_ai.receive()
            while(observations != None and observations.header.status == 200):
                roller_ai.input = np.array(observations.agent_info.stacked_vector_observation)
                roller_ai.feed_forward()
                response = RoboMessage()
                response.header.status = 200
                for action in roller_ai.output:
                    response.agent_action.vector_actions.append(action)
                roller_ai.send(response.SerializeToString())
                observations = roller_ai.receive()
            # print("AI: Closing connection. PORT:%s" % str(roller_ai.port_number))
            roller_ai.connection.close()
    except TimeoutError:
        pass
    roller_ai.close_connection()
    exit(0)

if __name__ == "__main__":
    main()