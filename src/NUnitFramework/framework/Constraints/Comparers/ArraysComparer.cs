// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Array"/>s.
    /// </summary>
    internal sealed class ArraysComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;
        private readonly EnumerablesComparer _enumerablesComparer;

        internal ArraysComparer(NUnitEqualityComparer equalityComparer, EnumerablesComparer enumerablesComparer)
        {
            _equalityComparer = equalityComparer;
            _enumerablesComparer = enumerablesComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (!x.GetType().IsArray || !y.GetType().IsArray || _equalityComparer.CompareAsCollection)
                return null;

            Array xArray = (Array)x;
            Array yArray = (Array)y;

            int rank = xArray.Rank;

            if (rank != yArray.Rank)
                return false;

            for (int r = 1; r < rank; r++)
                if (xArray.GetLength(r) != yArray.GetLength(r))
                    return false;

            return _enumerablesComparer.Equal(xArray, yArray, ref tolerance, state);
        }
    }
}
