// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The ConstraintOperator class is used internally by a
    /// ConstraintBuilder to represent an operator that
    /// modifies or combines constraints.
    /// 
    /// Constraint operators use left and right precedence
    /// values to determine whether the top operator on the
    /// stack should be reduced before pushing a new operator.
    /// </summary>
    public abstract class ConstraintOperator
    {
        private object? leftContext;
        private object? rightContext;

        /// <summary>
        /// The precedence value used when the operator
        /// is about to be pushed to the stack.
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected int left_precedence;
#pragma warning restore IDE1006

        /// <summary>
        /// The precedence value used when the operator
        /// is on the top of the stack.
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected int right_precedence;
#pragma warning restore IDE1006

        /// <summary>
        /// The syntax element preceding this operator
        /// </summary>
        public object? LeftContext
        {
            get => leftContext;
            set => leftContext = value;
        }

        /// <summary>
        /// The syntax element following this operator
        /// </summary>
        public object? RightContext
        {
            get => rightContext;
            set => rightContext = value;
        }

        /// <summary>
        /// The precedence value used when the operator
        /// is about to be pushed to the stack.
        /// </summary>
        public virtual int LeftPrecedence => left_precedence;

        /// <summary>
        /// The precedence value used when the operator
        /// is on the top of the stack.
        /// </summary>
        public virtual int RightPrecedence => right_precedence;

        /// <summary>
        /// Reduce produces a constraint from the operator and
        /// any arguments. It takes the arguments from the constraint
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        /// <param name="stack"></param>
        public abstract void Reduce(ConstraintBuilder.ConstraintStack stack);
    }
}
