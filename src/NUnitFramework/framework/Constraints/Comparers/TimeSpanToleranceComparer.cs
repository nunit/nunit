// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DateTime"/>s or <see cref="TimeSpan"/>s.
    /// </summary>
    internal static class TimeSpanToleranceComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (tolerance?.Amount is TimeSpan amount)
            {
                bool result;

                if (x is DateTime xDateTime && y is DateTime yDateTime)
                    result = (xDateTime - yDateTime).Duration() <= amount;
                else if (x is TimeSpan xTimeSpan && y is TimeSpan yTimeSpan)
                    result = (xTimeSpan - yTimeSpan).Duration() <= amount;
                else
                    return EqualMethodResult.TypesNotSupported;

                return result ?
                    EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
            }

            return EqualMethodResult.TypesNotSupported;
        }
    }
}
