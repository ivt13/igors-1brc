package ca.igor.jbrc;


import java.io.RandomAccessFile;
import java.nio.channels.FileChannel.MapMode;
import java.util.HashMap;

public class FileChunk implements Runnable {

    private static final byte DELIMITER = ';';
    private static final byte NEWLINE = '\n';
    private static final int BUFFER_SIZE = 40; 

    public String filePath;
    public long start;
    public long length;
    public HashMap<String,Temperature> fileChunkResult = new HashMap<String,Temperature>(500);

    @Override
    public void run() {
        
        var buffer = new byte[BUFFER_SIZE];
        var bufferPos = 0;
           
        try {
            try(var file = new RandomAccessFile(filePath, "r")) {

                var map = file.getChannel().map(MapMode.READ_ONLY, start, length);
                map.load();

                for(var i = 0; i < length; ++i) {
                    var b = map.get();

                    if(b == NEWLINE) {
                        procLine(fileChunkResult, buffer);
                        clearBuffer(buffer);
                        bufferPos = 0;
                        continue;
                    }
                    buffer[bufferPos++] = b;
                }
            }
        } catch(Exception ex) {
            System.out.println(ex);
        }

    }
    
    private void procLine(HashMap<String,Temperature> result, byte[] buffer)
    {
        if(buffer[0] == 0) {
            return;
        }       

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

    private void clearBuffer(byte[] buffer)
    {
        var len = buffer.length;
        for(var i = 0; i < len; ++i) {
            buffer[0] = 0;
        }

    }

    private int indexOfToken(byte[] buffer, int startIndex, byte token) {
        var length = buffer.length;
        for(var i = 0; i < length; ++i) {
            if(buffer[i] == token) {
                return i;
            }
        }
        return -1;
    }

    private float customFloatParse(byte[] input, int startFrom)
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
