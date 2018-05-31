// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The Tolerance class generalizes the notion of a tolerance
    /// within which an equality test succeeds. Normally, it is
    /// used with numeric types, but it can be used with any
    /// type that supports taking a difference between two 
    /// objects and comparing that difference to a value.
    /// </summary>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public sealed class Tolerance
    {
        #region Constants and Static Properties

        private const string ModeMustFollowTolerance = "Tolerance amount must be specified before setting mode";
        private const string MultipleToleranceModes = "Tried to use multiple tolerance modes at the same time";
        private const string NumericToleranceRequired = "A numeric tolerance is required";

        /// <summary>
        /// Returns a default Tolerance object, equivalent to an exact match.
        /// </summary>
        public static Tolerance Default
        {
            get { return new Tolerance(0, ToleranceMode.Unset); }
        }

        /// <summary>
        /// Returns an empty Tolerance object, equivalent to an exact match.
        /// </summary>
        public static Tolerance Exact
        {
            get { return new Tolerance(0, ToleranceMode.Linear); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a linear tolerance of a specified amount
        /// </summary>
        public Tolerance(object amount) : this(amount, ToleranceMode.Linear) { }

        /// <summary>
        /// Constructs a tolerance given an amount and <see cref="ToleranceMode"/>
        /// </summary>
        private Tolerance(object amount, ToleranceMode mode)
        {
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
        /// Returns true if the current tolerance has not been set or is using the .
        /// </summary>
        public bool IsUnsetOrDefault
        {
            get { return Mode == ToleranceMode.Unset; }
        }

        #endregion

        #region Public Methods

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
        /// Tests that the current Tolerance is linear with a 
        /// numeric value, throwing an exception if it is not.
        /// </summary>
        private void CheckLinearAndNumeric()
        {
            if (Mode != ToleranceMode.Linear)
                throw new InvalidOperationException(Mode == ToleranceMode.Unset
                    ? ModeMustFollowTolerance
                    : MultipleToleranceModes);

            if (!Numerics.IsNumericType(Amount))
                throw new InvalidOperationException(NumericToleranceRequired);
        }

        private Range LinearRange(object value)
        {
            if (Amount is double || value is double)
            {
                var amount = Convert.ToDouble(Amount);
                var v = Convert.ToDouble(value);
                return new Range(v - amount, v + amount);
            }

            if (Amount is float || value is float)
            {
                var amount = Convert.ToSingle(Amount);
                var v = Convert.ToSingle(value);
                return new Range(v - amount, v + amount);
            }

            if (Amount is decimal || value is decimal)
            {
                var amount = Convert.ToDecimal(Amount);
                var v = Convert.ToDecimal(value);
                return new Range(v - amount, v + amount);
            }

            if (Amount is ulong || value is ulong)
            {
                var amount = Convert.ToUInt64(Amount);
                var v = Convert.ToUInt64(value);
                return new Range(v - amount, v + amount);
            }

            if (Amount is long || value is long)
            {
                var amount = Convert.ToInt64(Amount);
                var v = Convert.ToInt64(value);
                return new Range(v - amount, v + amount);
            }

            if (Amount is uint || value is uint)
            {
                var amount = Convert.ToUInt32(Amount);
                var v = Convert.ToUInt32(value);
                return new Range(v - amount, v + amount);
            }

            if (Numerics.IsFixedPointNumeric(Amount) && Numerics.IsFixedPointNumeric(value))
            {
                var amount = Convert.ToInt32(Amount);
                var v = Convert.ToInt32(value);
                return new Range(v - amount, v + amount);
            }

            throw new InvalidOperationException("Cannot create range for a non-numeric value");
        }

        private Range PercentRange(object value)
        {
            if (!Numerics.IsNumericType(Amount) || !Numerics.IsNumericType(value))
                throw new InvalidOperationException("Cannot create range for a non-numeric value");

            var v = Convert.ToDouble(value);
            var offset = v * Convert.ToDouble(Amount) / 100.0;

            return new Range(v - offset, v + offset);
        }

        #endregion

        #region Nested Range Class

        /// <summary>
        /// Tolerance.Range represents the range of values that match
        /// a specific tolerance, when applied to a specific value.
        /// </summary>
        public struct Range
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
    }
}
