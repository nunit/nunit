// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DateTimeOffset"/>s.
    /// </summary>
    internal static class DateTimeOffsetsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
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

            if (result && equalityComparer.WithSameOffset)
            {
                result = xOffset.Offset == yOffset.Offset;
            }

            return result;
        }
    }
}
