import pyvisa
from time import sleep
rm = pyvisa.ResourceManager()
print("Resources detected\n{}\n".format(rm.list_resources()))

supply = rm.open_resource('ASRL13::INSTR') # resource id for power supply
dmm = rm.open_resource('ASRL6::INSTR')    # resource id for digital multi meter

# set digital multimeter to dc voltage mode
dmm.write('VDC')


# turn power supply (ch1) off, set ch1 with 200mA and then turn ch1 on
supply.write('OP1 0')  # turn off
supply.write('I1 0.2') # set 200mA
supply.write('OP1 1')  # turn on

v = 0
while v <= 10.0:  # loop through multiple voltages, measure and write results to console
    supply.write('V1 ' + str(v))                 # Set the voltage on ch1 to the value of v
    sleep(1)                                     # Wait for reading to settle
    vMeasured = float(dmm.query('READ?')[1:8])   # measure the voltage
    vExponent = str(dmm.query('READ?')[8:11])    # retrieve exponent
    sleep(1)                           
    
    # Write results to console
    print("{}  {}  {}".format(v, vMeasured, vExponent))

    v += 0.5 # increment v

# Test complete. Turn supply off and zero the voltage and amps
supply.write('OP1 0')
supply.write('V1 0')
supply.write('I1 0')

