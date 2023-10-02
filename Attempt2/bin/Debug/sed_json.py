import numpy as np
import json

class JsonDataFormat:
       def __init__(self, id, length, values):
              self.Id     = id
              self.Length = length
              if type(values) == type(np.array(0)):
                     self.Values = values.tolist()
              else:
                     self.Values = values

SKIP = True

def export_json(file, list_of_jsons):
       # Attempt to open and read the file.
        if not SKIP:
            print("go away")
            '''
            try:
                  f = open(file, 'r+')
            except:
                  print("Can't open and read the export file!")
                  return
           # Read in the existing data from the file.
           try:
                   existing_data = json.load(f)
                    # Check through existing data and either 
                    # overwrite or append new elements.
                   for object_out in list_of_jsons:
                           i = -1
                           match = False
                           for existing_object in existing_data:
                               i = i + 1
                               if (object_out.Id == existing_object['Id']):
                                        existing_data[i] = object_out.__dict__
                                        match = True
                                        break
                           if match == False:
                                existing_data.append(object_out.__dict__)
                   data_out = existing_data
           except:
                   data_out = []
                   for object_out in list_of_jsons:
                        data_out.append(object_out.__dict__) 
            '''
        else:
            data_out = []
            for object_out in list_of_jsons:
                    data_out.append(object_out.__dict__)    
        # Write out the JSON file.
        json_object = json.dumps(data_out, indent=2)
        f.close()
       # Attempt to open and write over the file.
        try:
            f = open(file, 'w')
        except:
            print("Can't open and write the export file!")
            return
        f.write(json_object)
        f.close()

def import_json(file):
       # Attempt to open and read the file.
       try:
              f = open(file, 'r')
       except:
              print("Can't open and read the import file!")
              return False
       # Read in the existing data from the file.
       try:
              dummy = json.load(f)
              f.close()
              return parse_json_to_std_class(dummy)
       except Exception as e:
              print("No data imported!")
              print("Error: ", e)
              f.close()
              return False
       

def find_and_return_dict(id, json_buffer):
       for obj in json_buffer:
              if obj['Id'] == id:
                     return obj
       return False

def parse_json_to_std_class(json_dicts):
       list_out = []
       for json in json_dicts:
              dummy_obj = JsonDataFormat(0, 0, 0)
              dummy_obj.Id = json['Id']
              dummy_obj.Length = json['Length']
              dummy_obj.Values = json['Values']
              list_out.append(dummy_obj)
       return list_out

