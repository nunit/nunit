// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
