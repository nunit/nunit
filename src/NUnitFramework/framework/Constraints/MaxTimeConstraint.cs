// *****************************************************************************
//Copyright(c) 2017 Charlie Poole, Rob Prouse

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
// ****************************************************************************

using System;
using System.Diagnostics;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// MaxTimeConstraint is used to test if a delegate
    /// executes within the specified maximum time by
    /// applying a constraint to it.
    /// </summary>
    public class MaxTimeConstraint : Constraint
    {
        private readonly TimeSpan _maxTime;

        /// <summary>
        /// Constructs MaxTimeConstraint with the specified timeout.
        /// </summary>
        public MaxTimeConstraint(TimeSpan maxTime) : base(maxTime)
        {
            _maxTime = maxTime;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => $"Maximum time {_maxTime}";

        private static Stopwatch InvokeTestDelegate(object testDelegate)
        {
            Stopwatch stopwatch = new Stopwatch();
            var invocationDescriptor = InvocationDescriptorExtensions.GetInvocationDescriptor(testDelegate);

#if ASYNC
            if (AsyncInvocationRegion.IsAsyncOperation(invocationDescriptor.Delegate))
            {
                using (var region = AsyncInvocationRegion.Create(invocationDescriptor.Delegate))
                {
                    stopwatch.Start();
                    object result = invocationDescriptor.Invoke();
                    region.WaitForPendingOperationsToComplete(result);
                    stopwatch.Stop();
                }
            }
            else
#endif
            {
                using (new TestExecutionContext.IsolatedContext())
                {
                    stopwatch.Start();
                    invocationDescriptor.Invoke();
                    stopwatch.Stop();
                }
            }

            return stopwatch;
        }

        /// <summary>
        /// Executes the code of the passed delegate, measures the duration of
        /// the execution and applies the constraint, that the operation should
        /// finish within the specified maximum time.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Stopwatch stopwatch = InvokeTestDelegate(actual);
            return new ConstraintResult(this, stopwatch.Elapsed, stopwatch.Elapsed <= _maxTime);
        }

        /// <summary>
        /// Retrieves the value to be tested from an ActualValueDelegate
        /// wrapped in into <see cref="NUnit.Framework.TestDelegate"/>.
        /// </summary>
        protected override object GetTestObject<TActual>(ActualValueDelegate<TActual> actualValueDelegate)
        {
            return (TestDelegate)(() => actualValueDelegate());
        }
    }
}
