// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// TrueConstraint tests that the actual value is true
    /// </summary>
    public class TrueConstraint : Constraint<bool?>
    {
        /// <inheritdoc/>
        public override string Description => "True";

        /// <inheritdoc/>
        public override ConstraintResult ApplyTo(bool? actual)
        {
            return new ConstraintResult(this, actual, true.Equals(actual));
        }
    }
}
