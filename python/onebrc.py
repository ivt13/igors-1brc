from ast import List
from multiprocessing import Process,Manager
from queue import Queue
import multiprocessing
import sys
import os
from io import TextIOWrapper
from file_chunk import FileChunk
from temprature_stat import Stat

DELIMITER = b';'
NEWLINE = b'\n'

def process_file_chunk(file_chunk:FileChunk,queue:Queue) -> None:
    
    input_file_path = file_chunk.file_path
    start_pos = file_chunk.start_pos
    length = file_chunk.length
  
    proc_result = dict()
    
    with open(file=input_file_path,mode="r+b") as fh:
        
        fh.seek(start_pos)
        
        line = bytearray()

        end_pos = start_pos + length

        for i in range(start_pos,end_pos):

            b = fh.read(1)

            if(b == b''):
                break

            if(b == NEWLINE):       

                if(len(line) == 0):
                    continue

                indexOfDelimiter = line.index(DELIMITER)
                
                name = line[0:indexOfDelimiter].decode('utf-8')
                tempStr = line[indexOfDelimiter+1:]
                temp = float(tempStr)
                
                stat = proc_result.get(name)

                if stat == None:
                    stat = Stat()
                    proc_result[name] = stat

                if temp < stat.min:
                    stat.min = temp
                if temp > stat.max:
                    stat.max = temp
                stat.sum += temp
                stat.count += 1
                
                line.clear()
                continue
                
            line.append(b[0])

        fh.close()

    queue.put(proc_result)

def main():

    input_file_path = sys.argv[1]
    result = dict()

    with open(file=input_file_path,mode="r+b") as fh:

        chunk_procs = []
        
        core_count = multiprocessing.cpu_count()
        file_size = os.stat(input_file_path).st_size
    
        if os.cpu_count() != None:
            core_count = os.cpu_count()
        # if the file is small enough, just use one core
        if file_size < 1024*1024:
            core_count = 1
    
        queue = multiprocessing.Manager().Queue(core_count)
            
        bytes_per_proc = int(file_size / core_count)
        current_file_pos = 0
        current_chunk_start = 0
        chunk_length = bytes_per_proc

        while 1:
                    
            b = fh.read(1)
        
            # file end
            if b == b'':

                # if we are going to go past file end, subtract offset
                # that means the last chunk will be the smallest by a little
                if current_chunk_start + chunk_length > file_size and core_count > 1:
                    offset = current_chunk_start + chunk_length - file_size
                    chunk_length -= offset
            
                file_chunk = FileChunk()
                file_chunk.file_path = input_file_path
                file_chunk.start_pos = current_chunk_start
                file_chunk.length = chunk_length
                
                proc = Process(target=process_file_chunk,args=(file_chunk,queue,))
                proc.start()
                chunk_procs.append(proc)
            
                break
        
            if b == NEWLINE:
            
                shouldStop = False

                # if we are going to go past file end, subtract offset
                # that means the last chunk will be the smallest by a little
                if current_chunk_start + chunk_length > file_size and core_count > 1:
                    offset = current_chunk_start + chunk_length - file_size
                    chunk_length -= offset
                    shouldStop = True
            
                file_chunk = FileChunk()
                file_chunk.file_path = input_file_path
                file_chunk.start_pos = current_chunk_start
                file_chunk.length = chunk_length
                
                proc = Process(target=process_file_chunk,args=(file_chunk,queue,))
                proc.start()
                chunk_procs.append(proc)
                        
                if(shouldStop):
                    break
            
                # go to the next one
        
                current_chunk_start = current_file_pos + 1
                chunk_length = bytes_per_proc
                current_file_pos = current_chunk_start + chunk_length
                fh.seek(current_file_pos)

                continue
        
            current_file_pos += 1
            chunk_length += 1
    
        fh.close()
    
        for p in chunk_procs:
            p.join()
            
            proc_result = queue.get();

            for key in proc_result.keys():
                proc_value = proc_result[key]
                existing_stat = result.get(key)
                if(existing_stat == None):
                    result[key] = proc_value
                else:
                    if proc_value.min < existing_stat.min:
                        existing_stat.min = proc_value.min
                    if proc_value.max > existing_stat.max:
                        existing_stat.max = proc_value.max
                    existing_stat.sum += proc_value.sum
                    existing_stat.count += proc_value.count 


    keys = list(result.keys())
    keys.sort()

    print('{',end='')

    len_keys = len(keys)

    for i in range(0,len_keys):
    
        key = keys[i]
        stat = result[key]
    
        print(f"{key}={stat.min}/{stat.avg():.1f}/{stat.max}",end='')

        if(i < len_keys - 1):
            print(", ",end='')

    print('}',end='')
    
if __name__ == "__main__":
    multiprocessing.set_start_method('spawn')
    main()