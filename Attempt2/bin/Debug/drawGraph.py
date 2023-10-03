# Take a csv file and plot it using matplotlib

import matplotlib.pyplot as plt
import numpy as np
#import pandas as pd

def main():
    fileName = getName()
    toPlot = np.loadtxt(fileName)
    plt.title(fileName)
    plt.plot(toPlot)
    plt.ylabel("voltage (v)")
    plt.xlabel("time (fractional seconds)")

def getName():
    file = open("fileNameToGraph.txt","r")
    name = file.readline()
    return name

