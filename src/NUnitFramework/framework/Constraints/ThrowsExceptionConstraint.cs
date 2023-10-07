// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
        /// Applies the constraint to an ActualValueDelegate that returns
        /// the value to be tested. The default implementation simply evaluates
        /// the delegate but derived classes may override it to provide for
        /// delayed processing.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return ApplyTo((Delegate)del);
        }

        #region Nested Result Class

        private class ThrowsExceptionConstraintResult : ConstraintResult
        {
            public ThrowsExceptionConstraintResult(ThrowsExceptionConstraint constraint, Exception? caughtException)
                : base(constraint, caughtException, caughtException is not null) { }

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
