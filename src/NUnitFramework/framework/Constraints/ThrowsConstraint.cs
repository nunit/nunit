// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private Exception? _caughtException;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowsConstraint"/> class,
        /// using a constraint to be applied to the exception.
        /// </summary>
        /// <param name="baseConstraint">A constraint to apply to the caught exception.</param>
        public ThrowsConstraint(IConstraint baseConstraint)
            : base(baseConstraint, string.Empty) { }

        /// <summary>
        /// Get the actual exception thrown - used by Assert.Throws.
        /// </summary>
        public Exception? ActualException => _caughtException;

        #region Constraint Overrides

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => BaseConstraint.Description;

        /// <summary>
        /// Executes the code of the delegate and captures any exception.
        /// If a non-null base constraint was provided, it applies that
        /// constraint to the exception.
        /// </summary>
        /// <param name="actual">A delegate representing the code to be tested</param>
        /// <returns>True if an exception is thrown and the constraint succeeds, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var @delegate = ConstraintUtils.RequireActual<Delegate>(actual, nameof(actual));

            _caughtException = ExceptionHelper.RecordException(@delegate, nameof(actual));

            if (_caughtException is not null)
            {
                return new ThrowsConstraintResult(
                    this,
                    _caughtException,
                    BaseConstraint.ApplyTo(_caughtException));

            }
            return new ThrowsConstraintResult(this);
        }

        /// <summary>
        /// Converts an ActualValueDelegate to a TestDelegate
        /// before calling the primary overload.
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
        {
            return ApplyTo((Delegate)del);
        }

        #endregion

        #region Nested Result Class

        private sealed class ThrowsConstraintResult : ConstraintResult
        {
            private readonly ConstraintResult? _baseResult;

            public ThrowsConstraintResult(ThrowsConstraint constraint)
                : base(constraint, null)
            {
                Status = ConstraintStatus.Failure;
            }

            public ThrowsConstraintResult(ThrowsConstraint constraint,
                Exception caughtException,
                ConstraintResult baseResult)
                : base(constraint, caughtException)
            {
                if (baseResult.IsSuccess)
                    Status = ConstraintStatus.Success;
                else
                    Status = ConstraintStatus.Failure;

                _baseResult = baseResult;
            }

            /// <summary>
            /// Write the actual value for a failing constraint test to a
            /// MessageWriter. This override only handles the special message
            /// used when an exception is expected but none is thrown.
            /// </summary>
            /// <param name="writer">The writer on which the actual value is displayed</param>
            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (_baseResult is null)
                    writer.Write("no exception thrown");
                else
                    _baseResult.WriteActualValueTo(writer);
            }
        }

        #endregion
    }
}
