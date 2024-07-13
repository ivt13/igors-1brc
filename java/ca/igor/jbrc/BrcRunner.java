package ca.igor.jbrc;

import java.io.EOFException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.io.RandomAccessFile;
import java.util.Scanner;


public class BrcRunner {

    private static final byte NEWLINE = '\n';

    public static void main(String[] args) throws IOException {
        if(args.length < 1) {
            System.out.println("Missing file path");
            return;
        }

        if(args.length > 1 && args[1].equals("-p")) {

            var userInput = new Scanner(System.in);
            while(true) {
                System.out.println("Press Enter key after profiler is connected.");
                userInput.nextLine();
                break;
            }
        }

        var result = new HashMap<ByteArray,Temperature>(500);
        var cores = Runtime.getRuntime().availableProcessors();

        var fileChunks = new ArrayList<FileChunk>(cores);
                
        try(var file = new RandomAccessFile(args[0],"r")) {
            
            var fileSize = file.length();

            if(fileSize < 10000)
            {
                cores = 1;
            }

            var bytesPerChunk = (long)((double)fileSize/cores + 0.5);
            var chunkLength = bytesPerChunk;

            var filePtr = file.getFilePointer();

            file.seek(filePtr + chunkLength);   

            while (true) {
                
                var shouldStop = false;
                try {
                    while(file.readByte() != NEWLINE) {
                        ++chunkLength;
                    }
                } catch(EOFException ex){
                    shouldStop = true;
                }

            
                var fileChunk = new FileChunk();
                fileChunk.filePath = args[0];
                fileChunk.start = filePtr;
                fileChunk.length = chunkLength;
                fileChunks.add(fileChunk);
                
                if(shouldStop) {
                    break;
                }

                filePtr += chunkLength + 1;
                file.seek(filePtr + bytesPerChunk); 
                chunkLength = bytesPerChunk;
                               
                if(filePtr + chunkLength > fileSize) {
                    var offset = filePtr + chunkLength - fileSize;
                    chunkLength -= offset;
                }

            }

        }

        var threads = new ArrayList<Thread>(fileChunks.size());

        for(var fileChunk : fileChunks) {
            var thread = new Thread(fileChunk);
            thread.setPriority(Thread.MAX_PRIORITY);
            thread.start();
            threads.add(thread);
        }

        for(var i = 0; i < fileChunks.size(); ++i) {
            try {
                var thread = threads.get(i);
                thread.join();
                var fileChunk = fileChunks.get(i);
                merge(result, fileChunk.fileChunkResult);
            } catch(InterruptedException ex) {
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

    private static void merge(HashMap<ByteArray,Temperature> globalResult, HashMap<ByteArray,Temperature> threadResult) {

        for(var entry : threadResult.entrySet()) {

            var key = entry.getKey();
            var valueFromThread = entry.getValue();
            var existingValue = globalResult.get(key);
            if(existingValue == null) {
                globalResult.put(key, valueFromThread);
            } else {
                existingValue.merge(valueFromThread);
            }

        }

    }

    
}