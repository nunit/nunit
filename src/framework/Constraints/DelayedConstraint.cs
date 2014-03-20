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
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    ///<summary>
    /// Applies a delay to the match so that a match can be evaluated in the future.
    ///</summary>
    public class DelayedConstraint : PrefixConstraint
    {
        // TODO: Needs error message tests

        private readonly int delayInMilliseconds;
        private readonly int pollingInterval;

        ///<summary>
        /// Creates a new DelayedConstraint
        ///</summary>
        ///<param name="baseConstraint">The inner constraint two decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed</param>
        ///<exception cref="InvalidOperationException">If the value of <paramref name="delayInMilliseconds"/> is less than 0</exception>
        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds)
            : this(baseConstraint, delayInMilliseconds, 0) { }

        ///<summary>
        /// Creates a new DelayedConstraint
        ///</summary>
        ///<param name="baseConstraint">The inner constraint two decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed</param>
        ///<param name="pollingInterval">The time interval used for polling</param>
        ///<exception cref="InvalidOperationException">If the value of <paramref name="delayInMilliseconds"/> is less than 0</exception>
        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds, int pollingInterval)
            : base(baseConstraint)
        {
            if (delayInMilliseconds < 0)
                throw new ArgumentException("Cannot check a condition in the past", "delayInMilliseconds");

            this.delayInMilliseconds = delayInMilliseconds;
            this.pollingInterval = pollingInterval;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description
        {
            get { return string.Format("{0} after {1} millisecond delay", baseConstraint.Description, delayInMilliseconds); }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for if the base constraint fails, false if it succeeds</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            int remainingDelay = delayInMilliseconds;

            while (pollingInterval > 0 && pollingInterval < remainingDelay)
            {
                remainingDelay -= pollingInterval;
                Thread.Sleep(pollingInterval);
                ConstraintResult result = baseConstraint.ApplyTo(actual);
                if (result.IsSuccess)
                    return new ConstraintResult(this, actual, true);
            }

            if (remainingDelay > 0)
                Thread.Sleep(remainingDelay);
            return new ConstraintResult(this, actual, baseConstraint.ApplyTo(actual).IsSuccess);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a delegate
        /// </summary>
        /// <param name="del">The delegate whose value is to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            int remainingDelay = delayInMilliseconds;
            object actual;

            while (pollingInterval > 0 && pollingInterval < remainingDelay)
            {
                remainingDelay -= pollingInterval;
                Thread.Sleep(pollingInterval);
                actual = InvokeDelegate(del);
                
                try
                {
                    ConstraintResult result = baseConstraint.ApplyTo(actual);
                    if (result.IsSuccess)
                        return new ConstraintResult(this, actual, true);
                }
                catch(Exception)
                {
                    // Ignore any exceptions when polling
                }
            }

            if (remainingDelay > 0)
                Thread.Sleep(remainingDelay);
            actual = InvokeDelegate(del);

            return new ConstraintResult(this, actual, baseConstraint.ApplyTo(actual).IsSuccess);
        }

        private static object InvokeDelegate<T>(ActualValueDelegate<T> del)
        {
#if NET_4_0 || NET_4_5
            if (AsyncInvocationRegion.IsAsyncOperation(del))
                using (AsyncInvocationRegion region = AsyncInvocationRegion.Create(del))
                    return region.WaitForPendingOperationsToComplete(del());
#endif

            return del();
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given reference.
        /// Overridden to wait for the specified delay period before
        /// calling the base constraint with the dereferenced value.
        /// </summary>
        /// <param name="actual">A reference to the value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(ref TActual actual)
        {
            int remainingDelay = delayInMilliseconds;

            while (pollingInterval > 0 && pollingInterval < remainingDelay)
            {
                remainingDelay -= pollingInterval;
                Thread.Sleep(pollingInterval);
                
                try
                {
                    ConstraintResult result = baseConstraint.ApplyTo(actual);
                    if (result.IsSuccess)
                        return new ConstraintResult(this, actual, true);
                }
                catch(Exception)
                {
                    // Ignore any exceptions when polling
                }
            }

            if (remainingDelay > 0)
                Thread.Sleep(remainingDelay);
            return new ConstraintResult(this, actual, baseConstraint.ApplyTo(actual).IsSuccess);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return string.Format("<after {0} {1}>", delayInMilliseconds, baseConstraint);
        }
    }
}
