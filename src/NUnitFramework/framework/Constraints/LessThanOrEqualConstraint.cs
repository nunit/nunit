// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Tests whether a value is less than or equal to the value supplied to its constructor
    /// </summary>
    public class LessThanOrEqualConstraint : ComparisonConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LessThanOrEqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public LessThanOrEqualConstraint(object expected)
            : base(expected, "less than or equal to ")
        {
        }

        /// <summary>
        /// Perform the comparison
        /// </summary>
        protected override bool PerformComparison(ComparisonAdapter comparer, object actual, object expected, Tolerance tolerance)
        {
            return comparer.Compare(actual, tolerance.ApplyToValue(expected).UpperBound) <= 0;
        }
    }
}
