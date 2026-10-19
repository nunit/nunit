// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Abstractions
{
    internal static class BitsFactory
    {
        /// <summary>
        /// Creates an <see cref="IBits"/> instance sized for the requested capacity.
        /// </summary>
        /// <remarks>
        /// The implementations do not check the supplied argument for validity,
        /// so the caller must ensure that the index is within the range of 0 to Capacity - 1.
        /// </remarks>
        /// <param name="capacity">The number of bits to support. Must be greater than zero.</param>
        /// <returns>An IBits implementation that can represent the requested number of bits.</returns>
        public static IBits Create(int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity, nameof(capacity));

            if (capacity <= 32)
                return new Int32Bits();
            else if (capacity <= 64)
                return new Int64Bits();
            else
                return new BitArrayBits(capacity);
        }
    }
}
