#from pydoc import Doc
#from re import S
#from tkinter import Variable
import pyvisa
from time import sleep
from commandClass import channelInstrument 
import sed_json as fm #file manager ### C# Interaction


def devices():
    rm = pyvisa.ResourceManager()
    print("Resources detected\n{}\n".format(rm.list_resources()))
    supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply   
    #dmm = rm.open_resource('ASRL6::INSTR')    # resource id for digital multi meter
    # set digital multimeter to dc voltage mode
    #dmm.write('VDC')
    return (supply,  rm)



def getValues():
    program_out_script_in_json_buffer = fm.import_json(".data.json")

    fileCall = program_out_script_in_json_buffer[0]
    command = fileCall.Values[0]
    channelNo = fileCall.Values[1]
    value = fileCall.Values[2]

    return command, channelNo, value
       
def doCommand(channel, command, value):
    if command == 1: 
        channel.turnOn()
    elif command == 2: 
        channel.turnOff()
    elif command == 3:
        channel.setVoltage(value)
    elif command == 4:
        channel.setAmps(value)
         
        

def main():
    supply,  rm = devices()  
    
    command, channelNo, value = getValues()
    
    channel = channelInstrument(channelNo, supply) 
    doCommand(channel, command, value)
    
    rm.close()

 
    