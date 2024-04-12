mod temperature;

use temperature::Temperature;
use std::env;
use std::fs::File;
use std::io::{BufRead, BufReader};
use std::collections::HashMap;

const DELIMITER:char = ';';

fn custom_float_parsing(input:&String, start_index:usize) -> f64 {

    let mut is_negative = false;
    let mut index = 0;
    let input_bytes = input[start_index..].as_bytes();

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

    let file = match File::open(&args[1]) {
        Err(why) => panic!("couldn't open {}: {}", &args[1], why),
        Ok(file) => file,
    };

    let mut result_map = HashMap::<String,Temperature>::new();

    let reader = BufReader::new(file);
    
    for line in reader.lines() {
        let line_str:String;
        match line {
            Ok(line) => line_str = line,
            Err(err) => panic!("Error reading line {0}",err),
        }

        let delimiter_pos = line_str.find(DELIMITER).unwrap();

        let name = line_str[0..delimiter_pos].to_string();
        let temp = custom_float_parsing(&line_str, delimiter_pos+1);

        let new_struct = Temperature {
                min: 1000.0,
                max: -1000.0,
                sum:0.0,
                count:0
            };
        let temp_struct = result_map.entry(name).or_insert_with(|| new_struct);
            
        temp_struct.add(temp);
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
