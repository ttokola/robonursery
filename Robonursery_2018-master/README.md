# Robonursery 2018 Guide

## Introduction

[Unity Machine Learning Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) is an 
open-source Unity plugin that enables games and simulations to serve as environments for 
training intelligent agents. ML-Agents natively supports only certain learning / optimization 
algorithms ([See ML-Agents documentation](https://github.com/Unity-Technologies/ml-agents/tree/master/docs)).

Robonursery 2018 project was developed to allow researchers of general intelligence algorithms to 
use their own custom algorithms with Unity Machine Learning Agents Toolkit. Robonursery 2018 
framework utilizes Python API provided by the ML-Agents Toolkit to create an interface between 
custom made algorithm and the ML-Agents toolkit. 

Robonursery 2018 interfaces between the Unity simulation and the custom training algorithms by 
relaying the observations form Unity simulation environment to the learning algorithm. 
When the learning algorithm receives these observations ( Robonursery 2018 supports vector and 
visual observations.), it is supposed to create a response/action back to the Unity environment. 
Robonursery 2018 framework will relay the responses created by the algorithm to the Unity simulation. 

The responses produced by the learning algorithm will trigger some action in the Unity simulation. 
These actions are rewarded during the simulation. The rewarding policy is implemented inside the 
Unity simulation scene. When the simulation is completed (i.e simulation is run for predefined 
amount of steps), Robonursery framework collects all the rewards and breeds a new population 
generation using genetic algorithm. The simulations are then repeated with new population until the 
predefined amount of population generations have been simulated.

The simulated scenes are standalone binary builds made with Unity 3D environment. As the simulation 
scenes are pre-build binaries, Robonursery 2018 framework does not require the presence of Unity 3D 
environment to run the simulations. Unity 3D environment is only needed for editing the scenes or 
to create new scenes.  

## Description of Robonursery 2018 Internal Operation

Below is a short execution flow description of the Robonursery 2018 framework:
 
1. Read and validate the configuration file ("config.json" by default. Configuration filename can 
also be provided as command line parameter, for example, "python robonursery.py my_config.json")
2. Create a population using parameters defined in the configuration file. The population is saved to 
a directory that is also defined in the configuration file. 
3. Simulate scene #1 (Simulations are run parallel. There will be as many simulations running in 
parallel as there are individuals in the population)
4. When all the simulation processes have finished, the simulation will proceed to scene #2. 
Simulations will continue until all the scenes have been simulated.
5. When all scenes have been simulated, the individuals in the population are ranked based on the
sum of all rewards they have received during simulations
6. Individuals with highest rewards are used to breed the next population generation
7. Steps 3-6 are repeated until the predefined number of generations (defined in config file) 
have been simulated. 


## Usage Instructions

1. develop artificial intelligence (AI) script
2. Create scene(s) with Unity for training the robot. The testing of Robonursery 2018 framework was
done with Rollerball demo which is fairly simple environment. The Windows x64 binaries for this 
environment can be found from Robonursery 2018 github. 
3. Configure config file
4. Run the simulation with "python robonursery.py" command

### Requirements from user developed AI script

The AI script must be able to: 
1. Receive socket communication
2. Read a csv file containing the genes
3. Configure the AI brain using the genes 
4. Process the the inputs sent by Robonursery (e.g. observations)
5. Make a decision for actions based on the received observations
6. Transmit actions via socket communication back to Robonursery 2018 framework 

Robonursery 2018 includes simple AI script (RollerAI.py) which was used for testing the framework. 
This example script can be used as a reference for creating custom AI script.  

### Configuration file

The configuration file (config.json) can be used to configure the Robonursery 2018 framework 
simulation parameters. The file should contain json structure as shown below:

    {
        "ai": "C:\\Robonursery_2018\\AI\\RollerAI.py",
        "directory_for_genes": "C:\\Robonursery_2018\\Genes",
        "population_size":40,
        "number_of_population_generations":50,
        "no_graphics_during_simulation":1,
        "ai_base_port": 20005,
        "number_of_genes_per_individual": 144,
        "gene_type": "uint8",
        "seed_number_for_random_generator":1189,
        "DEAP_parameters":
        {
            "fittest_percentage_for_breeding": 30,
            "gene_mutation_propability_in_breeding": 0.05,
            "individual_mutation_propability_in_breeding":0.5,
            "cross_mutation_propability_in_breeding": 1.0,
            "eta_Crowding_degree_of_the_mutation": 10
        },
        "use_existing_population_in_simulation": 0,
        "existing_population_path": "C:\\Robonursery_2018\\Genes\\Generation_0.csv",
        "scenes":
        [
            {
                "id" : "scene1",
                "file_name_of_Unity_builds":"ML_RollerBall",
                "scene_location_path":"C:\\Robonursery_2018\\Builds\\",
                "number_of_steps":10000,
                "base_communication_port":6005
            },
            {
                "id" : "scene2",
                "file_name_of_Unity_builds":"ML_RollerBall",
                "scene_location_path":"C:\\Robonursery_2018\\Builds\\",
                "number_of_steps":10000,
                "base_communication_port":6005
            }

        ]
    }

"ai" parameter defines the location of the python script that contains the artificial intelligence 
algorithm to be simulated. The script must be able to receive the observations from the 
Robonursery 2018 framework and produce actions for the simulation. The path can be defined as relative 
to the robonursery.py script (e.g. ".\\\AI\\\ai_script.py") or absolute (e.g. "c:\\\robonursery\\\AI\\\ai_script.py").
NOTE! The paths in the config file can be defined using Windows or Unix style notation. If the 
path is defined in Windows style using backslashes ("\\"), there must be two backslashes between 
directories (e.g. "c:\\\robonursery\\\AI\\\ai_script.py"). If using Unix style notation with normal 
slashes, only one slash is needed between the directories (e.g. "c:/robonursery/AI/ai_script.py").

"directory_for_genes" parameter defines the path where the genes of simulated individuals are 
saved. When Robonursery 2018 framework is run, a new directory is created under the defined path.
The name of the diretory is a time stamp of the simulation start time.    

"population_size" parameter defines the size of the simulated population. Parameter must be integer
and greater than 1.

"number_of_population_generations" parameter defines how many generations will be simulated.
Parameter must be integer and greater than 0.

"no_graphics_during_simulation" parameter defines whether simulation graphics are displayed 
or not. Simulation will run much faster if graphics are disabled. Set value to '0' to enable 
graphics and '1' to disable graphics.

"ai_base_port" parameter defines the base value for communication socket between the AI script
and the robonursery 2018 framework. As simulations are run parallel the base port is incremented
by one for every individual simulation. For example, if population size is 100 and base port is
5000, ports 5000-5099 are used for simulations.

"number_of_genes_per_individual" parameter defines how many genes there are per single individual.
These genes are used by the AI script to calculate the response for the observations received
from the simulated scene via Robonursery 2018 framework. The genes are delivered to the AI script
as file path string. The AI python script should read this binary file and use those values in 
response creation. Parameter must be integer and greater than 0.

"gene_type" parameter defines the data type of single gene. Type can be 'int8', 'int16', 'int32', 
'uint8', 'uint16' or 'uint32'. One gene equals a number within the range that the data type can 
contain. For example if "gene_type" = uint16, one gene is positive number that can be represented
with 16 bits (i.e. 0...65535). If the "number_of_genes_per_individual" = 100 and "gene_type" = 
uint16, the total length of the AI's chromosome is therefore 100 * 16 bits = 1600bits = 200 bytes.

"seed_number_for_random_generator" parameter defines the seed number that will be used for random
number generators in Robonursery 2018. This enables reproducible simulations and therefore allows
developer to easily compare the results with different AI algorithms. 

"DEAP_parameters" structure. Robonursery 2018 uses [DEAP library](https://deap.readthedocs.io/en/master/) 
for breeding and mutating the population. Following parameters can be used to configure the 
breeding and gene mutation process. 

          "fittest_percentage_for_breeding" parameter defines how many individuals (percentage from
          the whole population) are selected for breeding the next generation. The individuals are 
          ranked according to the rewards they have gained from the simulation and only the top 
          performing are used for breeding. 

          "individual_mutation_propability_in_breeding" parameter defines the probability that 
          individual agent can mutate. If the probability is 0.0 no mutation will happen. With 
          probability of 1.0 every individual agent will be mutated. A good starting value is 0.5. 
        
          "gene_mutation_propability_in_breeding" parameter defines the probability that a single gene
          of an individual agent can mutate. If the probability is 0.0 no mutation will happen. With 
          probability of 1.0 every gene will be mutated. A good starting value is around 0.1. 
          Robonursery 2018 uses DEAP library mutPolynomialBounded function for mutation.
        
          "eta_Crowding_degree_of_the_mutation" parameter defines how big is the change when a gene
          is mutated. Lower value will produce bigger variations during mutation and thus will most 
          likely converge much quicker. High parameter value will produce lower variation and thus 
          will converge much slower. It is recommended that low value (e.g. 10) is used when the 
          initial population is randomly created. When more accuracy is needed then higher value 
          should be chosen (e.g. 1000)

          "cross_mutation_propability_in_breeding" parameter is currently not used for anything

"use_existing_population_in_simulation" parameter enables user to use existing population in the
simulation or even manually create a new one. Set value to '1' to use population defined in
"existing_population_path". When parameter is set to '0' Robonursery creates a random population
as the first population generation to be simulated. 

"existing_population_path" parameter defines the location of the population that user wants to use
as the first simulated population. The population file must be constructed so that it will contain
as many individuals as defined in the "population_size" parameter. Each individual must have as 
many genes as defined in the "number_of_genes_per_individual" parameter and the data type must
match "gene_type" parameter. Genes must be separated with comma (,). One row in the file equals one 
individual. For example, "population_size" = 3, "number_of_genes_per_individual" = 3 and "gene_type"
= int16 the file might be as follows:

          –32768,0,32767
          0,1,2
          -2,-1,0

"scenes" structure configures the simulation environment. Following parameters can be used to configure the 
simulation scenes. 

          "scene_location_path" defines the base location of the scene (e.g. "c:/robonursery/builds/") 

          "id" parameter defines the name of the simulated scene. It is also the name of the directory
          where the binary file of the simulation scene is located (e.g. if the "scene_location_path" = 
          "c:/robonursery/builds/" and "id" = "scene1" the binary file must be located in 
          "c:/robonursery/builds/scene1" directory)
          
          "file_name_of_Unity_builds" parameter defines the binary executable file name. For example,
          if the binary file name is "Scene1.exe" parameter value must be "Unity.exe" or "Unity"  
          
          "number_of_steps" parameter defines how many steps the scene will be simulated.

          "base_communication_port" parameter defines the base value for communication socket 
          between the scene binary file and the robonursery 2018 framework. As simulations are run 
          parallel the base port is incremented by one for every individual simulation. For example,
          if population size is 100 and base port is 6000, ports 6000-6099 are used for simulations. 

## System Requirements

Robonursery 2018 project was developed and tested with following system configuration:

- Windows 64-bit operating system
- Unity v2018.2.7f1 (only required when modifying scenes or creating new ones) 
- ML agents Release v0.5
- Python v3.6.6
- DEAP 1.2.2
