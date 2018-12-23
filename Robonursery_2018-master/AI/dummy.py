# pylint: disable=E1101
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

class DummyAI:
    """
    Placeholder "AI" for testing robonursery

    Attributes:
        port_number (int): Port number used for listening to new connections
        buffer_size: (int): Size of receive buffer.
        socket (:obj:`Socket`): Socket reserved for accepting incoming connections
        connection (:obj:`Socket`): Socket for the current active connection
        timeout (int): timeout for communication sockets in seconds

    """
    def __init__(self, port ):
        """
        Dummy AI constructor

        Args:
            port(int): port number used for listening to incoming connections

        """
        self.port_number : int = port
        self.buffer_size = 12000
        self.socket = None
        self.connection = None
        self.timeout = 30
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.socket.bind(("localhost", port))
        # self.socket.settimeout(self.timeout )
        self.socket.listen()

    def open_connection(self):
        """
        Open socket and listen for connections. 
        """
        try:
            self.connection,addr = self.socket.accept()
            print(addr)
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

    # def exchange(self, inputs: UnityInput) -> UnityOutput:
    #     message = self.receive_observations()
    #     if(message == None):

    #     message.header.status = 200
    #     message.unity_input.CopyFrom(inputs)
    #     self._communicator_send(message.SerializeToString())
    #     outputs = UnityMessage()
    #     outputs.ParseFromString(self._communicator_receive())
    #     if outputs.header.status != 200:
    #         return None
    #     return outputs.unity_output
    
    def close_connection(self):
        if self.socket != None:
            self.socket.close()
        if self.connection != None:
            self.connection.close()


if __name__ == "__main__":
    if(len(sys.argv) != 3):
        print("Incorrect number of arguments!")
    port = parseInt(sys.argv[2])
    print(sys.argv[1])
    try:
        dummy = DummyAI(port)
        while(True):
            dummy.open_connection()
            observations = dummy.receive()
            while(observations != None and observations.header.status == 200):
                response = RoboMessage()
                response.header.status = 200
                for i in range(observations.agent_info.vector_actions_size):
                    #generate random value in range -0.5 ... 0.5 and scale it
                    randomAction = (random.random() - 0.5) * 0.04
                    #response.agent_action.vector_actions.append(1.0)
                    response.agent_action.vector_actions.append(randomAction)
                dummy.send(response.SerializeToString())
                observations = dummy.receive()
            print("AI: Closing connection. PORT:%s" % str(dummy.port_number))
            dummy.connection.close()
    except TimeoutError:
        pass
    dummy.close_connection()
    exit(0)