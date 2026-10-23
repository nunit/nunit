// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NETFRAMEWORK
using System.Numerics;
#endif
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
        /// Checks the type of the object, returning true if
        /// the object is a fixed point numeric type.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>true if the object is a fixed point numeric type</returns>
        internal static bool IsUnsignedFixedPointNumeric(object? obj)
        {
            if (obj is uint or ulong or ushort or byte or nuint)
                return true;

#if !NETFRAMEWORK
            if (obj is UInt128)
                return true;
#endif

            return false;
        }

        /// <summary>
        /// Checks the type of the value, returning true if
        /// the value is a fixed point numeric type.
        /// </summary>
        /// <remarks>
        /// This method is generic and constrained the same as the generic AreEqual to prevent boxing of value types.
        /// </remarks>
        /// <param name="value">The value to check</param>
        /// <returns>true if the value is a fixed point numeric type</returns>
        internal static bool IsUnsignedFixedPointNumeric<T>(T value)
            where T : struct
        {
            if (value is uint or ulong or ushort or byte or nuint)
                return true;

#if !NETFRAMEWORK
            if (value is UInt128)
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
            // nuint doesn't implement IConvertible, convert it to ulong using a cast.
            if (expected is nuint expectedAsNUInt)
                expected = (ulong)expectedAsNUInt;
            if (actual is nuint actualAsNUInt)
                actual = (ulong)actualAsNUInt;

            // nint doesn't implement IConvertible, convert it to long using a cast.
            if (expected is nint expectedAsNInt)
                expected = (long)expectedAsNInt;
            if (actual is nint actualAsNInt)
                actual = (long)actualAsNInt;

            // Step from highest precision to lowest, converting as necessary.
            // Use the most precise type that can hold both values.
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

#if !NETFRAMEWORK
            if (expected is UInt128 expectedAsUInt128)
            {
                return AreEqualWhenOneOperandIsUInt128(expectedAsUInt128, actual, tolerance, uint128IsExpected: true);
            }
            else if (actual is UInt128 actualAsUInt128)
            {
                return AreEqualWhenOneOperandIsUInt128(actualAsUInt128, expected, tolerance, uint128IsExpected: false);
            }

            if (expected is Int128 || actual is Int128)
                return AreEqual(ConvertToValue<Int128>(expected), ConvertToValue<Int128>(actual), tolerance);
#endif

            if (expected is decimal || actual is decimal)
                return AreEqual(ConvertToValue<decimal>(expected), ConvertToValue<decimal>(actual), tolerance);

            if (expected is ulong expectedAsULong)
            {
                return AreEqualWhenOneOperandIsUInt64(expectedAsULong, actual, tolerance, uint64IsExpected: true);
            }
            else if (actual is ulong actualAsULong)
            {
                return AreEqualWhenOneOperandIsUInt64(actualAsULong, expected, tolerance, uint64IsExpected: false);
            }

            if (expected is long || actual is long)
                return AreEqual(Convert.ToInt64(expected), Convert.ToInt64(actual), tolerance);

            if (expected is uint expectedUInt)
            {
                if (actual is uint actualUInt)
                    return AreEqual(expectedUInt, actualUInt, tolerance);
                else // Promote to long for comparison with signed fixed point numeric types
                    return AreEqual(expectedUInt, Convert.ToInt64(actual), tolerance);
            }
            else if (actual is uint actualUInt)
            {
                // Promote to long for comparison with signed fixed point numeric types
                return AreEqual(Convert.ToInt64(expected), actualUInt, tolerance);
            }

            // Promote anything smaller than int to int for comparison
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
            where T : struct, IConvertible, IComparable<T>
        {
            // We know that both arguments are the same type, so we don't need or and conversions.
            if (expected is double expectedDouble && actual is double actualDouble)
                return AreIeeeFloatsEqual(expectedDouble, actualDouble, ref tolerance);

            if (expected is float expectedFloat && actual is float actualFloat)
                return AreIeeeFloatsEqual(expectedFloat, actualFloat, ref tolerance);

            if (tolerance.Mode == ToleranceMode.Ulps)
                throw new InvalidOperationException("Ulps may only be specified for floating point arguments");

            if (expected is decimal expectedDecimal && actual is decimal actualDecimal)
                return AreEqual(expectedDecimal, actualDecimal, tolerance);

            if (expected is ulong expectedULong && actual is ulong actualULong)
                return AreEqual(expectedULong, actualULong, tolerance);

            if (expected is long expectedLong && actual is long actualLong)
                return AreEqual(expectedLong, actualLong, tolerance);

            if (expected is uint expectedUInt && actual is uint actualUInt)
            {
                return AreEqual(expectedUInt, actualUInt, tolerance);
            }

            // Promote anything smaller than int to int for comparison
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

#if !NETFRAMEWORK
            if (expected is UInt128 || actual is UInt128)
                return ConvertToValue<UInt128>(expected).CompareTo(ConvertToValue<UInt128>(actual));

            if (expected is Int128 || actual is Int128)
                return ConvertToValue<Int128>(expected).CompareTo(ConvertToValue<Int128>(actual));
#endif

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

                var difference = expectedValue - actualValue;
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

#if !NETFRAMEWORK
        private static T Difference<T>(T expected, T actual, bool isAbsolute)
            where T : INumber<T>
        {
            var difference = expected - actual;
            var hundred = T.CreateChecked(100);

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

        private static T ConvertToValue<T>(object o)
#if NETFRAMEWORK
        {
            if (o is T tol)
                return tol;
            if (typeof(T) == typeof(long))
                return (T)(object)Convert.ToInt64(o);
            if (typeof(T) == typeof(ulong))
                return (T)(object)Convert.ToUInt64(o);
            if (typeof(T) == typeof(nint))
                return (T)(object)Convert.ToInt64(o);
            if (typeof(T) == typeof(nuint))
                return (T)(object)Convert.ToUInt64(o);
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
            if (o is long int64Value)
                return T.CreateChecked(int64Value);
            if (o is ulong uint64Value)
                return T.CreateChecked(uint64Value);
            if (o is Half half)
                return T.CreateChecked(half);
            if (o is nint nintValue)
                return T.CreateChecked(nintValue);
            if (o is nuint nuintValue)
                return T.CreateChecked(nuintValue);
            return T.CreateChecked(Convert.ToDouble(o));
        }
#endif

#if !NETFRAMEWORK
        private static bool AreEqualWhenOneOperandIsUInt128(UInt128 uint128Value, object other, Tolerance tolerance, bool uint128IsExpected)
        {
            if (IsUnsignedFixedPointNumeric(other))
            {
                // 'other' is an unsigned fixed point numeric type, so we can safely convert it to UInt128 for comparison
                UInt128 otherUInt128 = ConvertToValue<UInt128>(other);
                return uint128IsExpected
                    ? AreEqual(uint128Value, otherUInt128, tolerance)
                    : AreEqual(otherUInt128, uint128Value, tolerance);
            }

            // 'other' is a signed fixed point numeric type, so we can safely convert it to Int128 for comparison
            Int128 otherInt128 = ConvertToValue<Int128>(other);

            if (otherInt128 >= 0)
            {
                // 'otherInt128' can be safely converted to UInt128 for comparison
                UInt128 otherAsUInt128 = (UInt128)otherInt128;
                return uint128IsExpected
                    ? AreEqual(uint128Value, otherAsUInt128, tolerance)
                    : AreEqual(otherAsUInt128, uint128Value, tolerance);
            }

            if (uint128Value < (UInt128)Int128.MaxValue)
            {
                // 'uint128Value' can be safely converted to Int128 for comparison
                Int128 uint128AsInt128 = (Int128)uint128Value;
                return uint128IsExpected
                    ? AreEqual(uint128AsInt128, otherInt128, tolerance)
                    : AreEqual(otherInt128, uint128AsInt128, tolerance);
            }

            // 'uint128Value' is too large to be converted to Int128, and 'otherInt128' is negative, so they cannot be equal
            return false;
        }
#endif

        private static bool AreEqualWhenOneOperandIsUInt64(ulong uint64Value, object other, Tolerance tolerance, bool uint64IsExpected)
        {
            if (IsUnsignedFixedPointNumeric(other))
            {
                // 'other' is an unsigned fixed point numeric type, so we can safely convert it to ulong for comparison
                ulong otherUInt64 = ConvertToValue<ulong>(other);
                return uint64IsExpected
                    ? AreEqual(uint64Value, otherUInt64, tolerance)
                    : AreEqual(otherUInt64, uint64Value, tolerance);
            }

            // 'other' is a signed fixed point numeric type, so we can safely convert it to long for comparison
            long otherInt64 = ConvertToValue<long>(other);

            if (otherInt64 >= 0)
            {
                // 'otherInt64' can be safely converted to ulong for comparison
                ulong otherAsUInt64 = (ulong)otherInt64;
                return uint64IsExpected
                    ? AreEqual(uint64Value, otherAsUInt64, tolerance)
                    : AreEqual(otherAsUInt64, uint64Value, tolerance);
            }

            if (uint64Value < (ulong)long.MaxValue)
            {
                // 'uint64Value' can be safely converted to long for comparison
                long uint64AsInt64 = (long)uint64Value;
                return uint64IsExpected
                    ? AreEqual(uint64AsInt64, otherInt64, tolerance)
                    : AreEqual(otherInt64, uint64AsInt64, tolerance);
            }

            // 'uint64Value' is too large to be converted to long, and 'otherInt64' is negative, so they cannot be equal
            return false;
        }
    }
}
