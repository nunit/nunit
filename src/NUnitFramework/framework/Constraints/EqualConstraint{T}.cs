// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualConstraint<T> : Constraint
        where T : IEquatable<T>
    {
        #region Static and Instance Fields

        private readonly T _expected;

        private Tolerance _tolerance = Tolerance.Default;
        private Func<T, T, bool> _comparer = null!;
        private readonly NUnitEqualityComparer _fallbackComparer = new();

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualConstraint(T expected)
            : base(expected)
        {
            _expected = expected;
        }
        #endregion

        #region Properties

        // TODO: Remove public properties
        // They are only used by EqualConstraintResult
        // EqualConstraint should inject them into the constructor.

        /// <summary>
        /// Gets the tolerance for this comparison.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public Tolerance Tolerance
        {
            get { return _tolerance; }
        }

        #endregion

        #region Constraint Modifiers

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualConstraint Within(object amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            return new EqualConstraint(_expected).Within(amount);
        }

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Within(T amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            _tolerance = new Tolerance(amount);
            return this;
        }

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a distance in representable values (see remarks).
        /// </summary>
        /// <returns>Self.</returns>
        /// <remarks>
        /// Ulp stands for "unit in the last place" and describes the minimum
        /// amount a given value can change. For any integers, an ulp is 1 whole
        /// digit. For floating point values, the accuracy of which is better
        /// for smaller numbers and worse for larger numbers, an ulp depends
        /// on the size of the number. Using ulps for comparison of floating
        /// point results instead of fixed tolerances is safer because it will
        /// automatically compensate for the added inaccuracy of larger numbers.
        /// </remarks>
        public EqualConstraint<T> Ulps
        {
            get
            {
                _tolerance = _tolerance.Ulps;
                return this;
            }
        }

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a percentage that the actual values is allowed to deviate from
        /// the expected value.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Percent
        {
            get
            {
                _tolerance = _tolerance.Percent;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in days.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Days
        {
            get
            {
                _tolerance = _tolerance.Days;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in hours.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Hours
        {
            get
            {
                _tolerance = _tolerance.Hours;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in minutes.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Minutes
        {
            get
            {
                _tolerance = _tolerance.Minutes;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in seconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Seconds
        {
            get
            {
                _tolerance = _tolerance.Seconds;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in milliseconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Milliseconds
        {
            get
            {
                _tolerance = _tolerance.Milliseconds;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in clock ticks.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint<T> Ticks
        {
            get
            {
                _tolerance = _tolerance.Ticks;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using(IComparer comparer)
        {
            return new EqualConstraint(_expected).Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(IComparer<T> comparer)
        {
            _comparer = (l, r) => comparer.Compare(l, r) == 0;
            _fallbackComparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The boolean-returning delegate to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
            _fallbackComparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(Comparison<T> comparer)
        {
            _comparer = (l, r) => comparer(l, r) == 0;
            _fallbackComparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(IEqualityComparer comparer)
        {
            _comparer = (l, r) => comparer.Equals(l, r);
            _fallbackComparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(IEqualityComparer<T> comparer)
        {
            _comparer = (l, r) => comparer.Equals(l, r);
            _fallbackComparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is T a)
            {
                bool isSuccess = false;

                if (_comparer is null)
                {
                    GetTolerance(ref _tolerance);

                    if (_tolerance.IsUnsetOrDefault)
                    {
                        isSuccess = a.Equals(_expected);
                    }
                    else
                    {
                        isSuccess = Numerics.AreEqual(_expected, a, ref _tolerance);
                    }
                }
                else
                {
                    isSuccess = _comparer(_expected, a);
                }

                return new EqualConstraintResult(this, GetConstraintResultData(), actual, isSuccess);
            }
            else
            {
                var value = _fallbackComparer.AreEqual(_expected, actual, ref _tolerance);
                return new EqualConstraintResult(this, GetConstraintResultData(), actual, value);
            }

            throw new ArgumentException("Wrong type", nameof(actual));

            EqualConstraintResult.ResultData GetConstraintResultData() => new()
            {
                // TODO: Handle string-specific cases separately?
                ExpectedValue = Arguments[0],
                Tolerance = Tolerance,
                CaseInsensitive = false,
                IgnoringWhiteSpace = false,
                ComparingProperties = false,
                ClipStrings = false,
                FailurePoints = Array.Empty<NUnitEqualityComparer.FailurePoint>()
            };
        }

        private static void GetTolerance(ref Tolerance tolerance)
        {
            if (tolerance.IsUnsetOrDefault && (typeof(T) == typeof(double) || typeof(T) == typeof(float)))
            {
                var defaultFloatingPointTolerance = TestExecutionContext.CurrentContext?.DefaultFloatingPointTolerance;
                if (defaultFloatingPointTolerance is not null && !defaultFloatingPointTolerance.IsUnsetOrDefault)
                {
                    tolerance = defaultFloatingPointTolerance;
                }
            }
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(MsgUtils.FormatValue(_expected));

                if (_tolerance is not null && !_tolerance.IsUnsetOrDefault)
                {
                    sb.Append(" +/- ");
                    sb.Append(MsgUtils.FormatValue(_tolerance.Amount));
                    if (_tolerance.Mode != ToleranceMode.Linear)
                    {
                        sb.Append(" ");
                        sb.Append(_tolerance.Mode.ToString());
                    }
                }

                return sb.ToString();
            }
        }

        #endregion
    }
}
