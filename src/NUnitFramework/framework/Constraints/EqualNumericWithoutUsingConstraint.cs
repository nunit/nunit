// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualNumericConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
#pragma warning disable CS3024 // Constraint type is not CLS-compliant
    public class EqualNumericWithoutUsingConstraint<T> : Constraint
        where T : unmanaged, IConvertible, IEquatable<T>
#pragma warning restore CS3024 // Constraint type is not CLS-compliant
    {
        #region Static and Instance Fields

        private readonly T _expected;

        private Tolerance _tolerance = Tolerance.Default;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualNumericWithoutUsingConstraint(T expected)
            : base(expected)
        {
            _expected = expected;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string DisplayName => "Equal";

        /// <summary>
        /// The expected value.
        /// </summary>
        public T Expected => _expected;

        /// <summary>
        /// Gets the tolerance for this comparison.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public Tolerance Tolerance => _tolerance;

        #endregion

        #region Constraint Modifiers

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualNumericWithoutUsingConstraint<T> Within(T amount)
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
        public EqualNumericWithoutUsingConstraint<T> Ulps
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
        public EqualNumericWithoutUsingConstraint<T> Percent
        {
            get
            {
                _tolerance = _tolerance.Percent;
                return this;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public ConstraintResult ApplyTo(T actual)
        {
            bool hasSucceeded = Numerics.AreEqual(_expected, actual, ref _tolerance);

            return ConstraintResult(actual, hasSucceeded);
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            bool hasSucceeded;

            if (actual is null)
            {
                hasSucceeded = false;
            }
            else if (actual is T t)
            {
                hasSucceeded = Numerics.AreEqual(_expected, t, ref _tolerance);
            }
            else if (actual is IEquatable<T> equatable)
            {
                hasSucceeded = equatable.Equals(_expected);
            }
            else if (actual is not string and IConvertible)
            {
                hasSucceeded = Numerics.AreEqual(actual, _expected, ref _tolerance);
            }
            else
            {
                hasSucceeded = false;
            }

            return ConstraintResult(actual, hasSucceeded);
        }

        private ConstraintResult ConstraintResult<TActual>(TActual actual, bool hasSucceeded)
        {
            return new EqualConstraintResult(this, actual, _tolerance, hasSucceeded);
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

                if (_tolerance is not null && _tolerance.HasVariance)
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
