// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Array"/>s.
    /// </summary>
    internal static class ArraysComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!x.GetType().IsArray || !y.GetType().IsArray || equalityComparer.CompareAsCollection)
                return null;

            Array xArray = (Array)x;
            Array yArray = (Array)y;

            int rank = xArray.Rank;

            if (rank != yArray.Rank)
                return false;

            for (int r = 1; r < rank; r++)
            {
                if (xArray.GetLength(r) != yArray.GetLength(r))
                    return false;
            }

            return EnumerablesComparer.Equal(xArray, yArray, ref tolerance, state, equalityComparer);
        }
    }
}
