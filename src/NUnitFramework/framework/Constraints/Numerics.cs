// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NETFRAMEWORK
using System.Numerics;
#endif

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The Numerics class contains common operations on numeric values.
    /// </summary>
    internal static class Numerics
    {
        private const string UlpsRequiresFloatingPoint = "Ulps may only be specified for floating point arguments";

        #region Numeric Type Recognition
        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a numeric type</returns>
        public static bool IsNumericType(object obj)
            => IsNumericType(obj.GetType());

        internal static bool IsNumericType(Type type)
            => IsFloatingPointNumeric(type) || IsFixedPointNumeric(type);

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a floating point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a floating point numeric type</returns>
        public static bool IsFloatingPointNumeric(object? obj)
            => obj is not null && IsFloatingPointNumeric(obj.GetType());

        private static bool IsFloatingPointNumeric(Type type)
        {
            if (type == typeof(double))
                return true;
            if (type == typeof(float))
                return true;
            if (type == typeof(decimal))
                return true;
#if !NETFRAMEWORK
            if (type == typeof(Half))
                return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks the type of the object, returning true if
        /// the object is a fixed point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a fixed point numeric type</returns>
        public static bool IsFixedPointNumeric(object? obj)
            => obj is not null && IsFixedPointNumeric(obj.GetType());

        private static bool IsFixedPointNumeric(Type type)
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
            if (type == typeof(nint))
                return true;
            if (type == typeof(nuint))
                return true;
#if !NETFRAMEWORK
            if (type == typeof(Int128))
                return true;
            if (type == typeof(UInt128))
                return true;
#endif
            return false;
        }

        /// <summary>
        /// Checks whether one value is a signed integer and the other an unsigned integer
        /// that does not always fit in an <see cref="int"/>, so neither type can hold all values of the other.
        /// </summary>
        private static bool IsMixedSignInteger(object expected, object actual)
            => (IsSignedInteger(expected) && IsUnsignedInteger(actual)) ||
               (IsUnsignedInteger(expected) && IsSignedInteger(actual));

        private static bool IsSignedInteger(object value)
#if NETFRAMEWORK
            => value is sbyte or short or int or long or nint;
#else
            => value is sbyte or short or int or long or nint or Int128;
#endif

        // byte, ushort and char are not included as they always fit in an int.
        private static bool IsUnsignedInteger(object value)
#if NETFRAMEWORK
            => value is uint or ulong or nuint;
#else
            => value is uint or ulong or nuint or UInt128;
#endif

        private static bool Is32BitOrSmallerInteger(object value)
            => value is sbyte or byte or short or ushort or char or int or uint;

#if !NETFRAMEWORK
        private static readonly Int128 Int128DecimalMinValue = (Int128)decimal.MinValue;
        private static readonly Int128 Int128DecimalMaxValue = (Int128)decimal.MaxValue;
        private static readonly UInt128 UInt128DecimalMaxValue = (UInt128)decimal.MaxValue;
#endif

        private static bool IsWithinDecimalRange(object value)
        {
            switch (value)
            {
                case decimal:
                    return true;

                case double or float:
                    // decimal.MaxValue rounds up to 2^96 as double or float, which is outside the decimal range.
                    return Math.Abs(ConvertToValue<double>(value)) < (double)decimal.MaxValue;

#if !NETFRAMEWORK
                // Check 128-bit integers exactly, as rounding to double can move values
                // just outside the decimal range onto its boundary.
                case Int128 int128Value:
                    return int128Value >= Int128DecimalMinValue && int128Value <= Int128DecimalMaxValue;
                case UInt128 uint128Value:
                    return uint128Value <= UInt128DecimalMaxValue;
#endif

                default:
                    // Half and all other integer types fit in a decimal.
                    return true;
            }
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
            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
            // Use ConvertToValue<T> to convert until the only remaining types are IConvertible.

            if (expected is decimal || actual is decimal)
            {
                // Treat as decimal if both values fit, otherwise fall back to double, like Compare does
                if (!IsWithinDecimalRange(expected) || !IsWithinDecimalRange(actual))
                    return AreIeeeFloatsEqual(ConvertToValue<double>(expected), ConvertToValue<double>(actual), ref tolerance);

                if (tolerance.Mode == ToleranceMode.Ulps)
                    throw new InvalidOperationException(UlpsRequiresFloatingPoint);

                return AreEqual(ConvertToValue<decimal>(expected), ConvertToValue<decimal>(actual), tolerance);
            }

            if (expected is double || actual is double)
                return AreIeeeFloatsEqual(ConvertToValue<double>(expected), ConvertToValue<double>(actual), ref tolerance);

            if (expected is float || actual is float)
                return AreIeeeFloatsEqual(ConvertToValue<float>(expected), ConvertToValue<float>(actual), ref tolerance);

#if !NETFRAMEWORK
            if (expected is Half || actual is Half)
                return AreIeeeFloatsEqual(ConvertToValue<Half>(expected), ConvertToValue<Half>(actual), ref tolerance);
#endif

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException(UlpsRequiresFloatingPoint);

            // Converting a negative value to an unsigned type would overflow,
            // so use a signed type that can hold both values.
            if (IsMixedSignInteger(expected, actual))
            {
                // Like C#, promote uint mixed with a signed type of 32 bits or less to long
                if (Is32BitOrSmallerInteger(expected) && Is32BitOrSmallerInteger(actual))
                    return AreEqual(ConvertToValue<long>(expected), ConvertToValue<long>(actual), tolerance);

#if NETFRAMEWORK
                return AreEqual(ConvertToValue<decimal>(expected), ConvertToValue<decimal>(actual), tolerance);
#else
                if (expected is UInt128 || actual is UInt128)
                    return AreEqual(ConvertToValue<BigInteger>(expected), ConvertToValue<BigInteger>(actual), tolerance);

                return AreEqual(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), tolerance);
#endif
            }

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return AreEqual(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), tolerance);

            if (expected is Int128 || actual is Int128)
                return AreEqual(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), tolerance);
#endif

            if (expected is ulong || actual is ulong)
                return AreEqual(ConvertToValue<ulong>(expected), ConvertToValue<ulong>(actual), tolerance);

            if (expected is long || actual is long)
                return AreEqual(ConvertToValue<long>(expected), ConvertToValue<long>(actual), tolerance);

            if (expected is nuint || actual is nuint)
                return AreEqual(ConvertToValue<nuint>(expected), ConvertToValue<nuint>(actual), tolerance);

            if (expected is nint || actual is nint)
                return AreEqual(ConvertToValue<nint>(expected), ConvertToValue<nint>(actual), tolerance);

            if (expected is uint || actual is uint)
                return AreEqual(ConvertToValue<uint>(expected), ConvertToValue<uint>(actual), tolerance);

            return AreEqual(ConvertToValue<int>(expected), ConvertToValue<int>(actual), tolerance);
        }

        /// <summary>
        /// Test two numeric values for equality, performing the usual numeric
        /// conversions and using a provided or default tolerance. If the tolerance
        /// provided is Empty, this method may set it to a default tolerance.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="tolerance">A reference to the tolerance in effect</param>
        /// <returns>True if the values are equal</returns>
        public static bool AreEqual<T>(T expected, T actual, ref Tolerance tolerance)
            where T : struct, IConvertible
        {
            if (expected is double || actual is double)
                return AreIeeeFloatsEqual(expected.ToDouble(null), actual.ToDouble(null), ref tolerance);

            if (expected is float || actual is float)
                return AreIeeeFloatsEqual(expected.ToSingle(null), actual.ToSingle(null), ref tolerance);

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException(UlpsRequiresFloatingPoint);

            if (expected is decimal || actual is decimal)
                return AreEqual(expected.ToDecimal(null), actual.ToDecimal(null), tolerance);

            if (expected is ulong || actual is ulong)
                return AreEqual(expected.ToUInt64(null), actual.ToUInt64(null), tolerance);

            if (expected is long || actual is long)
                return AreEqual(expected.ToInt64(null), actual.ToInt64(null), tolerance);

            if (expected is uint || actual is uint)
                return AreEqual(expected.ToUInt32(null), actual.ToUInt32(null), tolerance);

            return AreEqual(expected.ToInt32(null), actual.ToInt32(null), tolerance);
        }

#if NETFRAMEWORK
        private static bool AreIeeeFloatsEqual(double expected, double actual, ref Tolerance tolerance)
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

            Tolerance.ApplyDefaultFloatingPointTolerance(ref tolerance);

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

        private static bool AreIeeeFloatsEqual(float expected, float actual, ref Tolerance tolerance)
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

            Tolerance.ApplyDefaultFloatingPointTolerance(ref tolerance);

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
#endif

        // Used on .NET and .NET Framework, as decimal arithmetic can overflow.
        private static bool AreEqual(decimal expected, decimal actual, Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    // A difference outside the decimal range can still be within a tolerance outside the decimal range.
                    if (!TryAbsoluteDifference(expected, actual, out decimal difference))
                        return Math.Abs((double)expected - (double)actual) <= ConvertToValue<double>(tolerance.Amount);

                    return !IsWithinDecimalRange(tolerance.Amount) || difference <= ConvertToValue<decimal>(tolerance.Amount);

                case ToleranceMode.Percent:
                    if (expected == 0m)
                        return expected.Equals(actual);

                    double absoluteDifference = TryAbsoluteDifference(expected, actual, out decimal exactDifference)
                        ? (double)exactDifference
                        : Math.Abs((double)expected - (double)actual);
                    double relativeError = absoluteDifference / Math.Abs((double)expected);
                    return relativeError <= ConvertToValue<double>(tolerance.Amount) / 100.0;

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        // Subtracting decimals with different signs can exceed the decimal range.
        private static bool TryAbsoluteDifference(decimal x, decimal y, out decimal difference)
        {
            if ((x < 0m) != (y < 0m) && Math.Abs(x) > decimal.MaxValue - Math.Abs(y))
            {
                difference = decimal.MaxValue;
                return false;
            }

            difference = Math.Abs(x - y);
            return true;
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

            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
            // Use ConvertToValue<T> to convert until the only remaining types are IConvertible.

            if (expected is decimal || actual is decimal)
            {
                // Treat as decimal if one is decimal and other can be treated as decimal
                if (expected is decimal eDec && IsWithinDecimalRange(actual))
                    return eDec.CompareTo(ConvertToValue<decimal>(actual));
                else if (actual is decimal aDec && IsWithinDecimalRange(expected))
                    return ConvertToValue<decimal>(expected).CompareTo(aDec);
                else
                    return ConvertToValue<double>(expected).CompareTo(ConvertToValue<double>(actual));
            }

            if (expected is double || actual is double)
                return ConvertToValue<double>(expected).CompareTo(ConvertToValue<double>(actual));

            if (expected is float || actual is float)
                return ConvertToValue<float>(expected).CompareTo(ConvertToValue<float>(actual));

#if !NETFRAMEWORK
            if (expected is Half || actual is Half)
                return ConvertToValue<Half>(expected).CompareTo(ConvertToValue<Half>(actual));
#endif

            // Converting a negative value to an unsigned type would overflow,
            // so use a signed type that can hold both values.
            if (IsMixedSignInteger(expected, actual))
            {
                // Like C#, promote uint mixed with a signed type of 32 bits or less to long
                if (Is32BitOrSmallerInteger(expected) && Is32BitOrSmallerInteger(actual))
                    return ConvertToValue<long>(expected).CompareTo(ConvertToValue<long>(actual));

#if NETFRAMEWORK
                return ConvertToValue<decimal>(expected).CompareTo(ConvertToValue<decimal>(actual));
#else
                if (expected is UInt128 || actual is UInt128)
                    return ConvertToValue<BigInteger>(expected).CompareTo(ConvertToValue<BigInteger>(actual));

                return ConvertToValue<Int128>(expected).CompareTo(ConvertToValue<Int128>(actual));
#endif
            }

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return ConvertToValue<UInt128>(expected).CompareTo(ConvertToValue<UInt128>(actual));

            if (expected is Int128 || actual is Int128)
                return ConvertToValue<Int128>(expected).CompareTo(ConvertToValue<Int128>(actual));
#endif

            if (expected is ulong || actual is ulong)
                return ConvertToValue<ulong>(expected).CompareTo(ConvertToValue<ulong>(actual));

            if (expected is long || actual is long)
                return ConvertToValue<long>(expected).CompareTo(ConvertToValue<long>(actual));

            if (expected is nuint || actual is nuint)
                return ConvertToValue<nuint>(expected).CompareTo(ConvertToValue<nuint>(actual));

            if (expected is nint || actual is nint)
                return ConvertToValue<nint>(expected).CompareTo(ConvertToValue<nint>(actual));

            if (expected is uint || actual is uint)
                return ConvertToValue<uint>(expected).CompareTo(ConvertToValue<uint>(actual));

            return ConvertToValue<int>(expected).CompareTo(ConvertToValue<int>(actual));
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
        internal static object Difference(object expected, object actual, ToleranceMode toleranceMode)
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

        private static object Difference(object expected, object actual, bool isAbsolute)
        {
            // In case the difference cannot be calculated return NaN to prevent unhandled runtime exceptions
            if (!IsNumericType(expected) || !IsNumericType(actual))
                return double.NaN;

            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
            // Use ConvertToValue<T> to convert until the only remaining types are IConvertible.

            // Treat as decimal if one is decimal and other can be treated as decimal
            if (expected is decimal eDec && IsWithinDecimalRange(actual))
            {
                var difference = eDec - ConvertToValue<decimal>(actual);
                return isAbsolute ? difference : difference / eDec * 100;
            }
            else if (actual is decimal aDec && IsWithinDecimalRange(expected))
            {
                decimal expectedValue = ConvertToValue<decimal>(expected);
                var difference = expectedValue - aDec;
                return isAbsolute ? difference : difference / expectedValue * 100;
            }

#if !NETFRAMEWORK
            // EXCEPTION: Check half before other floats to preserve precision of reported differences in certain cases.
            if (expected is Half expectedHalf && actual is Half actualHalf)
            {
                var difference = expectedHalf - actualHalf;
                return isAbsolute ? difference : difference / expectedHalf * Half.CreateChecked(100);
            }
#endif

            if (IsFloatingPointNumeric(expected) || IsFloatingPointNumeric(actual))
            {
                var difference = ConvertToValue<double>(expected) - ConvertToValue<double>(actual);
                return isAbsolute ? difference : difference / ConvertToValue<double>(expected) * 100;
            }

            // Converting a negative value to an unsigned type would overflow,
            // so use a signed type that can hold both values.
            if (IsMixedSignInteger(expected, actual))
            {
                // Like C#, promote uint mixed with a signed type of 32 bits or less to long
                if (Is32BitOrSmallerInteger(expected) && Is32BitOrSmallerInteger(actual))
                {
                    long expectedValue = ConvertToValue<long>(expected);
                    var difference = expectedValue - ConvertToValue<long>(actual);
                    return isAbsolute ? difference : difference / (double)expectedValue * 100;
                }

#if NETFRAMEWORK
                decimal expectedDecimal = ConvertToValue<decimal>(expected);
                var decimalDifference = expectedDecimal - ConvertToValue<decimal>(actual);
                return isAbsolute ? decimalDifference : (double)decimalDifference / (double)expectedDecimal * 100;
#else
                if (expected is UInt128 || actual is UInt128)
                    return SignedDifference(ConvertToValue<BigInteger>(expected), ConvertToValue<BigInteger>(actual), isAbsolute);

                return SignedDifference(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), isAbsolute);
#endif
            }

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return Difference(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), isAbsolute);

            if (expected is Int128 || actual is Int128)
                return Difference(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), isAbsolute);
#endif

            if (expected is ulong || actual is ulong)
            {
                var eUlong = ConvertToValue<ulong>(expected);
                var aUlong = ConvertToValue<ulong>(actual);
                var difference = eUlong >= aUlong ? eUlong - aUlong : aUlong - eUlong;
                return isAbsolute ? difference : difference / (double)eUlong * 100;
            }

            if (expected is long || actual is long)
            {
                var difference = ConvertToValue<long>(expected) - ConvertToValue<long>(actual);
                return isAbsolute ? difference : difference / (double)ConvertToValue<long>(expected) * 100;
            }

            if (expected is nuint || actual is nuint)
            {
                nuint expectedValue = ConvertToValue<nuint>(expected);
                nuint actualValue = ConvertToValue<nuint>(actual);

                var difference = expectedValue >= actualValue ? expectedValue - actualValue : actualValue - expectedValue;
                return isAbsolute ? difference : difference / (double)expectedValue * 100;
            }

            if (expected is nint || actual is nint)
            {
                nint expectedValue = ConvertToValue<nint>(expected);
                nint actualValue = ConvertToValue<nint>(actual);

                var difference = expectedValue - actualValue;
                return isAbsolute ? difference : difference / (double)expectedValue * 100;
            }

            if (expected is uint || actual is uint)
            {
                var eUint = ConvertToValue<uint>(expected);
                var aUint = ConvertToValue<uint>(actual);
                var difference = eUint >= aUint ? eUint - aUint : aUint - eUint;
                return isAbsolute ? difference : difference / (double)eUint * 100;
            }

            var intDifference = ConvertToValue<int>(expected) - ConvertToValue<int>(actual);
            return isAbsolute ? intDifference : intDifference / (double)ConvertToValue<int>(expected) * 100;
        }
        #endregion

#if !NETFRAMEWORK
        private static T Difference<T>(T expected, T actual, bool isAbsolute)
            where T : INumber<T>
        {
            var difference = expected - actual;
            var hundred = T.CreateChecked(100);

            return isAbsolute ? difference : difference * hundred / expected;
        }

        // Signed overload used for mixed signed and unsigned values: calculate
        // the percentage as double so an expected value of 0 does not throw.
        private static object SignedDifference<T>(T expected, T actual, bool isAbsolute)
            where T : INumber<T>
        {
            T difference = expected - actual;

            return isAbsolute ? difference : double.CreateChecked(difference) / double.CreateChecked(expected) * 100;
        }

        // Unsigned-only overload: subtract the smaller value from the larger one
        // so the difference can never underflow when the actual value exceeds
        // the expected value (e.g. UInt128).
        private static UInt128 Difference(UInt128 expected, UInt128 actual, bool isAbsolute)
        {
            var difference = expected >= actual ? expected - actual : actual - expected;
            var hundred = UInt128.CreateChecked(100);

            return isAbsolute ? difference : difference * hundred / expected;
        }

        private static bool AreIeeeFloatsEqual<T>(T expected, T actual, ref Tolerance tolerance)
            where T : INumber<T>, IFloatingPointIeee754<T>
        {
            if (T.IsNaN(expected) && T.IsNaN(actual))
                return true;

            // Handle infinity specially since subtracting two infinite values gives
            // NaN and the following test fails. mono also needs NaN to be handled
            // specially although ms.net could use either method. Also, handle
            // situation where no tolerance is used.
            if (T.IsInfinity(expected) || T.IsNaN(expected) || T.IsNaN(actual))
            {
                return expected.Equals(actual);
            }

            Tolerance.ApplyDefaultFloatingPointTolerance(ref tolerance);

            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    return T.Abs(expected - actual) <= ConvertToValue<T>(tolerance.Amount);

                case ToleranceMode.Percent:
                    if (T.IsZero(expected))
                        return expected.Equals(actual);

                    T relativeError = T.Abs((expected - actual) / expected);
                    return (relativeError <= T.CreateChecked(ConvertToValue<double>(tolerance.Amount) / 100.0));

                case ToleranceMode.Ulps:
                    // NOTE: We cannot convert types here because it will affect the ulps calculation.
                    // For example, converting a Half to a double will change the ulps distance.
                    return FloatingPointNumerics.AreAlmostEqualUlps(expected, actual, Convert.ToInt64(tolerance.Amount));

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

        private static bool AreEqual<T>(T expected, T actual, Tolerance tolerance)
            where T : INumber<T>
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return expected.Equals(actual);

                case ToleranceMode.Linear:
                    T toleranceValue = ConvertToValue<T>(tolerance.Amount);
                    if (T.IsZero(toleranceValue))
                        return expected.Equals(actual);

                    T diff = expected >= actual ? expected - actual : actual - expected;
                    return diff <= toleranceValue;

                case ToleranceMode.Percent:
                    if (T.IsZero(expected))
                        return expected.Equals(actual);

                    // Can't do a simple Abs() here in case it's unsigned or could overflow
                    T difference = (expected >= actual ? expected - actual : actual - expected);
                    double relativeError = Math.Abs(double.CreateChecked(difference) / double.CreateChecked(expected));
                    return (relativeError <= ConvertToValue<double>(tolerance.Amount) / 100.0);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }
#endif

        /// <summary>
        /// Converts a numeric value to <typeparamref name="T"/>.
        /// All operand conversions in this class should use this method, as it also handles
        /// types that <see cref="Convert"/> does not, like <see cref="nint"/>, <see cref="nuint"/>
        /// and <see cref="char"/> to floating point types.
        /// </summary>
        internal static T ConvertToValue<T>(object o)
#if NETFRAMEWORK
        {
            if (o is T value)
                return value;

            // nint and nuint are not IConvertible, so convert them from long/ulong.
            if (o is nint nintValue)
                return ConvertFromInt64<T>(nintValue);
            if (o is nuint nuintValue)
                return ConvertFromUInt64<T>(nuintValue);

            if (typeof(T) == typeof(int))
                return (T)(object)Convert.ToInt32(o);
            if (typeof(T) == typeof(uint))
                return (T)(object)Convert.ToUInt32(o);
            if (typeof(T) == typeof(long))
                return (T)(object)Convert.ToInt64(o);
            if (typeof(T) == typeof(ulong))
                return (T)(object)Convert.ToUInt64(o);
            if (typeof(T) == typeof(nint))
                return (T)(object)new IntPtr(Convert.ToInt64(o));
            if (typeof(T) == typeof(nuint))
                return (T)(object)new UIntPtr(Convert.ToUInt64(o));

            // Convert cannot convert char to floating point types, so use an implicit conversion instead.
            if (typeof(T) == typeof(double))
                return (T)(object)(o is char doubleChar ? doubleChar : Convert.ToDouble(o));
            if (typeof(T) == typeof(float))
                return (T)(object)(o is char floatChar ? floatChar : Convert.ToSingle(o));
            if (typeof(T) == typeof(decimal))
                return (T)(object)(o is char decimalChar ? decimalChar : Convert.ToDecimal(o));
            return (T)Convert.ChangeType(o, typeof(T));
        }
#else
            where T : INumber<T>
        {
            // Convert directly from the source type, so values are converted exactly and
            // fractions truncated, instead of going through double or Convert which rounds.
            return o switch
            {
                T value => value,
                int intValue => T.CreateChecked(intValue),
                long longValue => T.CreateChecked(longValue),
                double doubleValue => T.CreateChecked(doubleValue),
                uint uintValue => T.CreateChecked(uintValue),
                ulong ulongValue => T.CreateChecked(ulongValue),
                decimal decimalValue => T.CreateChecked(decimalValue),
                float floatValue => T.CreateChecked(floatValue),
                short shortValue => T.CreateChecked(shortValue),
                ushort ushortValue => T.CreateChecked(ushortValue),
                byte byteValue => T.CreateChecked(byteValue),
                sbyte sbyteValue => T.CreateChecked(sbyteValue),
                char charValue => T.CreateChecked(charValue),
                nint nintValue => T.CreateChecked(nintValue),
                nuint nuintValue => T.CreateChecked(nuintValue),
                Int128 int128Value => T.CreateChecked(int128Value),
                UInt128 uint128Value => T.CreateChecked(uint128Value),
                Half halfValue => T.CreateChecked(halfValue),
                _ => T.CreateChecked(Convert.ToDouble(o)),
            };
        }
#endif

#if NETFRAMEWORK
        // Converts nint values without boxing them as long.
        private static T ConvertFromInt64<T>(long value)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)checked((int)value);
            if (typeof(T) == typeof(uint))
                return (T)(object)checked((uint)value);
            if (typeof(T) == typeof(long))
                return (T)(object)value;
            if (typeof(T) == typeof(ulong))
                return (T)(object)checked((ulong)value);
            if (typeof(T) == typeof(nint))
                return (T)(object)new IntPtr(value);
            if (typeof(T) == typeof(nuint))
                return (T)(object)new UIntPtr(checked((ulong)value));
            if (typeof(T) == typeof(double))
                return (T)(object)(double)value;
            if (typeof(T) == typeof(float))
                return (T)(object)(float)value;
            if (typeof(T) == typeof(decimal))
                return (T)(object)(decimal)value;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        // Converts nuint values without boxing them as ulong.
        private static T ConvertFromUInt64<T>(ulong value)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)checked((int)value);
            if (typeof(T) == typeof(uint))
                return (T)(object)checked((uint)value);
            if (typeof(T) == typeof(long))
                return (T)(object)checked((long)value);
            if (typeof(T) == typeof(ulong))
                return (T)(object)value;
            if (typeof(T) == typeof(nint))
                return (T)(object)new IntPtr(checked((long)value));
            if (typeof(T) == typeof(nuint))
                return (T)(object)new UIntPtr(value);
            if (typeof(T) == typeof(double))
                return (T)(object)(double)value;
            if (typeof(T) == typeof(float))
                return (T)(object)(float)value;
            if (typeof(T) == typeof(decimal))
                return (T)(object)(decimal)value;
            return (T)Convert.ChangeType(value, typeof(T));
        }
#endif
    }
}
