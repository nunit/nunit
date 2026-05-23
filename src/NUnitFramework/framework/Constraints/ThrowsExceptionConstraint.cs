// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;

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
        public override string Description => "an exception to be thrown";

        /// <summary>
        /// Executes the code and returns success if an exception is thrown.
        /// </summary>
        /// <param name="actual">A delegate representing the code to be tested</param>
        /// <returns>True if an exception is thrown, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var @delegate = ConstraintUtils.RequireActual<Delegate>(actual, nameof(actual));

            var exception = ExceptionHelper.RecordException(@delegate, nameof(actual));

            return new ThrowsExceptionConstraintResult(this, exception);
        }

        /// <summary>
        /// Converts a Func to a Delegate before calling the primary overload.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(Func<TActual> code)
        {
            return ApplyTo<Delegate>(code);
        }

        /// <inheritdoc/>
        public override async Task<ConstraintResult> ApplyToAsync<TActual>(Func<Task<TActual>> actual)
        {
            var exception = await ExceptionHelper.RecordExceptionAsync(actual, nameof(actual));

            return new ThrowsExceptionConstraintResult(this, exception);
        }

        #region Nested Result Class

        private class ThrowsExceptionConstraintResult(ThrowsExceptionConstraint constraint, Exception? caughtException)
            : ConstraintResult(constraint, caughtException, caughtException is not null)
        {
            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (Status == ConstraintStatus.Failure)
                    writer.Write("no exception thrown");
                else
                    base.WriteActualValueTo(writer);
            }
        }

        #endregion
    }
}
