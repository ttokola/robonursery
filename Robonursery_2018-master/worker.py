from subprocess import Popen 
from genome import Genome

class Worker:
    def __init__(self, genome:Genome, port:int ):
        # self.gene_file_name = "" # name of the gene file e.g. "gene_21038129.gene"
        self.genome = genome
        self.rewards = [] # list of rewards per scene
        self.port = port
        self.process_handle:Popen = None # process handle
        self.pid = ""
        
    def calculateTotalReward (self):
        totalReward = 0
        for i in range(len(self.rewards)):
            totalReward += self.rewards[i]
        return totalReward
        
    
    
