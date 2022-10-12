// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two types related by <see cref="IStructuralEquatable"/>.
    /// </summary>
    internal static class StructuralComparer
    {
        public static bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state, NUnitEqualityComparer equalityComparer)
        {
            if (equalityComparer.CompareAsCollection && state.TopLevelComparison)
                return null;

            if (x is IStructuralEquatable xEquatable && y is IStructuralEquatable yEquatable)
            {
                // We can't pass tolerance as a ref so we pass by value and reassign later
                var equalityComparison = new NUnitEqualityComparison(equalityComparer, tolerance);

                // Check both directions in case they are different implementations, and only one is aware of the other.
                // Like how ImmutableArray<int> is structurally equatable to int[]
                // but int[] is NOT structurally equatable to ImmutableArray<int>
                var xResult = xEquatable.Equals(y, equalityComparison);
                var yResult = yEquatable.Equals(x, equalityComparison);

                // Keep all the refs up to date
                tolerance = equalityComparison.Tolerance;

                return xResult || yResult;
            }

            return null;
        }

        private sealed class NUnitEqualityComparison : IEqualityComparer
        {
            private readonly NUnitEqualityComparer _comparer;

            private Tolerance _tolerance;
            public Tolerance Tolerance => _tolerance;

            public NUnitEqualityComparison(NUnitEqualityComparer comparer, Tolerance tolerance)
            {
                _comparer = comparer;
                _tolerance = tolerance;
            }

            public new bool Equals(object x, object y)
            {
                return _comparer.AreEqual(x, y, ref _tolerance);
            }

            public int GetHashCode(object obj)
            {
                // TODO: Better hashcode generation, likely based on the corresponding comparer to ensure types which can be
                // compared with each other end up in the same bucket
                return 0;
            }
        }
    }
}
