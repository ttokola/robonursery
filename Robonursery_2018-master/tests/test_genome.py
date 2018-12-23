import genome as g
import unittest
import os
import random
import numpy as np
class GenomeTest(unittest.TestCase):

    def test_init_valid(self):
        genes = np.array([1,2,3,4,5,6,7,9,10])
        gene_type = 'int8'
        genome = g.Genome(gene_type, genes)
        self.assertListEqual(genes.tolist() , genome.genes.tolist())
        self.assertEqual(gene_type, genome.gene_type)
        
    def test_init_invalid_genes_too_large(self):
        genes = [1,2,9999]
        gene_type = 'int8'
        with(self.assertRaises(ValueError)):
            genome = g.Genome(gene_type, genes)

    def test_init_invalid_genes_too_small(self):
        genes = [1,2,-9999]  
        gene_type = 'int8'
        with(self.assertRaises(ValueError)):
            genome = g.Genome(gene_type, genes)
    
    def test_init_invalid_type(self):
        genes = [1,2,3,4,5,6,7,9,10]
        gene_type = 'ASD'
        with self.assertRaises(ValueError):
            g.Genome(gene_type, genes)

    def test_to_bytes_int8(self):
        genes = [1,2,3,4,5,6,7,9,10]
        gene_type = 'int8'
        file_path = './GENE%d.gene' % random.randint(1000,9999)
        genome = g.Genome(gene_type, genes)
        self.assertFalse(os.path.exists(file_path))
        genome.ToBytes(file_path)
        self.assertTrue(os.path.exists(file_path))
        os.remove(file_path)

    def test_to_bytes_int16(self):
        genes = np.uint16([1,2,3,4,5,6,7,9,10])
        gene_type = 'int16'
        file_path = './GENE%d.gene' % random.randint(1000,9999)
        genome = g.Genome(gene_type, genes)
        self.assertFalse(os.path.exists(file_path))
        genome.ToBytes(file_path)
        self.assertTrue(os.path.exists(file_path))
        os.remove(file_path)


    def test_from_bytes_invalid_type(self):
        with self.assertRaises(ValueError):
            g.Genome.FromBytes('ASD', 'ASD')
        
    def test_from_bytes_int8(self):
        file_path = './GENE%d.gene' % random.randint(1000,9999)
        try:
            genes = np.uint8([1,2,3,4,5,6,7,9,10])
            gene_type = 'int8'
            genome = g.Genome(gene_type, genes)
            genome.ToBytes(file_path)
            new_genome = g.Genome.FromBytes(file_path, 'int8')
            self.assertListEqual(genes.tolist(), new_genome.genes.tolist())
            self.assertEqual(gene_type, new_genome.gene_type)
        finally:
            os.remove(file_path)

    def test_from_bytes_int16(self):
        file_path = './GENE%d.gene' % random.randint(1000,9999)
        try:
            genes = np.uint16([1,2,3,4,5,6,7,9,10])
            gene_type = 'int16'
            genome = g.Genome(gene_type, genes)
            genome.ToBytes(file_path)
            new_genome = g.Genome.FromBytes(file_path, 'int16')
            self.assertListEqual(genes.tolist(), new_genome.genes.tolist())
            self.assertEqual(gene_type, new_genome.gene_type)
        finally:
            os.remove(file_path)

    def test_from_bytes_int32(self):
        file_path = './GENE%d.gene' % random.randint(1000,9999)
        try:
            genes = np.uint32([1,2,3,4,5,6,7,9,10])
            print(genes.tobytes())
            gene_type = 'int32'
            genome = g.Genome(gene_type, genes)
            genome.ToBytes(file_path)
            new_genome = g.Genome.FromBytes(file_path, 'int32')
            self.assertListEqual(genes.tolist(), new_genome.genes.tolist())
            self.assertEqual(gene_type, new_genome.gene_type)
        finally:
            os.remove(file_path)

    


