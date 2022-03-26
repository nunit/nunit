// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="String"/>s.
    /// </summary>
    internal static class StringsComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!(x is string xString) || !(y is string yString))
                return null;

            if (equalityComparer.IgnoreCase)
                return xString.Equals(yString, StringComparison.CurrentCultureIgnoreCase);

            return xString.Equals(yString);
        }
    }
}
