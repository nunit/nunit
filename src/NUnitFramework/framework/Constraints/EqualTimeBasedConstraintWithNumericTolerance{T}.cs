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
    public class EqualTimeBasedConstraintWithNumericTolerance<T>
        where T : notnull, IEquatable<T>, IComparable<T>
    {
        private readonly T _expected;
        private readonly Func<T, long> _getTicks;
        private readonly double _tolerance;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="getTicks">Method to extract the Ticks from an instance of <typeparamref name="T"/>.</param>
        /// <param name="tolerance">The tolerance to apply when qualified with a unit.</param>
        public EqualTimeBasedConstraintWithNumericTolerance(T expected, Func<T, long> getTicks, double tolerance)
        {
            _expected = expected;
            _tolerance = tolerance;
            _getTicks = getTicks;
        }

        #endregion

        /// <summary>
        /// The ConstraintBuilder holding this constraint
        /// </summary>
        public ConstraintBuilder? Builder { get; set; }

        #region Constraint Modifiers

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in days.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Days => Within(TimeSpan.FromDays(_tolerance));

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in hours.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Hours => Within(TimeSpan.FromHours(_tolerance));

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in minutes.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Minutes => Within(TimeSpan.FromMinutes(_tolerance));

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in seconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Seconds => Within(TimeSpan.FromSeconds(_tolerance));

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in milliseconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Milliseconds => Within(TimeSpan.FromMilliseconds(_tolerance));

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in clock ticks.
        /// </summary>
        /// <returns>Self</returns>
        public EqualTimeBasedConstraintWithTimeSpanTolerance<T> Ticks => Within(TimeSpan.FromTicks((long)_tolerance));

        private EqualTimeBasedConstraintWithTimeSpanTolerance<T> Within(TimeSpan amount)
        {
            var constraint = new EqualTimeBasedConstraintWithTimeSpanTolerance<T>(_expected, _getTicks, amount);
            Builder?.Replace(constraint);
            return constraint;
        }

        #endregion
    }
}
