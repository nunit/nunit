// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The Constraint class is the base of all built-in constraints
    /// within NUnit. It provides the operator overloads used to combine
    /// constraints.
    /// </summary>
    public abstract class Constraint<T> : ConstraintBase, IAsyncConstraint<T>
    {
        #region Constructor

        /// <summary>
        /// Construct a constraint with optional arguments
        /// </summary>
        /// <param name="args">Arguments to be saved</param>
        protected Constraint(params object?[] args)
            : base(args)
        {
        }

        #endregion

        #region Abstract and Virtual Methods

        /// <inheritdoc/>
        public abstract ConstraintResult ApplyTo(T actual);

        /// <inheritdoc/>
        public virtual ConstraintResult ApplyTo(Func<T> del)
        {
            if (AsyncToSyncAdapter.IsAsyncOperation(del))
                return ApplyTo(AsyncToSyncAdapter.Await<T>(TestExecutionContext.CurrentContext, () => del.Invoke())!);

            return ApplyTo(GetTestObject(del));
        }

        /// <inheritdoc/>
        public virtual async Task<ConstraintResult> ApplyToAsync(Func<Task<T>> taskDel)
        {
            return ApplyTo(await taskDel());
        }

        /// <summary>
        /// Retrieves the value to be tested from a Func.
        /// The default implementation simply evaluates the delegate but derived
        /// classes may override it to provide for delayed processing.
        /// </summary>
        /// <param name="del">A Func</param>
        /// <returns>Delegate evaluation result</returns>
        protected virtual T GetTestObject(Func<T> del)
        {
            return del();
        }

        #endregion

        #region After Modifier

        /// <summary>
        /// Returns a DelayedConstraint.WithRawDelayInterval with the specified delay time.
        /// </summary>
        /// <param name="delay">The delay, which defaults to milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint<T>.WithRawDelayInterval After(int delay)
        {
            return new DelayedConstraint<T>.WithRawDelayInterval(
                new DelayedConstraint<T>(
                    this,
                    delay));
        }

        /// <summary>
        /// Returns a DelayedConstraint{T} with the specified delay time
        /// and polling interval.
        /// </summary>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <param name="pollingInterval">The interval at which to test the constraint, in milliseconds.</param>
        /// <returns></returns>
        public DelayedConstraint<T> After(int delayInMilliseconds, int pollingInterval)
        {
            return new DelayedConstraint<T>(
                this,
                delayInMilliseconds,
                pollingInterval);
        }

        #endregion
    }
}
