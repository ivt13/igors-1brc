The 1 billion row challenge, in python.

All timing will be done on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core.

run using:

time python3 onebrc.py /path/to/measurements-1000000000.txt


 |         Change                      |      time   |     Python version | 
 |-------------------------------------|-------------|--------------------|
 | base implementation                 | 16m57.382s  |     3.10.12        |
 | base impl. with cached bytecode     | 16m33.732s  |     3.10.12        |
 | inline add function, use rindex()   | 16m28.758s  |     3.10.12        |
 | adding multiprocessing              | 03m57.250s  |     3.10.12        |
