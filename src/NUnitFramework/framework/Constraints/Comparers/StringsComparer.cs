// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="String"/>s.
    /// </summary>
    internal static class StringsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not string xString || y is not string yString)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            return Equals(xString, yString, equalityComparer.IgnoreCase, equalityComparer.IgnoreWhiteSpace, equalityComparer.IgnoreLineEndingFormat) ?
                EqualMethodResult.ComparedEqual :
                EqualMethodResult.ComparedNotEqual;
        }

        public static bool Equals(string x, string y, bool ignoreCase, bool ignoreWhiteSpace, bool ignoreLineEndingFormat)
        {
            if (ignoreWhiteSpace)
            {
                (int mismatchExpected, int mismatchActual) = MsgUtils.FindMismatchPosition(x, y, ignoreCase, ignoreWhiteSpace: true, /* not used but required */ignoreLineEndingFormat);
                return mismatchExpected == -1 && mismatchActual == -1;
            }
            else
            {
                IEqualityComparer<string> comparer = (ignoreCase, ignoreLineEndingFormat) switch
                {
                    (true, true) => IgnoreLineEndingFormatStringComparer.CurrentCultureIgnoreCase,
                    (true, false) => StringComparer.CurrentCultureIgnoreCase,
                    (false, true) => IgnoreLineEndingFormatStringComparer.Ordinal,
                    (false, false) => StringComparer.Ordinal,
                };
                return comparer.Equals(x, y);
            }
        }
    }
}
