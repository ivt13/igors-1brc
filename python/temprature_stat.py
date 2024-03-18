import sys

class Stat:

    min:float = sys.float_info.max
    max:float = sys.float_info.min
    sum:float = 0.0
    count:int = 0

    
    def add(self,temprature:float):
        if temprature < self.min:
            self.min = temprature
        if temprature > self.max:
            self.max = temprature
        
        self.sum += temprature
        self.count += 1
            
    def avg(self):
        return self.sum/self.count