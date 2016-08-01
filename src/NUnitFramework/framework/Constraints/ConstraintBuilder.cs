// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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
    public class ConstraintBuilder : IResolveConstraint
    {
        #region Nested Operator Stack Class

        /// <summary>
        /// OperatorStack is a type-safe stack for holding ConstraintOperators
        /// </summary>
        public class OperatorStack
        {
            private readonly Stack<ConstraintOperator> stack = new Stack<ConstraintOperator>();

            /// <summary>
            /// Initializes a new instance of the <see cref="OperatorStack"/> class.
            /// </summary>
            /// <param name="builder">The ConstraintBuilder using this stack.</param>
            public OperatorStack(ConstraintBuilder builder)
            {
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="OperatorStack"/> is empty.
            /// </summary>
            /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
            public bool Empty
            {
                get { return stack.Count == 0; }
            }

            /// <summary>
            /// Gets the topmost operator without modifying the stack.
            /// </summary>
            public ConstraintOperator Top
            {
                get { return stack.Peek(); }
            }

            /// <summary>
            /// Pushes the specified operator onto the stack.
            /// </summary>
            /// <param name="op">The operator to put onto the stack.</param>
            public void Push(ConstraintOperator op)
            {
                stack.Push(op);
            }

            /// <summary>
            /// Pops the topmost operator from the stack.
            /// </summary>
            /// <returns>The topmost operator on the stack</returns>
            public ConstraintOperator Pop()
            {
                return stack.Pop();
            }
        }

        #endregion

        #region Nested Constraint Stack Class

        /// <summary>
        /// ConstraintStack is a type-safe stack for holding Constraints
        /// </summary>
        public class ConstraintStack
        {
            private readonly Stack<IConstraint> stack = new Stack<IConstraint>();
            private readonly ConstraintBuilder builder;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConstraintStack"/> class.
            /// </summary>
            /// <param name="builder">The ConstraintBuilder using this stack.</param>
            public ConstraintStack(ConstraintBuilder builder)
            {
                this.builder = builder;
            }

            /// <summary>
            /// Gets a value indicating whether this <see cref="ConstraintStack"/> is empty.
            /// </summary>
            /// <value><c>true</c> if empty; otherwise, <c>false</c>.</value>
            public bool Empty
            {
                get { return stack.Count == 0; }
            }

            /// <summary>
            /// Pushes the specified constraint. As a side effect,
            /// the constraint's Builder field is set to the 
            /// ConstraintBuilder owning this stack.
            /// </summary>
            /// <param name="constraint">The constraint to put onto the stack</param>
            public void Push(IConstraint constraint)
            {
                stack.Push(constraint);
                constraint.Builder = this.builder;
            }

            /// <summary>
            /// Pops this topmost constraint from the stack.
            /// As a side effect, the constraint's Builder
            /// field is set to null.
            /// </summary>
            /// <returns>The topmost contraint on the stack</returns>
            public IConstraint Pop()
            {
                IConstraint constraint = stack.Pop();
                constraint.Builder = null;
                return constraint;
            }
        }

        #endregion

        #region Instance Fields

        private readonly OperatorStack ops;

        private readonly ConstraintStack constraints;

        private object lastPushed;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintBuilder"/> class.
        /// </summary>
        public ConstraintBuilder()
        {
            this.ops = new OperatorStack(this);
            this.constraints = new ConstraintStack(this);
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
            op.LeftContext = lastPushed;
            if (lastPushed is ConstraintOperator)
                SetTopOperatorRightContext(op);

            // Reduce any lower precedence operators
            ReduceOperatorStack(op.LeftPrecedence);
            
            ops.Push(op);
            lastPushed = op;
        }

        /// <summary>
        /// Appends the specified constraint to the expression by pushing
        /// it on the constraint stack.
        /// </summary>
        /// <param name="constraint">The constraint to push.</param>
        public void Append(Constraint constraint)
        {
            if (lastPushed is ConstraintOperator)
                SetTopOperatorRightContext(constraint);

            constraints.Push(constraint);
            lastPushed = constraint;
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
            int oldPrecedence = ops.Top.LeftPrecedence;

            ops.Top.RightContext = rightContext;

            // If the precedence increased, we may be able to
            // reduce the region of the stack below the operator
            if (ops.Top.LeftPrecedence > oldPrecedence)
            {
                ConstraintOperator changedOp = ops.Pop();
                ReduceOperatorStack(changedOp.LeftPrecedence);
                ops.Push(changedOp);
            }
        }

        /// <summary>
        /// Reduces the operator stack until the topmost item
        /// precedence is greater than or equal to the target precedence.
        /// </summary>
        /// <param name="targetPrecedence">The target precedence.</param>
        private void ReduceOperatorStack(int targetPrecedence)
        {
            while (!ops.Empty && ops.Top.RightPrecedence < targetPrecedence)
                ops.Pop().Reduce(constraints);
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

            while (!ops.Empty)
            {
                ConstraintOperator op = ops.Pop();
                op.Reduce(constraints);
            }

            return constraints.Pop();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a value indicating whether this instance is resolvable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is resolvable; otherwise, <c>false</c>.
        /// </value>
        private bool IsResolvable
        {
            get { return lastPushed is Constraint || lastPushed is SelfResolvingOperator; }
        }

        #endregion
    }
}
