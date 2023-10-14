// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AndConstraint succeeds only if both members succeed.
    /// </summary>
    public class AndConstraint : BinaryConstraint
    {
        //private enum FailurePoint
        //{
        //    None,
        //    Left,
        //    Right
        //};

        //private FailurePoint failurePoint;

        /// <summary>
        /// Create an AndConstraint from two other constraints
        /// </summary>
        /// <param name="left">The first constraint</param>
        /// <param name="right">The second constraint</param>
        public AndConstraint(IConstraint left, IConstraint right) : base(left, right)
        {
        }

        /// <summary>
        /// Gets text describing a constraint
        /// </summary>
        public override string Description => Left.Description + " and " + Right.Description;

        /// <summary>
        /// Apply both member constraints to an actual value,
        /// succeeding only if both of them succeed.
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <returns>True if the constraints both succeeded</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var leftResult = Left.ApplyTo(actual);
            var rightResult = leftResult.IsSuccess
                ? Right.ApplyTo(actual)
                : new ConstraintResult(Right, actual);

            return new AndConstraintResult(this, actual, leftResult, rightResult);
        }

        #region Nested Result Class

        private class AndConstraintResult : ConstraintResult
        {
            private readonly ConstraintResult _leftResult;
            private readonly ConstraintResult _rightResult;

            public AndConstraintResult(AndConstraint constraint, object? actual, ConstraintResult leftResult, ConstraintResult rightResult)
                : base(constraint, actual, leftResult.IsSuccess && rightResult.IsSuccess)
            {
                _leftResult = leftResult;
                _rightResult = rightResult;
            }

            /// <summary>
            /// Write the actual value for a failing constraint test to a
            /// MessageWriter. The default implementation simply writes
            /// the raw value of actual, leaving it to the writer to
            /// perform any formatting.
            /// </summary>
            /// <param name="writer">The writer on which the actual value is displayed</param>
            public override void WriteActualValueTo(MessageWriter writer)
            {
                if (IsSuccess)
                    base.WriteActualValueTo(writer);
                else if (!_leftResult.IsSuccess)
                    _leftResult.WriteActualValueTo(writer);
                else
                    _rightResult.WriteActualValueTo(writer);
            }

            public override void WriteAdditionalLinesTo(MessageWriter writer)
            {
                if (IsSuccess)
                    base.WriteAdditionalLinesTo(writer);
                else if (!_leftResult.IsSuccess)
                    _leftResult.WriteAdditionalLinesTo(writer);
                else
                    _rightResult.WriteAdditionalLinesTo(writer);
            }
        }

        #endregion
    }
}
