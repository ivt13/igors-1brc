

using System.IO.MemoryMappedFiles;

namespace OneBillionRowChallenge
{
    internal unsafe class ThreadData
    {
        internal int Length;
        internal byte* Ptr; 
        internal Dictionary<Key, Stat> ThreadResult = new Dictionary<Key, Stat>(500);
    }
}
