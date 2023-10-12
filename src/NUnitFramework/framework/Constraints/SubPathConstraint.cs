// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SubPathConstraint tests that the actual path is under the expected path
    /// </summary>
    public class SubPathConstraint : PathConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubPathConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected path</param>
        public SubPathConstraint(string expected) : base(expected)
        {
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "Subpath of " + MsgUtils.FormatValue(expected);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string actual)
        {
            return actual is not null && IsSubPath(Canonicalize(expected), Canonicalize(actual));
        }
    }
}
