extern crate memmap2;

mod temperature;

use memmap2::MmapRaw;
use temperature::Temperature;
use std::env;
use std::collections::HashMap;
use std::fs::OpenOptions;
use std::path::Path;

const DELIMITER:u8 = b';';
const NEWLINE:u8 = b'\n';
const BUFFER_LENGTH:usize = 40;

fn custom_float_parsing(input:&[u8], start_index:usize,end_index:usize) -> f64 {

    let mut is_negative = false;
    let mut index = 0;
    let input_bytes = &input[start_index..end_index];

    let negative_ascii = 45;
    let dot_ascii = 46;
    let zero_ascii = 48;

    if input_bytes[index] == negative_ascii {
        is_negative = true;
        index += 1;
    }

    let mut result = (input_bytes[index] - zero_ascii) as f64;
    index += 1;

    if input_bytes[index] != dot_ascii {
        result = result*10.0 + (input_bytes[index] - zero_ascii) as f64;
        index += 1;
    }

    index += 1;
    result += (input_bytes[index] - zero_ascii) as f64/10.0;

    if is_negative {
        result *= -1.0;
    }

    return result;
}

fn main() {

    let args: Vec<String> = env::args().collect();

    if args.len() < 2 {
        println!("Missing file path argument");
        return;
    }

    let path = Path::new(&args[1]);

    let file =  OpenOptions::new().read(true).write(true).open(path).unwrap();

    let mmap = MmapRaw::map_raw(&file).expect("failed to map the file");
    let mut mmap_ptr = mmap.as_mut_ptr();

    let mut result_map = HashMap::<String,Temperature>::new();

    let mut buffer:[u8;BUFFER_LENGTH] = [0;BUFFER_LENGTH];
    let mut buffer_pos = 0;

        
    unsafe
    {
        while 1 == 1 {
            
            let mut should_proc_line = false;
            let mut should_stop = false;

            if mmap_ptr.is_null() || *mmap_ptr == 0 {
                should_stop = true;
                should_proc_line = buffer_pos > 0;
            }    

            if *mmap_ptr == NEWLINE {   
                should_proc_line = true;
            }

            if should_proc_line {
            
                let delimiter_pos = buffer.iter().position(|&x| x == DELIMITER).unwrap();

                let name = String::from_utf8(buffer[0..delimiter_pos].to_vec()).unwrap();
                let temp = custom_float_parsing(&buffer, delimiter_pos+1,buffer_pos);

                let new_struct = Temperature {
                        min: 1000.0,
                        max: -1000.0,
                        sum:0.0,
                        count:0
                    };
                let temp_struct = result_map.entry(name).or_insert_with(|| new_struct);
                    
                temp_struct.add(temp);
                clear_line(&mut buffer);
                buffer_pos = 0;
                mmap_ptr = mmap_ptr.byte_add(1);
                
                if should_stop {
                    break;
                }
                continue;
            }

            if should_stop {
                break;
            }

            buffer[buffer_pos] = *mmap_ptr;
            buffer_pos += 1;
            mmap_ptr = mmap_ptr.byte_add(1)
            
        }
    }


    print!("{{");

    let map_size = result_map.len();
    let mut keys:Vec<&String> = result_map.keys().collect();
    keys.sort();

    for i in 0..map_size {
        let key = keys[i];
        let value = result_map.get(key).expect("Temperature not found");

        let min= value.min;
        let avg = format!("{:.1}",value.avg());
        let max = value.max;

        if i < map_size - 1 {
            print!("{0}={1}/{2}/{3}, ",key,min,avg,max);
        } else {
            print!("{0}={1}/{2}/{3}",key,min,avg,max);
        }

    }

    print!("}}")
    

}

fn clear_line(buffer:&mut [u8;BUFFER_LENGTH]) -> &mut [u8;BUFFER_LENGTH] {

    let len = buffer.len();

    for i in 0..len {
        buffer[i] = 0;
    }

    return buffer;
}
