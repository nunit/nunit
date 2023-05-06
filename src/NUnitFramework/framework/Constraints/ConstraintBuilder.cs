// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ConstraintBuilder maintains the stacks that are used in
    /// processing a ConstraintExpression. An OperatorStack
    /// is used to hold operators that are waiting for their
    /// operands to be reorganized. a ConstraintStack holds
    /// input constraints as well as the results of each
    /// operator applied.
    /// </summary>
    public sealed class ConstraintBuilder : IResolveConstraint
    {
        #region Nested Operator Stack Class

        /// <summary>
        /// OperatorStack is a type-safe stack for holding ConstraintOperators
        /// </summary>
        private sealed class OperatorStack
        {
            private readonly Stack<ConstraintOperator> _stack = new Stack<ConstraintOperator>();

            /// <summary>
            /// Initializes a new instance of the <see cref="OperatorStack"/> class.
            /// </summary>
            public OperatorStack()
            {
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="OperatorStack"/> is empty.
            /// </summary>
            /// <value><see langword="true"/> if empty; otherwise, <see langword="false"/>.</value>
            public bool Empty => _stack.Count == 0;

            /// <summary>
            /// Gets the topmost operator without modifying the stack.
            /// </summary>
            public ConstraintOperator Top => _stack.Peek();

            /// <summary>
            /// Pushes the specified operator onto the stack.
            /// </summary>
            /// <param name="op">The operator to put onto the stack.</param>
            public void Push(ConstraintOperator op)
            {
                _stack.Push(op);
            }

            /// <summary>
            /// Pops the topmost operator from the stack.
            /// </summary>
            /// <returns>The topmost operator on the stack</returns>
            public ConstraintOperator Pop()
            {
                return _stack.Pop();
            }
        }

        #endregion

        #region Nested Constraint Stack Class

        /// <summary>
        /// ConstraintStack is a type-safe stack for holding Constraints
        /// </summary>
        public sealed class ConstraintStack
        {
            private readonly Stack<IConstraint> _stack = new Stack<IConstraint>();
            private readonly ConstraintBuilder _builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstraintStack"/> class.
            /// </summary>
            /// <param name="builder">The ConstraintBuilder using this stack.</param>
            public ConstraintStack(ConstraintBuilder builder)
            {
                _builder = builder;
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="ConstraintStack"/> is empty.
            /// </summary>
            /// <value><see langword="true"/> if empty; otherwise, <see langword="false"/>.</value>
            public bool Empty => _stack.Count == 0;

            /// <summary>
            /// Pushes the specified constraint. As a side effect,
            /// the constraint's Builder field is set to the
            /// ConstraintBuilder owning this stack.
            /// </summary>
            /// <param name="constraint">The constraint to put onto the stack</param>
            public void Push(IConstraint constraint)
            {
                _stack.Push(constraint);
                constraint.Builder = _builder;
            }

            /// <summary>
            /// Pops this topmost constraint from the stack.
            /// As a side effect, the constraint's Builder
            /// field is set to null.
            /// </summary>
            /// <returns>The topmost constraint on the stack</returns>
            public IConstraint Pop()
            {
                IConstraint constraint = _stack.Pop();
                constraint.Builder = null;
                return constraint;
            }
        }

        #endregion

        #region Instance Fields

        private readonly OperatorStack _ops;

        private readonly ConstraintStack _constraints;

        private object? _lastPushed;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintBuilder"/> class.
        /// </summary>
        public ConstraintBuilder()
        {
            _ops = new OperatorStack();
            _constraints = new ConstraintStack(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Appends the specified operator to the expression by first
        /// reducing the operator stack and then pushing the new
        /// operator on the stack.
        /// </summary>
        /// <param name="op">The operator to push.</param>
        public void Append(ConstraintOperator op)
        {
            op.LeftContext = _lastPushed;
            if (_lastPushed is ConstraintOperator)
                SetTopOperatorRightContext(op);

            // Reduce any lower precedence operators
            ReduceOperatorStack(op.LeftPrecedence);

            _ops.Push(op);
            _lastPushed = op;
        }

        /// <summary>
        /// Appends the specified constraint to the expression by pushing
        /// it on the constraint stack.
        /// </summary>
        /// <param name="constraint">The constraint to push.</param>
        public void Append(Constraint constraint)
        {
            if (_lastPushed is ConstraintOperator)
                SetTopOperatorRightContext(constraint);

            _constraints.Push(constraint);
            _lastPushed = constraint;
            constraint.Builder = this;
        }

        /// <summary>
        /// Sets the top operator right context.
        /// </summary>
        /// <param name="rightContext">The right context.</param>
        private void SetTopOperatorRightContext(object rightContext)
        {
            // Some operators change their precedence based on
            // the right context - save current precedence.
            int oldPrecedence = _ops.Top.LeftPrecedence;

            _ops.Top.RightContext = rightContext;

            // If the precedence increased, we may be able to
            // reduce the region of the stack below the operator
            if (_ops.Top.LeftPrecedence > oldPrecedence)
            {
                ConstraintOperator changedOp = _ops.Pop();
                ReduceOperatorStack(changedOp.LeftPrecedence);
                _ops.Push(changedOp);
            }
        }

        /// <summary>
        /// Reduces the operator stack until the topmost item
        /// precedence is greater than or equal to the target precedence.
        /// </summary>
        /// <param name="targetPrecedence">The target precedence.</param>
        private void ReduceOperatorStack(int targetPrecedence)
        {
            while (!_ops.Empty && _ops.Top.RightPrecedence < targetPrecedence)
                _ops.Pop().Reduce(_constraints);
        }

        #endregion

        #region IResolveConstraint Implementation

        /// <summary>
        /// Resolves this instance, returning a Constraint. If the Builder
        /// is not currently in a resolvable state, an exception is thrown.
        /// </summary>
        /// <returns>The resolved constraint</returns>
        public IConstraint Resolve()
        {
            if (!IsResolvable)
                throw new InvalidOperationException("A partial expression may not be resolved");

            while (!_ops.Empty)
            {
                ConstraintOperator op = _ops.Pop();
                op.Reduce(_constraints);
            }

            return _constraints.Pop();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a value indicating whether this instance is resolvable.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if this instance is resolvable; otherwise, <see langword="false"/>.
        /// </value>
        private bool IsResolvable => _lastPushed is Constraint || _lastPushed is SelfResolvingOperator;

        #endregion
    }
}
