package ca.igor.jbrc;

public class BufferHelpers {
    
    public static void clearBuffer(byte[] buffer)
    {
        var len = buffer.length;
        for(var i = 0; i < len; ++i) {
            buffer[0] = 0;
        }

    }
}
