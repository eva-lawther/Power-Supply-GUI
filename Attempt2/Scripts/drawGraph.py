# Take a csv file and plot it using matplotlib

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd

def main():
    fileName = getName()
    print(fileName)
    plt.rcParams["figure.figsize"] = [7.00, 3.50]
    plt.rcParams["figure.autolayout"] = True

    x = "Time"
    y1 = "Voltage set"
    y2 = "Voltage measured"

    columns = [x,y1,y2]
    df = pd.read_csv(fileName, usecols=columns)


    plt.ylabel("voltage (v)")
    plt.xlabel("time (fractional seconds)")

    plt.plot(df[x], df[y1], label = y1)
    plt.plot(df[x], df[y2], label = y2)

    plt.legend()
    plt.show()



def getName():
    file = open("fileNameToGraph.txt","r")
    name = file.readline()
    return name

