// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

namespace NUnit.Framework.Internal
{
    internal struct DecimalParts
    {
        public uint Low { get; }
        public uint Mid { get; }
        public uint High { get; }
        public bool IsNegative { get; }
        public byte Scale { get; }

        public static DecimalParts FromValue(decimal value)
        {
            var parts = decimal.GetBits(value);

            unchecked
            {
                var flags = (uint)parts[3];

                return new DecimalParts(
                    low: (uint)parts[0],
                    mid: (uint)parts[1],
                    high: (uint)parts[2],
                    isNegative: (flags & 0x80000000) != 0,
                    scale: (byte)(flags >> 16));
            }
        }

        private DecimalParts(uint low, uint mid, uint high, bool isNegative, byte scale)
        {
            Low = low;
            Mid = mid;
            High = high;
            IsNegative = isNegative;
            Scale = scale;
        }
    }
}
