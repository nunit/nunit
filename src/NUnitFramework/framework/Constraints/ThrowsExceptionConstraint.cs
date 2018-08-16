// ***********************************************************************
// Copyright (c) 2012 Charlie Poole, Rob Prouse
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

#if ASYNC
using System.Threading.Tasks;
using NUnit.Framework.Internal;
#endif

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ThrowsExceptionConstraint tests that an exception has
    /// been thrown, without any further tests.
    /// </summary>
    public class ThrowsExceptionConstraint : Constraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "an exception to be thrown"; }
        }

        /// <summary>
        /// Executes the code and returns success if an exception is thrown.
        /// </summary>
        /// <param name="actual">A delegate representing the code to be tested</param>
        /// <returns>True if an exception is thrown, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            TestDelegate code = actual as TestDelegate;
            Exception caughtException = null;

            if (code != null)
            {
                try
                {
                    code();
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
            }
#if ASYNC
            AsyncTestDelegate asyncCode = actual as AsyncTestDelegate;
            if (asyncCode != null)
            {
                try
                {
                    AsyncToSyncAdapter.Await(asyncCode.Invoke);
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
            }
            if (code == null && asyncCode == null)
#else
            else
#endif
            {
                throw new ArgumentException(string.Format("The actual value must be a TestDelegate or AsyncTestDelegate but was {0}", actual.GetType().Name), nameof(actual));
            }
            return new ThrowsExceptionConstraintResult(this, caughtException);
        }

        /// <summary>
        /// Applies the constraint to an ActualValueDelegate that returns
        /// the value to be tested. The default implementation simply evaluates
        /// the delegate but derived classes may override it to provide for
        /// delayed processing.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
#if ASYNC
            if (typeof(TActual) == typeof(Task)) return ApplyTo(new AsyncTestDelegate(() => (Task)(object)del.Invoke()));
#endif
            return ApplyTo(new TestDelegate(() => del.Invoke()));
        }

        #region Nested Result Class

        class ThrowsExceptionConstraintResult : ConstraintResult
        {
            public ThrowsExceptionConstraintResult(ThrowsExceptionConstraint constraint, Exception caughtException)
                : base(constraint, caughtException, caughtException != null) { }

            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (this.Status == ConstraintStatus.Failure)
                    writer.Write("no exception thrown");
                else
                    base.WriteActualValueTo(writer);
            }
        }

        #endregion
    }
}
