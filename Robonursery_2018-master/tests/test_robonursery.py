import unittest
from robonursery import Robonursery
class TestRobonursery(unittest.TestCase):
    
    def test_robonursery_read_config(self):
        """
        Read config file from root folder. (Positive test case)
        """
        robonursery =  Robonursery()
        robonursery.read_config("./config.json")
        self.assertIsNotNone(robonursery.config)

    def test_robonursery_read_config_file_missing(self):
        """
        Read config file from root folder. (Positive test case)
        """
        robonursery =  Robonursery()
        with self.assertRaises(FileNotFoundError):
            robonursery.read_config("./ADQWEAD")
            
    def test_robonursery_read_config_value_out_of_range(self):
        """
        Read erroneous config file (value out of range). (Positive test case)
        """
        robonursery =  Robonursery()
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_1.json")
            robonursery.validate_config("./tests/config_out_of_range_1.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_2.json")
            robonursery.validate_config("./tests/config_out_of_range_2.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_3.json")
            robonursery.validate_config("./tests/config_out_of_range_3.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_4.json")
            robonursery.validate_config("./tests/config_out_of_range_4.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_5.json")
            robonursery.validate_config("./tests/config_out_of_range_5.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_6.json")
            robonursery.validate_config("./tests/config_out_of_range_6.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_7.json")
            robonursery.validate_config("./tests/config_out_of_range_7.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_8.json")
            robonursery.validate_config("./tests/config_out_of_range_8.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_9.json")
            robonursery.validate_config("./tests/config_out_of_range_9.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_10.json")
            robonursery.validate_config("./tests/config_out_of_range_10.json")
        with self.assertRaises(TypeError):
            robonursery.read_config("./tests/config_out_of_range_13.json")
            robonursery.validate_config("./tests/config_out_of_range_13.json")
        with self.assertRaises(TypeError):
            robonursery.scene_list = []
            robonursery.read_config("./tests/config_out_of_range_11.json")
            robonursery.validate_config("./tests/config_out_of_range_11.json")
        with self.assertRaises(TypeError):
            robonursery.scene_list = []
            robonursery.read_config("./tests/config_out_of_range_12.json")
            robonursery.validate_config("./tests/config_out_of_range_12.json")

    def test_robonursery_read_config_key_error(self):
        """
        Read erroneous config file (parameter missing or mistyped). (Positive test case)
        """
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_1.json")
            robonursery.validate_config("./tests/config_key_error_1.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_2.json")
            robonursery.validate_config("./tests/config_key_error_2.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_3.json")
            robonursery.validate_config("./tests/config_key_error_3.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_4.json")
            robonursery.validate_config("./tests/config_key_error_4.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_5.json")
            robonursery.validate_config("./tests/config_key_error_5.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_6.json")
            robonursery.validate_config("./tests/config_key_error_6.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_7.json")
            robonursery.validate_config("./tests/config_key_error_7.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_8.json")
            robonursery.validate_config("./tests/config_key_error_8.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_9.json")
            robonursery.validate_config("./tests/config_key_error_9.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_10.json")
            robonursery.validate_config("./tests/config_key_error_10.json")
        robonursery =  Robonursery()
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_11.json")
            robonursery.validate_config("./tests/config_key_error_11.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_12.json")
            robonursery.validate_config("./tests/config_key_error_12.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_13.json")
            robonursery.validate_config("./tests/config_key_error_13.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_14.json")
            robonursery.validate_config("./tests/config_key_error_14.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_15.json")
            robonursery.validate_config("./tests/config_key_error_15.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_16.json")
            robonursery.validate_config("./tests/config_key_error_16.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_17.json")
            robonursery.validate_config("./tests/config_key_error_17.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_18.json")
            robonursery.validate_config("./tests/config_key_error_18.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_19.json")
            robonursery.validate_config("./tests/config_key_error_19.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_20.json")
            robonursery.validate_config("./tests/config_key_error_20.json")
        with self.assertRaises(KeyError):
            robonursery.read_config("./tests/config_key_error_21.json")
            robonursery.validate_config("./tests/config_key_error_21.json")
            