// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The DateTimes class contains common operations on Date and Time values.
    /// </summary>
    public static class DateTimes
    {
        #region DateTimes Difference

        internal static object Difference(object x, object y)
        {
            if (x is DateTime && y is DateTime)
                return ((DateTime)x - (DateTime)y).Duration();

            if (x is TimeSpan && y is TimeSpan)
                return ((TimeSpan)x - (TimeSpan)y).Duration();

            if (x is DateTimeOffset && y is DateTimeOffset)
                return ((DateTimeOffset)x - (DateTimeOffset)y).Duration();

            throw new ArgumentException("Both arguments must be DateTime, DateTimeOffset or TimeSpan");
        }

        #endregion
    }
}
