from re import S
from tkinter import Variable
import pyvisa
from time import sleep
from commandClass import channelInstrument 
import sed_json as fm #file manager ### C# Interaction


def devices():
    rm = pyvisa.ResourceManager()
    print("Resources detected\n{}\n".format(rm.list_resources()))
    supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply   
    dmm = rm.open_resource('ASRL6::INSTR')    # resource id for digital multi meter
    # set digital multimeter to dc voltage mode
    dmm.write('VDC')
    return (supply, dmm, rm)


def variableVoltage(constant, start, end, increment, supply, dmm):
    set.setAmps(constant)
    set.setVoltage(start)
    set.turnOn()
    while start <= end:
        set.setVoltage(start)
        sleep(1)                                     # Wait for reading to settle
        vMeasured = float(dmm.query('READ?')[1:8])   # measure the voltage
        vExponent = str(dmm.query('READ?')[8:11])    # retrieve exponent
        sleep(1)
        # Write results to console
        print("{}  {}  {}".format(start, vMeasured, vExponent))

        start += increment 

def variableAmps(constant, start, end, increment, supply, dmm): # variable == 'Amplitude'
    set.setVoltage(constant)
    set.setAmps(start)
    set.turnOn()
    while start <= end:
        set.setAmps(start)
        sleep(1)                                     # Wait for reading to settle
        aMeasured = float(dmm.query('READ?')[1:8])   # measure the voltage
        aExponent = str(dmm.query('READ?')[8:11])    # retrieve exponent
        sleep(1)
        # Write results to console
        print("{}  {}  {}".format(start, aMeasured, aExponent))

        start += increment 

def getValues():
    program_out_script_in_json_buffer = fm.import_json(".data.json")

    dummy = program_out_script_in_json_buffer[0]
    channel = dummy.Values[0]
    variable = dummy.Values[1]
    direction = dummy.Values[2]
    start = dummy.Values[3]
    end = dummy.Values[4]
    increment = dummy.Values[5]
    constant = dummy.Values[6]

def main():
    supply, dmm, rm = devices()  
    
    channel, variable, direction, start, end, increment, constant = getValues()

    set = channelInstrument(channel, supply)
    set.turnOff() # turn off

    if variable == "Voltage":
        variableVoltage(constant, start, end, increment, supply, dmm)
    else:
        variableAmps(constant, start, end, increment,supply, dmm)   
    rm.close()
    set.end() 