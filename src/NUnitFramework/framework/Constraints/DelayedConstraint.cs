// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Constraints
{
    ///<summary>
    /// Applies a delay to the match so that a match can be evaluated in the future.
    ///</summary>
    public class DelayedConstraint : PrefixConstraint
    {
        /// <summary>
        /// Allows only changing the time dimension of delay interval and setting a polling interval of a DelayedConstraint
        /// </summary>
        public class WithRawDelayInterval : DelayedConstraint
        {
            private readonly DelayedConstraint _parent;

            /// <summary>
            /// Creates a new DelayedConstraint.WithRawDelayInterval
            /// </summary>
            /// <param name="parent">Parent DelayedConstraint on which delay interval dimension is required to be set</param>
            public WithRawDelayInterval(DelayedConstraint parent)
                : base(parent.BaseConstraint, parent.DelayInterval, parent.PollingInterval)
            {
                DelayInterval = parent.DelayInterval;
                _parent = parent;
            }

            /// <summary>
            /// Changes delay interval dimension to minutes
            /// </summary>
            public WithDimensionedDelayInterval Minutes
            {
                get
                {
                    _parent.DelayInterval = _parent.DelayInterval.InMinutes;
                    return new WithDimensionedDelayInterval(_parent);
                }
            }

            /// <summary>
            /// Changes delay interval dimension to seconds
            /// </summary>
            public WithDimensionedDelayInterval Seconds
            {
                get
                {
                    DelayInterval = DelayInterval.InSeconds;
                    return new WithDimensionedDelayInterval(_parent);
                }
            }

            /// <summary>
            /// Changes delay interval dimension to milliseconds
            /// </summary>
            public WithDimensionedDelayInterval MilliSeconds
            {
                get
                {
                    DelayInterval = DelayInterval.InMilliseconds;
                    return new WithDimensionedDelayInterval(_parent);
                }
            }

            /// <summary>
            /// Set polling interval, in milliseconds
            /// </summary>
            /// <param name="milliSeconds">A time interval, in milliseconds</param>
            /// <returns></returns>
            public WithRawPollingInterval PollEvery(int milliSeconds)
            {
                if (milliSeconds < 0)
                    throw new ArgumentException("Interval cannot be negative");

                _parent.PollingInterval = new Interval(milliSeconds).InMilliseconds;
                return new WithRawPollingInterval(_parent);
            }
        }

        /// <summary>
        /// Allows only setting the polling interval of a DelayedConstraint
        /// </summary>
        public class WithDimensionedDelayInterval : DelayedConstraint
        {
            private readonly DelayedConstraint _parent;

            /// <summary>
            /// Creates a new DelayedConstraint.WithDimensionedDelayInterval
            /// </summary>
            /// <param name="parent">Parent DelayedConstraint on which polling interval is required to be set</param>
            public WithDimensionedDelayInterval(DelayedConstraint parent)
                : base(parent.BaseConstraint, parent.DelayInterval, parent.PollingInterval)
            {
                DelayInterval = parent.DelayInterval;
                _parent = parent;
            }

            /// <summary>
            /// Set polling interval, in milliseconds
            /// </summary>
            /// <param name="milliSeconds">A time interval, in milliseconds</param>
            /// <returns></returns>
            public WithRawPollingInterval PollEvery(int milliSeconds)
            {
                if (milliSeconds < 0)
                    throw new ArgumentException("Interval cannot be negative");

                _parent.PollingInterval = new Interval(milliSeconds).InMilliseconds;
                return new WithRawPollingInterval(_parent);
            }
        }

        /// <summary>
        /// Allows only changing the time dimension of the polling interval of a DelayedConstraint
        /// </summary>
        public class WithRawPollingInterval : DelayedConstraint
        {
            private readonly DelayedConstraint _parent;

            /// <summary>
            /// Creates a new DelayedConstraint.WithRawPollingInterval
            /// </summary>
            /// <param name="parent">Parent DelayedConstraint on which polling dimension is required to be set</param>
            public WithRawPollingInterval(DelayedConstraint parent)
                : base(parent.BaseConstraint, parent.DelayInterval, parent.PollingInterval)
            {
                _parent = parent;
            }

            /// <summary>
            /// Changes polling interval dimension to minutes
            /// </summary>
            public DelayedConstraint Minutes
            {
                get
                {
                    _parent.PollingInterval = _parent.PollingInterval.InMinutes;
                    return _parent;
                }
            }

            /// <summary>
            /// Changes polling interval dimension to seconds
            /// </summary>
            public DelayedConstraint Seconds
            {
                get
                {
                    _parent.PollingInterval = _parent.PollingInterval.InSeconds;
                    return _parent;
                }
            }

            /// <summary>
            /// Changes polling interval dimension to milliseconds
            /// </summary>
            public DelayedConstraint MilliSeconds
            {
                get
                {
                    _parent.PollingInterval = _parent.PollingInterval.InMilliseconds;
                    return _parent;
                }
            }
        }

        // TODO: Needs error message tests
        /// <summary>
        /// Delay value store as an Interval object
        /// </summary>
        protected Interval DelayInterval { get; set; }

        /// <summary>
        /// Polling value stored as an Interval object
        /// </summary>
        protected Interval PollingInterval { get; set; }

        ///<summary>
        /// Creates a new DelayedConstraint
        ///</summary>
        ///<param name="baseConstraint">The inner constraint to decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed</param>
        ///<exception cref="InvalidOperationException">If the value of <paramref name="delayInMilliseconds"/> is less than 0</exception>
        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds)
            : this(baseConstraint, delayInMilliseconds, 0)
        {
        }

        ///<summary>
        /// Creates a new DelayedConstraint
        ///</summary>
        ///<param name="baseConstraint">The inner constraint to decorate</param>
        ///<param name="delayInMilliseconds">The time interval after which the match is performed, in milliseconds</param>
        ///<param name="pollingIntervalInMilliseconds">The time interval used for polling, in milliseconds</param>
        ///<exception cref="InvalidOperationException">If the value of <paramref name="delayInMilliseconds"/> is less than 0</exception>
        public DelayedConstraint(IConstraint baseConstraint, int delayInMilliseconds, int pollingIntervalInMilliseconds)
            : base(baseConstraint, string.Empty)
        {
            if (delayInMilliseconds < 0)
                throw new ArgumentException("Cannot check a condition in the past", nameof(delayInMilliseconds));
            if (pollingIntervalInMilliseconds < 0)
                throw new ArgumentException("Interval cannot be negative", nameof(pollingIntervalInMilliseconds));

            DelayInterval = new Interval(delayInMilliseconds).InMilliseconds;
            PollingInterval = new Interval(pollingIntervalInMilliseconds).InMilliseconds;
        }

        private DelayedConstraint(IConstraint baseConstraint, Interval delayInterval, Interval pollingInterval)
            : base(baseConstraint, string.Empty)
        {
            DelayInterval = delayInterval;
            PollingInterval = pollingInterval;
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => $"After {DelayInterval} delay";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for if the base constraint fails, false if it succeeds</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            return PollLoop(() => BaseConstraint.ApplyTo(actual));
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a delegate
        /// </summary>
        /// <param name="del">The delegate whose value is to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return PollLoop(() => BaseConstraint.ApplyTo(del));
        }

        /// <inheritdoc/>
        public override async Task<ConstraintResult> ApplyToAsync<TActual>(Func<Task<TActual>> taskDel)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (PollingInterval.IsNotZero)
            {
                while (await PollingDelayAsync(stopwatch))
                {
                    try
                    {
                        ConstraintResult result = await BaseConstraint.ApplyToAsync(taskDel);
                        if (result.IsSuccess)
                            return new DelegatingConstraintResult(this, result);
                    }
                    catch (Exception)
                    {
                        // Ignore any exceptions when polling
                    }
                }
            }

            await FinalDelayAsync(stopwatch);

            return new DelegatingConstraintResult(this, await BaseConstraint.ApplyToAsync(taskDel));
        }

        private async Task<bool> PollingDelayAsync(Stopwatch stopwatch)
        {
            TimeSpan maxDelay = DelayInterval.AsTimeSpan - stopwatch.Elapsed;
            TimeSpan pollingDelay = maxDelay < PollingInterval.AsTimeSpan ? maxDelay : PollingInterval.AsTimeSpan;
            bool needsToWait = pollingDelay > TimeSpan.Zero;
            if (needsToWait)
                await Task.Delay(pollingDelay);
            return needsToWait;
        }

        private Task FinalDelayAsync(Stopwatch stopwatch)
        {
            TimeSpan maxDelay = DelayInterval.AsTimeSpan - stopwatch.Elapsed;
            if (maxDelay > TimeSpan.Zero)
                return Task.Delay(maxDelay);
            return Task.CompletedTask;
        }

        private ConstraintResult PollLoop(Func<ConstraintResult> applyBaseConstraint)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (PollingInterval.IsNotZero)
            {
                while (PollingDelay(stopwatch))
                {
                    try
                    {
                        ConstraintResult result = applyBaseConstraint();
                        if (result.IsSuccess)
                            return new DelegatingConstraintResult(this, result);
                    }
                    catch (Exception)
                    {
                        // Ignore any exceptions when polling
                    }
                }
            }

            FinalDelay(stopwatch);

            return new DelegatingConstraintResult(this, applyBaseConstraint());
        }

        private bool PollingDelay(Stopwatch stopwatch)
        {
            TimeSpan maxDelay = DelayInterval.AsTimeSpan - stopwatch.Elapsed;
            TimeSpan pollingDelay = maxDelay < PollingInterval.AsTimeSpan ? maxDelay : PollingInterval.AsTimeSpan;
            bool needsToWait = pollingDelay > TimeSpan.Zero;
            if (needsToWait)
                Thread.Sleep(pollingDelay);
            return needsToWait;
        }

        private void FinalDelay(Stopwatch stopwatch)
        {
            TimeSpan maxDelay = DelayInterval.AsTimeSpan - stopwatch.Elapsed;
            if (maxDelay > TimeSpan.Zero)
                Thread.Sleep(maxDelay);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return $"<after {DelayInterval.AsTimeSpan.TotalMilliseconds} {BaseConstraint}>";
        }

        private class DelegatingConstraintResult : ConstraintResult
        {
            private readonly ConstraintResult _innerResult;

            public DelegatingConstraintResult(IConstraint constraint, ConstraintResult innerResult)
                : base(constraint, innerResult.ActualValue, innerResult.Status)
            {
                _innerResult = innerResult;
            }

            public override void WriteMessageTo(MessageWriter writer)
            {
                writer.WriteMessageLine(Description);
                _innerResult.WriteMessageTo(writer);
            }
        }
    }
}
