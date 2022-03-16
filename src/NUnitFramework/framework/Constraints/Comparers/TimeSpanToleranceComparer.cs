// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="DateTime"/>s or <see cref="TimeSpan"/>s.
    /// </summary>
    internal sealed class TimeSpanToleranceComparer : IChainComparer
    {
        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (tolerance?.Amount is TimeSpan amount)
            {
                if (x is DateTime xDateTime && y is DateTime yDateTime)
                    return (xDateTime - yDateTime).Duration() <= amount;

                if (x is TimeSpan xTimeSpan && y is TimeSpan yTimeSpan)
                    return (xTimeSpan - yTimeSpan).Duration() <= amount;
            }

            return null;
        }
    }
}
