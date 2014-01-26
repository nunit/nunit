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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AndConstraint succeeds only if both members succeed.
    /// </summary>
    public class AndConstraint : BinaryConstraint
    {
        //private enum FailurePoint
        //{
        //    None,
        //    Left,
        //    Right
        //};

        //private FailurePoint failurePoint;

        /// <summary>
        /// Create an AndConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        public AndConstraint(IConstraint left, IConstraint right) : base(left, right) { }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return Left.Description + " and " + Right.Description; }
        }

        /// <summary>
        /// Apply both member constraints to an actual value, succeeding 
        /// succeeding only if both of them succeed.
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <returns>True if the constraints both succeeded</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var leftResult = Left.ApplyTo(actual);
            var rightResult = leftResult.IsSuccess
                ? Right.ApplyTo(actual)
                : new ConstraintResult(Right, actual);

            return new AndConstraintResult(this, actual, leftResult, rightResult);
        }

        #region Nested Result Class

        class AndConstraintResult : ConstraintResult
        {
            private ConstraintResult leftResult;
            private ConstraintResult rightResult;

            public AndConstraintResult(AndConstraint constraint, object actual, ConstraintResult leftResult, ConstraintResult rightResult)
                : base(constraint, actual, leftResult.IsSuccess && rightResult.IsSuccess) 
            {
                this.leftResult = leftResult;
                this.rightResult = rightResult;
            }

            /// <summary>
            /// Write the actual value for a failing constraint test to a
            /// MessageWriter. The default implementation simply writes
            /// the raw value of actual, leaving it to the writer to
            /// perform any formatting.
            /// </summary>
            /// <param name="writer">The writer on which the actual value is displayed</param>
            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (this.IsSuccess)
                    base.WriteActualValueTo(writer);
                else if (!leftResult.IsSuccess)
                    leftResult.WriteActualValueTo(writer);
                else
                    rightResult.WriteActualValueTo(writer);
            }
        }

        #endregion
    }
}