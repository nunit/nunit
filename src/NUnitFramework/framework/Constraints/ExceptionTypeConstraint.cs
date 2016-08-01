// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExceptionTypeConstraint is a special version of ExactTypeConstraint
    /// used to provided detailed info about the exception thrown in
    /// an error message.
    /// </summary>
    public class ExceptionTypeConstraint : ExactTypeConstraint
    {
        /// <summary>
        /// Constructs an ExceptionTypeConstraint
        /// </summary>
        public ExceptionTypeConstraint(Type type) : base(type) { }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Exception castedToException = actual as Exception;

            if (actual != null && castedToException == null)
                throw new ArgumentException("Actual value must be an Exception", "actual");

            actualType = actual == null ? null : actual.GetType();

            return new ExceptionTypeConstraintResult(this, actual, actualType, this.Matches(actual));
        }

        #region Nested Result Class
        class ExceptionTypeConstraintResult : ConstraintResult
        {
            private readonly object caughtException;

            public ExceptionTypeConstraintResult(ExceptionTypeConstraint constraint, object caughtException, Type type, bool matches)
                : base(constraint, type, matches) 
            { 
                this.caughtException = caughtException;
            }

            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (this.Status == ConstraintStatus.Failure)
                {
                    Exception ex = caughtException as Exception;

                    if (ex == null)
                    {
                        base.WriteActualValueTo(writer);
                    }
                    else
                    {
                        writer.WriteActualValue(ex);
                    }
                }
            }
        }
        #endregion
    }
}

