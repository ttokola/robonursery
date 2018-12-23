import sys
import numpy as np
import os

class Genome:
    """ Container object for genetic algorithm genome.

        Attributes:
            genes (:obj:`numpy.ndarray`): List of all gene values
            type (str): Type of the genes contained in genome.
                Possible values are 'int8', 'int16', 'int32'
            path (str): File path of the gene file, if it has been created, None otherwise.

    """
    def __init__(self, gene_type, genes):
        """ Initialize a new genome with a given set of genes.

            Args:
                genes: List of gene values
                gene_type (str): Type of the genes in the genome.
                    Possible values are \"int8\", \"int16\", \"int32\", \"uint8\", \"uint16\", \"uint32\"
        """
        valid_types = ['int8', 'int16', 'int32','uint8', 'uint16', 'uint32' ]
        if( gene_type not in valid_types ):
            print('Error! "%s" is not a valid type for genome.' % gene_type)
            print("Valid values are \"int8\", \"int16\", \"int32\", \"uint8\", \"uint16\", \"uint32\"")
            raise ValueError
        self.gene_type = gene_type
        self.path = None # Initialize gene file path to None

        iinfo = np.iinfo(np.dtype(gene_type))
        if(max(genes) > iinfo.max):
            print('Error! Given gene array contains values too large for given gene type \'%s\'' % gene_type)
            raise ValueError
        if(min(genes) < iinfo.min):
            print('Error! Given gene array contains values too small for given gene type \'%s\'' % gene_type)
            raise ValueError

        self.genes = np.array(genes, dtype=gene_type)

    def ToBytes(self, path):
        """ Write genome content to to a bytes file at given path

            Args:
                path (str): Full filepath to gene file
        """
        gene_bytes = self.genes.tobytes()
        try:
            gene_file = open(path, mode='xb')
            gene_file.write(gene_bytes)
            self.path = os.path.abspath(path) # Save gene file path
        except Exception as err:
            print("Exception: %s" %str(err))
            print("Error in generating the population binary files.")
            exit(1)
        finally:
            gene_file.close()

    @staticmethod
    def FromBytes(gene_path, gene_type = 'int8'):
        """ Read gene data from bytes file and returns a new Genome object

            Args:
                gene_path (str): File path of the gene bytes file
                type (str): Type of the gene Valid values are \"int8\", \"int16\", \"int32\", \"uint8\", \"uint16\", \"uint32\"

            Returns:
                Genome: A new Genome object with genes extracted from the bytes file
        """
        valid_types = ['int8', 'int16', 'int32','uint8', 'uint16', 'uint32' ]
        if gene_type not in valid_types:
            print('Error! Gene type "%s" not supported.' % gene_type)
            raise ValueError

        genes = []
        with open(gene_path, mode='rb') as gene_file:
            gene_bytes = gene_file.read()
            genes = np.frombuffer(gene_bytes, dtype=gene_type)
            # while gene_bytes:
            #     genes.append(int.from_bytes(gene_bytes, sys.byteorder))
            #     gene_bytes = gene_file.read(gene_length)
        genome = Genome(gene_type, genes)
        return genome

    def Breed(self, other):
        """ NOT IMPLEMENTED
            Breed this genome with another Genome object to produce a new Genome

            Args:
                other (Genome): Other Genome object used to produce the offspring

            Returns:
                Genome: New Genome object, inheriting from both parent Genomes.

        """

        if (other.gene_type != self.gene_type):
            print("Error! Can't genome of type %s can't breed with genome of type %s" %(self.gene_type, other.gene_type) )
            raise ValueError

        return NotImplemented

    def Mutate(self, num_mutations = 1):
        """ NOT IMPLEMENTED
            Mutates a gene belonging to this genome.

            Args:
                num_mutations (int, optional): Number or genes that will be
                    affected by the mutation. Default value = 1. 
        """
        return NotImplemented

