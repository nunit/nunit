// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints.Comparers
{
    internal readonly ref struct ComparisonState
    {
        /// <summary>
        /// Flag indicating whether or not this is the top level comparison.
        /// </summary>
        public bool TopLevelComparison { get; }

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
            public object X { get; }
            public object Y { get; }

            public Comparison(object x, object y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
