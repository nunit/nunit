// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Represents a constraint that succeeds if the specified 
    /// count of members of a collection match a base constraint.
    /// </summary>
    public class ExactCountOperator : SelfResolvingOperator
    {
        private readonly int expectedCount;

        /// <summary>
        /// Construct an ExactCountOperator for a specified count
        /// </summary>
        /// <param name="expectedCount">The expected count</param>
        public ExactCountOperator(int expectedCount)
        {
            // Collection Operators stack on everything
            // and allow all other ops to stack on them
            this.left_precedence = 1;
            this.right_precedence = 10;

            this.expectedCount = expectedCount;
        }

        /// <summary>
        /// Reduce produces a constraint from the operator and 
        /// any arguments. It takes the arguments from the constraint 
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        /// <param name="stack"></param>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext == null || RightContext is BinaryOperator)
                stack.Push(new ExactCountConstraint(expectedCount));
            else
                stack.Push(new ExactCountConstraint(expectedCount, stack.Pop()));
        }
    }
}

