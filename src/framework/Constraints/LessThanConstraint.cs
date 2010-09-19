
namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Tests whether a value is less than the value supplied to its constructor
    /// </summary>
    public class LessThanConstraint : ComparisonConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LessThanConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public LessThanConstraint(object expected) : base(expected, true, false, false, "less than") { }
    }
}