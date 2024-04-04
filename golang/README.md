This is the 1 billion Go challenge using Go version 1.22. 

Timing is done on Ubuntu 22.04 WSL on 12 core AMD Ryzen 3900X.

Run: time ./main path/to/measurements-1000000000.txt

 |         Change                            |      time   | 
 |-------------------------------------------|-------------|
 | base implementation                       | 2m26.328s   |
 | manually parse float64                    | 1m52.766s   |