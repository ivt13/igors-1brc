using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace OneBillionRowChallenge
{
    internal unsafe class Program
    {
        static void Main(string[] args)
        {
            var filePath = args[0];
            var fileSize = new FileInfo(filePath).Length;

            var result = new Dictionary<Key, Stat>(500);

            using var mmap = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            using var view = mmap.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Read);

            var viewHandle = view.SafeMemoryMappedViewHandle;

            byte* ptr = (byte*)0;
            viewHandle.AcquirePointer(ref ptr);

            var threadDatas = SplitIntoChunks(fileSize, ptr).ToArray();
            var threads = new Thread[threadDatas.Length];

            for (int i = 0; i < threadDatas.Length; ++i)
            {
                var thread = new Thread(ThreadRunner);
                thread.Start(threadDatas[i]);
                threads[i] = thread;
            }

            for (var i = 0; i < threads.Length; ++i)
            {
                var thread = threads[i];
                thread.Join();
                Merge(result, threadDatas[i]);
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

        private static void Merge(Dictionary<Key, Stat> globalResult, ThreadData data)
        {
            foreach (var kvp in data.ThreadResult)
            {
                ref var stat = ref CollectionsMarshal.GetValueRefOrAddDefault(globalResult, kvp.Key, out _);
                var statFromThread = kvp.Value;
                stat.Merge(ref statFromThread);
            }
        }

        private static List<ThreadData> SplitIntoChunks(long fileSize, byte* ptr)
        {
            const long minFileSize = 1024*1024;
            const byte newLine = 10; // \n

            var cores = Environment.ProcessorCount;
            if (fileSize <= minFileSize)
            {
                cores = 1;
            }

            var threadDatas = new List<ThreadData>(cores);

            var bytesPerCore = fileSize / cores;

            var partLength = (int)bytesPerCore;

            byte* partStartPtr = ptr;
            byte* fileEndPtr = ptr + fileSize;

            ptr += bytesPerCore;
            var offset = 0;
            
            /* because we don't know how \n's are distributed
             * within the file, each core will process
             * bytesPerCore + bytes until new line
             */
            while (true)
            {
                if (*ptr == 0)
                {
                    if (partStartPtr + partLength > fileEndPtr)
                    {
                        partLength -= offset;
                    }

                    var data = new ThreadData
                    {
                        Ptr = partStartPtr,
                        Length = partLength
                    };
                    threadDatas.Add(data);
                    break;
                }

                if (*ptr == newLine)
                {
                    var shouldStop = false;
                    if (partStartPtr + partLength > fileEndPtr)
                    {
                        partLength -= offset;
                        shouldStop = true;
                    }

                    var data = new ThreadData
                    {
                        Ptr = partStartPtr,
                        Length = partLength
                    };
                    threadDatas.Add(data);

                    if (shouldStop)
                    {
                        break;
                    }

                    partStartPtr = ptr + 1;
                    partLength = (int)bytesPerCore;
                    ptr = partStartPtr + partLength;
                }

                ++ptr;
                ++partLength;
                ++offset;
            }

            return threadDatas;
        }
        
        private static void ThreadRunner(object? dataObject)
        {
            var threadData = dataObject as ThreadData;
            if (threadData == null || threadData.Ptr == null)
            {
                return;
            }

            var span = new ReadOnlySpan<byte>(threadData.Ptr, threadData.Length);

            const byte newLine = 10; // \n

            int lineStart = 0;

            var result = threadData.ThreadResult;

            for (var i = 0; i < span.Length; ++i)
            {
                var b = span[i];
                if (b == newLine)
                {
                    HandleLine(threadData.Ptr+lineStart,i-lineStart,result);
                    lineStart = i+1;
                }
            }

        }

        private static void HandleLine(byte* ptr,int length, Dictionary<Key, Stat> result)
        {
            if (length == 0 || ptr == null)
            {
                return;
            }

            const byte delimiter = 59; // ;
            var indexOfDelimiter = 0;

            var delimiterPtr = ptr;

            for (var i = 0; i < length; ++i)
            {
                if (*delimiterPtr == delimiter)
                {
                    indexOfDelimiter = i;
                    break;
                }

                ++delimiterPtr;
            }

            var nextAfterDelimiter = indexOfDelimiter + 1;
            
            var name = new Key(ptr, indexOfDelimiter);
            var tempSlice = new ReadOnlySpan<byte>(ptr+nextAfterDelimiter,length-nextAfterDelimiter);
            
#if NET8_0_OR_GREATER
            var temp = double.Parse(tempSlice);
#else
            var tempStr = Encoding.UTF8.GetString(tempSlice);
            var temp = double.Parse(tempStr);
#endif

            ref var stat = ref CollectionsMarshal.GetValueRefOrAddDefault(result, name, out _);
            stat.Add(ref temp);
        }
    }
}
