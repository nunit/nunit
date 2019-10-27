using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    internal struct ComparisonState
    {
        /// <summary>
        /// Flag indicating whether or not this is the top level comparison.
        /// </summary>
        public readonly bool TopLevelComparison;

        /// <summary>
        /// A list of tracked comparisons
        /// </summary>
        private readonly Comparison[] _comparisons;

        public ComparisonState(bool topLevelComparison)
        {
            TopLevelComparison = topLevelComparison;
            _comparisons = new Comparison[0];
        }

        private ComparisonState(bool topLevelComparison, Comparison[] comparisons)
        {
            TopLevelComparison = topLevelComparison;
            _comparisons = comparisons;
        }

        public ComparisonState PushComparison(bool topLevelComparison, object x, object y)
        {
            var comparisons = new Comparison[_comparisons.Length + 1];
            _comparisons.CopyTo(comparisons, 0);
            comparisons[_comparisons.Length] = new Comparison(x, y);

            return new ComparisonState(
                topLevelComparison,
                comparisons
            );
        }

        public bool DidCompare(object x, object y)
        {
            for(var i = 0; i < _comparisons.Length; i++)
                if (_comparisons[i].X == x && _comparisons[i].Y == y)
                    return true;

            return false;
        }

        private struct Comparison
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
