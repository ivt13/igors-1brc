1 billion row challenge in Java.

Built using OpenJDK version of Java 21

All timing will be done on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core.

run using: 

time java ca.igor.jbrc.BrcRunner /path/to/1brc.data/measurements-1000000000.txt

Results:

 |         Change                                          |    time     | 
 |---------------------------------------------------------|-------------|
 | base implementation                                     |  2m09.244s  |
 | custom float parsing, one less String allocation        |  1m41.198s  |
 | memory mapped file                                      |  1m21.358s  |
