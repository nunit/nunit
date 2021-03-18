// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// PrefixOperator takes a single constraint and modifies
    /// its action in some way.
    /// </summary>
    public abstract class PrefixOperator : ConstraintOperator
    {
        /// <summary>
        /// Reduce produces a constraint from the operator and 
        /// any arguments. It takes the arguments from the constraint 
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        /// <param name="stack"></param>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            stack.Push(ApplyPrefix(stack.Pop()));
        }

        /// <summary>
        /// Returns the constraint created by applying this
        /// prefix to another constraint.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public abstract IConstraint ApplyPrefix(IConstraint constraint);
    }
}
