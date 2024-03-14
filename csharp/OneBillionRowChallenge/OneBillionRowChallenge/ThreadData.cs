

using System.IO.MemoryMappedFiles;

namespace OneBillionRowChallenge
{
    internal unsafe class ThreadData
    {
        internal int LineLength;
        internal byte* Ptr; 
        internal Dictionary<string, Stat> ThreadResult = new Dictionary<string, Stat>(500,StringComparer.Ordinal);
    }
}
