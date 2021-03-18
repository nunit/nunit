// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Negates the test of the constraint it wraps.
    /// </summary>
    public class NotOperator : PrefixOperator
    {
        /// <summary>
        /// Constructs a new NotOperator
        /// </summary>
        public NotOperator()
        {
            // Not stacks on anything and only allows other
            // prefix ops to stack on top of it.
            this.left_precedence = this.right_precedence = 1;
        }

        /// <summary>
        /// Returns a NotConstraint applied to its argument.
        /// </summary>
        public override IConstraint ApplyPrefix(IConstraint constraint)
        {
            return new NotConstraint(constraint);
        }
    }
}