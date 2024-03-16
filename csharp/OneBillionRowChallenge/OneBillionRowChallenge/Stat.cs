

using System.Runtime.CompilerServices;

namespace OneBillionRowChallenge
{
    internal struct Stat
    {
        public double Min = Double.MaxValue;
        public double Max = Double.MinValue;
        public double Sum;
        public long Count;

        public Stat()
        {
            Min = double.MaxValue;
            Max = double.MinValue;
            Sum = 0.0;
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ref double value)
        {
            if(value < Min) Min = value;
            if(value > Max) Max = value;
            Sum += value;
            ++Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Merge(ref Stat other)
        {
            if(other.Min < Min) Min = other.Min;
            if(other.Max > Max) Max = other.Max;
            Sum += other.Sum;
            Count += other.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Average() => Sum / Count;
    }
}
