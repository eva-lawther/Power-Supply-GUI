from msilib import Directory
from re import S
from tkinter import Variable
import pyvisa
from time import sleep
from commandClass import channelInstrument 
import sed_json as fm #file manager ### C# Interaction
import csv
import os
import time

def devices():
    rm = pyvisa.ResourceManager()
    #print("Resources detected\n{}\n".format(rm.list_resources()))
    supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply   
    dmm = rm.open_resource('ASRL6::INSTR')    # resource id for digital multi meter
    # set digital multimeter to dc voltage 
    return (supply, dmm, rm)

def doVoltageReadings(dmm, start, set, rows):
    set.setVoltage(start)
    sleep(2)                                     # Wait for reading to settle
    vMeasured = float(dmm.query('READ?')[1:8])   # measure the voltage
    vExponent = str(dmm.query('READ?')[8:11])    # retrieve exponent
    sleep(2)
    # Write results to console
    rows.append([start, vMeasured, vExponent])
    return rows, start, set

    
def variableVoltage(set, constant, start, end, direction, increment, dmm):
    dmm.write('VDC')
    set.setAmps(constant)
    set.setVoltage(start)
    set.turnOn()
    rows = []
    if direction == 1: #up
        while start <= end:
           rows, start, set = doVoltageReadings(dmm, start,set,rows)
           start += increment 
    else:
        while start >= end:
            rows, start, set = doVoltageReadings(dmm,start,set, rows)
            start -= increment 
    return set, rows

def doAmpsReadings(dmm, start, set, rows):
    set.setAmps(start)
    sleep(1)                                     # Wait for reading to settle
    aMeasured = float(dmm.query('READ?')[1:8])   # measure the voltage
    aExponent = str(dmm.query('READ?')[8:11])    # retrieve exponent
    sleep(1)
    # Write results to console
    rows.append([start, aMeasured, aExponent])
    return rows, start, set


def variableAmps(set, constant, start, end, direction, increment,  dmm): # variable == 'Amplitude'
    set.setVoltage(constant)
    set.setAmps(start)
    set.turnOn()
    rows = []
    if direction == 1:
        while start <= end:
            rows, start, set = doAmpsReadings(dmm, start, set, rows)        
            start += increment 
    else:
        while start <= end:
            rows, start, set = doAmpsReadings(dmm, start, set, rows)        
            start += increment
    return set, rows

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

    return (channel, variable, direction, start, end, increment, constant)

def csvFile(rows):
    workingDir = os.getcwd()
    timeStr = time.strftime("%H%M%S")
    fileName = "testData_" + timeStr + ".csv"
    header = ['VariableIn', 'VariableRead', 'Exponent']
    with open(workingDir + "/" + fileName, "w", newline='') as file:
        writer = csv.writer(file)
        writer.writerow(header)
        writer.writerows(rows)
    return (workingDir + "/" + fileName)


def main():
    try:
        supply, dmm, rm = devices()  
    except:
        print("turn machines on")
    
    channel, variable, direction, start, end, increment, constant = getValues()

    set = channelInstrument(channel, supply)
    set.turnOff() # turn off
    

   

    if variable == 0: #variable voltage = 0
        set, rows = variableVoltage(set, constant, start, end, direction, increment, dmm)
    else: #variable amps = 1
        set, rows = variableAmps(set, constant, start, end, direction, increment, dmm)   
    
    fileLocation = csvFile(rows)
    
    set.end() 
    rm.close()
    
    '''
    data_out = [ fm.JsonDataFormat(fileLocation, 1, [1])]
    if fm.program_out_script_in_json_buffer != False:
        data_out = data_out + fm.program_out_script_in_json_buffer
        fm.export_json(".dataReady.json", data_out)
    '''

    print("your test data is stored at {}".format(fileLocation))
    