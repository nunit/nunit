// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if !NET35
using System.Collections;

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two types related by <see cref="IStructuralEquatable"/>.
    /// </summary>
    class StructuralComparer : IChainComparer
    {
        private readonly NUnitEqualityComparer _equalityComparer;

        internal StructuralComparer(NUnitEqualityComparer equalityComparer)
        {
            _equalityComparer = equalityComparer;
        }

        public bool? Equal(object x, object y, ref Tolerance tolerance, ComparisonState state)
        {
            if (_equalityComparer.CompareAsCollection && state.TopLevelComparison)
                return null;

            if (x is IStructuralEquatable xEquatable && y is IStructuralEquatable yEquatable)
            {
                // We can't pass tolerance as a ref so we pass by value and reassign later
                var equalityComparison = new NUnitEqualityComparison(_equalityComparer, tolerance);

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
            public Tolerance Tolerance { get { return _tolerance; } }

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
#endif
