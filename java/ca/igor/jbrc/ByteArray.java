package ca.igor.jbrc;

import java.util.Arrays;

public class ByteArray implements Comparable<ByteArray> {
    
    public byte[] array;

    public ByteArray(byte[] bytes) {
        array = bytes;
    }

    @Override
    public boolean equals(Object other) {
        return Arrays.equals(array, ((ByteArray) other).array);
    }

    @Override
    public int hashCode() {
        return Arrays.hashCode(array);
    }

    @Override
    public String toString() {
        return new String(array); 
    }

    @Override
    public int compareTo(ByteArray o) {
       
        return Arrays.compare(array, o.array);
    }
}
