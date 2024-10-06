// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualDateTimeOffsetConstraintWithSameOffset : Constraint
    {
        private readonly DateTimeOffset _expected;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualDateTimeOffsetConstraintWithSameOffset(DateTimeOffset expected)
            : base(expected)
        {
            _expected = expected;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public ConstraintResult ApplyTo(DateTimeOffset actual)
        {
            bool hasSucceeded = _expected.Equals(actual) && _expected.Offset == actual.Offset;

            return new ConstraintResult(this, actual, hasSucceeded);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is DateTimeOffset dateTimeOffset)
            {
                return ApplyTo(dateTimeOffset);
            }

            return new ConstraintResult(this, actual, false);
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                var sb = new StringBuilder(MsgUtils.FormatValue(_expected));

                sb.Append(" with the same offset");

                return sb.ToString();
            }
        }

        #endregion
    }
}
