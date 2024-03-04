

namespace OneBillionRowChallenge
{
    internal class Stat
    {
        public double Min = Double.MaxValue;
        public double Max = Double.MinValue;
        public double Sum;
        public long Count;

        public void Add(double value)
        {
            if(value < Min) Min = value;
            if(value > Max) Max = value;
            Sum += value;
            ++Count;
        }

        public double Average() => Sum / Count;
    }
}
