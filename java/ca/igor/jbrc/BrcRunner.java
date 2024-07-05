package ca.igor.jbrc;

import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import java.io.RandomAccessFile;
import java.nio.channels.FileChannel;



public class BrcRunner {

    private static final byte DELIMITER = ';';
    private static final byte NEWLINE = '\n';
    private static final int BUFFER_SIZE = 40; 

    public static void main(String[] args) throws IOException {
        if(args.length < 1) {
            System.out.println("Missing file path");
            return;
        }


        var result = new HashMap<String,Temperature>(500);

        var buffer = new byte[BUFFER_SIZE];
        var bufferPos = 0;

        
        
        try(var file = new RandomAccessFile(args[0],"r")) {
            
            var filePos = 0L;
            var fileLength = file.length();

            while(filePos < fileLength) {
                var segmentLength = fileLength - filePos <= Integer.MAX_VALUE ? fileLength - filePos : Integer.MAX_VALUE; 

                var map = file.getChannel().map(FileChannel.MapMode.READ_ONLY, filePos, segmentLength);
                
                for(var i = 0; i < segmentLength; ++i) {

                    var b = map.get();
                    if(b == 0) break;

                    if(b == NEWLINE) {
                        procLine(result,buffer);
                        clearBuffer(buffer);
                        bufferPos = 0;
                        continue;
                    }

                    buffer[bufferPos++] = b;
                }

                filePos += segmentLength;
            }


        }

        var keys = result.keySet().toArray();
        Arrays.sort(keys);

        System.out.print("{");

        for(var i = 0; i < keys.length; ++i) {
            var name = keys[i];
            var t = result.get(name);

            if(i < keys.length - 1) {
                System.out.printf("%s=%.1f/%.1f/%.1f, ", name,t.min,t.avg(),t.max);
            } else {
                System.out.printf("%s=%.1f/%.1f/%.1f", name,t.min,t.avg(),t.max);
            }
        }

        System.out.println("}");

    }

    private static void procLine(HashMap<String,Temperature> result, byte[] buffer)
    {
        var indexOfDelimiter = indexOfToken(buffer, 0, DELIMITER);
        var name = new String(buffer,0,indexOfDelimiter);
        var temp = customFloatParse(buffer, indexOfDelimiter+1);

        var existingTemp = result.get(name);
        if(existingTemp == null)
        {
            existingTemp = new Temperature();
            result.put(name, existingTemp);
        }
        existingTemp.add(temp);
    }

    private static void clearBuffer(byte[] buffer)
    {
        var len = buffer.length;
        for(var i = 0; i < len; ++i) {
            buffer[0] = 0;
        }

    }

    private static int indexOfToken(byte[] buffer, int startIndex, byte token) {
        var length = buffer.length;
        for(var i = 0; i < length; ++i) {
            if(buffer[i] == token) {
                return i;
            }
        }
        return -1;
    }

    private static float customFloatParse(byte[] input, int startFrom)
    {
        var result = 0.0f;
        var index = startFrom;
        var negative = false;

        if (input[index] == '-')
        {
            negative = true;
            ++index;
        }

        result = input[index] - '0';
        ++index;

        if (input[index] != '.')
        {
            result = result * 10 + input[index] - '0';
            ++index;
        }

        ++index;
        result += ((float)input[index] - '0') / 10;
        if (negative)
        {
            result *= -1f;
        }
        return result;
    }

    
}