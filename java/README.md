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
 | using memory mapped file                                |  1m21.358s  |
 | multithreading                                          |  0m09.477s  |
 | using byte[] Wrapper class as key for HashMap           |  0m08.689s  |
 | setting thread priority                                 |  0m08.199s  |
 | switching to GravaalVM JDK 22 + -Xmx8g                  |  0m07.992s  |