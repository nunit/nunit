// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Internal.Abstractions
{
    internal sealed class BitArrayBits : IBits
    {
        private readonly BitArray _bits;
        private int _bitsSet;

        public BitArrayBits(int length)
        {
            _bits = new BitArray(length);
        }

        public int Capacity => _bits.Length;

        public int BitsSet => _bitsSet;

        public void Set(int index)
        {
            _bits.Set(index, true);
            _bitsSet++;
        }

        public bool Get(int index) => _bits.Get(index);
    }
}
