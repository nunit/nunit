// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator that tests that an exception is thrown and
    /// optionally applies further tests to the exception.
    /// </summary>
    public class ThrowsOperator : SelfResolvingOperator
    {
        /// <summary>
        /// Construct a ThrowsOperator
        /// </summary>
        public ThrowsOperator()
        {
            // ThrowsOperator stacks on everything but
            // it's always the first item on the stack
            // anyway. It is evaluated last of all ops.
            this.left_precedence = 1;
            this.right_precedence = 100;
        }

        /// <summary>
        /// Reduce produces a constraint from the operator and
        /// any arguments. It takes the arguments from the constraint
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext is null || RightContext is BinaryOperator)
                stack.Push(new ThrowsExceptionConstraint());
            else
                stack.Push(new ThrowsConstraint(stack.Pop()));
        }
    }
}
