
from re import S
import sys
import pyvisa

rm = pyvisa.ResourceManager()
#from channelClass import  channelPrint

supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply

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



"""
command = sys.argv[1]
channelNo = sys.argv[2]
value = sys.argv[3]
"""
command = "on"
channelNo = "1"
value = ""
channel = channelInstrument(channelNo)



if command == 'on':
    channel.turnOn()
elif command == 'off':
    channel.turnOff()
elif command == 'setVoltage':
    channel.setVoltage(value)
elif command == 'setAmps':
    channel.setAmps(value)