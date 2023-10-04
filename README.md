# PowerSupplyGUI
In SED we use the lab instruments to run tests on our products and designs. This can be a long process and automating it will be a valuable, time saving practice. 

It should be noted that this GUI is not a comprehensive interface for the power supply but a solid proof of concept with examples of what can be done for many of the lab instruments and points of interest for someone who wishes to automate a specific test.

The GUI created allows a user to configure a lab instrument (the power supply), run predefined tests (specifically a sweeping voltage test) and store results as a CSV file to plot at a later point.

## Installation

There are multiple python modules required these can be installed using the package manager [pip](https://pip.pypa.io/en/stable/).
```bash
pip install time
pip install pyvisa
pip install matplotlib
pip install pandas
pip install os
pip install csv
pip install numpy
pip install json
```

We interface between c# and python, this implementation may be problematic as the version of **PowerShell used must have 'Set-ExecutionPolicy RemoteSigned'**. Note: two verison may exist version x86 and x64. The command above must be ran for the relevant version.

## Future coding
The two manuals required for editing this code are:
-	 the TTI power supply [manual](https://resources.aimtti.com/manuals/MX100Q+MX100QP_Instruction_Manual-Iss2.pdf)
-	 the TTI multimeter [manual](https://resources.aimtti.com/manuals/1908_Instruction_Manual-Iss5.pdf) 

# Notable Points of Code
### Python C# Interface
The GUI used to create this is the .NET GUI in C#, this is important as the tests being ran are all python. We therefore use an SED implementation to run the python modules.

We aim to create a .DLL library to store the C# interface, however, as of current these functions are repeated at the top of the code files.

The way the interface works is as follows:
- When initialising the programme, we populate a power shell script with all the .py files in the local directory
- When we call RunPowerShellScript() we create a pipeline to the power shell from which we run the python file
- To share data (variables parsing) we use Json files. JsonDataFormat is a class that specifies the format of the data being sent to python and returned to C#

When running the python python files we use back workers to trigger an interrupt when python has completed the given task. Using multiple threads allow for us to keep using the GUI while python code is running, and to get data back from python in a timely manner.

### Graphing capabilities
The graphing capabality is currently limited to csv files with data frames of type ("Time", "Voltage set", "Voltage Measured"). This function will plot the csv file with voltage on the y axis and time on the x axis.

## Version Control
This code was written using:
- Python 3.11.6
- C# 7.3
- .NET Framework 4.7.2
- Visual Studio 2022 (2) - community edition


# Document Details
###### Document: ATE GUI ReadMe.MD 
###### Author: Eva Lawther - elawther@slipstream-design.co.uk
###### Company: Slipstream Engineering Design Ltd.
###### Date: 04/10/2023