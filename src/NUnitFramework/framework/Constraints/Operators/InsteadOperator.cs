// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator that replaces the left constraint with the right constraint
    /// </summary>
    public class InsteadOperator : BinaryOperator
    {
        /// <summary>
        /// Construct an InsteadOperator
        /// </summary>
        public InsteadOperator()
        {
            left_precedence = 1;
            right_precedence = 99;
        }

        /// <summary>
        /// Apply the operator to replace the left constraint
        /// </summary>
        public override IConstraint ApplyOperator(IConstraint left, IConstraint right)
        {
            return right;
        }
    }
}
