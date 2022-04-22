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
            if (x is DateTime xDateTime && y is DateTime yDateTime)
                return (xDateTime - yDateTime).Duration();

            if (x is TimeSpan xTimeSpan && y is TimeSpan yTimeSpan)
                return (xTimeSpan - yTimeSpan).Duration();

            if (x is DateTimeOffset xDateTimeOffset && y is DateTimeOffset yDateTimeOffset)
                return (xDateTimeOffset - yDateTimeOffset).Duration();

            throw new ArgumentException("Both arguments must be DateTime, DateTimeOffset or TimeSpan");
        }

        #endregion
    }
}
