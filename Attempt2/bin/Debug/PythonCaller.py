
from re import S
import sys
import time
import pyvisa
import json
import matplotlib.pyplot as plt
import numpy as np
import sed_json as fm #file manager ### C# Interaction

#exit()

rm = pyvisa.ResourceManager()
supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply (changes each time???)

program_out_script_in_json_buffer = []

### Instrument classes
class channelPrint:
    def __init__(self, channel):
        self._channel = channel
        
    def turnOn(self):
        print('OP{n} 1'.format(n = self._channel))  

    def turnOff(self):
        print('OP{n} 0'.format(n = self._channel))  
                 
    def setVoltage(self, v):
        print('V{n} {V}'.format(n = self._channel, V = str(v)))

    def setVoltageRange(self, r):
        print('VRANGE{n} {R}'.format(n = self._channel, R = str(r)))
        
    def setAmps(self, a):
        print('I{n} {A}'.format(n=self._channel, A=a))
                     
    def reset(self):
        self.setAmps(0)
        self.setVoltage(0) 
        
class channelInstrument:
    def __init__(self, channel):
        self._channel = channel
        
    def turnOn(self):
        supply.write('OP{n} 1'.format(n = self._channel))  

    def turnOff(self):
        supply.write('OP{n} 0'.format(n = self._channel))  
                 
    def setVoltage(self, v):
        supply.write('V{n} {V}'.format(n = self._channel, V = str(v)))

    def setVoltageRange(self, r):
        supply.write('VRANGE{n} {R}'.format(n = self._channel, R = str(r)))
        
    def setAmps(self, a):
        supply.write('I{n} {A}'.format(n=self._channel, A=a))
                     
    def reset(self):
        self.setAmps(0)
        self.setVoltage(0)
        
    
    """
    def setIncrement(t):
    
    def setTimer(t):
    """


program_out_script_in_json_buffer = fm.import_json(".data.json")



dummy = program_out_script_in_json_buffer[0]
command = dummy.Values[0]
channelNo = dummy.Values[1]
value = dummy.Values[2]
channel = channelInstrument(channelNo)

print(command)

if command == 1: 
    channel.turnOn()
elif command == 2: 
    channel.turnOff()
elif command == 3:
    channel.setVoltage(value)
elif command == 4:
    channel.setAmps(value)