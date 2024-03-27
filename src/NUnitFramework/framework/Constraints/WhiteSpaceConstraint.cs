// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// WhiteSpaceConstraint tests whether a string contains white space.
    /// </summary>
    public class WhiteSpaceConstraint : StringConstraint
    {
        private const string WhiteSpace = "white-space";

        /// <inheritdoc/>
        public override string Description => WhiteSpace;

        /// <inheritdoc/>
        public override string DisplayName => WhiteSpace;

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        protected override bool Matches(string? actual)
        {
            return string.IsNullOrWhiteSpace(actual);
        }
    }
}
