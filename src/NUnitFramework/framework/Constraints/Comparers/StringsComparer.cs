// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="String"/>s.
    /// </summary>
    internal sealed class StringsComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal StringsComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is string xString) || !(y is string yString))
                return null;

            bool caseInsensitive = _equalityComparer.IgnoreCase;

            string s1 = caseInsensitive ? xString.ToLower() : xString;
            string s2 = caseInsensitive ? yString.ToLower() : yString;

            return s1.Equals(s2);
        }
    }
}
