#cython: language_level=3

from __future__ import print_function
from ast import List
from multiprocessing import Process,Manager
from queue import Queue
import multiprocessing
import sys
import os
import platform
import cython
from io import TextIOWrapper
from file_chunk import FileChunk
from temprature_stat import Stat

DELIMITER:bytes = b';'
NEWLINE:bytes = b'\n'

@cython.ccall
@cython.boundscheck(False)
@cython.wraparound(False)
def process_file_chunk(file_chunk:FileChunk,queue:Queue):
    
    input_file_path = cython.declare(cython.basestring,file_chunk.file_path)
    start_pos = cython.declare(cython.int,file_chunk.start_pos)
    length = cython.declare(cython.int,file_chunk.length)
  
    proc_result:cython.dict = dict()
    
    with open(file=input_file_path,mode="r+b") as fh:
        
        fh.seek(start_pos)
        
        line = bytearray()

        end_pos:cython.int = start_pos + length

        for i in range(start_pos,end_pos):

            b = fh.read(1)

            if(b == b''):
                break

            if(b == NEWLINE):       

                if(len(line) == 0):
                    continue

                indexOfDelimiter:cython.int = line.index(DELIMITER)
                
                name:cython.basestring = line[0:indexOfDelimiter].decode('utf-8')
                tempStr:cython.basestring = line[indexOfDelimiter+1:]
                temp:cython.float = float(tempStr)
                
                stat:Stat = proc_result.get(name)

                if stat == None:
                    stat = Stat()
                    proc_result[name] = stat

                if temp < stat.minimum:
                    stat.minimum = temp
                if temp > stat.maximum:
                    stat.maximum = temp
                stat.sum += temp
                stat.count += 1
                
                line.clear()
                continue
                
            line.append(b[0])

        fh.close()

    queue.put(proc_result)

@cython.ccall
@cython.boundscheck(False)
@cython.wraparound(False)
def main():
    if platform.system() == "Linux":
        multiprocessing.set_start_method('fork',force=True)    
    else:
        multiprocessing.set_start_method('spawn',force=True)    
    multiprocessing.freeze_support()

    input_file_path = cython.declare(cython.basestring,sys.argv[1])
    result = dict()

    with open(file=input_file_path,mode="r+b") as fh:

        chunk_procs = []
        
        core_count = cython.declare(cython.int, multiprocessing.cpu_count())
        file_size = cython.declare(cython.int,os.stat(input_file_path).st_size)
    
        if os.cpu_count() != None:
            core_count = os.cpu_count()
        # if the file is small enough, just use one core
        if file_size < 1024*1024:
            core_count = 2
    
        queue = multiprocessing.Manager().Queue(core_count)
            
        bytes_per_proc:cython.int = int(file_size / core_count)
        current_file_pos:cython.int = bytes_per_proc
        current_chunk_start:cython.int = 0
        chunk_length:cython.int = bytes_per_proc
        offset = 0
        fh.seek(current_file_pos)

        while 1:
                    
            b:cython.bint = fh.read(1)
        
            # file end
            if b == b'':

                # if we are going to go past file end, subtract offset
                # that means the last chunk will be the smallest by a little
                if current_chunk_start + chunk_length > file_size and core_count > 1:
                    chunk_length -= offset
            
                file_chunk = FileChunk()
                file_chunk.file_path = input_file_path
                file_chunk.start_pos = current_chunk_start
                file_chunk.length = chunk_length
                
                proc:Process = Process(target=process_file_chunk,args=(file_chunk,queue,))
                proc.start()
                chunk_procs.append(proc)
            
                break
        
            if b == NEWLINE:
            
                shouldStop = False

                # if we are going to go past file end, subtract offset
                # that means the last chunk will be the smallest by a little
                if current_chunk_start + chunk_length > file_size and core_count > 1:
                    chunk_length -= offset
                    shouldStop = True
            
                file_chunk = FileChunk()
                file_chunk.file_path = input_file_path
                file_chunk.start_pos = current_chunk_start
                file_chunk.length = chunk_length
                
                proc:Process = Process(target=process_file_chunk,args=(file_chunk,queue,))
                proc.start()
                chunk_procs.append(proc)
                        
                if(shouldStop):
                    break
            
                # go to the next one
        
                current_chunk_start = current_file_pos + 1
                chunk_length = bytes_per_proc
                current_file_pos = current_chunk_start + chunk_length
                fh.seek(chunk_length,1) # advance chunk_length bytes relative to current position
                continue
        
            current_file_pos += 1
            chunk_length += 1
            offset += 1
    
        fh.close()
    
        for p in chunk_procs:
            p.join()
            
            proc_result:cython.dict = queue.get()

            for result_key in proc_result.keys():
                proc_value = proc_result[result_key]
                existing_stat = result.get(result_key)
                if(existing_stat == None):
                    result[result_key] = proc_value
                else:
                    if proc_value.minimum < existing_stat.minimum:
                        existing_stat.minimum = proc_value.minimum
                    if proc_value.maximum > existing_stat.maximum:
                        existing_stat.maximum = proc_value.maximum
                    existing_stat.sum += proc_value.sum
                    existing_stat.count += proc_value.count 


    keys:cython.list = list(result.keys())
    keys.sort()

    print('{',end='')

    len_keys = cython.declare(cython.int, len(keys))

    for i in range(0,len_keys):
    
        result_key: cython.basestring = keys[i]
        stat = result[result_key]
    
        print(f"{result_key}={stat.minimum}/{stat.avg():.1f}/{stat.maximum}",end='')

        if(i < len_keys - 1):
            print(", ",end='')

    print('}',end='')
    
if __name__ == "__main__":
    
    main()