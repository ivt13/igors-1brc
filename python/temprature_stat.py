#cython: language_level=3

import cython

@cython.cclass
class Stat:

    minimum:cython.float
    maximum:cython.float
    sum:cython.float 
    count:cython.int

    def __init__(self):
        self.minimum = 100000.0
        self.maximum = -100000.0
        self.sum = 0.0
        self.count = 0

    @cython.returns(cython.float)      
    def avg(self) -> cython.float:
        return self.sum/self.count