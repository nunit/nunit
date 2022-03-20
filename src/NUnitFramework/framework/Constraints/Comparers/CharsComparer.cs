// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Char"/>s.
    /// </summary>
    internal static class CharsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!(x is char xChar) || !(y is char yChar))
                return null;

            bool caseInsensitive = equalityComparer.IgnoreCase;

            char c1 = caseInsensitive ? Char.ToLower(xChar) : xChar;
            char c2 = caseInsensitive ? Char.ToLower(yChar) : yChar;

            return c1 == c2;
        }
    }
}
