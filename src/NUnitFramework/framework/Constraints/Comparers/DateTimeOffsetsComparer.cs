// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DateTimeOffset"/>s.
    /// </summary>
    internal static class DateTimeOffsetsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not DateTimeOffset xOffset || y is not DateTimeOffset yOffset)
                return EqualMethodResult.TypesNotSupported;

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

            return result ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
