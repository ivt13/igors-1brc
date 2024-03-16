
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace OneBillionRowChallenge
{
    internal readonly unsafe struct Key : IEquatable<Key>, IComparable<Key>
    {
        internal readonly byte* _ptr;
        internal readonly int _length;

        public Key(byte* ptr, int length)
        {
            _ptr = ptr;
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int IComparable<Key>.CompareTo(Key other)
        {
            var thisStr = Encoding.UTF8.GetString(_ptr, _length);
            var otherStr = Encoding.UTF8.GetString(other._ptr, other._length);
            return string.CompareOrdinal(thisStr,otherStr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IEquatable<Key>.Equals(Key other)
        {
            var thisSpan = new ReadOnlySpan<byte>(_ptr, _length);
            var otherSpan = new ReadOnlySpan<byte>(other._ptr, other._length);
            return thisSpan.SequenceEqual(otherSpan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? other)
        {
            if(!(other is Key)) return false;
            var otherKey = (Key)other;

            return ((IEquatable<Key>)this).Equals(otherKey);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            var hashcode = *((int*)_ptr);
            return hashcode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return Encoding.UTF8.GetString(_ptr, _length);
        }
    }
}
