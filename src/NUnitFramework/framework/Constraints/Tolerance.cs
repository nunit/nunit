// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The Tolerance class generalizes the notion of a tolerance
    /// within which an equality test succeeds. Normally, it is
    /// used with numeric types, but it can be used with any
    /// type that supports taking a difference between two
    /// objects and comparing that difference to a value.
    /// </summary>
    [Serializable]
    public sealed class Tolerance
    {
        #region Constants and Static Properties

        private const string ModeMustFollowTolerance = "Tolerance amount must be specified before setting mode";
        private const string MultipleToleranceModes = "Tried to use multiple tolerance modes at the same time";
        private const string NumericToleranceRequired = "A numeric tolerance is required";
        private const string NoNegativeTolerance = "Tolerance amount must not be negative";

        /// <summary>
        /// Returns a default Tolerance object, equivalent to a default matching rules.
        /// </summary>
        public static readonly Tolerance Default = new(0, ToleranceMode.Unset);

        /// <summary>
        /// Returns an empty Tolerance object, equivalent to an exact match.
        /// </summary>
        public static readonly Tolerance Exact = new(0, ToleranceMode.Linear);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a linear tolerance of a specified amount
        /// </summary>
        public Tolerance(object amount) : this(amount, ToleranceMode.Linear)
        {
        }

        /// <summary>
        /// Constructs a tolerance given an amount and <see cref="ToleranceMode"/>
        /// </summary>
        private Tolerance(object amount, ToleranceMode mode)
        {
            CheckNotNegative(amount);
            Amount = amount;
            Mode = mode;
        }

        #endregion

        #region Modifier Properties

        /// <summary>
        /// Returns a new tolerance, using the current amount as a percentage.
        /// </summary>
        public Tolerance Percent
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(Amount, ToleranceMode.Percent);
            }
        }

        /// <summary>
        /// Returns a new tolerance, using the current amount in Ulps
        /// </summary>
        public Tolerance Ulps
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(Amount, ToleranceMode.Ulps);
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of days.
        /// </summary>
        public Tolerance Days
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromDays(Convert.ToDouble(Amount)));
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of hours.
        /// </summary>
        public Tolerance Hours
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromHours(Convert.ToDouble(Amount)));
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of minutes.
        /// </summary>
        public Tolerance Minutes
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromMinutes(Convert.ToDouble(Amount)));
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of seconds.
        /// </summary>
        public Tolerance Seconds
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromSeconds(Convert.ToDouble(Amount)));
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of milliseconds.
        /// </summary>
        public Tolerance Milliseconds
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromMilliseconds(Convert.ToDouble(Amount)));
            }
        }

        /// <summary>
        /// Returns a new tolerance with a <see cref="TimeSpan"/> as the amount, using
        /// the current amount as a number of clock ticks.
        /// </summary>
        public Tolerance Ticks
        {
            get
            {
                CheckLinearAndNumeric();
                return new Tolerance(TimeSpan.FromTicks(Convert.ToInt64(Amount)));
            }
        }

        #endregion

        #region Other Public Properties

        /// <summary>
        /// Gets the <see cref="ToleranceMode"/> for the current Tolerance
        /// </summary>
        public ToleranceMode Mode { get; }

        /// <summary>
        /// Gets the magnitude of the current Tolerance instance.
        /// </summary>
        public object Amount { get; }

        /// <summary>
        /// Returns true if the current tolerance has not been set or is using the default.
        /// </summary>
        public bool IsUnsetOrDefault => Mode == ToleranceMode.Unset;

        /// <summary>
        /// Returns true if the current tolerance varies from exact and default. Indicating tolerance needs processing.
        /// </summary>
        public bool HasVariance => Mode != ToleranceMode.Unset && this != Exact && this != Default;
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the provided tolerance to the current TestExecutionContext.DefaultFloatingPointTolerance when the
        /// provided tolerance is unset or default.
        /// </summary>
        /// <param name="tolerance" />
        internal static void ApplyDefaultFloatingPointTolerance(ref Tolerance tolerance)
        {
            if (tolerance.IsUnsetOrDefault)
            {
                var temp = TestExecutionContext.CurrentContext?.DefaultFloatingPointTolerance;
                if (temp is not null && !temp.IsUnsetOrDefault)
                    tolerance = temp;
            }
        }

        /// <summary>
        /// Apply the tolerance to an expected value and return
        /// a Tolerance.Range that represents the acceptable values.
        /// </summary>
        public Range ApplyToValue(object value)
        {
            // TODO: This should really be a generic, but we will
            // first have to make Tolerance and the constraints
            // that use it generic as well.
            switch (Mode)
            {
                default:
                case ToleranceMode.Unset:
                    return new Range(value, value);

                case ToleranceMode.Linear:
                    return LinearRange(value);

                case ToleranceMode.Percent:
                    return PercentRange(value);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Tests that <paramref name="value" /> is not negative.
        ///     Throws an exception otherwise.
        /// </summary>
        private static void CheckNotNegative(object value)
        {
            if (Numerics.IsNumericType(value) && Numerics.Compare(value, 0) < 0)
                throw new ArgumentException(NoNegativeTolerance);

            if (value is TimeSpan timeSpan && timeSpan.Ticks < 0)
                throw new ArgumentException(NoNegativeTolerance);
        }

        /// <summary>
        /// Tests that the current Tolerance is linear with a
        /// numeric value, throwing an exception if it is not.
        /// </summary>
        private void CheckLinearAndNumeric()
        {
            if (Mode != ToleranceMode.Linear)
            {
                throw new InvalidOperationException(Mode == ToleranceMode.Unset
                    ? ModeMustFollowTolerance
                    : MultipleToleranceModes);
            }

            if (!Numerics.IsNumericType(Amount))
                throw new InvalidOperationException(NumericToleranceRequired);
        }

        private Range LinearRange(object value)
        {
            if (Amount is double || value is double)
            {
                var v = Numerics.ConvertToValue<double>(value);
                var offset = Numerics.ConvertToValue<double>(Amount);

                return new Range(v - offset, v + offset);
            }

            if (Amount is float || value is float)
            {
                var v = Numerics.ConvertToValue<float>(value);
                var offset = Numerics.ConvertToValue<float>(Amount);

                return new Range(v - offset, v + offset);
            }

#if !NETFRAMEWORK
            if (Amount is Half || value is Half)
            {
                var v = Numerics.ConvertToValue<Half>(value);
                var offset = Numerics.ConvertToValue<Half>(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return new Range((decimal)v - (decimal)offset, (decimal)v + (decimal)offset);

                return new Range(lowerBound, upperBound);
            }
#endif

            if (Amount is decimal || value is decimal)
            {
                var v = Numerics.ConvertToValue<decimal>(value);
                var offset = Numerics.ConvertToValue<decimal>(Amount);

                return new Range(v - offset, v + offset);
            }

#if !NETFRAMEWORK
            if (Amount is UInt128 || value is UInt128)
            {
                var v = Numerics.ConvertToValue<UInt128>(value);
                var offset = Numerics.ConvertToValue<UInt128>(Amount);

                // We can type promote smaller precision types, but not here
                // So we naively calculate the range and let it overflow if needed.
                // There is not much we can do about it, and this will provide the clearest failure message.
                return new Range(v - offset, v + offset);
            }

            if (Amount is Int128 || value is Int128)
            {
                var v = Numerics.ConvertToValue<Int128>(value);
                var offset = Numerics.ConvertToValue<Int128>(Amount);

                // We can type promote smaller precision types, but not here
                // So we naively calculate the range and let it overflow if needed.
                // There is not much we can do about it, and this will provide the clearest failure message.
                return new Range(v - offset, v + offset);
            }
#endif

            if (Amount is nuint || value is nuint)
            {
                var v = Numerics.ConvertToValue<nuint>(value);
                var offset = Numerics.ConvertToValue<nuint>(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return GetInt128Range(v, offset);

                return new Range(lowerBound, upperBound);
            }

            if (Amount is nint || value is nint)
            {
                var v = Numerics.ConvertToValue<nint>(value);
                var offset = Numerics.ConvertToValue<nint>(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return GetInt128Range(v, offset);

                return new Range(lowerBound, upperBound);
            }

            if (Amount is ulong || value is ulong)
            {
                var v = Convert.ToUInt64(value);
                var offset = Convert.ToUInt64(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return GetInt128Range(v, offset);

                return new Range(lowerBound, upperBound);
            }

            if (Amount is long || value is long)
            {
                var v = Convert.ToInt64(value);
                var offset = Convert.ToInt64(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return GetInt128Range(v, offset);

                return new Range(lowerBound, upperBound);
            }

            if (Amount is uint || value is uint)
            {
                var v = Convert.ToUInt32(value);
                var offset = Convert.ToUInt32(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return new Range((long)v - offset, (long)v + offset);

                return new Range(lowerBound, upperBound);
            }

            if (Numerics.IsFixedPointNumeric(Amount) && Numerics.IsFixedPointNumeric(value))
            {
                var v = Convert.ToInt32(value);
                var offset = Convert.ToInt32(Amount);

                var lowerBound = v - offset;
                var upperBound = v + offset;

                if (lowerBound >= v || upperBound <= v) // overflow or underflow
                    return new Range((long)v - offset, (long)v + offset);

                return new Range(lowerBound, upperBound);
            }

            if (Amount is TimeSpan interval && value is DateTime dateTime)
            {
                return new Range(dateTime - interval, dateTime + interval);
            }

            throw new InvalidOperationException("Cannot create range for a non-numeric value");

            static Range GetInt128Range(object value, object amount)
            {
#if !NETFRAMEWORK
                var v = Numerics.ConvertToValue<Int128>(value);
                var offset = Numerics.ConvertToValue<Int128>(amount);
                return new Range(v - offset, v + offset);
#else
                throw new InvalidOperationException("The requested values are out of calculable range.");
#endif
            }
        }

        private Range PercentRange(object value)
        {
            if (!Numerics.IsNumericType(Amount) || !Numerics.IsNumericType(value))
                throw new InvalidOperationException("Cannot create range for a non-numeric value");

#if !NETFRAMEWORK
            if (Amount is UInt128 || value is UInt128)
            {
                double v = Numerics.ConvertToValue<double>(value);
                double amount = Numerics.ConvertToValue<double>(Amount);
                return CalculateDoubleBasedRange(v, amount);
            }

            if (Amount is Int128 || value is Int128)
            {
                double v = Numerics.ConvertToValue<double>(value);
                double amount = Numerics.ConvertToValue<double>(Amount);
                return CalculateDoubleBasedRange(v, amount);
            }

            if (Amount is Half || value is Half)
            {
                double v = Numerics.ConvertToValue<double>(value);
                double amount = Numerics.ConvertToValue<double>(Amount);
                return CalculateDoubleBasedRange(v, amount);
            }
#endif
            if (Amount is nuint || value is nuint)
            {
                ulong v = value is nuint nuintValue ? nuintValue : Convert.ToUInt64(value);
                ulong amount = Amount is nuint nuintAmount ? nuintAmount : Convert.ToUInt64(Amount);
                return CalculateDoubleBasedRange(v, amount);
            }

            if (Amount is nint || value is nint)
            {
                long v = value is nint nintValue ? nintValue : Convert.ToInt64(value);
                long amount = Amount is nint nintAmount ? nintAmount : Convert.ToInt64(Amount);
                return CalculateDoubleBasedRange(v, amount);
            }

            return CalculateDoubleBasedRange(value, Amount);
        }
        private static Range CalculateDoubleBasedRange(object value, object amount)
                => CalculateDoubleBasedRange(Convert.ToDouble(value), Convert.ToDouble(amount));

        private static Range CalculateDoubleBasedRange(double value, double amount)
        {
            // Calculate the absolute value of the offset in case 'value' is negative to ensure lower bounds is always less than upper bounds
            var offset = Math.Abs(value * amount / 100.0);

            var lowerBound = value - offset;
            var upperBound = value + offset;
            return new Range(lowerBound, upperBound);
        }

        #endregion

        #region Nested Range Class

        /// <summary>
        /// Tolerance.Range represents the range of values that match
        /// a specific tolerance, when applied to a specific value.
        /// </summary>
        public readonly struct Range
        {
            /// <summary>
            /// The lower bound of the range
            /// </summary>
            public readonly object LowerBound;

            /// <summary>
            /// The upper bound of the range
            /// </summary>
            public readonly object UpperBound;

            /// <summary>
            /// Constructs a range
            /// </summary>
            public Range(object lowerBound, object upperBound)
            {
                LowerBound = lowerBound;
                UpperBound = upperBound;
            }
        }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string? ToString()
        {
            if (Amount == Exact.Amount && Mode == Exact.Mode)
            {
                return "Exact";
            }
            switch (Mode)
            {
                case ToleranceMode.Unset:
                    return "Unset";

                case ToleranceMode.Linear:
                    return Amount.ToString();

                case ToleranceMode.Percent:
                    return Amount.ToString() + " Percent";

                case ToleranceMode.Ulps:
                    return Amount.ToString() + " Ulps";

                default:
                    return "Unknown"; // Unreachable without reflection
            }
        }
    }
}
