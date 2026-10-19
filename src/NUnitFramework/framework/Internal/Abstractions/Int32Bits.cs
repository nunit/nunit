// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Abstractions
{
    internal sealed class Int32Bits : IBits
    {
        private uint _bits;
        private int _bitsSet;

        public int Capacity => 32;

        public int BitsSet => _bitsSet;

        public void Set(int index)
        {
            _bits |= 1U << index;
            _bitsSet++; // Simplistic increment; assumes caller does not set the same bit multiple times
        }

        public bool Get(int index)
        {
            return (_bits & (1U << index)) != 0;
        }
    }
}
