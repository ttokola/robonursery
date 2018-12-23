import unittest
# Example test module. 
# Asserts: https://docs.python.org/3/library/unittest.html#assert-methods
# Run with 
class ExampleTest(unittest.TestCase):
    # Example test 
    def test_example1(self):
        self.assertEqual("Moi", "Moi")

    # Example test 2
    def test_example2(self):
        self.assertFalse( 1 == 2)

    def test_example3(self):
        vals = ['a','b', 'c']
        self.assertIn('a', vals)
