// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ThrowsConstraint is used to test the exception thrown by 
    /// a delegate by applying a constraint to it.
    /// </summary>
    public class ThrowsConstraint : PrefixConstraint
    {
        private Exception caughtException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowsConstraint"/> class,
        /// using a constraint to be applied to the exception.
        /// </summary>
        /// <param name="baseConstraint">A constraint to apply to the caught exception.</param>
        public ThrowsConstraint(IConstraint baseConstraint)
            : base(baseConstraint) { }

        /// <summary>
        /// Get the actual exception thrown - used by Assert.Throws.
        /// </summary>
        public Exception ActualException
        {
            get { return caughtException; }
        }

        #region Constraint Overrides

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return BaseConstraint.Description; }
        }

        /// <summary>
        /// Executes the code of the delegate and captures any exception.
        /// If a non-null base constraint was provided, it applies that
        /// constraint to the exception.
        /// </summary>
        /// <param name="actual">A delegate representing the code to be tested</param>
        /// <returns>True if an exception is thrown and the constraint succeeds, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var @delegate = ConstraintUtils.RequireActual<Delegate>(actual, nameof(actual));

            caughtException = ExceptionHelper.RecordException(@delegate, nameof(actual));

            return new ThrowsConstraintResult(
                this,
                caughtException,
                caughtException != null
                    ? BaseConstraint.ApplyTo(caughtException)
                    : null);
        }

        /// <summary>
        /// Converts an ActualValueDelegate to a TestDelegate
        /// before calling the primary overload.
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return ApplyTo((Delegate)del);
        }

        #endregion

        #region Nested Result Class

        private sealed class ThrowsConstraintResult : ConstraintResult
        {
            private readonly ConstraintResult baseResult;

            public ThrowsConstraintResult(ThrowsConstraint constraint,
                Exception caughtException,
                ConstraintResult baseResult)
                : base(constraint, caughtException)
            {
                if (caughtException != null && baseResult.IsSuccess)
                    Status = ConstraintStatus.Success;
                else
                    Status = ConstraintStatus.Failure;

                this.baseResult = baseResult;
            }

            /// <summary>
            /// Write the actual value for a failing constraint test to a
            /// MessageWriter. This override only handles the special message
            /// used when an exception is expected but none is thrown.
            /// </summary>
            /// <param name="writer">The writer on which the actual value is displayed</param>
            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (ActualValue == null)
                    writer.Write("no exception thrown");
                else
                    baseResult.WriteActualValueTo(writer);
            }
        }

        #endregion
    }
}
