
import pyvisa
import time
from time import sleep
import os
import csv



def devices():
    rm = pyvisa.ResourceManager()
    #print("Resources detected\n{}\n".format(rm.list_resources()))
    dmm = rm.open_resource('ASRL6::INSTR')    # resource id for digital multi meter
    supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply   
    
    # set digital multimeter to dc voltage mode
    #dmm.write('VDC')
    return (supply, dmm, rm)

def runCommandList(supply,dmm):
    rows = []
    file = open("runningFile.txt", "r")
    while True:
        line = file.readline()
        if not line:
            break
        
        elif(line[0:2] == "PS"):
            line = line[4:]
            if (line[0:5] == "sleep" ):
                time = line.split(" ")[1]
                sleep(time)
            else:
                if (line[-1] == "?"):
                    read = str(supply.query(line))
                    rows.append(["PS", line, read])
                else:
                    supply.write(line)  
                    rows.append(["PS",line,None])
                
        elif(line[0:2]=="MM"):
            line = line[4:]
            if (line[-1] == "?"):
                read = str(dmm.query(line))
                rows.append("MM",line,read)
            else:
                dmm.write(line)
                rows.append("MM", line, None)
    return rows

def csvFile(rows):
    workingDir = os.getcwd()
    timeStr = time.strftime("%H%M%S")
    fileName = "commandListOutput_" + timeStr + ".csv"
    header = ['Machine', 'Command', 'Response']
    with open(workingDir + "/" + fileName, "w", newline='') as file:
        writer = csv.writer(file)
        #writer.writerow(header)
        writer.writerows(rows)
    return (workingDir + "/" + fileName) 
        

def main():
    supply,dmm,  rm = devices()  
    rows = runCommandList(supply,dmm)
    
    fileLocation = csvFile(rows)
    print("your results data is stored at {}".format(fileLocation))
    rm.close()

 