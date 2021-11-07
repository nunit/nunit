// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Char"/>s.
    /// </summary>
    internal sealed class CharsComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal CharsComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!(x is char) || !(y is char))
                return null;

            char xChar = (char)x;
            char yChar = (char)y;

            bool caseInsensitive = _equalityComparer.IgnoreCase;

            char c1 = caseInsensitive ? Char.ToLower(xChar) : xChar;
            char c2 = caseInsensitive ? Char.ToLower(yChar) : yChar;

            return c1 == c2;
        }
    }
}
