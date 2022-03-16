// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DateTimeOffset"/>s.
    /// </summary>
    internal sealed class DateTimeOffsetsComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal DateTimeOffsetsComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is DateTimeOffset xOffset) || !(y is DateTimeOffset yOffset))
                return null;

            bool result;

            if (tolerance?.Amount is TimeSpan amount)
            {
                result = (xOffset - yOffset).Duration() <= amount;
            }
            else
            {
                result = xOffset == yOffset;
            }

            if (result && _equalityComparer.WithSameOffset)
            {
                result = xOffset.Offset == yOffset.Offset;
            }

            return result;
        }
    }
}
