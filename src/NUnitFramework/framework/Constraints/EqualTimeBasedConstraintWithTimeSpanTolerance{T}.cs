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
    public class EqualTimeBasedConstraintWithTimeSpanTolerance<T> : Constraint
        where T : notnull, IEquatable<T>, IComparable<T>
    {
        private readonly T _expected;
        private readonly Func<T, long> _getTicks;
        private readonly TimeSpan _tolerance;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="tolerance">The tolerance to apply when comparing for equality.</param>
        /// <param name="getTicks">Method to extract the Ticks from an instance of <typeparamref name="T"/>.</param>
        public EqualTimeBasedConstraintWithTimeSpanTolerance(T expected, Func<T, long> getTicks, TimeSpan tolerance)
            : base(expected)
        {
            _expected = expected;
            _getTicks = getTicks;
            _tolerance = tolerance;
        }

        #endregion

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public ConstraintResult ApplyTo(T actual)
        {
            long ticksExpected = _getTicks(_expected);
            long ticksActual = _getTicks(actual);

            bool hasSucceeded = Math.Abs(ticksExpected - ticksActual) <= _tolerance.Ticks;

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
            else if (actual is IEquatable<T>)
            {
                throw new NotSupportedException($"Specified Tolerance of type {_tolerance} not supported for instances of type '{actual.GetType()}' and '{typeof(T)}'");
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

                sb.Append(" +/- ");
                sb.Append(MsgUtils.FormatValue(_tolerance));

                return sb.ToString();
            }
        }
    }
}
