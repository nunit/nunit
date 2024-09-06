// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualConstraint<T> : EqualConstraint
    {
        #region Static and Instance Fields

        private readonly T _expected;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualConstraint(T expected)
            : base(expected)
        {
            AdjustArgumentIfNeeded(ref expected);
            _expected = expected;
        }
        #endregion

        #region Constraint Modifiers
        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public new EqualConstraint<T> IgnoreCase => (EqualConstraint<T>)base.IgnoreCase;

        /// <summary>
        /// Flag the constraint to ignore white space and return self.
        /// </summary>
        public new EqualConstraint<T> IgnoreWhiteSpace => (EqualConstraint<T>)base.IgnoreWhiteSpace;

        /// <summary>
        /// Flag the constraint to suppress string clipping
        /// and return self.
        /// </summary>
        public new EqualConstraint<T> NoClip => (EqualConstraint<T>)base.NoClip;

        /// <summary>
        /// Flag the constraint to compare arrays as collections
        /// and return self.
        /// </summary>
        public new EqualConstraint<T> AsCollection => (EqualConstraint<T>)base.AsCollection;

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Within(T amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            _tolerance = new Tolerance(amount!);
            return this;
        }

        /// <summary>
        /// Flags the constraint to include <see cref="DateTimeOffset.Offset"/>
        /// property in comparison of two <see cref="DateTimeOffset"/> values.
        /// </summary>
        /// <remarks>
        /// Using this modifier does not allow to use the <see cref="Within"/>
        /// constraint modifier.
        /// </remarks>
        public new EqualConstraint<T> WithSameOffset => (EqualConstraint<T>)base.WithSameOffset;

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
        public new EqualConstraint<T> Ulps => (EqualConstraint<T>)base.Ulps;

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a percentage that the actual values is allowed to deviate from
        /// the expected value.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Percent => (EqualConstraint<T>)base.Percent;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in days.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Days => (EqualConstraint<T>)base.Days;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in hours.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Hours => (EqualConstraint<T>)base.Hours;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in minutes.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Minutes => (EqualConstraint<T>)base.Minutes;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in seconds.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Seconds => (EqualConstraint<T>)base.Seconds;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in milliseconds.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Milliseconds => (EqualConstraint<T>)base.Milliseconds;

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in clock ticks.
        /// </summary>
        /// <returns>Self</returns>
        public new EqualConstraint<T> Ticks => (EqualConstraint<T>)base.Ticks;

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(IComparer<T> comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public new EqualConstraint<T> Using<TOther>(IComparer<TOther> comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The boolean-returning delegate to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(Func<T, T, bool> comparer)
        {
            return (EqualConstraint<T>)base.Using<T>(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The boolean-returning delegate to use.</param>
        /// <returns>Self.</returns>
        public new EqualConstraint<T> Using<TOther>(Func<TOther, TOther, bool> comparer)
        {
            return (EqualConstraint<T>)base.Using<TOther>(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(Comparison<T> comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public new EqualConstraint<T> Using<TOther>(Comparison<TOther> comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public new EqualConstraint<T> Using(IEqualityComparer comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint<T> Using(IEqualityComparer<T> comparer)
        {
            return (EqualConstraint<T>)base.Using<T>(comparer);
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public new EqualConstraint<T> Using<TOther>(IEqualityComparer<TOther> comparer)
        {
            return (EqualConstraint<T>)base.Using(comparer);
        }

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        public new EqualConstraint<T> UsingPropertiesComparer()
        {
            return (EqualConstraint<T>)base.UsingPropertiesComparer();
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
                AdjustArgumentIfNeeded(ref a);
                bool isSuccess = _comparer.AreEqual(_expected, a, ref _tolerance);

                return new EqualConstraintResult(this, GetConstraintResultData(), actual, isSuccess);
            }
            else
            {
                return base.ApplyTo(actual);
            }

            EqualConstraintResult.ResultData GetConstraintResultData() => new()
            {
                ExpectedValue = _expected,
                Tolerance = Tolerance,
                CaseInsensitive = CaseInsensitive,
                IgnoringWhiteSpace = IgnoringWhiteSpace,
                ComparingProperties = ComparingProperties,
                ClipStrings = ClipStrings,
                FailurePoints = HasFailurePoints ? FailurePoints : Array.Empty<NUnitEqualityComparer.FailurePoint>()
            };
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
