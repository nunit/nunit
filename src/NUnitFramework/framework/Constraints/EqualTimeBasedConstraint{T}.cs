// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualTimeBaseConstraint<T> : Constraint
        where T : struct, IEquatable<T>, IComparable<T>
    {
        #region Static and Instance Fields

        private readonly T _expected;
        private readonly Func<T, long> _getTicks;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="getTicks">Method to extract the Ticks from an instance of <typeparamref name="T"/>.</param>
        public EqualTimeBaseConstraint(T expected, Func<T, long> getTicks)
            : base(expected)
        {
            _expected = expected;
            _getTicks = getTicks;
        }

        #endregion

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        public T Expected => _expected;

        #region Constraint Modifiers

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Within(TimeSpan amount)
        {
            var constraint = new EqualTimeBasedConstraintWithTimeSpanTolerance<T>(_expected, _getTicks, amount);
            Builder?.Replace(constraint);
            return constraint;
        }

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualTimeBasedConstraintWithNumericTolerance<T> Within(double amount)
        {
            return new EqualTimeBasedConstraintWithNumericTolerance<T>(_expected, _getTicks, amount)
            {
                Builder = Builder,
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public virtual ConstraintResult ApplyTo(T actual)
        {
            bool hasSucceeded = _expected.Equals(actual);

            return new ConstraintResult(this, actual, hasSucceeded);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is T t)
            {
                return ApplyTo(t);
            }
            else if (actual is IEquatable<T> equatable)
            {
                return new ConstraintResult(this, actual, equatable.Equals(_expected));
            }
            else
            {
                // We fall back to pre 4.3 EqualConstraint behavior
                // But if the actual value cannot be convert to a DateTime nor can be compared to one
                // not sure if that makes any difference.
                return new EqualConstraint(_expected).ApplyTo(actual);
            }
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => MsgUtils.FormatValue(_expected);

        #endregion
    }
}
