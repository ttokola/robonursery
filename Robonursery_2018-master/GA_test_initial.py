#    This file is part of DEAP.
#
#    DEAP is free software: you can redistribute it and/or modify
#    it under the terms of the GNU Lesser General Public License as
#    published by the Free Software Foundation, either version 3 of
#    the License, or (at your option) any later version.
#
#    DEAP is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
#    GNU Lesser General Public License for more details.
#
#    You should have received a copy of the GNU Lesser General Public
#    License along with DEAP. If not, see <http://www.gnu.org/licenses/>.


#    example which rewards individuals when they have many genes that are the integer '1'
#    genes are integers in range of [1,15]

import random
import csv
import json

from pathlib import Path
from deap import base
from deap import creator
from deap import tools

creator.create("FitnessMax", base.Fitness, weights=(1.0,))
creator.create("Individual", list, fitness=creator.FitnessMax)

toolbox = base.Toolbox()

# Attribute generator 
#                      define 'attr_bool' to be an attribute ('gene')
#                      which corresponds to integers sampled uniformly
#                      from the range [1,15].
###  Temporary implementation. Actual attributes TDL
toolbox.register("attr_bool", random.randint, 1, 15)

# Structure initializers
#                         define 'individual' to be an individual
#                         consisting of 100 'attr_bool' elements ('genes')
### number of attributes TDL
toolbox.register("individual", tools.initRepeat, creator.Individual, 
    toolbox.attr_bool, 100)

# define the population to be a list of individuals
toolbox.register("population", tools.initRepeat, list, toolbox.individual)

# the goal ('fitness') function to be maximized
### actual fitness funcion could be for example the sum of rewards from all scenes. TDL
def evalOneMax(individual ):
    sum = 0
    for i in individual:
        if i == 1:
            sum = sum + 1
    return sum


def map_int(string):
    return int(string)

#----------
# Operator registration
#----------
# register the goal / fitness function
toolbox.register("evaluate", evalOneMax)

# register the crossover operator
toolbox.register("mate", tools.cxTwoPoint)

# register a mutation operator with a probability to
# flip each attribute/gene of 0.05
toolbox.register("mutate", tools.mutUniformInt, low=1, up=15, indpb=0.05)

# operator for selecting individuals for breeding the next
# generation: each individual of the current generation
# is replaced by the 'fittest' (best) of three individuals
# drawn randomly from the current generation.
toolbox.register("select", tools.selTournament, tournsize=2)

#----------

def main():
    with open('rewards_by_individual.json', 'r') as jsonFile:
        rewards_by_ind = json.load(jsonFile)
    print(rewards_by_ind)
    for individual in rewards_by_ind:
        print(json.dumps(rewards_by_ind[individual]))
        for reward in rewards_by_ind[individual]:
            print(reward)
            print(rewards_by_ind[individual][reward])

    #Load ai gene file into program, if it exists
    my_file = Path("ais.csv")
    if my_file.is_file():
        # print("\n\nyup\n\n")
        f = open("ais.csv","r")
        pop = []
        with open("ais.csv", newline='\n') as csvfile:
            # ind = []
            row = csv.reader(csvfile, delimiter=' ', quotechar='|')
            for individual_in in row:
                individual = creator.Individual(list(map(map_int,individual_in[0].split(","))))
                pop.append(individual)
        #
        # TBDL
        #
    else:
        random.seed(64)
        pop = toolbox.population(n=300)
        # create an initial population of 300 individuals (where
        # each individual is a list of integers)
    

    # CXPB  is the probability with which two individuals
    #       are crossed
    #
    # MUTPB is the probability for mutating an individual
    CXPB, MUTPB = 0.5, 0.2
    
    # print("Start of evolution")
    
    # Evaluate the entire population
    ### This initial evaluation needs to be either removed or replaced with a uniform evaluation.
    fitnesses = list(map(toolbox.evaluate, pop))
    for ind, fit in zip(pop, fitnesses):
        ind.fitness.values = fit
    
    # print("  Evaluated %i individuals" % len(pop))

    # Extracting all the fitnesses of 
    fits = [ind.fitness.values[0] for ind in pop]

    # Variable keeping track of the number of generations
    g = 0
    
    # Begin the evolution
    ### While condition most likely constricted to number of generations. I.e. "g < 30", or something
    while max(fits) < 100 and g < 200:
        # A new generation
        g = g + 1
        print("-- Generation %i --" % g)
        
        # Select the next generation individuals
        offspring = toolbox.select(pop, len(pop))
        # Clone the selected individuals
        offspring = list(map(toolbox.clone, offspring))
    
        # Apply crossover and mutation on the offspring
        for child1, child2 in zip(offspring[::2], offspring[1::2]):

            # cross two individuals with probability CXPB
            if random.random() < CXPB:
                toolbox.mate(child1, child2)

                # fitness values of the children
                # must be recalculated later
                del child1.fitness.values
                del child2.fitness.values

        for mutant in offspring:
            print(mutant)
            # mutate an individual with probability MUTPB
            if random.random() < MUTPB:
                toolbox.mutate(mutant)
                del mutant.fitness.values

        """
        Somewhere here a part where genes are made into actual AI's. Most likely done in a different module.
        This module waits while AI's are evaluated in Unity side. After that This module does own evaluation of AI's.
        """
    
        # Evaluate the individuals with an invalid fitness
        invalid_ind = [ind for ind in offspring if not ind.fitness.valid]
        fitnesses = map(toolbox.evaluate, invalid_ind)
        for ind, fit in zip(invalid_ind, fitnesses):
            ind.fitness.values = fit
        
        print("  Evaluated %i individuals" % len(invalid_ind))
        
        # The population is entirely replaced by the offspring
        pop[:] = offspring
        
        # Gather all the fitnesses in one list and print the stats
        fits = [ind.fitness.values[0] for ind in pop]
        
        length = len(pop)
        mean = sum(fits) / length
        sum2 = sum(x*x for x in fits)
        std = abs(sum2 / length - mean**2)**0.5
        
        print("  Min %s" % min(fits))
        print("  Max %s" % max(fits))
        print("  Avg %s" % mean)
        print("  Std %s" % std)
    
    print("-- End of (successful) evolution --")
    
    best_ind = tools.selBest(pop, 1)[0]
    print(best_ind)
    f = open("ais.csv","w")
    for individual_out in pop:
        # print(individual)
        # print("")
        i = 0
        for gene_out in individual_out:
            i += 1
            if i == len(individual_out):
                f.write(str(gene_out))
            else:
                f.write(str(gene_out)+",")
        f.write("\n")
    # print("Best individual is %s, %s" % (best_ind, best_ind.fitness.values))

if __name__ == "__main__":
    main()