import os 
import os.path
import json
import unittest
import pprint



class TestMyFunctions(unittest.TestCase):
    def test_read_file_data(self):
        dir_path = os.path.dirname(os.path.realpath(__file__))
        json_data=open(dir_path+"/config.json").read()

        # sample_json = config.json that will be used for testing
        sample_json = json.loads(json_data)
        parsed_json = sample_json['scenes']
        for scene in parsed_json:
            sceneid = scene['id']
            #Some one fix this scenepath.....
            scenepath = "/TestBuild/"
            print (dir_path + scenepath + sceneid)
            if os.path.exists(dir_path + scenepath + sceneid): {
            }
            else: {
                print ("\n"+ dir_path + scenepath + sceneid +" Does not exist")
                }

        # lol2 = lol['id']
        # print (lol2)
        

        # with open('config.json') as json_data:
        #     d = json.load(json_data['ai_base_port'])
        #     print(d)
        # sample_json = json.dumps(sample_json, ensure_ascii=False)
        # path = dir_path

        filename = "config.json"
        
        # self.assertEqual(read_file_data(filename, path), sample_json)

    # def config_exists():
        
    #     try:
    #         dir_path = os.path.dirname(os.path.realpath(__file__))
    #         print(dir_path)
    #         os.path.exists(dir_path +"/'Config.json")

            

    #     except FileNotFoundError:
    #         print("The config.json file does not exists")
    #     else:
    #         print("Config.json file does exist")
if __name__ == "__main__":
    unittest.main()


