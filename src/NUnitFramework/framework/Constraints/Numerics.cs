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

        /// <summary>
        /// The type in which two numeric values are compared.
        /// </summary>
        private enum CommonNumericType
        {
            Int32,
            UInt32,
            Int64,
            UInt64,
            NInt,
            NUInt,
            Decimal,
            Single,
            Double,
#if !NETFRAMEWORK
            Half,
            Int128,
            UInt128,
            BigInteger,
#endif
        }

        /// <summary>
        /// Determines the most precise type that can hold both numeric values.
        /// All operations on two numeric values should use this, so they convert them consistently.
        /// </summary>
        private static CommonNumericType GetCommonNumericType(object expected, object actual)
        {
            if (expected is decimal || actual is decimal)
            {
                // Treat as decimal if both values fit, otherwise fall back to double
                return IsWithinDecimalRange(expected) && IsWithinDecimalRange(actual)
                    ? CommonNumericType.Decimal
                    : CommonNumericType.Double;
            }

            if (expected is double || actual is double)
                return CommonNumericType.Double;

            if (expected is float || actual is float)
                return CommonNumericType.Single;

#if !NETFRAMEWORK
            if (expected is Half || actual is Half)
                return CommonNumericType.Half;
#endif

            // Converting a negative value to an unsigned type would overflow,
            // so use a signed type that can hold both values.
            if (IsMixedSignInteger(expected, actual))
            {
                // Like C#, promote uint mixed with a signed type of 32 bits or less to long
                if (Is32BitOrSmallerInteger(expected) && Is32BitOrSmallerInteger(actual))
                    return CommonNumericType.Int64;

#if NETFRAMEWORK
                return CommonNumericType.Decimal;
#else
                return expected is UInt128 || actual is UInt128
                    ? CommonNumericType.BigInteger
                    : CommonNumericType.Int128;
#endif
            }

            // Check the widest types first, as nuint and nint can be smaller than ulong and long.
#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return CommonNumericType.UInt128;

            if (expected is Int128 || actual is Int128)
                return CommonNumericType.Int128;
#endif

            if (expected is ulong || actual is ulong)
                return CommonNumericType.UInt64;

            if (expected is long || actual is long)
                return CommonNumericType.Int64;

            if (expected is nuint || actual is nuint)
                return CommonNumericType.NUInt;

            if (expected is nint || actual is nint)
                return CommonNumericType.NInt;

            if (expected is uint || actual is uint)
                return CommonNumericType.UInt32;

            return CommonNumericType.Int32;
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
            CommonNumericType commonType = GetCommonNumericType(expected, actual);

            switch (commonType)
            {
                case CommonNumericType.Double:
                    return AreIeeeFloatsEqual(ConvertToValue<double>(expected), ConvertToValue<double>(actual), ref tolerance);
                case CommonNumericType.Single:
                    return AreIeeeFloatsEqual(ConvertToValue<float>(expected), ConvertToValue<float>(actual), ref tolerance);
#if !NETFRAMEWORK
                case CommonNumericType.Half:
                    return AreIeeeFloatsEqual(ConvertToValue<Half>(expected), ConvertToValue<Half>(actual), ref tolerance);
#endif
            }

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException(UlpsRequiresFloatingPoint);

            return commonType switch
            {
                CommonNumericType.Decimal => AreEqual(ConvertToValue<decimal>(expected), ConvertToValue<decimal>(actual), tolerance),
#if !NETFRAMEWORK
                CommonNumericType.BigInteger => AreEqual(ConvertToValue<BigInteger>(expected), ConvertToValue<BigInteger>(actual), tolerance),
                CommonNumericType.UInt128 => AreEqual(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), tolerance),
                CommonNumericType.Int128 => AreEqual(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), tolerance),
#endif
                CommonNumericType.UInt64 => AreEqual(ConvertToValue<ulong>(expected), ConvertToValue<ulong>(actual), tolerance),
                CommonNumericType.Int64 => AreEqual(ConvertToValue<long>(expected), ConvertToValue<long>(actual), tolerance),
                CommonNumericType.NUInt => AreEqual(ConvertToValue<nuint>(expected), ConvertToValue<nuint>(actual), tolerance),
                CommonNumericType.NInt => AreEqual(ConvertToValue<nint>(expected), ConvertToValue<nint>(actual), tolerance),
                CommonNumericType.UInt32 => AreEqual(ConvertToValue<uint>(expected), ConvertToValue<uint>(actual), tolerance),
                CommonNumericType.Int32 => AreEqual(ConvertToValue<int>(expected), ConvertToValue<int>(actual), tolerance),
                _ => throw new InvalidOperationException($"Unexpected numeric type {commonType}"),
            };
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
#endif

        // Integer comparisons: the difference between two integers is calculated without overflow in the
        // unsigned type of the same size and compared with the largest difference the tolerance allows.
        // For a percentage tolerance that is floor(|expected| * percentage / 100), as the difference is a whole number.
        private static bool AreEqual(int expected, int actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(AbsoluteDifference(expected, 0), tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(uint expected, uint actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(expected, tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(long expected, long actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(AbsoluteDifference(expected, 0), tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(ulong expected, ulong actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(expected, tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(nint expected, nint actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(AbsoluteDifference(expected, 0), tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(nuint expected, nuint actual, Tolerance tolerance)
        {
            ulong maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(expected, tolerance.Amount)
                : MaxLinearDifference(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

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

#if !NETFRAMEWORK
        private static bool AreEqual(Int128 expected, Int128 actual, Tolerance tolerance)
        {
            UInt128 maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(AbsoluteDifference(expected, Int128.Zero), tolerance.Amount)
                : MaxLinearDifference128(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(UInt128 expected, UInt128 actual, Tolerance tolerance)
        {
            UInt128 maxDifference = tolerance.Mode == ToleranceMode.Percent
                ? PercentageOf(expected, tolerance.Amount)
                : MaxLinearDifference128(tolerance);
            return AbsoluteDifference(expected, actual) <= maxDifference;
        }

        private static bool AreEqual(BigInteger expected, BigInteger actual, Tolerance tolerance)
        {
            BigInteger difference = AbsoluteDifference(expected, actual);
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return difference.IsZero;

                case ToleranceMode.Linear:
                    return IsPositiveInfinity(tolerance.Amount) || difference <= ConvertToValue<BigInteger>(tolerance.Amount);

                case ToleranceMode.Percent:
                    return difference <= PercentageOf(BigInteger.Abs(expected), PercentageAsDecimal(tolerance.Amount));

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }
#endif

        // The difference between two integers fits in the unsigned type of the same size.
        // Subtracting the smaller from the larger value using unsigned arithmetic gives the exact result,
        // even when the signed subtraction would overflow, e.g. int.MaxValue - int.MinValue.
        private static uint AbsoluteDifference(int x, int y)
            => x >= y ? unchecked((uint)x - (uint)y) : unchecked((uint)y - (uint)x);

        private static uint AbsoluteDifference(uint x, uint y)
            => x >= y ? x - y : y - x;

        private static ulong AbsoluteDifference(long x, long y)
            => x >= y ? unchecked((ulong)x - (ulong)y) : unchecked((ulong)y - (ulong)x);

        private static ulong AbsoluteDifference(ulong x, ulong y)
            => x >= y ? x - y : y - x;

        private static nuint AbsoluteDifference(nint x, nint y)
            => x >= y ? unchecked((nuint)x - (nuint)y) : unchecked((nuint)y - (nuint)x);

        private static nuint AbsoluteDifference(nuint x, nuint y)
            => x >= y ? x - y : y - x;

#if !NETFRAMEWORK
        private static UInt128 AbsoluteDifference(Int128 x, Int128 y)
            => x >= y ? unchecked((UInt128)x - (UInt128)y) : unchecked((UInt128)y - (UInt128)x);

        private static UInt128 AbsoluteDifference(UInt128 x, UInt128 y)
            => x >= y ? x - y : y - x;

        private static BigInteger AbsoluteDifference(BigInteger x, BigInteger y)
            => BigInteger.Abs(x - y);
#endif

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

        // 2^64 as decimal, the first value that no longer fits in a ulong.
        private const decimal UInt64Range = 18446744073709551616m;

        /// <summary>
        /// The largest difference between two integers allowed by a tolerance that is not a percentage.
        /// A fractional amount is rounded down, which is exact as the difference is a whole number:
        /// it is within 1.5 exactly when it is within 1. An amount too large for the type allows any difference.
        /// </summary>
        private static ulong MaxLinearDifference(Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return 0;

                case ToleranceMode.Linear:
                    if (tolerance.Amount is decimal decimalValue)
                        return decimalValue >= UInt64Range ? ulong.MaxValue : (ulong)decimalValue;
                    if (IsBinaryFloatingPoint(tolerance.Amount))
                    {
                        double doubleValue = ConvertToValue<double>(tolerance.Amount);
                        return doubleValue >= (double)UInt64Range ? ulong.MaxValue : (ulong)doubleValue;
                    }
#if !NETFRAMEWORK
                    if (tolerance.Amount is Int128 or UInt128)
                    {
                        UInt128 wideValue = ConvertToValue<UInt128>(tolerance.Amount);
                        return wideValue > ulong.MaxValue ? ulong.MaxValue : (ulong)wideValue;
                    }
#endif
                    return ConvertToValue<ulong>(tolerance.Amount);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }

#if !NETFRAMEWORK
        private static UInt128 MaxLinearDifference128(Tolerance tolerance)
        {
            switch (tolerance.Mode)
            {
                case ToleranceMode.Unset:
                    return UInt128.Zero;

                case ToleranceMode.Linear:
                    return IsBinaryFloatingPoint(tolerance.Amount)
                        ? UInt128.CreateSaturating(ConvertToValue<double>(tolerance.Amount))
                        : ConvertToValue<UInt128>(tolerance.Amount);

                default:
                    throw new ArgumentException("Unknown tolerance mode specified", "mode");
            }
        }
#endif

        /// <summary>
        /// The largest difference between two integers allowed by a percentage of the expected value:
        /// floor(|expected| * percentage / 100), as the difference is a whole number.
        /// </summary>
        private static ulong PercentageOf(ulong magnitude, object percentage)
        {
            decimal percent = PercentageAsDecimal(percentage);
            if (magnitude == 0)
                return 0;

            // The result does not fit in a ulong if the product does not fit in a decimal.
            if (percent > decimal.MaxValue / magnitude)
                return ulong.MaxValue;

            decimal maxDifference = decimal.Floor(magnitude * percent / 100m);
            return maxDifference >= UInt64Range ? ulong.MaxValue : (ulong)maxDifference;
        }

#if !NETFRAMEWORK
        private static UInt128 PercentageOf(UInt128 magnitude, object percentage)
        {
            BigInteger maxDifference = PercentageOf((BigInteger)magnitude, PercentageAsDecimal(percentage));
            return maxDifference >= UInt128.MaxValue ? UInt128.MaxValue : (UInt128)maxDifference;
        }

        // Calculates exactly using the decimal's mantissa and scale: magnitude * mantissa / (100 * 10^scale).
        private static BigInteger PercentageOf(BigInteger magnitude, decimal percent)
        {
            int[] bits = decimal.GetBits(percent);
            BigInteger mantissa = ((BigInteger)(uint)bits[2] << 64) | ((BigInteger)(uint)bits[1] << 32) | (uint)bits[0];
            int scale = (bits[3] >> 16) & 0xFF;

            return magnitude * mantissa / (100 * BigInteger.Pow(10, scale));
        }
#endif

        // A percentage that does not fit in a decimal is so large that any difference is allowed.
        // The amount is never negative or NaN, as Tolerance does not allow that.
        private static decimal PercentageAsDecimal(object percentage)
        {
            if (percentage is decimal decimalValue)
                return decimalValue;

            return ConvertToValue<double>(percentage) < (double)decimal.MaxValue
                ? ConvertToValue<decimal>(percentage)
                : decimal.MaxValue;
        }

        private static bool IsBinaryFloatingPoint(object value)
#if NETFRAMEWORK
            => value is double or float;
#else
            => value is double or float or Half;
#endif

        private static bool IsPositiveInfinity(object amount)
            => IsBinaryFloatingPoint(amount) && double.IsPositiveInfinity(ConvertToValue<double>(amount));
        #endregion

        #region Tolerance Ranges

        /// <summary>
        /// Calculates the range value - amount to value + amount, used by the comparison constraints.
        /// The type of the value determines the type of the bounds. Bounds that do not fit in that type
        /// use a wider type, so custom comparers see the type of the value whenever possible.
        /// </summary>
        internal static Tolerance.Range LinearRange(object value, object amount)
        {
            switch (value)
            {
                case double doubleValue:
                    double doubleOffset = ConvertToValue<double>(amount);
                    return new Tolerance.Range(doubleValue - doubleOffset, doubleValue + doubleOffset);

                case float floatValue:
                    float floatOffset = ConvertToValue<float>(amount);
                    return new Tolerance.Range(floatValue - floatOffset, floatValue + floatOffset);

#if !NETFRAMEWORK
                case Half halfValue:
                    return HalfRange(halfValue, ConvertToValue<double>(amount));
#endif

                case decimal decimalValue:
                    return DecimalRange(decimalValue, amount);
            }

            if (IsPositiveInfinity(amount))
                return new Tolerance.Range(double.NegativeInfinity, double.PositiveInfinity);

            // A range of integers cannot be fractional: there is no bound that is correct for both > and >=,
            // e.g. a > 3.5 is a > 3, but a >= 3.5 is a >= 4.
            if (!IsWholeNumber(amount))
                throw new InvalidOperationException($"Tolerance {amount} must be a whole number for the integer value {value}");

            if (TryGetDecimal(value, out decimal decimalValueOfInteger) && TryGetDecimal(amount, out decimal offset) &&
                offset <= decimal.MaxValue - Math.Abs(decimalValueOfInteger))
            {
                return new Tolerance.Range(
                    NarrowToTypeOf(value, decimalValueOfInteger - offset),
                    NarrowToTypeOf(value, decimalValueOfInteger + offset));
            }

#if !NETFRAMEWORK
            BigInteger bigValue = ConvertToValue<BigInteger>(value);
            BigInteger bigOffset = ConvertToValue<BigInteger>(amount);
            return new Tolerance.Range(
                NarrowToTypeOf(value, bigValue - bigOffset, isLowerBound: true),
                NarrowToTypeOf(value, bigValue + bigOffset, isLowerBound: false));
#else
            // The amount does not fit in a decimal, so the bounds are far outside the range of any integer type.
            double approximateValue = ConvertToValue<double>(value);
            double approximateOffset = ConvertToValue<double>(amount);
            return new Tolerance.Range(approximateValue - approximateOffset, approximateValue + approximateOffset);
#endif
        }

        /// <summary>
        /// Calculates the range value - percentage of value to value + percentage of value,
        /// used by the comparison constraints. The type of the value determines the type of the bounds.
        /// As the offset of an integer value is usually fractional, it uses exact decimal bounds unless
        /// a bound is a whole number that fits in the type of the value.
        /// </summary>
        internal static Tolerance.Range PercentRange(object value, object percentage)
        {
            if (IsBinaryFloatingPoint(value))
            {
                double offset = Math.Abs(ConvertToValue<double>(value) * ConvertToValue<double>(percentage) / 100);

                switch (value)
                {
                    case double doubleValue:
                        return new Tolerance.Range(doubleValue - offset, doubleValue + offset);
                    case float floatValue:
                        return new Tolerance.Range(floatValue - (float)offset, floatValue + (float)offset);
#if !NETFRAMEWORK
                    case Half halfValue:
                        return HalfRange(halfValue, offset);
#endif
                }
            }

            const decimal limit = decimal.MaxValue / 2;
            if (TryGetDecimal(value, out decimal decimalValue) && TryGetDecimal(percentage, out decimal percent) &&
                Math.Abs(decimalValue) <= limit && (decimalValue == 0 || percent <= limit / Math.Abs(decimalValue)))
            {
                decimal decimalOffset = Math.Abs(decimalValue) * percent / 100;
                return new Tolerance.Range(
                    NarrowToTypeOf(value, decimalValue - decimalOffset),
                    NarrowToTypeOf(value, decimalValue + decimalOffset));
            }

            // Values or percentages outside the decimal range.
            double approximateValue = ConvertToValue<double>(value);
            double approximateOffset = Math.Abs(approximateValue * ConvertToValue<double>(percentage) / 100);
            return new Tolerance.Range(approximateValue - approximateOffset, approximateValue + approximateOffset);
        }

        private static Tolerance.Range DecimalRange(decimal value, object amount)
        {
            if (TryGetDecimal(amount, out decimal offset) && offset <= decimal.MaxValue - Math.Abs(value))
                return new Tolerance.Range(value - offset, value + offset);

            // The bounds are outside the decimal range.
            double approximateValue = (double)value;
            double approximateOffset = ConvertToValue<double>(amount);
            return new Tolerance.Range(approximateValue - approximateOffset, approximateValue + approximateOffset);
        }

#if !NETFRAMEWORK
        private static Tolerance.Range HalfRange(Half value, double offset)
        {
            Half halfOffset = (Half)offset;
            Half lowerBound = value - halfOffset;
            Half upperBound = value + halfOffset;

            // An offset too small for the precision of Half does not change the value, so use decimal instead.
            if (!double.IsInfinity(offset) && (lowerBound >= value || upperBound <= value))
                return new Tolerance.Range((decimal)value - (decimal)offset, (decimal)value + (decimal)offset);

            return new Tolerance.Range(lowerBound, upperBound);
        }
#endif

        private static bool IsWholeNumber(object amount)
        {
            if (amount is decimal decimalValue)
                return decimalValue == decimal.Truncate(decimalValue);

            if (IsBinaryFloatingPoint(amount))
            {
                double doubleValue = ConvertToValue<double>(amount);
                return Math.Floor(doubleValue) == doubleValue;
            }

            return true;
        }

        private static bool TryGetDecimal(object value, out decimal decimalValue)
        {
            bool fits = IsWithinDecimalRange(value);
            decimalValue = fits ? ConvertToValue<decimal>(value) : 0m;
            return fits;
        }

        /// <summary>
        /// Converts a bound to the type in which the integer value is compared if it is a whole number that fits,
        /// otherwise keeps it as decimal.
        /// </summary>
        private static object NarrowToTypeOf(object value, decimal bound)
        {
            if (bound != decimal.Truncate(bound))
                return bound;

            switch (GetCommonNumericType(value, value))
            {
                case CommonNumericType.Int32 when bound >= int.MinValue && bound <= int.MaxValue:
                    return (int)bound;
                case CommonNumericType.UInt32 when bound >= uint.MinValue && bound <= uint.MaxValue:
                    return (uint)bound;
                case CommonNumericType.Int64 when bound >= long.MinValue && bound <= long.MaxValue:
                    return (long)bound;
                case CommonNumericType.UInt64 when bound >= ulong.MinValue && bound <= ulong.MaxValue:
                    return (ulong)bound;
                case CommonNumericType.NInt when IntPtr.Size == 8 ? bound >= long.MinValue && bound <= long.MaxValue : bound >= int.MinValue && bound <= int.MaxValue:
                    return (nint)(long)bound;
                case CommonNumericType.NUInt when IntPtr.Size == 8 ? bound >= ulong.MinValue && bound <= ulong.MaxValue : bound >= uint.MinValue && bound <= uint.MaxValue:
                    return (nuint)(ulong)bound;
#if !NETFRAMEWORK
                case CommonNumericType.Int128:
                    return (Int128)bound;
                case CommonNumericType.UInt128 when bound >= 0m:
                    return (UInt128)bound;
#endif
                default:
                    return bound;
            }
        }

#if !NETFRAMEWORK
        private static readonly BigInteger DecimalMinValue = new(decimal.MinValue);
        private static readonly BigInteger DecimalMaxValue = new(decimal.MaxValue);

        /// <summary>
        /// Converts a bound outside the decimal range to Int128 or UInt128 when it fits,
        /// otherwise to double, rounded away from the range so it does not exclude any value in it.
        /// </summary>
        private static object NarrowToTypeOf(object value, BigInteger bound, bool isLowerBound)
        {
            if (bound >= DecimalMinValue && bound <= DecimalMaxValue)
                return NarrowToTypeOf(value, (decimal)bound);

            if (bound >= Int128.MinValue && bound <= Int128.MaxValue && !(value is UInt128 && bound >= 0))
                return (Int128)bound;

            if (bound >= 0 && bound <= UInt128.MaxValue)
                return (UInt128)bound;

            double approximation = (double)bound;
            if (isLowerBound && (BigInteger)approximation > bound)
                approximation = Math.BitDecrement(approximation);
            else if (!isLowerBound && (BigInteger)approximation < bound)
                approximation = Math.BitIncrement(approximation);

            return approximation;
        }
#endif
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

            CommonNumericType commonType = GetCommonNumericType(expected, actual);

            return commonType switch
            {
                CommonNumericType.Decimal => ConvertToValue<decimal>(expected).CompareTo(ConvertToValue<decimal>(actual)),
                CommonNumericType.Double => ConvertToValue<double>(expected).CompareTo(ConvertToValue<double>(actual)),
                CommonNumericType.Single => ConvertToValue<float>(expected).CompareTo(ConvertToValue<float>(actual)),
#if !NETFRAMEWORK
                CommonNumericType.Half => ConvertToValue<Half>(expected).CompareTo(ConvertToValue<Half>(actual)),
                CommonNumericType.BigInteger => ConvertToValue<BigInteger>(expected).CompareTo(ConvertToValue<BigInteger>(actual)),
                CommonNumericType.UInt128 => ConvertToValue<UInt128>(expected).CompareTo(ConvertToValue<UInt128>(actual)),
                CommonNumericType.Int128 => ConvertToValue<Int128>(expected).CompareTo(ConvertToValue<Int128>(actual)),
#endif
                CommonNumericType.UInt64 => ConvertToValue<ulong>(expected).CompareTo(ConvertToValue<ulong>(actual)),
                CommonNumericType.Int64 => ConvertToValue<long>(expected).CompareTo(ConvertToValue<long>(actual)),
                CommonNumericType.NUInt => ConvertToValue<nuint>(expected).CompareTo(ConvertToValue<nuint>(actual)),
                CommonNumericType.NInt => ConvertToValue<nint>(expected).CompareTo(ConvertToValue<nint>(actual)),
                CommonNumericType.UInt32 => ConvertToValue<uint>(expected).CompareTo(ConvertToValue<uint>(actual)),
                CommonNumericType.Int32 => ConvertToValue<int>(expected).CompareTo(ConvertToValue<int>(actual)),
                _ => throw new InvalidOperationException($"Unexpected numeric type {commonType}"),
            };
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

            CommonNumericType commonType = GetCommonNumericType(expected, actual);

            return commonType switch
            {
                CommonNumericType.Decimal => Difference(ConvertToValue<decimal>(expected), ConvertToValue<decimal>(actual), isAbsolute),
#if !NETFRAMEWORK
                // Keep the difference between two Half values as Half, as converting them to double shows their imprecision.
                CommonNumericType.Half when isAbsolute && expected is Half expectedHalf && actual is Half actualHalf => Half.Abs(expectedHalf - actualHalf),
                CommonNumericType.Double or CommonNumericType.Single or CommonNumericType.Half => Difference(ConvertToValue<double>(expected), ConvertToValue<double>(actual), isAbsolute),
                CommonNumericType.BigInteger => Difference(ConvertToValue<BigInteger>(expected), ConvertToValue<BigInteger>(actual), isAbsolute),
                CommonNumericType.UInt128 => Difference(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), isAbsolute),
                CommonNumericType.Int128 => Difference(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), isAbsolute),
#else
                CommonNumericType.Double or CommonNumericType.Single => Difference(ConvertToValue<double>(expected), ConvertToValue<double>(actual), isAbsolute),
#endif
                CommonNumericType.UInt64 => Difference(ConvertToValue<ulong>(expected), ConvertToValue<ulong>(actual), isAbsolute),
                CommonNumericType.Int64 => Difference(ConvertToValue<long>(expected), ConvertToValue<long>(actual), isAbsolute),
                CommonNumericType.NUInt => Difference(ConvertToValue<nuint>(expected), ConvertToValue<nuint>(actual), isAbsolute),
                CommonNumericType.NInt => Difference(ConvertToValue<nint>(expected), ConvertToValue<nint>(actual), isAbsolute),
                CommonNumericType.UInt32 => Difference(ConvertToValue<uint>(expected), ConvertToValue<uint>(actual), isAbsolute),
                CommonNumericType.Int32 => Difference(ConvertToValue<int>(expected), ConvertToValue<int>(actual), isAbsolute),
                _ => throw new InvalidOperationException($"Unexpected numeric type {commonType}"),
            };
        }

        // The absolute difference is always positive. For integers it is a value of the unsigned type
        // of the same size, which cannot overflow. The percentage of the expected value is a double.
        // The absolute difference is cast to object, as otherwise the conditional would convert it to double.
        private static object Difference(int expected, int actual, bool isAbsolute)
        {
            uint difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(uint expected, uint actual, bool isAbsolute)
        {
            uint difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(long expected, long actual, bool isAbsolute)
        {
            ulong difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(ulong expected, ulong actual, bool isAbsolute)
        {
            ulong difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(nint expected, nint actual, bool isAbsolute)
        {
            nuint difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(nuint expected, nuint actual, bool isAbsolute)
        {
            nuint difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference(difference, expected);
        }

        private static object Difference(decimal expected, decimal actual, bool isAbsolute)
        {
            if (TryAbsoluteDifference(expected, actual, out decimal difference))
                return isAbsolute ? (object)difference : RelativeDifference((double)difference, (double)expected);

            // The difference is outside the decimal range.
            double approximateDifference = Math.Abs((double)expected - (double)actual);
            return isAbsolute ? approximateDifference : RelativeDifference(approximateDifference, (double)expected);
        }

        private static object Difference(double expected, double actual, bool isAbsolute)
        {
            double difference = Math.Abs(expected - actual);
            return isAbsolute ? difference : RelativeDifference(difference, expected);
        }

#if !NETFRAMEWORK
        private static object Difference(Int128 expected, Int128 actual, bool isAbsolute)
        {
            UInt128 difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference((double)difference, (double)expected);
        }

        private static object Difference(UInt128 expected, UInt128 actual, bool isAbsolute)
        {
            UInt128 difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference((double)difference, (double)expected);
        }

        private static object Difference(BigInteger expected, BigInteger actual, bool isAbsolute)
        {
            BigInteger difference = AbsoluteDifference(expected, actual);
            return isAbsolute ? (object)difference : RelativeDifference((double)difference, (double)expected);
        }
#endif

        // The difference as a percentage of the expected value. Multiplying first keeps
        // round percentages exact, e.g. 1504 * 100 / 10000 is 15.04, but 1504 / 10000 * 100 is not.
        private static double RelativeDifference(double difference, double expected)
            => difference * 100 / Math.Abs(expected);
        #endregion

#if !NETFRAMEWORK
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
