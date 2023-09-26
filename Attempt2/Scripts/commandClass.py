

class channelInstrument:
    def __init__(self, channel, supply):
        self._channel = str(channel)
        self._supply = supply
        
    def turnOn(self):
        self._supply.write('OP{n} 1'.format(n = self._channel))  

    def turnOff(self):
        self._supply.write('OP{n} 0'.format(n = self._channel))  
                 
    def setVoltage(self, v):
        self._supply.write('V{n} {V}'.format(n = self._channel, V = str(v)))

    def setVoltageRange(self, r):
        self._supply.write('VRANGE{n} {R}'.format(n = self._channel, R = str(r)))
        
    def setAmps(self, a):
        self._supply.write('I{n} {A}'.format(n=self._channel, A=a))
        
                     
    def end(self):
        self.turnOff()
        self.setAmps(0)
        self.setVoltage(0)
        