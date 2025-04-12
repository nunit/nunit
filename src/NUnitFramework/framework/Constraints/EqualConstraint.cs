// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are
    /// considered equal if both are null, or if both have the same
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualConstraint : Constraint
    {
        #region Static and Instance Fields

        private readonly object? _expected;

        private Tolerance _tolerance = Tolerance.Default;

        /// <summary>
        /// NUnitEqualityComparer used to test equality.
        /// </summary>
        private readonly NUnitEqualityComparer _comparer = new();

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualConstraint(object? expected)
            : base(expected)
        {
            AdjustArgumentIfNeeded(ref expected);

            _expected = expected;
            ClipStrings = true;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The underlying comparer used.
        /// </summary>
        protected internal NUnitEqualityComparer Comparer => _comparer;

        // TODO: Remove public properties
        // They are only used by EqualConstraintResult
        // EqualConstraint should inject them into the constructor.

        /// <summary>
        /// Gets the tolerance for this comparison.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public Tolerance Tolerance => _tolerance;

        /// <summary>
        /// Gets a value indicating whether to compare case insensitive.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if comparing case insensitive; otherwise, <see langword="false"/>.
        /// </value>
        public bool CaseInsensitive => _comparer.IgnoreCase;

        /// <summary>
        /// Gets a value indicating whether to compare ignoring white space.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if comparing ignoring white space; otherwise, <see langword="false"/>.
        /// </value>
        public bool IgnoringWhiteSpace => _comparer.IgnoreWhiteSpace;

        /// <summary>
        /// Gets a value indicating whether to compare after normalizing newlines.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if comparing after normalizing newlines; otherwise, <see langword="false"/>.
        /// </value>
        public bool NormalizingLineEndings => _comparer.NormalizeLineEndings;

        /// <summary>
        /// Gets a value indicating whether to compare separate properties.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if comparing separate properties; otherwise, <see langword="false"/>.
        /// </value>
        public bool ComparingProperties => _comparer.CompareProperties;

        /// <summary>
        /// Gets a value indicating whether or not to clip strings.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if set to clip strings otherwise, <see langword="false"/>.
        /// </value>
        public bool ClipStrings { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there is any additional Failure Information.
        /// </summary>
        public bool HasFailurePoints => _comparer.HasFailurePoints;

        /// <summary>
        /// Gets the failure points.
        /// </summary>
        /// <value>
        /// The failure points.
        /// </value>
        public IList<NUnitEqualityComparer.FailurePoint> FailurePoints => _comparer.FailurePoints;

        #endregion

        #region Constraint Modifiers
        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public EqualConstraint IgnoreCase
        {
            get
            {
                _comparer.IgnoreCase = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to ignore white space and return self.
        /// </summary>
        public EqualConstraint IgnoreWhiteSpace
        {
            get
            {
                _comparer.IgnoreWhiteSpace = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to normalize newlines and return self.
        /// </summary>
        public EqualConstraint NormalizeLineEndings
        {
            get
            {
                _comparer.NormalizeLineEndings = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to suppress string clipping
        /// and return self.
        /// </summary>
        public EqualConstraint NoClip
        {
            get
            {
                ClipStrings = false;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to compare arrays as collections
        /// and return self.
        /// </summary>
        public EqualConstraint AsCollection
        {
            get
            {
                _comparer.CompareAsCollection = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use a tolerance defined elsewhere when determining equality.
        /// </summary>
        /// <param name="tolerance">Tolerance to be used</param>
        /// <returns>Self.</returns>
        internal EqualConstraint WithinConfiguredTolerance(Tolerance tolerance)
        {
            _tolerance = tolerance;
            return this;
        }

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualConstraint Within(object amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            _tolerance = new Tolerance(amount);
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
        public EqualConstraint WithSameOffset
        {
            get
            {
                _comparer.WithSameOffset = true;
                return this;
            }
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
        public EqualConstraint Ulps
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
        public EqualConstraint Percent
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
        public EqualConstraint Days
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
        public EqualConstraint Hours
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
        public EqualConstraint Minutes
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
        public EqualConstraint Seconds
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
        public EqualConstraint Milliseconds
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
        public EqualConstraint Ticks
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
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied boolean-returning delegate.
        /// </summary>
        /// <param name="comparer">The boolean-returning delegate to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(Func<T, T, bool> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(Comparison<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using(IEqualityComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied predicate function
        /// </summary>
        /// <param name="comparison">The comparison function to use.</param>
        /// <typeparam name="TActual">The type of the actual value. Note for collection comparisons this is the element type.</typeparam>
        /// <typeparam name="TExpected">The type of the expected value. Note for collection comparisons this is the element type.</typeparam>
        /// <returns>Self.</returns>
        public EqualConstraint Using<TActual, TExpected>(Func<TActual, TExpected, bool> comparison)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparison));
            return this;
        }

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        public EqualConstraint UsingPropertiesComparer()
        {
            _comparer.CompareProperties = true;
            _comparer.ComparePropertiesConfiguration = null;
            return this;
        }

        /// <summary>
        /// Enables comparing of instance properties.
        /// </summary>
        /// <remarks>
        /// This allows comparing classes that don't implement <see cref="IEquatable{T}"/>
        /// without having to compare each property separately in own code.
        /// </remarks>
        /// <param name="configure">Function to configure the <see cref="PropertiesComparerConfiguration"/></param>
        public EqualConstraint UsingPropertiesComparer(Func<PropertiesComparerConfigurationUntyped, PropertiesComparerConfigurationUntyped> configure)
        {
            _comparer.CompareProperties = true;
            _comparer.ComparePropertiesConfiguration = configure(new PropertiesComparerConfigurationUntyped());
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
            // Reset the comparer before each use, e.g. for DelayedConstraint
            if (_comparer.HasFailurePoints)
                _comparer.FailurePoints.Clear();

            AdjustArgumentIfNeeded(ref actual);
            return new EqualConstraintResult(this, actual, _comparer.AreEqual(_expected, actual, ref _tolerance));
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
                        sb.Append(' ');
                        sb.Append(_tolerance.Mode.ToString());
                    }
                }

                if (_comparer.IgnoreCase)
                    sb.Append(", ignoring case");

                if (_comparer.IgnoreWhiteSpace)
                    sb.Append(", ignoring white-space");

                if (_comparer.IgnoreWhiteSpace)
                    sb.Append(", normalizing newlines");

                return sb.ToString();
            }
        }

        #endregion

        #region Helper Methods

        // Currently, we only adjust for ArraySegments that have a
        // null array reference. Others could be added in the future.
        private static void AdjustArgumentIfNeeded<T>(ref T arg)
        {
            if (arg is not null)
            {
                Type argType = arg.GetType();
                Type? genericTypeDefinition = argType.IsGenericType ? argType.GetGenericTypeDefinition() : null;

                if (genericTypeDefinition == typeof(ArraySegment<>) &&
                    argType.GetProperty("Array")?.GetValue(arg, null) is null)
                {
                    var elementType = argType.GetGenericArguments()[0];
                    var array = Array.CreateInstance(elementType, 0);
                    var ctor = argType.GetConstructor(new[] { array.GetType() })!;
                    arg = (T)ctor.Invoke(new object[] { array });
                }
            }
        }

        #endregion
    }
}
