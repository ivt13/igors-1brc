package ca.igor.jbrc;

import java.io.FileReader;
import java.io.IOException;
import java.io.BufferedReader;
import java.util.Arrays;
import java.util.HashMap;


public class BrcRunner {

    private static final char DELIMITER = ';';
    private static final char NEWLINE = '\n';

    public static void main(String[] args) throws IOException {
        if(args.length < 1)
        {
            System.out.println("Missing file path");
            return;
        }

        var result = new HashMap<String,Temperature>();

        try(var reader = new BufferedReader(new FileReader(args[0])))
        {
            String line = null;
            while ((line = reader.readLine()) != null) {
                
                var indexOfDelimiter = indexOfToken(line, 0, DELIMITER);
                var name = line.substring(0,indexOfDelimiter);

                var temperatureStr = line.substring(indexOfDelimiter+1);
                var temp = Float.parseFloat(temperatureStr);

                var existingTemp = result.get(name);
                if(existingTemp == null)
                {
                    existingTemp = new Temperature();
                    result.put(name, existingTemp);
                }
                existingTemp.add(temp);

            }
        }

        var keys = result.keySet().toArray();
        Arrays.sort(keys);

        System.out.print("{");

        for(var i = 0; i < keys.length; ++i)
        {
            var name = keys[i];
            var t = result.get(name);

            if(i < keys.length - 1)
            {
                System.out.printf("%s=%.1f/%.1f/%.1f, ", name,t.min,t.avg(),t.max);
            }
            else
            {
                System.out.printf("%s=%.1f/%.1f/%.1f", name,t.min,t.avg(),t.max);
            }
        }

        System.out.println("}");

    }

    private static int indexOfToken(String line, int startIndex, char token) {
        var index = line.indexOf(token,startIndex);
        return index;
    }
}