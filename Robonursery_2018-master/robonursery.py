import sys
import datetime
import csv
from worker import Worker
from uniscene import UniScene
from threading import Thread
import json
import os
import shutil 
import os.path
import unittest
import random
from scene_info import SceneInfo
from ga_spawner import GASpawner
from worker import Worker
from subprocess import Popen
from typing import List
from typing import Dict
from genome import Genome

class Robonursery:
    def __init__(self):
        self.environments = []
        self.workers : List[Worker] = []
        self.GA = None
        self.config = None
        self.ai:str = ""
        self.numberOfGenerations:int = 0
        self.sizeOfPopulation:int = 0 
        self.aiBasePort:int = 0
        self.noGraphicsDuringSimulation:int = None
        self.scene_list:List[SceneInfo] = []
        self.seedNumber:int = None
        self.geneDirectory:str = ""
        self.geneType:str = "int8"
        self.populationPath:str = ""
        self.useExistingPopulation:int = None
        self.ExistingPopulationPath:str = ""
        self.numberOfGenesPerIndividual:int = None
        self.deapParameters:Dict = {}

        
    def read_config(self, path):
        """
        Read the config json file at path

        Args:
            path: Filepath of the json file

        Raises:
            FileNotFoundError: Raised when file is not found
            AttributeError: Raised if config file is empty
        """
        try :
            with open(path, "r") as read_file:
                self.config = json.load(read_file)
            if self.config == None:
                raise AttributeError
            self.ai = self.config["ai"]
            self.numberOfGenerations = self.config["number_of_population_generations"]
            self.sizeOfPopulation = self.config["population_size"]
            self.aiBasePort = self.config["ai_base_port"]
            self.seedNumber = self.config["seed_number_for_random_generator"]
            self.numberOfGenesPerIndividual = self.config["number_of_genes_per_individual"]
            self.geneType = self.config["gene_type"]
            self.geneDirectory = self.config["directory_for_genes"]
            self.ExistingPopulationPath = self.config["existing_population_path"]
            self.useExistingPopulation = self.config["use_existing_population_in_simulation"]
            self.noGraphicsDuringSimulation = self.config["no_graphics_during_simulation"]
            self.deapParameters = self.config["DEAP_parameters"]
            # errorCheck is used only for config validation
            # KeyError exception is thrown if parameter is not found from config
            errorCheck = self.deapParameters["fittest_percentage_for_breeding"]
            errorCheck = self.deapParameters["gene_mutation_propability_in_breeding"]
            errorCheck = self.deapParameters["individual_mutation_propability_in_breeding"]
            errorCheck = self.deapParameters["cross_mutation_propability_in_breeding"]
            errorCheck = self.deapParameters["eta_Crowding_degree_of_the_mutation"]
            # print("DEAP parameters:")
            # print(self.deapParameters)
            # print(self.deapParameters["fittest_percentage_for_breeding"])
            # print(self.deapParameters["gene_mutation_propability_in_breeding"])
            # print(self.deapParameters["individual_mutation_propability_in_breeding"])
            # print(self.deapParameters["cross_mutation_propability_in_breeding"])
            for scene in self.config["scenes"]:
                self.scene_list.append(SceneInfo(scene))
                errorCheck = scene["id"]
                errorCheck = scene["file_name_of_Unity_builds"]
                errorCheck = scene["scene_location_path"]
                errorCheck = scene["number_of_steps"]
                errorCheck = scene["base_communication_port"]

            return
        except FileNotFoundError:
            print("Error! Config file could not be read at %s." % path)
            raise
        except AttributeError:
            print("Error! Config file at path %s is empty!" % path)
            raise
        except ValueError:
            print("Error when reading config file! Incorrect parameter type.")
            print("Check that parameters containing strings have quotation marks (e.g. \"uint8\") ")
            print("Check that integer and float parameters do not have letters or quotation marks (e.g. 5005) ")
            raise
        except KeyError as err:
            print("Error when reading config file!")
            print("Key %s was not found from the config file."  %err)
            raise
            
    def initialize(self, config_path):
        try:
            self.read_config(config_path)
            self.validate_config(config_path)
            # Create directory for this path
            gene_directory = os.path.join(self.geneDirectory, datetime.datetime.now().strftime("%Y-%m-%d_%H%M%S") )
            existing_population_path = None
            if(self.useExistingPopulation == 1):
                existing_population_path = self.ExistingPopulationPath
            self.GA = GASpawner(self.config["ai"], gene_directory, self.geneType, self.numberOfGenesPerIndividual, self.sizeOfPopulation, self.deapParameters , existing_population_path)
            # Write initial population
            self.GA.write_population()
        except ValueError:
            print("Initialization failed. Exiting...")
            exit(1)
        except TypeError:
            print("Initialization failed. Exiting...")
            exit(1)
        except FileNotFoundError:
            print("Initialization failed. Exiting...")
            exit(1)

    def validate_config(self, config_path):
        currentPath = os.getcwd()

        # check that scenes defined in config exists
        for scene in self.scene_list:
            scenePath = os.path.join(currentPath, scene.binaryFileLocation)
            if os.path.isfile(scenePath + ".exe") or os.path.isfile(scenePath):
                pass
            else:
                print ("ERROR! Scene file \"%s\" defined in config file not found." %scenePath)
                raise FileNotFoundError
                #exit(1)
            if not ( isinstance(scene.steps , int) and (scene.steps > 0) ):
                print("ERROR IN CONFIG FILE! 'number_of_steps' parameter must be an integer and greater than zero")
                raise TypeError
            if not ( isinstance(scene.simulationBasePort , int) and (scene.simulationBasePort > 0) ):
                print("ERROR IN CONFIG FILE! 'base_communication_port' parameter must be an integer and greater than zero")
                raise TypeError

        tempPath = os.path.join(currentPath, self.ai)
        if not os.path.isfile(tempPath):
            print ("ERROR! The AI script file \"%s\" defined in config was not found." %tempPath)
            raise FileNotFoundError

        if not ( isinstance(self.numberOfGenerations , int) and (self.numberOfGenerations > 0) ):
            print("ERROR IN CONFIG FILE! 'number_of_population_generations' parameter must be integer and greater than zero")
            raise TypeError

        if not ( isinstance(self.sizeOfPopulation , int) and (self.sizeOfPopulation > 1) ):
            print("ERROR IN CONFIG FILE! 'population_size' parameter must be integer and greater than two (2)")
            raise TypeError

        if not ( isinstance(self.noGraphicsDuringSimulation , int) and ((self.noGraphicsDuringSimulation == 0) \
                or (self.noGraphicsDuringSimulation == 1)) ):
            print("ERROR IN CONFIG FILE! 'no_graphics_during_simulation' parameter must be integer with value of '0' or '1'")
            raise TypeError

        if not ( isinstance(self.aiBasePort , int) and (self.aiBasePort > 0) ):
            print("ERROR IN CONFIG FILE! 'ai_base_port' parameter must be integer and greater than zero")
            raise TypeError

        if not isinstance(self.seedNumber , int):
            print("ERROR IN CONFIG FILE! 'seed_number_for_random_generator' parameter must be integer")
            raise TypeError
            
        if not ( isinstance(self.numberOfGenesPerIndividual , int) and (self.numberOfGenesPerIndividual > 0) ):
            print("ERROR IN CONFIG FILE! 'number_of_genes_per_individual' parameter must be integer and greater than zero")
            raise TypeError
            
        if not ( isinstance(self.deapParameters["fittest_percentage_for_breeding"] , int) \
                and ( self.deapParameters["fittest_percentage_for_breeding"] in range(1,101) ) ):
            print("ERROR IN CONFIG FILE! 'fittest_percentage_for_breeding' parameter must be integer in range 1...100")
            raise TypeError
            
        if not ( isinstance(self.deapParameters["gene_mutation_propability_in_breeding"] , float) \
                and ( (self.deapParameters["gene_mutation_propability_in_breeding"] >= 0) \
                and (self.deapParameters["gene_mutation_propability_in_breeding"] <= 1)  )):
            print("ERROR IN CONFIG FILE! 'gene_mutation_propability_in_breeding' parameter must be float in range 0.0-1.0")
            raise TypeError
            
        if not ( isinstance(self.deapParameters["individual_mutation_propability_in_breeding"] , float) \
                and ( (self.deapParameters["individual_mutation_propability_in_breeding"] >= 0) \
                and (self.deapParameters["individual_mutation_propability_in_breeding"] <= 1)  )):
            print("ERROR IN CONFIG FILE! 'individual_mutation_propability_in_breeding' parameter must be float in range 0.0-1.0")
            raise TypeError
            
        if not ( isinstance(self.deapParameters["cross_mutation_propability_in_breeding"] , float) \
                and ( (self.deapParameters["cross_mutation_propability_in_breeding"] >= 0) \
                and (self.deapParameters["cross_mutation_propability_in_breeding"] <= 1)  )):
            print("ERROR IN CONFIG FILE! 'cross_mutation_propability_in_breeding' parameter must be float in range 0.0-1.0")
            raise TypeError
            
        if not ( isinstance(self.deapParameters["eta_Crowding_degree_of_the_mutation"] , int) \
                and (self.deapParameters["eta_Crowding_degree_of_the_mutation"] > 0)  ):
            print("ERROR IN CONFIG FILE! 'eta_Crowding_degree_of_the_mutation' parameter must be integer and greater than zero")
            raise TypeError
            
        if not ( isinstance(self.useExistingPopulation , int) and ((self.useExistingPopulation == 0) \
                or (self.useExistingPopulation == 1)) ):
            print("ERROR IN CONFIG FILE! 'use_existing_population_in_simulation' parameter must be integer with value of '0' or '1'")
            raise TypeError
            
        tempPath = os.path.join(currentPath, self.ExistingPopulationPath)
        if ( (self.useExistingPopulation == 1) and not os.path.isfile(tempPath)):
            print ("ERROR! The existing population file \"%s\" defined in config was not found." %tempPath)
            raise FileNotFoundError

    def print_config(self):
        """
        Prints configuration
        """
        print()
        print("Configuration file content:")
        print ("AI : " + self.ai)
        print("Population size: %d" %self.sizeOfPopulation)
        print ("Number of generations: %d" %self.numberOfGenerations )
        print ("AI base port: %d" % self.aiBasePort) 
        for scene in self.scene_list:
            scene.print_config()

    def generate_random_population(self):
        if (self.geneType == "int8"):
            maximumGeneValue = 255
        elif (self.geneType == "int16"):
            maximumGeneValue = 65535
        elif (self.geneType == "int32"):
            maximumGeneValue = 4294967295
        else:
            print("Invalid \"gene_type\" parameter defined in config file")
            print("Valid values are \"int8\", \"int16\", \"int32\"")
            sys.exit(1)
        populationDirectoryName = datetime.datetime.now().strftime("%Y-%m-%d_%H%M%S")

        currentPath = os.getcwd()
        #genePath = os.path.join(currentPath, self.config["directory_for_genes"] )
        genePath = os.path.join(currentPath, self.geneDirectory )
        try:
            os.mkdir(genePath)
        except FileExistsError:
            pass
        except FileNotFoundError:
            print("Aborting simulation!")
            print("The directory for genes %s does not exist" %self.geneDirectory)
            print("Create the directory manually or specify an existing directory in config.json")
            sys.exit(1)
        os.mkdir(genePath + populationDirectoryName)
        os.mkdir(genePath + populationDirectoryName + "/Generation_0001")
        os.chdir(genePath + populationDirectoryName + "/Generation_0001")
        numberOfGenes = self.numberOfGenesPerIndividual
        random.seed(self.seedNumber)
        try:
            f = open("population.csv","w")
            for i in range(self.sizeOfPopulation):
                randomIntArray = []
                for i in range(numberOfGenes):
                    randomInt = random.randint(0,maximumGeneValue)
                    f.write(str(randomInt))
                    if ( i == (numberOfGenes - 1) ):
                        f.write("\n")
                    else:
                        f.write(",")
        except Exception as err:
            print("Exception: %s" %str(err))
            print("Error in generating the population csv-file.")
            exit(1)
        finally:
            f.close() 
        print()
        print ("New population created to directory %s%s" %(genePath, populationDirectoryName))
        print()
        self.populationPath = os.getcwd()
        os.chdir(currentPath)
        
    def convert_population_to_byte_files(self, filePath):
        geneFileList = []
        currentPath = os.getcwd()
        os.chdir(filePath)
        file1 = open("population.csv","r")
        with open("population.csv", newline='\n') as csvfile:
            population = csv.reader(csvfile)
            #going through population csv row by row (one row equals the genome of single individual)
            workerId = 0
            geneDirectory = os.getcwd()
            for individual in population:
                workerId += 1
                fileName = str(workerId).rjust(6, "0") + ".gene"
                geneFileList.append(os.path.join(geneDirectory, fileName ))
                geneArray = []
                # reading individual genes to array.
                for gene in individual:
                    geneArray.append(int(gene))
                # check that the number of genes equals the 'number_of_genes_per_individual' value in config.json
                if (len(geneArray) != self.numberOfGenesPerIndividual):
                    print("ABORTING SIMULATION!")
                    print("The number of genes in 'population.csv' for Worker #" + \
                    "%d does not match the value of 'number_of_genes_per_individual' in config.json" %workerId)
                    sys.exit(1)
                # converting integer array to byte format. 
                geneBytes = bytearray(geneArray)
                # saving individual file in byte format
                try:
                    file2 = open(fileName, mode='xb')
                    file2.write(geneBytes)
                except Exception as err:
                    print("Exception: %s" %str(err))
                    print("Error in generating the population binary files.")
                    exit(1)
                finally:
                    file2.close() 
        os.chdir(currentPath)
        file1.close()   
        return geneFileList
    
    def create_worker(self, worker_id, genome, port):        
        """
        Spawn a new AI process with given gene

        Args:
            worker_id: worker id of the connected environment
            genome: Genome object containing 
            port: Passed on to ai process. UniScene should try to connect to this port. 

        Returns:
            Worker: New worker object

        Raises:

        """
        if(genome.path == None):
            print("Error! Can't create new worker for genome. Byte file has not been written.")
            raise ValueError

        process = Popen([sys.executable, self.ai , genome.path, str(port)])
        worker = Worker(genome, port)
        worker.process_handle = process
        worker.pid = process.pid
        return worker
            
    def create_workers(self):
        """ Spawn worker 
        """
        if(self.config == None):
            print("Error! Can't create workers. Config not initialized.")
            raise AttributeError
        worker_base_port = int(self.config["ai_base_port"])

        for i in range(self.sizeOfPopulation):  
            self.workers.append( self.create_worker(i, self.GA.population[i],worker_base_port + i))
            
    def close_workers(self):
        for worker in self.workers:
            if(worker.process_handle != None):
                worker.process_handle.terminate()
        self.workers = []
                
    def rankGenomesByRewards(self):
        genomeRewardList = []
        for i in range(len(self.workers)):
            genome_reward = self.workers[i].calculateTotalReward()
            genomeRewardList.append(tuple((self.workers[i].genome.genes, genome_reward))) #self.workers[i].genome -> self.workers[i].genome.genes

        #print (str(geneRewardList))
        sortedRewardList = sorted(genomeRewardList, key=lambda worker: worker[1], reverse=True)
        return sortedRewardList


    # def write_json():
    #             #write rewards into json
    #     try:
    #         with open('rewards_by_individual.json', 'w') as jsonFile:
    #             data = {}
    #             for i in range(len(robonursery.workers)):
    #                 data[i] = {} #individual/worker level
    #                 for j in range(len(robonursery.scene_list)):
    #                     data[i][j] = str(robonursery.workers[i].rewards[j]) #scene level
    #             jsonFile.seek(0) #restore file position to beginning
    #             json.dump(data, jsonFile, indent=4)
    #             jsonFile.truncate #remove remainig part
    #     except Exception as err:
    #         print("Exception: %s" %str(err))
    #         print("Error in writing rewards into json file.")
    #         exit(1)
    #     finally:
    #             jsonFile.close()  
    #   
if __name__ == "__main__":
    robonursery = Robonursery()

    try:
        # Print scene information to console
        PRINT_CONFIG=False
        #scene's configurations in json
        scene_config=None
        jsonPath="config.json"
        SceneRewardsList = []

        if(len(sys.argv)>1):
            jsonPath = sys.argv[1]

        robonursery.initialize(jsonPath)
        
        genome_rewards = {}
        for generation in range(robonursery.numberOfGenerations):
            robonursery.create_workers()
            # RUN SCENES
            for scene_info in robonursery.scene_list:
                if PRINT_CONFIG:
                    robonursery.print_config()
                robonursery.environments = []
                # Create UniScene wrappers
                for j in range(robonursery.sizeOfPopulation):
                    try:
                        scene = UniScene(
                            file_name = scene_info.binaryFileLocation,
                            worker_id=j,
                            port=scene_info.simulationBasePort+j,
                            ai_addr=('localhost', robonursery.workers[j].port),
                            max_step = scene_info.steps
                        )
                        scene.initialize(robonursery.seedNumber, robonursery.noGraphicsDuringSimulation)
                        scene_t = Thread(target=scene.run_steps)
                        print(str(scene_t))
                        robonursery.environments.append((scene,scene_t))
                    except Exception as err:
                        print("Exception: %s" %str(err))
                        print("Run cancelled.")
                        robonursery.close_workers()
                        exit(1)

                # Start all simulations
                print("Starting simulations in scene %s..." % scene_info.scene_id)
                for j in range(len(robonursery.environments)):
                    robonursery.environments[j][1].start()
                print("Simulation running..." )
                # Wait for simulations to finish running
                for j in range(len(robonursery.environments)):
                    robonursery.environments[j][1].join()

                for j in range(len(robonursery.environments)):
                    robonursery.workers[j].rewards.append( robonursery.environments[j][0].totalReward)
                print("Scene %s done!" % scene_info.scene_id)

            print()
            print("Scenes completed!")
            print("Worker rewards:")
            for i in range(len(robonursery.workers)):
                workerRewards = []
                # print("Rewards for gene %s" % robonursery.workers[i].gene_file_name)
                for j in range(len(robonursery.scene_list)):
                    #scene_id = robonursery.scene_list[j].scene_id
                    reward = robonursery.workers[i].rewards[j]
                    #print("Scene %s: %d " % (scene_id, reward))
                    workerRewards.append(reward)
                print( "Worker '" + str(i) + "' = " + str(workerRewards) + " --> total = " + str(robonursery.workers[i].calculateTotalReward()) ) 
                
            sortedRewardList = robonursery.rankGenomesByRewards()
            
            print ()
            print ("The Genes have been ranked based on their achieved total rewards!")
            print (str(sortedRewardList))

            robonursery.close_workers()
            robonursery.GA.write_report(sortedRewardList)
            if(generation < robonursery.numberOfGenerations):
                robonursery.GA.breed_new_generation(sortedRewardList)
    finally:
        robonursery.close_workers()
        exit(0)
    
   
