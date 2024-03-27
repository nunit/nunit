// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// SamePathOrUnderConstraint tests that one path is under another
    /// </summary>
    public class SamePathOrUnderConstraint : PathConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SamePathOrUnderConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected path</param>
        public SamePathOrUnderConstraint(string expected) : base(expected)
        {
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => "Path under or matching " + MsgUtils.FormatValue(expected);

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual)
        {
            if (actual is null)
                return false;

            string path1 = Canonicalize(expected);
            string path2 = Canonicalize(actual);
            return StringUtil.StringsEqual(path1, path2, caseInsensitive) || IsSubPath(path1, path2);
        }
    }
}
