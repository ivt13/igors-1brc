import sys
from io import TextIOWrapper
from temprature_stat import Stat

input_file_path = sys.argv[1]

result = {}

with open(file=input_file_path,mode="r",encoding="utf-8") as fh:

    while True: 
    
        line = fh.readline()
        
        if(line == None or line == ""):
            break

        indexOfDelimiter = line.index(';')

        name = line[0:indexOfDelimiter]
        tempStr = line[indexOfDelimiter+1:-1]
        temp = float(tempStr)

        stat = result.get(name)

        if stat == None:
            stat = Stat()
            result[name] = stat

        stat.add(temp)


keys = list(result.keys())
keys.sort()

print('{',end='')

len = len(keys)

for i in range(0,len):
    
    key = keys[i]
    stat = result[key]
    
    print(f"{key}={stat.min}/{stat.avg():.1f}/{stat.max}",end='')

    if(i < len - 1):
        print(", ",end='')

print('}',end='')