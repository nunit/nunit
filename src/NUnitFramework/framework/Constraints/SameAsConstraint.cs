// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SameAsConstraint tests whether an object is identical to
    /// the object passed to its constructor
    /// </summary>
    public class SameAsConstraint : Constraint
    {
        private readonly object expected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SameAsConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        public SameAsConstraint(object expected) : base(expected)
        {
            this.expected = expected;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "same as " + MsgUtils.FormatValue(expected);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool hasSucceeded = ReferenceEquals(expected, actual);

            return new ConstraintResult(this, actual, hasSucceeded);
        }
    }
}
