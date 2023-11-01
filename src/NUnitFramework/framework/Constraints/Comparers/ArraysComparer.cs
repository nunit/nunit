// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="Array"/>s.
    /// </summary>
    internal static class ArraysComparer
    {
        public static EqualMethodResult Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (!x.GetType().IsArray || !y.GetType().IsArray || equalityComparer.CompareAsCollection)
                return EqualMethodResult.TypesNotSupported;

            Array xArray = (Array)x;
            Array yArray = (Array)y;

            int rank = xArray.Rank;

            if (rank != yArray.Rank)
                return EqualMethodResult.ComparedNotEqual;

            for (int r = 1; r < rank; r++)
            {
                if (xArray.GetLength(r) != yArray.GetLength(r))
                    return EqualMethodResult.ComparedNotEqual;
            }

            return EnumerablesComparer.Equal(xArray, yArray, ref tolerance, state, equalityComparer);
        }
    }
}
