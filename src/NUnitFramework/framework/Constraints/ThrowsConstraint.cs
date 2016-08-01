// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
            //TestDelegate code = actual as TestDelegate;
            //if (code == null)
            //    throw new ArgumentException(
            //        string.Format("The actual value must be a TestDelegate but was {0}", actual.GetType().Name), "actual");

            //caughtException = null;

            //try
            //{
            //    code();
            //}
            //catch (Exception ex)
            //{
            //    caughtException = ex;
            //}

            caughtException = ExceptionInterceptor.Intercept(actual);

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
            //TestDelegate testDelegate = new TestDelegate(delegate { del(); });
            //return ApplyTo((object)testDelegate);
            return ApplyTo(new GenericInvocationDescriptor<TActual>(del));
        }

        #endregion

        #region Nested Result Class

        private class ThrowsConstraintResult : ConstraintResult
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

        #region ExceptionInterceptor

        internal class ExceptionInterceptor
        {
            private ExceptionInterceptor() { }

            internal static Exception Intercept(object invocation)
            {
                var invocationDescriptor = GetInvocationDescriptor(invocation);

#if NET_4_0 || NET_4_5 || PORTABLE
                if (AsyncInvocationRegion.IsAsyncOperation(invocationDescriptor.Delegate))
                {
                    using (var region = AsyncInvocationRegion.Create(invocationDescriptor.Delegate))
                    {
                        try
                        {
                            object result = invocationDescriptor.Invoke();
                            region.WaitForPendingOperationsToComplete(result);
                            return null;
                        }
                        catch (Exception ex)
                        {
                            return ex;
                        }
                    }
                }
                else
#endif
                {
                    try
                    {
                        invocationDescriptor.Invoke();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                }
            }

            private static IInvocationDescriptor GetInvocationDescriptor(object actual)
            {
                var invocationDescriptor = actual as IInvocationDescriptor;

                if (invocationDescriptor == null)
                {
                    var testDelegate = actual as TestDelegate;

                    if (testDelegate != null)
                    {
                        invocationDescriptor = new VoidInvocationDescriptor(testDelegate);
                    }

#if NET_4_0 || NET_4_5 || PORTABLE
                    else
                    {
                        var asyncTestDelegate = actual as AsyncTestDelegate;
                        if (asyncTestDelegate != null)
                        {
                            invocationDescriptor = new GenericInvocationDescriptor<System.Threading.Tasks.Task>(() => asyncTestDelegate());
                        }
                    }
#endif
                }
                if (invocationDescriptor == null)
                    throw new ArgumentException(
                        String.Format(
                            "The actual value must be a TestDelegate or AsyncTestDelegate but was {0}",
                            actual.GetType().Name),
                        "actual");

                return invocationDescriptor;
            }
        }

#endregion

#region InvocationDescriptor

        internal class GenericInvocationDescriptor<T> : IInvocationDescriptor
        {
            private readonly ActualValueDelegate<T> _del;

            public GenericInvocationDescriptor(ActualValueDelegate<T> del)
            {
                _del = del;
            }

            public object Invoke()
            {
                return _del();
            }

            public Delegate Delegate
            {
                get { return _del; }
            }
        }

        private interface IInvocationDescriptor
        {
            Delegate Delegate { get; }
            object Invoke();
        }

        private class VoidInvocationDescriptor : IInvocationDescriptor
        {
            private readonly TestDelegate _del;

            public VoidInvocationDescriptor(TestDelegate del)
            {
                _del = del;
            }

            public object Invoke()
            {
                _del();
                return null;
            }

            public Delegate Delegate
            {
                get { return _del; }
            }
        }

#endregion
    }
}