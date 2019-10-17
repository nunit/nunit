using System.Collections.Generic;

namespace NUnit.Framework.Constraints.Comparers
{
    internal struct ComparisonState
    {
        public struct Comparison
        {
            public object X { get; set; }
            public object Y { get; set; }
        }

        /// <summary>
        /// Flag indicating whether or not this is the top level comparison.
        /// </summary>
        public bool TopLevelComparison { get; set; }
        public List<Comparison> Comparisons { get; set; }

        public void RecordComparison(object x, object y)
        {
            Comparisons.Add(new Comparison()
            {
                X = x,
                Y = y
            });
        }

        public bool HasCompared(object x, object y)
        {
            foreach (var item in Comparisons)
                if (item.X == x && item.Y == y)
                    return true;
            return false;
        }
    }
}
