// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints.Comparers
{
    internal readonly ref struct ComparisonState
    {
        /// <summary>
        /// Flag indicating whether or not this is the top level comparison.
        /// </summary>
        public readonly bool TopLevelComparison { get; }

        /// <summary>
        /// A list of tracked comparisons
        /// </summary>
        private readonly ImmutableStack<Comparison> _comparisons;

        public ComparisonState(bool topLevelComparison)
            : this(topLevelComparison, ImmutableStack<Comparison>.Empty)
        {
        }

        private ComparisonState(bool topLevelComparison, ImmutableStack<Comparison> comparisons)
        {
            TopLevelComparison = topLevelComparison;
            _comparisons = comparisons;
        }

        public ComparisonState PushComparison(object x, object y)
        {
            return new ComparisonState(
                false,
                _comparisons.Push(new Comparison(x, y))
            );
        }

        public bool DidCompare(object x, object y)
        {
            foreach (var comparison in _comparisons)
                if (ReferenceEquals(comparison.X, x) && ReferenceEquals(comparison.Y, y))
                    return true;

            return false;
        }

        private readonly struct Comparison
        {
            public readonly object X { get; }
            public readonly object Y { get; }

            public Comparison(object x, object y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
