One billion row challenge in Rust

Built using Rust 1.75 on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core

Run: cargo build -r
Run: time ./target/release/main /path/to/measurements-1000000000.txt

 |         Change                                     |      time   | 
 |----------------------------------------------------|-------------|
 | base implementation                                | 2m15.943s   |
 | struct function inlining                           | 2m14.372s   |
 | custom float parsing                               | 1m38.252s   |
 | memory mapped file + file pointer                  | 1m23.644s   |
