The 1 billion row challenge, in python.

All timing will be done on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core.

run using:

time python3 onebrc.py /path/to/measurements-1000000000.txt


 |         Change                                   |      time   |     Python version    | 
 |--------------------------------------------------|-------------|-----------------------|
 | base implementation                              | 16m57.382s  |     3.10.12           |
 | base impl. with cached bytecode                  | 16m33.732s  |     3.10.12           |
 | inline add function, use rindex()                | 16m28.758s  |     3.10.12           |
 | adding multiprocessing                           | 03m57.250s  |     3.10.12           |
 | upgrade to python 3.12.2                         | 03m44.617s  |     3.12.2            |
 | converting to Cython3 (pure Python mode)         | 03m41.310s  |     3.12.2 + Cython3  |
 | ensuring exactly 24 processes are created        | 03m36.001s  |     3.12.2 + Cython3  |
 | making file seek relative to current position    | 03m31.510s  |     3.12.2 + Cython3  |
