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

            var stringComparison = equalityComparer.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.Ordinal;
            return xString.Equals(yString, stringComparison) ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
