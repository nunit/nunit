// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ExceptionTypeConstraint is a special version of ExactTypeConstraint
    /// used to provided detailed info about the exception thrown in
    /// an error message.
    /// </summary>
    public class ExceptionTypeConstraint : ExactTypeConstraint
    {
        /// <summary>
        /// Constructs an ExceptionTypeConstraint
        /// </summary>
        public ExceptionTypeConstraint(Type type) : base(type) { }

        /// <summary>
        /// Applies the constraint to an actual value, returning a ConstraintResult.
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>A ConstraintResult</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            ConstraintUtils.RequireActual<Exception>(actual, nameof(actual), allowNull: true);

            actualType = actual?.GetType();

            return new ExceptionTypeConstraintResult(this, actual, actualType, this.Matches(actual));
        }

        #region Nested Result Class
        class ExceptionTypeConstraintResult : ConstraintResult
        {
            private readonly object? caughtException;

            public ExceptionTypeConstraintResult(ExceptionTypeConstraint constraint, object? caughtException, Type? type, bool matches)
                : base(constraint, type, matches)
            {
                this.caughtException = caughtException;
            }

            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (this.Status == ConstraintStatus.Failure)
                {
                    if (caughtException is Exception ex)
                    {
                        writer.WriteActualValue(ex);
                    }
                    else
                    {
                        base.WriteActualValueTo(writer);
                    }
                }
            }
        }
        #endregion
    }
}

