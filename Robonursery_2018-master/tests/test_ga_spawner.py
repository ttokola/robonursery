import datetime
from pathlib import Path
from os import listdir
from os.path import isfile, join
import unittest
import sys
import shutil
import ga_spawner as ga
import random 

class TestGaSpawner(unittest.TestCase):
    
    def test_generate_random_genes_uint8(self):
        gene_type = 'uint8'
        num_genes = random.randint(0, 100) 
        genes = ga.GASpawner.generate_random_genes(gene_type,num_genes)
        self.assertEqual(num_genes, len(genes))

    def test_generate_random_genes_int8(self):
        gene_type = 'int8'
        num_genes = random.randint(0, 100) 
        genes = ga.GASpawner.generate_random_genes(gene_type,num_genes)
        self.assertEqual(num_genes, len(genes))

    def test_generate_random_genes_invalid_gene_type(self):
        gene_type = 'ant8'
        num_genes = random.randint(0, 100)
        with self.assertRaises(ValueError):
            genes = ga.GASpawner.generate_random_genes(gene_type,num_genes)

    def test_generate_random_genes_invalid_num_genes(self):
        gene_type = 'int8'
        num_genes = -1
        with self.assertRaises(ValueError):
            genes = ga.GASpawner.generate_random_genes(gene_type,num_genes)
        num_genes = 'asd'
        with self.assertRaises(ValueError):
            genes = ga.GASpawner.generate_random_genes(gene_type,num_genes)

    def test_generate_random_population(self):
        gene_type = 'int8'
        num_genes = 10
        pop_size = 10
        genomes =  ga.GASpawner.generate_random_population(gene_type, num_genes, pop_size)
        self.assertEqual(pop_size, len(genomes))
        self.assertEqual(gene_type, genomes[0].gene_type)
        self.assertEqual(num_genes, len(genomes[0].genes))

    def test_read_population_csv(self):
        gene_type = 'uint8'
        filepath = './tests/ais.csv' # Test file contains 300 individuals and 100 genes per individual
        ga_spawner = ga.GASpawner('ai', './genes', gene_type, 100,  300)
        ga_spawner.read_population_csv(filepath)
        self.assertEqual(300,len( ga_spawner.population))
        self.assertEqual(100, len(ga_spawner.population[0].genes))

    def test_read_population_csv_invalid_num_genes(self):
        gene_type = 'uint8'
        filepath = './tests/ais_invalid_gene_num.csv' # Invalid 101 genes on row 299
        ga_spawner = ga.GASpawner('ai', './genes', gene_type, 100,  300)
        
        with self.assertRaises(ValueError):
            ga_spawner.read_population_csv(filepath)

    def test_read_population_csv_invalid_num_pop(self):
        gene_type = 'uint8'
        filepath = './tests/ais_invalid_pop_num.csv' # Invalid pop file contains only 299 individuals
        ga_spawner = ga.GASpawner('ai', './genes', gene_type, 100,  300)
        with self.assertRaises(ValueError):
            ga_spawner.read_population_csv(filepath)

    def test_write_population(self):
        gene_type = 'uint32'
        gene_base = './tests/TestGenes/'
        num_genes = 10
        num_pop = 10
        gene_path = Path(gene_base).joinpath( datetime.datetime.now().strftime("%Y-%m-%d_%H%M%S"))
        try:
            ga_spawner = ga.GASpawner('ai', gene_path, gene_type,num_genes, num_pop)
            generation_path = ga_spawner.write_population()
            files = [f for f in listdir(generation_path ) if isfile(join(generation_path , f))]
            print (files)
            self.assertTrue(Path(generation_path).exists())
            self.assertEqual(num_pop + 1, len(files))
        finally:
            # Clean up
            shutil.rmtree(gene_base)

if __name__ == '__main__':
    unittest.main()

        


