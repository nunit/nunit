// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Summary description for SamePathConstraint.
    /// </summary>
    public class SamePathConstraint : PathConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SamePathConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected path</param>
        public SamePathConstraint(string expected) : base(expected)
        {
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "Path matching " + MsgUtils.FormatValue(expected);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual)
        {
            return actual is not null && StringUtil.StringsEqual(Canonicalize(expected), Canonicalize(actual), caseInsensitive);
        }
    }
}
