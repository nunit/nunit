// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ThrowsNothingConstraint tests that a delegate does not
    /// throw an exception.
    /// </summary>
    public class ThrowsNothingConstraint : Constraint
    {
        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => "No Exception to be thrown";

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True if no exception is thrown, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var @delegate = ConstraintUtils.RequireActual<Delegate>(actual, nameof(actual));

            Exception? caughtException = ExceptionHelper.RecordException(@delegate, nameof(actual));

            return new ConstraintResult(this, caughtException, caughtException is null);
        }

        /// <summary>
        /// Applies the constraint to an ActualValueDelegate that returns
        /// the value to be tested. The default implementation simply evaluates
        /// the delegate but derived classes may override it to provide for
        /// delayed processing.
        /// </summary>
        /// <param name="del">An ActualValueDelegate</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return ApplyTo((Delegate)del);
        }

        /// <inheritdoc/>
        public override async Task<ConstraintResult> ApplyToAsync<TActual>(Func<Task<TActual>> taskDel)
        {
            Exception? caughtException = await ExceptionHelper.RecordExceptionAsync(taskDel, nameof(taskDel));

            return new ConstraintResult(this, caughtException, caughtException is null);
        }
    }
}
