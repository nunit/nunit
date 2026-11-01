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
            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
            // Use ConvertToValue<T> to convert until the only remaining types are IConvertible.
#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return AreEqual(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), tolerance);

            if (expected is Int128 || actual is Int128)
                return AreEqual(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), tolerance);
#endif

            if (expected is double || actual is double)
                return AreIeeeFloatsEqual(ConvertToValue<double>(expected), ConvertToValue<double>(actual), ref tolerance);

            if (expected is float || actual is float)
                return AreIeeeFloatsEqual(ConvertToValue<float>(expected), ConvertToValue<float>(actual), ref tolerance);

#if !NETFRAMEWORK
            if (expected is Half || actual is Half)
                return AreIeeeFloatsEqual(ConvertToValue<Half>(expected), ConvertToValue<Half>(actual), ref tolerance);
#endif

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException("Ulps may only be specified for floating point arguments");

            if (expected is decimal || actual is decimal)
                return AreEqual(Convert.ToDecimal(expected), Convert.ToDecimal(actual), tolerance);

            if (expected is ulong || actual is ulong)
                return AreEqual(Convert.ToUInt64(expected), Convert.ToUInt64(actual), tolerance);

            if (expected is long || actual is long)
                return AreEqual(Convert.ToInt64(expected), Convert.ToInt64(actual), tolerance);

            if (expected is nuint || actual is nuint)
            {
                nuint expectedValue = expected is nuint x ? x : (nuint)Convert.ToUInt64(expected);
                nuint actualValue = actual is nuint y ? y : (nuint)Convert.ToUInt64(actual);

                return AreEqual(expectedValue, actualValue, tolerance);
            }

            if (expected is nint || actual is nint)
            {
                nint expectedValue = expected is nint x ? x : (nint)Convert.ToInt64(expected);
                nint actualValue = actual is nint y ? y : (nint)Convert.ToInt64(actual);

                return AreEqual(expectedValue, actualValue, tolerance);
            }

            if (expected is uint || actual is uint)
                return AreEqual(Convert.ToUInt32(expected), Convert.ToUInt32(actual), tolerance);

            return AreEqual(Convert.ToInt32(expected), Convert.ToInt32(actual), tolerance);
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
                throw new InvalidOperationException("Ulps may only be specified for floating point arguments");

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

            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
            // Use ConvertToValue<T> to convert until the only remaining types are IConvertible.

            if (expected is decimal || actual is decimal)
            {
                // Treat as decimal if one is decimal and other can be treated as decimal
                if (expected is decimal eDec && IsWithinDecimalRange(ConvertToValue<double>(actual)))
                    return eDec.CompareTo(ConvertToValue<decimal>(actual));
                else if (actual is decimal aDec && IsWithinDecimalRange(ConvertToValue<double>(expected)))
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

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return ConvertToValue<UInt128>(expected).CompareTo(ConvertToValue<UInt128>(actual));

            if (expected is Int128 || actual is Int128)
                return ConvertToValue<Int128>(expected).CompareTo(ConvertToValue<Int128>(actual));
#endif

            if (expected is nuint || actual is nuint)
                return ConvertToValue<nuint>(expected).CompareTo(ConvertToValue<nuint>(actual));

            if (expected is nint || actual is nint)
                return ConvertToValue<nint>(expected).CompareTo(ConvertToValue<nint>(actual));

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

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return Difference(ConvertToValue<UInt128>(expected), ConvertToValue<UInt128>(actual), isAbsolute);

            if (expected is Int128 || actual is Int128)
                return Difference(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), isAbsolute);
#endif

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

            if (expected is ulong || actual is ulong)
            {
                var eUlong = Convert.ToUInt64(expected);
                var aUlong = Convert.ToUInt64(actual);
                var difference = eUlong >= aUlong ? eUlong - aUlong : aUlong - eUlong;
                return isAbsolute ? difference : difference / (double)eUlong * 100;
            }

            if (expected is long || actual is long)
            {
                var difference = Convert.ToInt64(expected) - Convert.ToInt64(actual);
                return isAbsolute ? difference : difference / (double)Convert.ToInt64(expected) * 100;
            }

            if (expected is uint || actual is uint)
            {
                var eUint = Convert.ToUInt32(expected);
                var aUint = Convert.ToUInt32(actual);
                var difference = eUint >= aUint ? eUint - aUint : aUint - eUint;
                return isAbsolute ? difference : difference / (double)eUint * 100;
            }

            var intDifference = Convert.ToInt32(expected) - Convert.ToInt32(actual);
            return isAbsolute ? intDifference : intDifference / (double)Convert.ToInt32(expected) * 100;
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

        internal static T ConvertToValue<T>(object o)
#if NETFRAMEWORK
        {
            if (o is T tol)
                return tol;

            // nint and nuint are not IConvertible, so we need to convert them to long/ulong first
            if (o is nint nintValue)
                o = (long)nintValue;
            else if (o is nuint nuintValue)
                o = (ulong)nuintValue;

            if (typeof(T) == typeof(nint))
                return (T)(object)new IntPtr(Convert.ToInt64(o));
            if (typeof(T) == typeof(nuint))
                return (T)(object)new UIntPtr(Convert.ToUInt64(o));
            if (typeof(T) == typeof(double))
                return (T)(object)Convert.ToDouble(o);
            if (typeof(T) == typeof(float))
                return (T)(object)Convert.ToSingle(o);
            return (T)Convert.ChangeType(o, typeof(T));
        }
#else
            where T : INumber<T>
        {
            if (o is T tol)
                return tol;
            if (o is Int128 int128)
                return T.CreateChecked(int128);
            if (o is UInt128 uint128)
                return T.CreateChecked(uint128);
            if (o is Half half)
                return T.CreateChecked(half);
            if (o is nint nintValue)
                return T.CreateChecked(nintValue);
            if (o is nuint nuintValue)
                return T.CreateChecked(nuintValue);
            return T.CreateChecked(Convert.ToDouble(o));
        }
#endif
    }
}
