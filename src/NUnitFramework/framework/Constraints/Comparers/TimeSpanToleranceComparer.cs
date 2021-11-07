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
            if (tolerance == null || !(tolerance.Amount is TimeSpan))
                return null;

            TimeSpan amount = (TimeSpan)tolerance.Amount;

            if (x is DateTime && y is DateTime)
                return ((DateTime)x - (DateTime)y).Duration() <= amount;

            if (x is TimeSpan && y is TimeSpan)
                return ((TimeSpan)x - (TimeSpan)y).Duration() <= amount;

            return null;
        }
    }
}
