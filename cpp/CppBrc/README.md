
1BRC in C++, built using g++ with -03

All timing will be done on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core.

run using: 

time ./CppBrc /path/to/measurements-1000000000.txt

Results:

 |         Change                                          |      time   | 
 |---------------------------------------------------------|-------------|
 | base implementation (MSVC)                              | 3m13.226s   |
 | base implementation (g++ -03)                           | 3m07.985s   |
 | memory mapped file using mio + custom float parsing     | 1m53.996s   |
 | using iterators instead of buffer + std::string_view    | 1m42.133s   |
 | removing use of std::string_view                        | 1m39.245s   |
 | making mio compile on Windows + multithreading          | 0m07.488s   |
 | use std::unordered_map instead of std::map              | 0m04.434s   |