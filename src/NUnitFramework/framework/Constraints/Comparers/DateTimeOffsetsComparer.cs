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
            if (!(x is DateTimeOffset) || !(y is DateTimeOffset))
                return null;

            bool result;

            DateTimeOffset xOffset = (DateTimeOffset)x;
            DateTimeOffset yOffset = (DateTimeOffset)y;

            if (tolerance != null && tolerance.Amount is TimeSpan)
            {
                TimeSpan amount = (TimeSpan)tolerance.Amount;
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
