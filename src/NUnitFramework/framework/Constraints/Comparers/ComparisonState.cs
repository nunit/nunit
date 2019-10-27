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
        private readonly List<Comparison> _comparisons;

        public ComparisonState(bool topLevelComparison)
        {
            TopLevelComparison = topLevelComparison;
            _comparisons = new List<Comparison>();
        }

        private ComparisonState(bool topLevelComparison, List<Comparison> comparisons)
        {
            TopLevelComparison = topLevelComparison;
            _comparisons = new List<Comparison>();
            _comparisons.AddRange(comparisons);
        }

        public ComparisonState WithTopLevelComparison(bool topLevelComparison)
        {
            return new ComparisonState(
                topLevelComparison,
                _comparisons
            );
        }

        public bool RecordComparison(object x, object y)
        {
            if (DidCompare(x, y))
                return false;

            _comparisons.Add(new Comparison(x, y));
            return true;
        }

        private bool DidCompare(object x, object y)
        {
            foreach (var item in _comparisons)
                if (item.X == x && item.Y == y)
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
