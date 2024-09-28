// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The Numerics class contains common operations on numeric values.
    /// </summary>
    internal static class Numerics
    {
        #region Numeric Type Recognition
        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a numeric type</returns>
        public static bool IsNumericType(object? obj)
        {
            return IsFloatingPointNumeric(obj) || IsFixedPointNumeric(obj);
        }

        internal static bool IsNumericType(Type type)
        {
            return IsFloatingPointNumeric(type) || IsFixedPointNumeric(type);
        }

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a floating point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a floating point numeric type</returns>
        public static bool IsFloatingPointNumeric(object? obj)
        {
            if (obj is not null)
            {
                if (obj is double)
                    return true;
                if (obj is float)
                    return true;
                if (obj is decimal)
                    return true;
            }
            return false;
        }

        internal static bool IsFloatingPointNumeric(Type type)
        {
            if (type is not null)
            {
                if (type == typeof(double))
                    return true;
                if (type == typeof(float))
                    return true;
                if (type == typeof(decimal))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a fixed point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a fixed point numeric type</returns>
        public static bool IsFixedPointNumeric(object? obj)
        {
            if (obj is not null)
            {
                if (obj is byte)
                    return true;
                if (obj is sbyte)
                    return true;
                if (obj is int)
                    return true;
                if (obj is uint)
                    return true;
                if (obj is long)
                    return true;
                if (obj is ulong)
                    return true;
                if (obj is short)
                    return true;
                if (obj is ushort)
                    return true;
                if (obj is char)
                    return true;
            }
            return false;
        }

        internal static bool IsFixedPointNumeric(Type type)
        {
            if (type is not null)
            {
                if (type == typeof(byte))
                    return true;
                if (type == typeof(sbyte))
                    return true;
                if (type == typeof(int))
                    return true;
                if (type == typeof(uint))
                    return true;
                if (type == typeof(long))
                    return true;
                if (type == typeof(ulong))
                    return true;
                if (type == typeof(short))
                    return true;
                if (type == typeof(ushort))
                    return true;
                if (type == typeof(char))
                    return true;
            }
            return false;
        }

        private static bool IsWithinDecimalRange(double value)
        {
            return value >= (double)decimal.MinValue && value <= (double)decimal.MaxValue;
        }
        #endregion

        #region Numeric Equality
        /// <summary>
        /// Test two numeric values for equality, performing the usual numeric
        /// conversions and using a provided or default tolerance. If the tolerance
        /// provided is Empty, this method may set it to a default tolerance.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="tolerance">A reference to the tolerance in effect</param>
        /// <returns>True if the values are equal</returns>
        public static bool AreEqual(object expected, object actual, ref Tolerance tolerance)
        {
            if (expected is double || actual is double)
                return AreEqual(Convert.ToDouble(expected), Convert.ToDouble(actual), ref tolerance);

            if (expected is float || actual is float)
                return AreEqual(Convert.ToSingle(expected), Convert.ToSingle(actual), ref tolerance);

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException("Ulps may only be specified for floating point arguments");

            if (expected is decimal || actual is decimal)
                return AreEqual(Convert.ToDecimal(expected), Convert.ToDecimal(actual), tolerance);

            if (expected is ulong || actual is ulong)
                return AreEqual(Convert.ToUInt64(expected), Convert.ToUInt64(actual), tolerance);

            if (expected is long || actual is long)
                return AreEqual(Convert.ToInt64(expected), Convert.ToInt64(actual), tolerance);

            if (expected is uint || actual is uint)
                return AreEqual(Convert.ToUInt32(expected), Convert.ToUInt32(actual), tolerance);

            return AreEqual(Convert.ToInt32(expected), Convert.ToInt32(actual), tolerance);
        }

        private static bool AreEqual(double expected, double actual, ref Tolerance tolerance)
        {
            if (double.IsNaN(expected) && double.IsNaN(actual))
                return true;

            // Handle infinity specially since subtracting two infinite values gives
            // NaN and the following test fails. mono also needs NaN to be handled
            // specially although ms.net could use either method. Also, handle
            // situation where no tolerance is used.
            if (double.IsInfinity(expected) || double.IsNaN(expected) || double.IsNaN(actual))
            {
                return expected.Equals(actual);
            }

            if (tolerance.IsUnsetOrDefault)
            {
                var temp = TestExecutionContext.CurrentContext?.DefaultFloatingPointTolerance;
                if (temp is not null && !temp.IsUnsetOrDefault)
                    tolerance = temp;
            }

            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance.Amount);

                case ToleranceMode.Percent:
                    if (expected == 0.0)
                        return expected.Equals(actual);

                    double relativeError = Math.Abs((expected - actual) / expected);
                    return (relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0);

                case ToleranceMode.Ulps:
                    return FloatingPointNumerics.AreAlmostEqualUlps(
                        expected, actual, Convert.ToInt64(tolerance.Amount));

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(float expected, float actual, ref Tolerance tolerance)
        {
            if (float.IsNaN(expected) && float.IsNaN(actual))
                return true;

            // handle infinity specially since subtracting two infinite values gives
            // NaN and the following test fails. mono also needs NaN to be handled
            // specially although ms.net could use either method.
            if (float.IsInfinity(expected) || float.IsNaN(expected) || float.IsNaN(actual))
            {
                return expected.Equals(actual);
            }

            if (tolerance.IsUnsetOrDefault)
            {
                var temp = TestExecutionContext.CurrentContext?.DefaultFloatingPointTolerance;
                if (temp is not null && !temp.IsUnsetOrDefault)
                    tolerance = temp;
            }

            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    return Math.Abs(expected - actual) <= Convert.ToDouble(tolerance.Amount);

                case ToleranceMode.Percent:
                    if (expected == 0.0f)
                        return expected.Equals(actual);
                    float relativeError = Math.Abs((expected - actual) / expected);
                    return (relativeError <= Convert.ToSingle(tolerance.Amount) / 100.0f);

                case ToleranceMode.Ulps:
                    return FloatingPointNumerics.AreAlmostEqualUlps(
                        expected, actual, Convert.ToInt32(tolerance.Amount));

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(decimal expected, decimal actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    decimal decimalTolerance = Convert.ToDecimal(tolerance.Amount);
                    if (decimalTolerance > 0m)
                        return Math.Abs(expected - actual) <= decimalTolerance;

                    return expected.Equals(actual);

                case ToleranceMode.Percent:
                    if (expected == 0m)
                        return expected.Equals(actual);

                    double relativeError = Math.Abs(
                        (double)(expected - actual) / (double)expected);
                    return (relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(ulong expected, ulong actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    ulong ulongTolerance = Convert.ToUInt64(tolerance.Amount);
                    if (ulongTolerance > 0ul)
                    {
                        ulong diff = expected >= actual ? expected - actual : actual - expected;
                        return diff <= ulongTolerance;
                    }

                    return expected.Equals(actual);

                case ToleranceMode.Percent:
                    if (expected == 0ul)
                        return expected.Equals(actual);

                    // Can't do a simple Math.Abs() here since it's unsigned
                    ulong difference = Math.Max(expected, actual) - Math.Min(expected, actual);
                    double relativeError = Math.Abs((double)difference / (double)expected);
                    return (relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(long expected, long actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    long longTolerance = Convert.ToInt64(tolerance.Amount);
                    if (longTolerance > 0L)
                        return Math.Abs(expected - actual) <= longTolerance;

                    return expected.Equals(actual);

                case ToleranceMode.Percent:
                    if (expected == 0L)
                        return expected.Equals(actual);

                    double relativeError = Math.Abs(
                        (double)(expected - actual) / (double)expected);
                    return relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0;

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(uint expected, uint actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    uint uintTolerance = Convert.ToUInt32(tolerance.Amount);
                    if (uintTolerance > 0)
                    {
                        uint diff = expected >= actual ? expected - actual : actual - expected;
                        return diff <= uintTolerance;
                    }

                    return expected.Equals(actual);

                case ToleranceMode.Percent:
                    if (expected == 0u)
                        return expected.Equals(actual);

                    // Can't do a simple Math.Abs() here since it's unsigned
                    double difference = Math.Max(expected, actual) - Math.Min(expected, actual);
                    double relativeError = Math.Abs(difference / expected);
                    return (relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual(int expected, int actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    int intTolerance = Convert.ToInt32(tolerance.Amount);
                    if (intTolerance > 0)
                        return Math.Abs(expected - actual) <= intTolerance;

                    return expected.Equals(actual);

                case ToleranceMode.Percent:
                    if (expected == 0)
                        return expected.Equals(actual);

                    double relativeError = Math.Abs(
                        (double)(expected - actual) / (double)expected);
                    return relativeError <= Convert.ToDouble(tolerance.Amount) / 100.0;

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }
        #endregion

        #region Numeric Comparisons

        /// <summary>
        /// Compare two numeric values, performing the usual numeric conversions.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <returns>The relationship of the values to each other</returns>
        public static int Compare(object expected, object actual)
        {
            if (!IsNumericType(expected) || !IsNumericType(actual))
                throw new ArgumentException("Both arguments must be numeric");

            if (expected is decimal || actual is decimal)
            {
                // Treat as decimal if one is decimal and other can be treated as decimal
                if (expected is decimal eDec && IsWithinDecimalRange(Convert.ToDouble(actual)))
                    return eDec.CompareTo(Convert.ToDecimal(actual));
                else if (actual is decimal aDec && IsWithinDecimalRange(Convert.ToDouble(expected)))
                    return Convert.ToDecimal(expected).CompareTo(aDec);
                else
                    return Convert.ToDouble(expected).CompareTo(Convert.ToDouble(actual));
            }

            if (expected is double || actual is double)
                return Convert.ToDouble(expected).CompareTo(Convert.ToDouble(actual));

            if (expected is float || actual is float)
                return Convert.ToSingle(expected).CompareTo(Convert.ToSingle(actual));

            if (expected is ulong || actual is ulong)
                return Convert.ToUInt64(expected).CompareTo(Convert.ToUInt64(actual));

            if (expected is long || actual is long)
                return Convert.ToInt64(expected).CompareTo(Convert.ToInt64(actual));

            if (expected is uint || actual is uint)
                return Convert.ToUInt32(expected).CompareTo(Convert.ToUInt32(actual));

            return Convert.ToInt32(expected).CompareTo(Convert.ToInt32(actual));
        }
        #endregion

        #region Numeric Difference

        /// <summary>
        /// Calculates the difference between 2 values in absolute/percent mode.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="toleranceMode">Tolerance mode to specify difference representation</param>
        /// <returns>The difference between the values</returns>
        internal static object Difference(object? expected, object? actual, ToleranceMode toleranceMode)
        {
            switch (toleranceMode)
            {
                case ToleranceMode.Linear:
                    return Difference(expected, actual, true);
                case ToleranceMode.Percent:
                    return Difference(expected, actual, false);
                default:
                    throw new InvalidOperationException("Cannot calculate a difference for specified tolerance mode");
            }
        }

        private static object Difference(object? expected, object? actual, bool isAbsolute)
        {
            // In case the difference cannot be calculated return NaN to prevent unhandled runtime exceptions
            if (!IsNumericType(expected) || !IsNumericType(actual))
                return double.NaN;

            // Treat as decimal if one is decimal and other can be treated as decimal
            if (expected is decimal eDec && IsWithinDecimalRange(Convert.ToDouble(actual)))
            {
                var difference = eDec - Convert.ToDecimal(actual);
                return isAbsolute ? difference : difference / eDec * 100;
            }
            else if (actual is decimal aDec && IsWithinDecimalRange(Convert.ToDouble(expected)))
            {
                var difference = Convert.ToDecimal(expected) - aDec;
                return isAbsolute ? difference : difference / Convert.ToDecimal(expected) * 100;
            }

            if (IsFloatingPointNumeric(expected) || IsFloatingPointNumeric(actual))
            {
                var difference = Convert.ToDouble(expected) - Convert.ToDouble(actual);
                return isAbsolute ? difference : difference / Convert.ToDouble(expected) * 100;
            }

            if (expected is ulong || actual is ulong)
            {
                var difference = Convert.ToUInt64(expected) - Convert.ToUInt64(actual);
                return isAbsolute ? difference : difference / (double)Convert.ToUInt64(expected) * 100;
            }

            if (expected is long || actual is long)
            {
                var difference = Convert.ToInt64(expected) - Convert.ToInt64(actual);
                return isAbsolute ? difference : difference / (double)Convert.ToInt64(expected) * 100;
            }

            if (expected is uint || actual is uint)
            {
                var difference = Convert.ToUInt32(expected) - Convert.ToUInt32(actual);
                return isAbsolute ? difference : difference / (double)Convert.ToUInt32(expected) * 100;
            }

            var intDifference = Convert.ToInt32(expected) - Convert.ToInt32(actual);
            return isAbsolute ? intDifference : intDifference / (double)Convert.ToInt32(expected) * 100;
        }

        #endregion
    }
}
