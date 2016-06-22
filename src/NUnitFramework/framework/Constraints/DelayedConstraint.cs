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

#if !PORTABLE
using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Compatibility;
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
        ///<param name="baseConstraint">The inner constraint to decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed</param>
        ///<exception cref="InvalidOperationException">If the value of <paramref name="delayInMilliseconds"/> is less than 0</exception>
        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds)
            : this(baseConstraint, delayInMilliseconds, 0) { }

        ///<summary>
        /// Creates a new DelayedConstraint
        ///</summary>
        ///<param name="baseConstraint">The inner constraint to decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed, in milliseconds</param>
        ///<param name="pollingInterval">The time interval used for polling, in milliseconds</param>
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
            get { return string.Format("{0} after {1} millisecond delay", BaseConstraint.Description, delayInMilliseconds); }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for if the base constraint fails, false if it succeeds</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            long now = Stopwatch.GetTimestamp();
            long delayEnd = TimestampOffset(now, TimeSpan.FromMilliseconds(delayInMilliseconds));

            if (pollingInterval > 0)
            {
                long nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));
                while ((now = Stopwatch.GetTimestamp()) < delayEnd)
                {
                    if (nextPoll > now)
                        Thread.Sleep((int)TimestampDiff(delayEnd < nextPoll ? delayEnd : nextPoll, now).TotalMilliseconds);
                    nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));

                    ConstraintResult result = BaseConstraint.ApplyTo(actual);
                    if (result.IsSuccess)
                        return new ConstraintResult(this, actual, true);
                }
            }
            if ((now = Stopwatch.GetTimestamp()) < delayEnd)
                Thread.Sleep((int)TimestampDiff(delayEnd, now).TotalMilliseconds);

            return new ConstraintResult(this, actual, BaseConstraint.ApplyTo(actual).IsSuccess);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a delegate
        /// </summary>
        /// <param name="del">The delegate whose value is to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            long now = Stopwatch.GetTimestamp();
            long delayEnd = TimestampOffset(now, TimeSpan.FromMilliseconds(delayInMilliseconds));

            object actual;
            if (pollingInterval > 0)
            {
                long nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));
                while ((now = Stopwatch.GetTimestamp()) < delayEnd)
                {
                    if (nextPoll > now)
                        Thread.Sleep((int)TimestampDiff(delayEnd < nextPoll ? delayEnd : nextPoll, now).TotalMilliseconds);
                    nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));

                    actual = InvokeDelegate(del);

                    try
                    {
                        ConstraintResult result = BaseConstraint.ApplyTo(actual);
                        if (result.IsSuccess)
                            return new ConstraintResult(this, actual, true);
                    }
                    catch(Exception)
                    {
                        // Ignore any exceptions when polling
                    }
                }
            }
            if ((now = Stopwatch.GetTimestamp()) < delayEnd)
                Thread.Sleep((int)TimestampDiff(delayEnd, now).TotalMilliseconds);

            actual = InvokeDelegate(del);
            return new ConstraintResult(this, actual, BaseConstraint.ApplyTo(actual).IsSuccess);
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
            long now = Stopwatch.GetTimestamp();
            long delayEnd = TimestampOffset(now, TimeSpan.FromMilliseconds(delayInMilliseconds));

            if (pollingInterval > 0)
            {
                long nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));
                while ((now = Stopwatch.GetTimestamp()) < delayEnd)
                {
                    if (nextPoll > now)
                        Thread.Sleep((int)TimestampDiff(delayEnd < nextPoll ? delayEnd : nextPoll, now).TotalMilliseconds);
                    nextPoll = TimestampOffset(now, TimeSpan.FromMilliseconds(pollingInterval));

                    try
                    {
                        ConstraintResult result = BaseConstraint.ApplyTo(actual);
                        if (result.IsSuccess)
                            return new ConstraintResult(this, actual, true);
                    }
                    catch(Exception)
                    {
                        // Ignore any exceptions when polling
                    }
                }
            }
            if ((now = Stopwatch.GetTimestamp()) < delayEnd)
                Thread.Sleep((int)TimestampDiff(delayEnd, now).TotalMilliseconds);

            return new ConstraintResult(this, actual, BaseConstraint.ApplyTo(actual).IsSuccess);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return string.Format("<after {0} {1}>", delayInMilliseconds, BaseConstraint);
        }

        /// <summary>
        /// Adjusts a Timestamp by a given TimeSpan
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static long TimestampOffset(long timestamp, TimeSpan offset)
        {
            return timestamp + (long)(offset.TotalSeconds * Stopwatch.Frequency);
        }

        /// <summary>
        /// Returns the difference between two Timestamps as a TimeSpan
        /// </summary>
        /// <param name="timestamp1"></param>
        /// <param name="timestamp2"></param>
        /// <returns></returns>
        private static TimeSpan TimestampDiff(long timestamp1, long timestamp2)
        {
            return TimeSpan.FromSeconds((double)(timestamp1 - timestamp2) / Stopwatch.Frequency);
        }
    }
}
#endif