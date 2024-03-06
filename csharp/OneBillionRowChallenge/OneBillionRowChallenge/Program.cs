using System.IO.MemoryMappedFiles;
using System.Text;

namespace OneBillionRowChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];

            var result = new Dictionary<string, Stat>(StringComparer.Ordinal);

            using var mmap = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            using var view = mmap.CreateViewAccessor();


            const int bufferSize = 512;
            var buffer = new byte[bufferSize];
            var bufferPos = 0;

            const byte newLine = 10;
            var i = 0;
            while (true)
            {
                try
                {
                    var b = view.ReadByte(i);
                    ++i;
                    buffer[bufferPos] = b;
                    if (b == newLine)
                    {
                        HandleLine(buffer, bufferPos, result);
                        bufferPos = 0;
                        Array.Clear(buffer);
                        continue;
                    }

                    ++bufferPos;
                }
                catch (ArgumentException)
                {
                    break;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }

            var j = 0;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("{");
            foreach (var kvp in result.OrderBy(x => x.Key))
            {
                if (j < result.Count - 1)
                {
                    Console.Write($"{kvp.Key}={kvp.Value.Min}/{kvp.Value.Average()}/{kvp.Value.Max},");
                }
                else
                {
                    Console.Write($"{kvp.Key}={kvp.Value.Min}/{kvp.Value.Average()}/{kvp.Value.Max}");
                }
                ++j;
            }
            Console.Write("}");
        }

        private static void HandleLine(byte[] buffer, int byteCount, Dictionary<string, Stat> result)
        {
            const byte delimiter = 59;
            var indexOfDelimiter = 0;
            
            for (var i = 0; i < byteCount; ++i)
            {
                if (buffer[i] == delimiter)
                {
                    indexOfDelimiter = i;
                    break;
                }
            }

            var nextAfterDelimiter = indexOfDelimiter + 1;
            
            var name = Encoding.UTF8.GetString(buffer,0,indexOfDelimiter);
            var tempStr = Encoding.UTF8.GetString(buffer, nextAfterDelimiter, byteCount - nextAfterDelimiter);

            var temp = double.Parse(tempStr);

            if (!result.TryGetValue(name, out var stat))
            {
                stat = new Stat();
                result[name] = stat;
            }

            stat.Add(temp);
        }
    }
}
