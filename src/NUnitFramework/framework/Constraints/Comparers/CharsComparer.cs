// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="char"/>s.
    /// </summary>
    internal static class CharsComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (x is not char xChar || y is not char yChar)
                return EqualMethodResult.TypesNotSupported;

            if (tolerance.HasVariance)
                return EqualMethodResult.ToleranceNotSupported;

            bool caseInsensitive = equalityComparer.IgnoreCase;

            char c1 = caseInsensitive ? char.ToLower(xChar) : xChar;
            char c2 = caseInsensitive ? char.ToLower(yChar) : yChar;

            return c1 == c2 ?
                EqualMethodResult.ComparedEqual : EqualMethodResult.ComparedNotEqual;
        }
    }
}
