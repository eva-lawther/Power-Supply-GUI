
import imp
from re import S
import sys
import time
import pyvisa
import json
import matplotlib.pyplot as plt
import numpy as np
import sed_json as fm #file manager ### C# Interaction
import sweeping
import configuration



program_out_script_in_json_buffer = []
program_out_script_in_json_buffer = fm.import_json(".data.json")
fileCall = program_out_script_in_json_buffer[0]




if (fileCall.Id == "sweeping.py"):
    sweeping.main() #what does it need
    
elif(fileCall.Id == "configuration.py"):
    configuration.main()
    
#elif(fileCall.Id == )
    




