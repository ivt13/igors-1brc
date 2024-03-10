using System.Buffers;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace OneBillionRowChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];
            var fileSize = new FileInfo(filePath).Length;

            var result = new Dictionary<string, Stat>(500,StringComparer.Ordinal);

            using var mmap = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            using var view = mmap.CreateViewStream(0,fileSize,MemoryMappedFileAccess.Read);

            const int bufferSize = 1024*128;
            var buffer = new byte[bufferSize];
            
            const byte newLine = 10; // \n
            var bytesRead = 0;

            while ((bytesRead = view.Read(buffer,0,bufferSize)) > 0)
            {
                var lineStart = 0;
                var stop = false;
                for (var bufferPos = 0; bufferPos < bytesRead; ++bufferPos)
                {
                    if (buffer[bufferPos] == newLine)
                    {
                        var line = new ReadOnlySpan<byte>(buffer, lineStart, bufferPos - lineStart);
                        HandleLine(ref line, result);
                        lineStart = bufferPos + 1;
                    }

                    if (buffer[bufferPos] == 0)
                    {
                        stop = true;
                        break;
                    }
                }

                if (stop)
                {
                    break;
                }

                Array.Clear(buffer);
                view.Position = view.Position - bytesRead + lineStart;
            }


            var j = 0;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("{");
            foreach (var kvp in result.OrderBy(x => x.Key))
            {
                if (j < result.Count - 1)
                {
                    Console.Write($"{kvp.Key}={kvp.Value.Min:0.#}/{kvp.Value.Average():0.#}/{kvp.Value.Max:0.#},");
                }
                else
                {
                    Console.Write($"{kvp.Key}={kvp.Value.Min:0.#}/{kvp.Value.Average():0.#}/{kvp.Value.Max:0.#}");
                }
                ++j;
            }
            Console.Write("}");
        }

        private static void HandleLine(ref ReadOnlySpan<byte> buffer, Dictionary<string, Stat> result)
        {
            if (buffer.IsEmpty)
            {
                return;
            }

            const byte delimiter = 59; // ;
            var indexOfDelimiter = 0;

            for (var i = 0; i < buffer.Length; ++i)
            {
                if (buffer[i] == delimiter)
                {
                    indexOfDelimiter = i;
                    break;
                }
            }

            var nextAfterDelimiter = indexOfDelimiter + 1;
            
            var name = Encoding.UTF8.GetString(buffer.Slice(0, indexOfDelimiter));
            var tempSlice = buffer.Slice(nextAfterDelimiter);
            
#if NET8_0_OR_GREATER
            var temp = double.Parse(tempSlice);
#else
            var tempStr = Encoding.UTF8.GetString(tempSlice);
            var temp = double.Parse(tempStr);
#endif

            ref Stat stat = ref CollectionsMarshal.GetValueRefOrAddDefault(result, name, out _);
            stat.Add(temp);

            stat.Add(temp);
        }
    }
}
