namespace NUnit.Framework.Constraints.Comparers
{
    internal struct ComparisonState
    {
        /// <summary>
        /// Flag indicating whether or not this is the top level comparison.
        /// </summary>
        public bool TopLevelComparison { get; set; }
    }
}
