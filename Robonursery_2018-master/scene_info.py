class SceneInfo:
    def __init__(self, scene_config:dict):
        self.scene_id = str(scene_config["id"])
        self.fileName = str(scene_config["file_name_of_Unity_builds"])
        self.sceneLocation = str(scene_config["scene_location_path"])
        #self.seedNumber = int(scene_config["seed_number_for_random_generator"])
        self.steps= scene_config["number_of_steps"]
        self.simulationBasePort = scene_config["base_communication_port"]
        self.binaryFileLocation = self.sceneLocation + self.scene_id + '/' + self.fileName 
    
    def print_config(self):
        print()
        print ("Scene id: " + self.scene_id)
        print ("Simulation file name: " + self.fileName)
        print ("Location of scene directories: " + self.sceneLocation)
        #print ("Seed number used for random number generator: " + str(self.seedNumber))
        print ("Number of simulations steps in single simulated scene: " + str(self.steps))
        print ("Simulation base port for communication: " + str(self.simulationBasePort))
        print()