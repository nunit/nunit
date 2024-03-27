// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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

            return Equals(xString, yString, equalityComparer.IgnoreCase, equalityComparer.IgnoreWhiteSpace) ?
                EqualMethodResult.ComparedEqual :
                EqualMethodResult.ComparedNotEqual;
        }

        public static bool Equals(string x, string y, bool ignoreCase, bool ignoreWhiteSpace)
        {
            if (ignoreWhiteSpace)
            {
                (int mismatchExpected, int mismatchActual) = MsgUtils.FindMismatchPosition(x, y, ignoreCase, true);
                return mismatchExpected == -1 && mismatchActual == -1;
            }
            else
            {
                return x.Equals(y, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.Ordinal);
            }
        }
    }
}
