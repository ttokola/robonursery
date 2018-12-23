import csv
import sys
from pathlib import Path
import numpy as np
from worker import Worker
from  subprocess import Popen
from genome import Genome
import random
import shutil
from deap import base
from deap import creator
from deap import tools
class GASpawner:
    """ Handles creating new populations, ranking and breeding existing populations and spawning AI processes for
        each round of simulation

        Attributes:
            ai (str): Filepath to the AI
            gene_path (str): Relative path to directory where gene files will be placed
            gene_type (str): Type of gene.  
                Valid values are: 'int8', 'int16', 'int32','uint8', 'uint16', 'uint32'
            num_genes (int): Number of genes for each genome
            num_population (int): Number of individuals
            population_path (str, optional): Filepath to some previous population
            population (:obj:`list` of :obj:`Genome`): List containing the current generation of genomes
            current_generation (int): Number of current population generation

    """
    def __init__(self, ai, gene_path, gene_type, num_genes, num_population, deapParameters, seedNumber, population_path = None):
        """ Create new ga spawner

            Args:
                ai (str): Filepath to the AI
                gene_path (str): Relative path to directory where gene files will be placed
                gene_type (str): Type of gene.  
                    Valid values are: 'int8', 'int16', 'int32','uint8', 'uint16', 'uint32'
                num_genes (int): Number of genes for each genome
                num_population (int): Number of individuals
                population_path (str, optional): Filepath to some previous population

        """
        self.ai = ai
        self.seedNumber = seedNumber
        self.gene_path = gene_path
        self.gene_type = gene_type
        self.num_genes = num_genes
        self.num_population = num_population
        self.current_generation = 0
        self.fittestPercentageForBreeding = deapParameters["fittest_percentage_for_breeding"]
        self.INDPB = deapParameters["gene_mutation_propability_in_breeding"]
        self.MUTPB = deapParameters["individual_mutation_propability_in_breeding"]
        self.CXPB = deapParameters["cross_mutation_propability_in_breeding"]
        self.eta = deapParameters["eta_Crowding_degree_of_the_mutation"]

        #DEAP
        # Helper function for evaluation in breed_new_generation
        #NOTE: Currently not in use. Individuals fitness directly set from fitness value given in genome_rewards.
        def evalOneMax(score ):
            # sum = 0
            # for i in individual:
            #     if i == 1:
            #         sum = sum + 1
            # return sum,
            return score,

        valid_types = ['int8', 'int16', 'int32','uint8', 'uint16', 'uint32' ]
        if( gene_type not in valid_types ):
            print('Error! "%s" is not a gene type.' % gene_type)
            print("Valid values are \"int8\", \"int16\", \"int32\", \"uint8\", \"uint16\", \"uint32\"")
            raise ValueError

        if(not isinstance(num_genes, int) or num_genes <= 0):
            print('Error! Number of genes must be a positive integer.')
            raise ValueError

        # define seed number for random generator
        random.seed(self.seedNumber)
        np.random.seed(seedNumber)

        self.iinfo = np.iinfo(np.dtype(gene_type))
        #DEAP
        #HINT: To get a better understanding on DEAP, turn to its project page:
        #https://deap.readthedocs.io/en/master/index.html
        creator.create("FitnessMax", base.Fitness, weights=(1.0,))
        creator.create("Individual", list, fitness=creator.FitnessMax)
        self.toolbox = base.Toolbox()
        # Attribute generator 
        self.toolbox.register("attr_bool", random.randint, self.iinfo.min, self.iinfo.max)
        # Structure initializers
        self.toolbox.register("individual", tools.initRepeat, creator.Individual, self.toolbox.attr_bool, num_genes)
        # define the population to be a list of individuals
        self.toolbox.register("population", tools.initRepeat, list, self.toolbox.individual)
        # register the goal / fitness function
        self.toolbox.register("evaluate", evalOneMax)
        # register the crossover operator
        self.toolbox.register("mate", tools.cxTwoPoint)
        # register a mutation operator 
        #self.toolbox.register("mutate", tools.mutUniformInt, low=self.iinfo.min, up=self.iinfo.max, indpb=self.INDPB)
        self.toolbox.register("mutate", tools.mutPolynomialBounded, eta=self.eta, low=self.iinfo.min, up=self.iinfo.max, indpb=self.INDPB)
        # operator for selecting individuals for breeding the next generation
        # self.toolbox.register("select", tools.selTournament, tournsize=2)
        self.toolbox.register("select", tools.selBest)
        

        if(population_path != None):
            # Read existing population from csv
            self.read_population_csv(population_path)
            print("Read previous population from %s" % population_path)
        else:
            # Generate new random population
            self.population = GASpawner.generate_random_population(self.gene_type, self.num_genes, self.num_population, self.seedNumber)

        # initialize summary
        Path(self.gene_path).mkdir(parents=True)
        sum_path =Path(self.gene_path).joinpath("summary.csv") 
        print(sum_path)
        try:
            with open(sum_path, 'w', newline='\n') as csvfile:
                writer = csv.writer(csvfile)
                writer.writerow(["generation","pop_size","reward_min", "reward_max", "reward_avg", "reward_std", "average_dev"])
        except Exception as e:
            print(e)
            raise
        self.instances = {}

    # def spawn_ai_instances(self):
    #     for i in range(self.num_instances):
    #         self.instances.append(self.spawn_ai(self.ai, "GENE", self.base_port + i))
    @staticmethod
    def generate_random_genes( gene_type, num_genes, seedNumber):
        """ Creates a array of random gene values

            Args:
                gene_type (str): Data type of created genes.
                    Valid values are: 'int8', 'int16', 'int32','uint8', 'uint16', 'uint32'
                num_genes (int): Number of genes created

            Returns:
                numpy.ndarray: Array of randomized gene values

            Raises: 
                ValueError: Raised if invalid gene type or number of genes is given.

        """
        valid_types = ['int8', 'int16', 'int32','uint8', 'uint16', 'uint32' ]
        if( gene_type not in valid_types ):
            print('Error! "%s" is not a gene type.' % gene_type)
            print("Valid values are \"int8\", \"int16\", \"int32\", \"uint8\", \"uint16\", \"uint32\"")
            raise ValueError

        if(not isinstance(num_genes, int) or num_genes <= 0):
            print('Error! Number of genes must be a positive integer.')
            raise ValueError

        iinfo = np.iinfo(np.dtype(gene_type))
        genes = np.random.randint(iinfo.min, iinfo.max, num_genes, gene_type)
        return genes

    def breed_new_generation(self, genome_rewards):
        """
            Breed and mutate new population generation and replace the current generation

            Args:
                genome_rewards: List of tuples where each items contains a genome and 
                    the total reward gathered by the genome.
        """
        # initialize new population as an empty list
        pop = []
        ## Rank the genes
        for ind, fit in genome_rewards:
            #NOTE: ind has to be a list of integers
            ind = creator.Individual(ind)
            ind.fitness.values = [fit]
            # print(ind)
            pop.append(ind)
        # Extracting all the fitnesses of
        fits = [ind.fitness.values[0] for ind in pop]
        ## Pick pairs for breeding. Number of pairs = self.num_population

        #calculate the number of parents to be used for breeding
        #percentage is defined in config file
        populationSize = len(genome_rewards)
        numberOfParents = int(self.fittestPercentageForBreeding * populationSize / 100)
        # There must be at least two parents for breeding
        if numberOfParents < 2:
            numberOfParents = 2
        print("Using %s parents for breeding" %numberOfParents)

        parents = self.toolbox.select(pop, numberOfParents)
        
        # start breeding. 
        i = 0
        while i < populationSize:
            # select random parents that will produce offspring
            # NOTE! with random selection it is possible that some selected parent does not
            # does not breed new offspring. This could be changed so that every selected 
            # parent will produce offspring. It is also possible that parent1 = parent2 and thus
            # there will be 2 childs with same genes as their parent
            parent1 = parents[ random.randint(0, numberOfParents - 1) ]
            parent2 = parents[ random.randint(0, numberOfParents - 1) ]
            # produce offspring. mate return 2 childs
            childs = self.toolbox.mate(parent1, parent2)
            # add children to population
            pop[i] = childs[0]
            i = i + 1
            # we must check that the population size is not exceeded
            if i < populationSize:
                pop[i] = childs[1]
                i = i + 1

        # mutation loop for every individual
        for mutant in pop:
            # mutate an individual with probability MUTPB
            if random.random() < self.MUTPB:
                self.toolbox.mutate(mutant)
                del mutant.fitness.values
        ind_no = 1
        population = []
        for individual in pop:
            genes = np.array(individual, dtype = self.gene_type)
            if(len(genes) != self.num_genes):
                print("Error while creating new population on individual no.%d. Number of genes in individual differed from number of genes in the config file." % ind_no)
                print("Expected number of genes: %d" % self.num_genes)
                print("Number of genes in individual %d: %d" % (ind_no, len(genes)))
                raise ValueError
            genome = Genome(self.gene_type, genes)
            population.append(genome)
            ind_no += 1
            
        ## Replace current population
        self.population = population
        self.current_generation += 1
        self.write_population()
    

    @staticmethod
    def generate_random_population(gene_type, num_genes, pop_size, seedNumber):
        """ Generate a random population of Genome objects

            Args: 
                gene_type (str): Type of gene.
                    Valid values are: 'int8', 'int16', 'int32','uint8', 'uint16', 'uint32'
                num_genes (int): Number of genes for individual genome
                pop_size (int): Number of genomes to create

            Returns:
                :obj:`list` of :obj:`Genome`: List of created genomes

            Raises:
                ValueError: Raised if population size is not valid
        """
        if(not isinstance(pop_size, int) or pop_size <= 0):
            print('Error! Population size must be a positive integer.')
            raise ValueError
        genomes = []
        for i in range(pop_size):
            genes = GASpawner.generate_random_genes(gene_type, num_genes, seedNumber)
            genome = Genome(gene_type, genes)
            genomes.append(genome)
        return genomes

    def read_population_csv(self, population_path):
        """ Read existing population from a csv file.

            Args:
                population_path (str): Filepath to population csv file
                gene_type (str): Type of genes

            Raises:
                ValueError: Raised if number of genes or individuals differs
                    from the expected. 

        """
        population = []
        with open(population_path, newline='\n') as csvfile:
            rows = csv.reader(csvfile)
            #going through population csv row by row (one row equals the genome of single individual)
            row_num = 1
            for row in rows:
                try:
                    genes = np.array(row, dtype = self.gene_type)
                except:
                    print("Error while reading row %d from %s. The row contained invalid data." %(row_num, population_path))
                    raise
                if(len(genes) != self.num_genes):
                    print("Error while reading row %d from population csv. Number of genes in population file differed from number of genes in the config file." % row_num)
                    print("Expected number of genes: %d" % self.num_genes)
                    print("Number of genes on row %d: %d" % (row_num, len(genes)))
                    raise ValueError
                genome = Genome(self.gene_type, genes)
                population.append(genome)
                row_num += 1

        if(len(population) != self.num_population):
            print("Error while reading from population csv. Number of individuals in population file differed from number of individuals in the config file.")
            print("Expected number of individuals: %d" % self.num_population)
            print("Number of individuals in population file: %d" % (len(population)))
            raise ValueError

        self.population = population

    def write_population(self):
        """ Writes current population to gene path

            Returns:
                str: Generation path
        """
        generation_path = Path(self.gene_path).joinpath("Generation_%d" % self.current_generation )
        generation_path.mkdir(parents=True)

        # Write population csv
        population_path = Path(generation_path).joinpath('Generation_%d.csv' % self.current_generation)
        with open(population_path, 'w', newline='\n') as csvfile:
            writer = csv.writer(csvfile)
            for genome in self.population:
                writer.writerow(genome.genes.tolist())

        # Write gene files
        gene_id = 0
        for genome in self.population:
            gene_path =  Path(generation_path).joinpath('GENE_%d.gene' % gene_id)
            genome.ToBytes(gene_path)                
            gene_id += 1

        return generation_path
    
    

    def write_report(self, reward_list):
        try:
            generation = self.current_generation
            pop_size = self.num_population
            genes, rewards = zip(*reward_list)

            reward_min = min(rewards)
            reward_max = max(rewards)
            reward_avg = np.average(rewards)
            reward_std = np.std(rewards)

            mean_genome = np.average(np.array(genes), axis=0)
            # diff = np.average(np.sqrt(np.sum(np.square(np.array(genes) - mean_genome), axis=1)))
            # diff_squared = np.square(diff)
            # diff_ss = diff_squared.sum()
            # diff_sqrt_ss = np.sqrt(diff_ss)
            average_dev = np.average(np.sqrt(np.sum(np.square(np.array(genes) - mean_genome), axis=1)))
            
            with open(Path(self.gene_path).joinpath("summary.csv"), 'a', newline='\n') as csvfile:
                writer = csv.writer(csvfile)
                writer.writerow([generation,pop_size,reward_min, reward_max, reward_avg, reward_std, average_dev])
        except Exception as e:
            print(e)
            raise
                

if __name__ == "__main__":
    """ GA-Demo: 
        GA will try to minimize the sum gene values inside it's population. 
    """
    seedNumber = 1

    deapParameters = {'fittest_percentage_for_breeding': 40, \
                        'gene_mutation_propability_in_breeding': 0.1, \
                        'individual_mutation_propability_in_breeding': 0.5, \
                        'cross_mutation_propability_in_breeding': 0.5, \
                        'eta_Crowding_degree_of_the_mutation': 100}
    
    gene_path= "./tests/Genes2/"
    if(Path(gene_path).exists()):
        shutil.rmtree(gene_path)
    ga = GASpawner("hoo",gene_path,"uint16",25,1000, deapParameters, seedNumber)
    from msvcrt import getch
    print("GA-DEMO")
    print("Initial population stats:")
    print("Pop size: %d" %len(ga.population))
    print("Gene type: %s" % ga.gene_type)
    print("Number of genes: %d" %ga.num_genes)
    print("Min sum: %d" % min([sum(genome.genes) for genome in ga.population]))
    print("Max sum: %d" % max([sum(genome.genes) for genome in ga.population]))
    print("Avg sum: %.2f" % (np.average(np.array([ sum(pop.genes) for pop in ga.population]))))
    print("STD sum: %.2f" % (np.std(np.array([ sum(pop.genes) for pop in ga.population]))))
    print()
    print("Press any key to start...")
    getch()

    for i in range (200):
        # Rank population based on sum of the gene values. Lowest sum gets the highest score
        sorted_pop = sorted(ga.population, key=lambda genome: sum(genome.genes))

        print("Generation %d:" % i )
        print("Min sum: %d" % sum(sorted_pop[0].genes))
        print("Max sum: %d" % sum(sorted_pop[-1].genes))
        print("Avg sum: %.2f" % (np.average(np.array([ sum(pop.genes) for pop in sorted_pop]))))
        print("STD sum: %.2f" % (np.std(np.array([ sum(pop.genes) for pop in sorted_pop]))))
        print()

        genome_rewards_list = []
        for j in range( len(sorted_pop)):
            genome_rewards_list.append((sorted_pop[j].genes, len(sorted_pop) - j))
        print("Population Size %s" %len(genome_rewards_list))
        ga.breed_new_generation(genome_rewards_list)
    
