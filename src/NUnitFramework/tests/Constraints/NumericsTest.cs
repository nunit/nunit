// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NETFRAMEWORK
using System.Numerics;
#endif
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NumericsTests
    {
        private Tolerance _tenPercent, _zeroTolerance, _absoluteTolerance;

        [OneTimeSetUp]
        public void SetUp()
        {
            _absoluteTolerance = new Tolerance(0.1);
            _tenPercent = new Tolerance(10.0).Percent;
            _zeroTolerance = Tolerance.Exact;
        }

        [TestCase(123456789)]
        [TestCase(123456789U)]
        [TestCase(123456789L)]
        [TestCase(123456789UL)]
        [TestCase(1234.5678f)]
        [TestCase(1234.5678)]
        [TestCaseSource(nameof(FloatingPointEqualsTestCases))]
        [TestCaseSource(nameof(FixedPointEqualsTestCases))]
        public void CanMatchWithoutToleranceMode(object value)
        {
            Assert.That(Numerics.AreEqual(value, value, ref _zeroTolerance), Is.True);
        }

#if !NETFRAMEWORK
        [Test]
        public void CanMatchMixedFloatingPointNumerics()
        {
            var a = 0.5d;
            var b = (Half)0.5d;

            Assert.That(Numerics.AreEqual(a, b, ref _zeroTolerance), Is.True);
        }

        [TestCaseSource(nameof(FractionalAndWideIntegerTestCases))]
        public void FractionalValueDoesNotEqualTruncatedWideInteger(object fractional, object integer)
        {
            Assert.That(Numerics.AreEqual(fractional, integer, ref _zeroTolerance), Is.False);
            Assert.That(Numerics.AreEqual(integer, fractional, ref _zeroTolerance), Is.False);
        }

        [TestCaseSource(nameof(FractionalAndWideIntegerTestCases))]
        public void FractionalValueComparesWithWideInteger(object fractional, object integer)
        {
            Assert.That(Numerics.Compare(fractional, integer), Is.GreaterThan(0));
            Assert.That(Numerics.Compare(integer, fractional), Is.LessThan(0));
        }

        private static TestCaseData[] FractionalAndWideIntegerTestCases()
        {
            return [
                new TestCaseData(1.5d, (Int128)1),
                new TestCaseData(1.5d, (UInt128)1),
                new TestCaseData(1.5f, (Int128)1),
                new TestCaseData(1.5f, (UInt128)1),
                new TestCaseData((Half)1.5, (Int128)1),
                new TestCaseData((Half)1.5, (UInt128)1),
                new TestCaseData(1.5m, (Int128)1),
                new TestCaseData(1.5m, (UInt128)1),
            ];
        }

        [TestCaseSource(nameof(WideIntegerAndExactValueTestCases))]
        public void WideIntegerEqualsSameValueWithoutPrecisionLoss(object wide, object other)
        {
            Assert.That(Numerics.AreEqual(wide, other, ref _zeroTolerance), Is.True);
            Assert.That(Numerics.AreEqual(other, wide, ref _zeroTolerance), Is.True);
        }

        [TestCaseSource(nameof(WideIntegerAndExactValueTestCases))]
        public void WideIntegerComparesSameValueWithoutPrecisionLoss(object wide, object other)
        {
            Assert.That(Numerics.Compare(wide, other), Is.Zero);
            Assert.That(Numerics.Compare(other, wide), Is.Zero);
        }

        private static TestCaseData[] WideIntegerAndExactValueTestCases()
        {
            return [
                new TestCaseData((Int128)long.MaxValue, long.MaxValue),
                new TestCaseData((UInt128)long.MaxValue, long.MaxValue),
                new TestCaseData((Int128)ulong.MaxValue, ulong.MaxValue),
                new TestCaseData((UInt128)ulong.MaxValue, ulong.MaxValue),
                new TestCaseData((Int128)decimal.MaxValue, decimal.MaxValue),
                new TestCaseData((UInt128)decimal.MaxValue, decimal.MaxValue),
            ];
        }

        [Test]
        public void WideIntegerOutsideDecimalRangeDoesNotEqualDecimal()
        {
            Assert.That(Numerics.AreEqual(UInt128.MaxValue, decimal.MaxValue, ref _zeroTolerance), Is.False);
        }

        [TestCaseSource(nameof(WideIntegerJustOutsideDecimalRangeTestCases))]
        public void WideIntegerJustOutsideDecimalRangeDoesNotEqualDecimal(object wide, double wideAsDouble)
        {
            Assert.That(Numerics.AreEqual(wide, 0m, ref _zeroTolerance), Is.False);
            Assert.That(Numerics.AreEqual(0m, wide, ref _zeroTolerance), Is.False);
        }

        [TestCaseSource(nameof(WideIntegerJustOutsideDecimalRangeTestCases))]
        public void WideIntegerJustOutsideDecimalRangeComparesWithDecimal(object wide, double wideAsDouble)
        {
            Assert.That(Numerics.Compare(wide, 0m), Is.EqualTo(Math.Sign(wideAsDouble)));
            Assert.That(Numerics.Compare(0m, wide), Is.EqualTo(-Math.Sign(wideAsDouble)));
        }

        [TestCaseSource(nameof(WideIntegerJustOutsideDecimalRangeTestCases))]
        public void CanCalculateDifferenceBetweenWideIntegerJustOutsideDecimalRangeAndDecimal(object wide, double wideAsDouble)
        {
            Assert.That(Numerics.Difference(wide, 0m, ToleranceMode.Linear), Is.EqualTo(Math.Abs(wideAsDouble)));
            Assert.That(Numerics.Difference(0m, wide, ToleranceMode.Linear), Is.EqualTo(Math.Abs(wideAsDouble)));
        }

        private static TestCaseData[] WideIntegerJustOutsideDecimalRangeTestCases()
        {
            // Rounded to double these values equal the rounded decimal limits.
            return [
                new TestCaseData((Int128)decimal.MaxValue + 1, (double)decimal.MaxValue),
                new TestCaseData((UInt128)decimal.MaxValue + 1, (double)decimal.MaxValue),
                new TestCaseData((Int128)decimal.MinValue - 1, (double)decimal.MinValue),
            ];
        }

        [Test]
        public void CanCalculateDifferenceBetweenWideIntegerAndDouble()
        {
            Assert.That(Numerics.Difference((Int128)10, 1.5d, ToleranceMode.Linear), Is.EqualTo(8.5d));
        }

        [Test]
        public void CanCalculateDifferenceBetweenWideIntegerAndDecimal()
        {
            Assert.That(Numerics.Difference((UInt128)10, 1.5m, ToleranceMode.Linear), Is.EqualTo(8.5m));
        }
#endif

        private static TestCaseData[] FloatingPointEqualsTestCases
        {
            get
            {
                return [
                    TestCaseData.Create(123m),
#if !NETFRAMEWORK
                    TestCaseData.Create((Half)123),
#endif
                ];
            }
        }

        private static TestCaseData[] FixedPointEqualsTestCases
        {
            get
            {
                return [
                    TestCaseData.Create((nint)130),
                    TestCaseData.Create((nuint)131),
#if !NETFRAMEWORK
                    TestCaseData.Create((Int128)132),
                    TestCaseData.Create((UInt128)133),
#endif
                ];
            }
        }

        [TestCase((int)9500)]
        [TestCase((int)10000)]
        [TestCase((int)10500)]
        [TestCase((uint)9500)]
        [TestCase((uint)10000)]
        [TestCase((uint)10500)]
        [TestCase((long)9500)]
        [TestCase((long)10000)]
        [TestCase((long)10500)]
        [TestCase((ulong)9500)]
        [TestCase((ulong)10000)]
        [TestCase((ulong)10500)]
        [TestCaseSource(nameof(FixedPointToleranceTestCases))]
        public void CanMatchFixedPointsWithPercentage(object value)
        {
            Assert.That(Numerics.AreEqual(10000, value, ref _tenPercent), Is.True);
        }

        private static TestCaseData[] FixedPointToleranceTestCases
        {
            get
            {
                return [
                    TestCaseData.Create((nint)10500),
                    TestCaseData.Create((nint)9500),
                    TestCaseData.Create((nuint)10500),
                    TestCaseData.Create((nuint)9500),
#if !NETFRAMEWORK
                    TestCaseData.Create((Int128)10500),
                    TestCaseData.Create((Int128)9500),
                    TestCaseData.Create((UInt128)10500),
                    TestCaseData.Create((UInt128)9500),
#endif
                ];
            }
        }

        [TestCaseSource(nameof(FloatingPointToleranceTestCases))]
        public void CanMatchFloatingPointWithPercentage(object value)
        {
            Assert.That(Numerics.AreEqual(10000m, value, ref _tenPercent), Is.True);
        }

        private static TestCaseData[] FloatingPointToleranceTestCases
        {
            get
            {
                return [
                    TestCaseData.Create(9500m),
                    TestCaseData.Create(10000m),
                    TestCaseData.Create(10500m),
#if !NETFRAMEWORK
                    TestCaseData.Create((Half)9500m),
                    TestCaseData.Create((Half)10000m),
                    TestCaseData.Create((Half)10500m)
#endif
                ];
            }
        }

        [TestCaseSource(nameof(GetEqualsWithPercentageEdgeCases))]
        public void EqualsWithPercentageEdgeCases<T>(T value)
            where T : notnull
        {
            var tolerance = new Tolerance(0.0).Percent;
            Assert.That(Numerics.AreEqual(value, value, ref tolerance), Is.True);
        }

        [TestCaseSource(nameof(FractionalLinearToleranceTestCases))]
        public void CanMatchFractionalLinearTolerance(object a, object b, Tolerance toleranceValue)
        {
            Assert.That(Numerics.AreEqual(a, b, ref toleranceValue), Is.True);
        }

        [TestCaseSource(nameof(UlpsToleranceTestCases))]
        public void CanMatchUlpsTolerance<T>(T a, T b, Tolerance toleranceValue)
            where T : notnull
        {
            Assert.That(Numerics.AreEqual(a, b, ref toleranceValue), Is.True);
        }

        private static TestCaseData[] UlpsToleranceTestCases()
        {
            return [
                new TestCaseData(1.0d, 1.0000000000000004d, new Tolerance(2).Ulps) { TypeArgs = [typeof(double)] },
                new TestCaseData(1.0f, 1.0000002384f, new Tolerance(2).Ulps) { TypeArgs = [typeof(float)] },
#if !NETFRAMEWORK
                new TestCaseData((Half)1, (Half)1.001953125f, new Tolerance(2).Ulps) { TypeArgs = [typeof(Half)] },
#endif
            ];
        }

        private static TestCaseData[] FractionalLinearToleranceTestCases()
        {
            return [
                new TestCaseData(1m, 1.1m, new Tolerance(0.5m)),
                new TestCaseData(1d, 1.1d, new Tolerance(0.5d)),
                new TestCaseData(1f, 1.1f, new Tolerance(0.5f)),
                new TestCaseData(1m, 1.001m, new Tolerance(0.5m).Percent),
                new TestCaseData(1d, 1.001d, new Tolerance(0.5d).Percent),
                new TestCaseData(1f, 1.001f, new Tolerance(0.5f).Percent),
#if !NETFRAMEWORK
                //new TestCaseData((Half)1, (Half)1.1, new Tolerance((Half)0.5)),
                //new TestCaseData((Half)1, (Half)1.001, new Tolerance((Half)0.5).Percent),
#endif
            ];
        }

        private static TestCaseData[] GetEqualsWithPercentageEdgeCases
        {
            get
            {
                return [
                    new TestCaseData(int.MaxValue),
                    new TestCaseData(int.MinValue),
                    new TestCaseData(long.MaxValue),
                    new TestCaseData(long.MinValue),
                    new TestCaseData(ulong.MaxValue),
                    new TestCaseData(ulong.MinValue),
                    new TestCaseData(decimal.MaxValue),
                    new TestCaseData(decimal.MinValue),
                    new TestCaseData(float.MaxValue),
                    new TestCaseData(float.MinValue),
                    new TestCaseData(float.PositiveInfinity),
                    new TestCaseData(float.NegativeInfinity),
                    new TestCaseData(float.NaN),
                    new TestCaseData(double.MaxValue),
                    new TestCaseData(double.MinValue),
                    new TestCaseData(double.PositiveInfinity),
                    new TestCaseData(double.NegativeInfinity),
                    new TestCaseData(double.NaN),
#if !NETFRAMEWORK
                    new TestCaseData(Half.MaxValue),
                    new TestCaseData(Half.MinValue),
                    new TestCaseData(Half.PositiveInfinity),
                    new TestCaseData(Half.NegativeInfinity),
                    new TestCaseData(Half.NaN),
                    new TestCaseData(Int128.MaxValue - 5),
                    new TestCaseData(Int128.MaxValue),
                    new TestCaseData(Int128.MinValue),
                    new TestCaseData(UInt128.MaxValue),
                    new TestCaseData(UInt128.MinValue),
#endif
                ];
            }
        }

        [TestCaseSource(nameof(CalculateAbsoluteDifferenceFixedPointTestCases))]
        public object CanCalculateAbsoluteDifferenceFixedPoint<T>(T a, T b)
            where T : notnull
            => Numerics.Difference(a, b, _absoluteTolerance.Mode);

        [TestCaseSource(nameof(CalculateAbsoluteDifferenceFloatingPointTestCases))]
        public void CanCalculateAbsoluteDifferenceFloatingPoint<T1, T2, T3>(T1 a, T2 b, T3 expected)
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
        {
            Assert.That(Numerics.Difference(a, b, _absoluteTolerance.Mode), Is.EqualTo(expected).Within(0.001));
        }

        private static TestCaseData[] CalculateAbsoluteDifferenceFixedPointTestCases()
        {
            return
            [
                new TestCaseData<decimal>(10000m, 9500m) { ExpectedResult = 500m },
                new TestCaseData<ulong>(500, 0) { ExpectedResult = 500 },
                new TestCaseData<ulong>(0, 500) { ExpectedResult = 500 },
                new TestCaseData<int>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<uint>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<uint>(500, 0) { ExpectedResult = 500 },
                new TestCaseData<uint>(0, 500) { ExpectedResult = 500 },
                new TestCaseData<nint>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<nuint>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<nuint>(500, 0) { ExpectedResult = 500 },
                new TestCaseData<nuint>(0, 500) { ExpectedResult = 500 },
                new TestCaseData<ushort>(500, 0) { ExpectedResult = 500 },
                new TestCaseData<ushort>(0, 500) { ExpectedResult = 500 },
#if !NETFRAMEWORK
                new TestCaseData<Int128>(Int128.MaxValue, Int128.MaxValue - 500) { ExpectedResult = 500 },
                new TestCaseData<UInt128>(UInt128.MaxValue, UInt128.MaxValue - 500) { ExpectedResult = 500 },
                new TestCaseData<UInt128>((UInt128)500, (UInt128)0) { ExpectedResult = 500 },
                new TestCaseData<UInt128>((UInt128)0, (UInt128)500) { ExpectedResult = 500 }
#endif
            ];
        }

        private static TestCaseData[] CalculateAbsoluteDifferenceFloatingPointTestCases()
        {
            return
            [
                new TestCaseData<double, double, double>(0.1, 0.05, 0.05),
                new TestCaseData<double, double, double>(0.1, 0.15, 0.05),
#if !NETFRAMEWORK
                new TestCaseData<Half, Half, Half>((Half)0.1, (Half)0.05, (Half)0.05),
                new TestCaseData<Half, Half, double>((Half)0.1, (Half)0.05, 0.05),
#endif
            ];
        }

        [TestCaseSource(nameof(CanCalculatePercentDifferenceTestCases))]
        public void CanCalculatePercentDifference<T1, T2, T3>(T1 expected, T2 actual, T3 expectedResult)
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            => Assert.That(Numerics.Difference(expected, actual, _tenPercent.Mode), Is.EqualTo(expectedResult));

        private static TestCaseData[] CanCalculatePercentDifferenceTestCases()
        {
            return
            [
                new TestCaseData<decimal, decimal, object>(10000m, 8500m, 15),
                new TestCaseData<decimal, decimal, object>(10000m, 11500m, 15),

                new TestCaseData<int, int, object>(10000, 8500, 15),
                new TestCaseData<int, int, object>(10000, 11500, 15),
                new TestCaseData<uint, uint, object>(10000u, 8500u, 15),
                new TestCaseData<uint, uint, object>(10000u, 11500u, 15),

                new TestCaseData<long, long, object>(10000, 8500, 15),
                new TestCaseData<long, long, object>(10000, 11500, 15),
                new TestCaseData<ulong, ulong, object>(10000u, 8500u, 15),
                new TestCaseData<ulong, ulong, object>(10000u, 11500u, 15),

                new TestCaseData<nint, nint, object>(10000, 8500, 15),
                new TestCaseData<nint, nint, object>(10000, 11500, 15),
                new TestCaseData<nuint, nuint, object>(10000u, 8500u, 15),
                new TestCaseData<nuint, nuint, object>(10000u, 11500u, 15),
#if !NETFRAMEWORK
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)8500, 15),
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)11500, 15),
                new TestCaseData<UInt128, UInt128, object>((UInt128)10000, (UInt128)8500, 15),

                new TestCaseData<Half, Half, object>((Half)10000, (Half)8500, 15.04),
                new TestCaseData<Half, Half, object>((Half)10000, (Half)11500, 15.04),
                new TestCaseData<Half, Half, object>((Half)5, (Half)0, 100),
#endif
            ];
        }

        [TestCaseSource(nameof(NegativeSignedAndUnsignedTestCases))]
        public void NegativeSignedDoesNotEqualUnsigned_Issue5472(object signed, object unsigned)
        {
            Assert.That(Numerics.AreEqual(signed, unsigned, ref _zeroTolerance), Is.False);
            Assert.That(Numerics.AreEqual(unsigned, signed, ref _zeroTolerance), Is.False);
        }

        [TestCaseSource(nameof(NegativeSignedAndUnsignedTestCases))]
        public void NegativeSignedComparesLessThanUnsigned_Issue5472(object signed, object unsigned)
        {
            Assert.That(Numerics.Compare(signed, unsigned), Is.LessThan(0));
            Assert.That(Numerics.Compare(unsigned, signed), Is.GreaterThan(0));
        }

        private static TestCaseData[] NegativeSignedAndUnsignedTestCases()
        {
            return [
                new TestCaseData((sbyte)-1, 1u),
                new TestCaseData((short)-1, 1u),
                new TestCaseData(-1, 1u),
                new TestCaseData(int.MinValue, uint.MaxValue),
                new TestCaseData(-1, 1ul),
                new TestCaseData(-1L, 1ul),
                new TestCaseData(-1L, ulong.MaxValue),
                new TestCaseData(long.MinValue, ulong.MaxValue),
                new TestCaseData(-1, (nuint)1),
                new TestCaseData((nint)(-1), 1u),
                new TestCaseData((nint)(-1), (nuint)1),
                new TestCaseData((nint)(-1), ulong.MaxValue),
#if !NETFRAMEWORK
                new TestCaseData((Int128)(-1), 1u),
                new TestCaseData((Int128)(-1), ulong.MaxValue),
                new TestCaseData(-1, (UInt128)1),
                new TestCaseData((Int128)(-1), (UInt128)1),
                new TestCaseData(Int128.MinValue, UInt128.MaxValue),
#endif
            ];
        }

        [TestCaseSource(nameof(EqualSignedAndUnsignedTestCases))]
        public void NonNegativeSignedEqualsUnsigned(object signed, object unsigned)
        {
            Assert.That(Numerics.AreEqual(signed, unsigned, ref _zeroTolerance), Is.True);
            Assert.That(Numerics.AreEqual(unsigned, signed, ref _zeroTolerance), Is.True);
        }

        [TestCaseSource(nameof(EqualSignedAndUnsignedTestCases))]
        public void NonNegativeSignedComparesEqualToUnsigned(object signed, object unsigned)
        {
            Assert.That(Numerics.Compare(signed, unsigned), Is.Zero);
            Assert.That(Numerics.Compare(unsigned, signed), Is.Zero);
        }

        private static TestCaseData[] EqualSignedAndUnsignedTestCases()
        {
            return [
                new TestCaseData(0, 0u),
                new TestCaseData(int.MaxValue, (uint)int.MaxValue),
                new TestCaseData(5, 5ul),
                new TestCaseData(long.MaxValue, (ulong)long.MaxValue),
                new TestCaseData((nint)5, (nuint)5),
#if !NETFRAMEWORK
                new TestCaseData(5, (UInt128)5),
                new TestCaseData(Int128.MaxValue, (UInt128)Int128.MaxValue),
#endif
            ];
        }

        [TestCaseSource(nameof(SignedAndUnsignedWithinToleranceTestCases))]
        public void NegativeSignedEqualsUnsignedWithinTolerance(object signed, object unsigned, Tolerance tolerance)
        {
            Assert.That(Numerics.AreEqual(signed, unsigned, ref tolerance), Is.True);
        }

        private static TestCaseData[] SignedAndUnsignedWithinToleranceTestCases()
        {
            return [
                new TestCaseData(-1, 1u, new Tolerance(2)),
                new TestCaseData(-1L, 1ul, new Tolerance(2ul)),
                new TestCaseData((nint)(-1), (nuint)1, new Tolerance(2)),
                new TestCaseData(-10, 9u, new Tolerance(200).Percent),
                new TestCaseData(-10L, 9ul, new Tolerance(200).Percent),
#if !NETFRAMEWORK
                new TestCaseData((Int128)(-1), (UInt128)1, new Tolerance(2)),
                new TestCaseData((Int128)(-10), (UInt128)9, new Tolerance(200).Percent),
#endif
            ];
        }

        [TestCaseSource(nameof(SignedAndUnsignedDifferenceTestCases))]
        public void CanCalculateDifferenceBetweenSignedAndUnsigned(object expected, object actual, ToleranceMode mode, object expectedDifference)
        {
            Assert.That(Numerics.Difference(expected, actual, mode), Is.EqualTo(expectedDifference));
        }

        private static TestCaseData[] SignedAndUnsignedDifferenceTestCases()
        {
            return [
                new TestCaseData(-1, 1u, ToleranceMode.Linear, 2),
                new TestCaseData(1u, -1, ToleranceMode.Linear, 2),
                new TestCaseData(-1L, 1ul, ToleranceMode.Linear, 2),
                new TestCaseData(ulong.MaxValue, -1L, ToleranceMode.Linear, (decimal)ulong.MaxValue + 1),
                new TestCaseData((nint)(-1), (nuint)1, ToleranceMode.Linear, 2),
                new TestCaseData(-10, 5u, ToleranceMode.Percent, 150),
                new TestCaseData(-10L, 5ul, ToleranceMode.Percent, 150),
                new TestCaseData(0L, 5ul, ToleranceMode.Percent, double.PositiveInfinity),
#if !NETFRAMEWORK
                new TestCaseData((Int128)(-1), (UInt128)1, ToleranceMode.Linear, (BigInteger)2),
                new TestCaseData(UInt128.MaxValue, Int128.MinValue, ToleranceMode.Linear, (BigInteger)UInt128.MaxValue - (BigInteger)Int128.MinValue),
                new TestCaseData((Int128)(-10), (UInt128)5, ToleranceMode.Percent, 150),
#endif
            ];
        }

        [Test]
        public void MixedSignedAndUnsignedConstraints_Issue5472()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(-10, Is.Not.EqualTo(10u));
                Assert.That(-1L, Is.Not.EqualTo(ulong.MaxValue));
                Assert.That(-1, Is.EqualTo(1u).Within(2));
                Assert.That(1u, Is.EqualTo(-1).Within(2u));
                Assert.That(-1, Is.LessThan(1u));
                Assert.That(1ul, Is.GreaterThan(-1L));
            }
        }

        // Whether these values are equal as decimal depends on the runtime: up to .NET 10 the conversion
        // from double or float to decimal rounds to 15 or 7 significant digits, since .NET 11 it does not.
        // Whatever the outcome, AreEqual and Compare must agree.
        [TestCaseSource(nameof(DecimalAndBinaryFloatingPointTestCases))]
        public void AreEqualAndCompareAgreeForDecimalAndBinaryFloatingPoint_Issue5539(decimal value, object floatingPoint)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.AreEqual(value, floatingPoint, ref _zeroTolerance), Is.EqualTo(Numerics.Compare(value, floatingPoint) == 0));
                Assert.That(Numerics.AreEqual(floatingPoint, value, ref _zeroTolerance), Is.EqualTo(Numerics.Compare(floatingPoint, value) == 0));
            }
        }

        private static TestCaseData[] DecimalAndBinaryFloatingPointTestCases()
        {
            return [
                new TestCaseData(0.3m, 0.1d + 0.2d),
                new TestCaseData(0.1m, 0.1f),
                new TestCaseData(0.1m, 0.1d),
#if !NETFRAMEWORK
                new TestCaseData(0.1m, (Half)0.1),
#endif
            ];
        }

        [Test]
        public void EqualityAndComparisonConstraintsOfDecimalAndDoubleAgree_Issue5539()
        {
            double value = 0.1 + 0.2;

            bool isEqual = Is.EqualTo(0.3m).ApplyTo(value).IsSuccess;
            bool isLessThanOrEqual = Is.LessThanOrEqualTo(0.3m).ApplyTo(value).IsSuccess;
            bool isGreaterThanOrEqual = Is.GreaterThanOrEqualTo(0.3m).ApplyTo(value).IsSuccess;

            Assert.That(isEqual, Is.EqualTo(isLessThanOrEqual && isGreaterThanOrEqual));
        }

        [TestCaseSource(nameof(DecimalAndExactBinaryFloatingPointTestCases))]
        public void DecimalAndExactlyRepresentableBinaryFloatingPointAreEqual_Issue5539(decimal value, object floatingPoint)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.AreEqual(value, floatingPoint, ref _zeroTolerance), Is.True);
                Assert.That(Numerics.AreEqual(floatingPoint, value, ref _zeroTolerance), Is.True);
                Assert.That(Numerics.Compare(value, floatingPoint), Is.Zero);
                Assert.That(Numerics.Compare(floatingPoint, value), Is.Zero);
                Assert.That(floatingPoint, Is.EqualTo(value));
                Assert.That(floatingPoint, Is.LessThanOrEqualTo(value));
                Assert.That(floatingPoint, Is.GreaterThanOrEqualTo(value));
            }
        }

        private static TestCaseData[] DecimalAndExactBinaryFloatingPointTestCases()
        {
            return [
                new TestCaseData(0.5m, 0.5d),
                new TestCaseData(0.25m, 0.25f),
                new TestCaseData(-1.375m, -1.375d),
                new TestCaseData(1024.125m, 1024.125f),
#if !NETFRAMEWORK
                new TestCaseData(0.5m, (Half)0.5),
#endif
            ];
        }

        [Test]
        public void UlpsToleranceIsNotSupportedForDecimalAndDouble()
        {
            var tolerance = new Tolerance(1).Ulps;
            Assert.Throws<InvalidOperationException>(() => Numerics.AreEqual(0.3m, 0.3d, ref tolerance));
        }

        [TestCaseSource(nameof(DecimalAndValueOutsideDecimalRangeTestCases))]
        public void UlpsToleranceIsNotSupportedForDecimalAndValueOutsideDecimalRange(object expected, object actual)
        {
            // Whether Ulps are supported depends on the types, not on whether the values are compared as double.
            var tolerance = new Tolerance(1).Ulps;
            Assert.Throws<InvalidOperationException>(() => Numerics.AreEqual(expected, actual, ref tolerance));
        }

        private static TestCaseData[] DecimalAndValueOutsideDecimalRangeTestCases()
        {
            return [
                new TestCaseData(decimal.MaxValue, double.MaxValue),
                new TestCaseData(double.MaxValue, decimal.MaxValue),
                new TestCaseData(decimal.MaxValue, float.MaxValue),
#if !NETFRAMEWORK
                new TestCaseData(1m, Int128.MaxValue),
                new TestCaseData(UInt128.MaxValue, 1m),
#endif
            ];
        }

        [Test]
        public void DecimalAndValueOutsideDecimalRangeAreComparedAsDouble()
        {
            Assert.That(Numerics.AreEqual(decimal.MaxValue, float.MaxValue, ref _zeroTolerance), Is.False);
            Assert.That(Numerics.AreEqual(float.MaxValue, decimal.MaxValue, ref _zeroTolerance), Is.False);
        }

        [TestCaseSource(nameof(BinaryFloatingPointAtDecimalBoundaryTestCases))]
        public void BinaryFloatingPointAtDecimalBoundaryIsComparedAsDouble(object floatingPoint)
        {
            // decimal.MaxValue rounds up to 2^96 as double or float, which is just outside the decimal range.
            int sign = Math.Sign(Numerics.ConvertToValue<double>(floatingPoint));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.AreEqual(floatingPoint, 1m, ref _zeroTolerance), Is.False);
                Assert.That(Numerics.AreEqual(1m, floatingPoint, ref _zeroTolerance), Is.False);
                Assert.That(Numerics.Compare(floatingPoint, 1m), Is.EqualTo(sign));
                Assert.That(Numerics.Compare(1m, floatingPoint), Is.EqualTo(-sign));
                Assert.That(() => Numerics.Difference(floatingPoint, 1m, ToleranceMode.Linear), Throws.Nothing);
                Assert.That(() => Numerics.Difference(1m, floatingPoint, ToleranceMode.Linear), Throws.Nothing);
            }
        }

        private static TestCaseData[] BinaryFloatingPointAtDecimalBoundaryTestCases()
        {
            return [
                new TestCaseData((double)decimal.MaxValue),
                new TestCaseData((double)decimal.MinValue),
                new TestCaseData((float)decimal.MaxValue),
                new TestCaseData((float)decimal.MinValue),
            ];
        }

#if !NETFRAMEWORK
        [TestCaseSource(nameof(NonFiniteHalfTestCases))]
        public void DecimalAndNonFiniteHalfAreComparedAsDouble(Half half)
        {
            double asDouble = (double)half;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.AreEqual(1m, half, ref _zeroTolerance), Is.False);
                Assert.That(Numerics.AreEqual(half, 1m, ref _zeroTolerance), Is.False);
                Assert.That(Numerics.Compare(1m, half), Is.EqualTo(Numerics.Compare(1m, asDouble)));
                Assert.That(Numerics.Compare(half, 1m), Is.EqualTo(Numerics.Compare(asDouble, 1m)));
                Assert.That(Numerics.Difference(1m, half, ToleranceMode.Linear), Is.EqualTo(Numerics.Difference(1m, asDouble, ToleranceMode.Linear)));
                Assert.That(Numerics.Difference(half, 1m, ToleranceMode.Linear), Is.EqualTo(Numerics.Difference(asDouble, 1m, ToleranceMode.Linear)));
            }
        }

        private static TestCaseData[] NonFiniteHalfTestCases()
        {
            return [
                new TestCaseData(Half.NaN),
                new TestCaseData(Half.PositiveInfinity),
                new TestCaseData(Half.NegativeInfinity),
            ];
        }
#endif

        [Test]
        public void LinearToleranceOutsideDecimalRangeIsNotSupportedForDecimal()
        {
            var tolerance = new Tolerance(1e30);
            using (Assert.EnterMultipleScope())
            {
                Assert.Throws<InvalidOperationException>(() => Numerics.AreEqual<decimal>(decimal.MaxValue, decimal.MinValue, ref tolerance));
                Assert.Throws<InvalidOperationException>(() => Numerics.AreEqual((object)decimal.MaxValue, decimal.MinValue, ref tolerance));

                // Also when the values are compared as double, as that depends on the types and not on the values.
                Assert.Throws<InvalidOperationException>(() => Numerics.AreEqual(1m, double.MaxValue, ref tolerance));

                // Also for comparisons, including an integer expected value that is compared as decimal.
                Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(decimal.MaxValue).Within(1e30).ApplyTo(0m));
                Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(0).Within(1e30).ApplyTo(0m));
#if !NETFRAMEWORK
                Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(decimal.MaxValue).Within((UInt128)1 << 96).ApplyTo(0m));
#endif

                // decimal.MaxValue itself is within the range.
                Assert.That(decimal.MaxValue, Is.EqualTo(0m).Within(decimal.MaxValue));
                Assert.That(0m, Is.GreaterThanOrEqualTo(decimal.MaxValue).Within(decimal.MaxValue));
            }
        }

        [TestCaseSource(nameof(DecimalDifferenceOutsideDecimalRangeTestCases))]
        public void DecimalToleranceDoesNotOverflow(object expected, object actual, Tolerance tolerance, bool expectedResult)
        {
            // Both values fit in a decimal, but their difference does not.
            Assert.That(Numerics.AreEqual(expected, actual, ref tolerance), Is.EqualTo(expectedResult));
        }

        private static TestCaseData[] DecimalDifferenceOutsideDecimalRangeTestCases()
        {
            // A percentage is relative to the expected value, so it is not symmetric.
            return [
                new TestCaseData(decimal.MaxValue, -1d, new Tolerance(200).Percent, true),
                new TestCaseData(-1d, decimal.MaxValue, new Tolerance(200).Percent, false),
                new TestCaseData(decimal.MaxValue, -1d, new Tolerance(1), false),
                new TestCaseData(-1d, decimal.MaxValue, new Tolerance(1), false),
                // The difference exceeds the tolerance by 1, but both round to the same double.
                new TestCaseData(decimal.MaxValue, -1d, new Tolerance(decimal.MaxValue), false),
                new TestCaseData(-1d, decimal.MaxValue, new Tolerance(decimal.MaxValue), false),
                // decimal.MaxValue - 0.25m rounds back to decimal.MaxValue, which must not hide the overflow.
                new TestCaseData(decimal.MaxValue, -0.25d, new Tolerance(decimal.MaxValue), false),
                new TestCaseData(-0.25d, decimal.MaxValue, new Tolerance(decimal.MaxValue), false),
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(1), false),
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(decimal.MaxValue), false),
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(10).Percent, false),
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(300).Percent, true),
            ];
        }

        [Test]
        public void ToleranceDoesNotOverflow_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // The difference between these values does not fit in an int.
                Assert.That(int.MaxValue, Is.Not.EqualTo(int.MinValue).Within(1));
                Assert.That(int.MinValue, Is.Not.Zero.Within(1));
                Assert.That(int.MinValue, Is.Not.EqualTo(int.MaxValue).Within(10).Percent);
            }
        }

        [TestCaseSource(nameof(FloatingPointDifferenceOutsideRangeTestCases))]
        public void FloatingPointToleranceDoesNotOverflow_Issue5507(object expected, object actual, Tolerance tolerance, bool expectedResult)
        {
            // The difference between these values does not fit in their type.
            Assert.That(Numerics.AreEqual(expected, actual, ref tolerance), Is.EqualTo(expectedResult));
        }

        private static TestCaseData[] FloatingPointDifferenceOutsideRangeTestCases()
        {
            return [
                new TestCaseData(float.MaxValue, -float.MaxValue, new Tolerance(5e38), false),
                new TestCaseData(float.MaxValue, -float.MaxValue, new Tolerance(1e39), true),
                new TestCaseData(double.MaxValue, -double.MaxValue, new Tolerance(double.MaxValue), false),
                // The difference is 200% of the expected value.
                new TestCaseData(float.MaxValue, -float.MaxValue, new Tolerance(150).Percent, false),
                new TestCaseData(float.MaxValue, -float.MaxValue, new Tolerance(300).Percent, true),
                new TestCaseData(double.MaxValue, -double.MaxValue, new Tolerance(150).Percent, false),
                new TestCaseData(double.MaxValue, -double.MaxValue, new Tolerance(300).Percent, true),
#if !NETFRAMEWORK
                new TestCaseData((Half)60000, (Half)(-60000), new Tolerance(70000), false),
                new TestCaseData((Half)60000, (Half)(-60000), new Tolerance(130000), true),
                new TestCaseData((Half)60000, (Half)(-60000), new Tolerance(150).Percent, false),
                new TestCaseData((Half)60000, (Half)(-60000), new Tolerance(300).Percent, true),
#endif
            ];
        }

        [Test]
        public void FloatingPointComparisonWithPercentageDoesNotOverflow_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // 200% of 1e307 is 2e307, but 1e307 * 200 does not fit in a double. The range is -1e307 to 3e307.
                Assert.That(-0.9e307, Is.GreaterThan(1e307).Within(200).Percent);
                Assert.That(-1e308, Is.Not.GreaterThan(1e307).Within(200).Percent);
                Assert.That(2.9e307, Is.LessThan(1e307).Within(200).Percent);
                Assert.That(1e308, Is.Not.LessThan(1e307).Within(200).Percent);
            }
        }

        [Test]
        public void FloatingPointComparisonRangeDoesNotOverflow_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // The offset does not fit in a float, but the lower bound does: it is about -1.6e38.
                Assert.That(-1e38f, Is.GreaterThan(float.MaxValue).Within(5e38));
                Assert.That(-float.MaxValue, Is.Not.GreaterThan(float.MaxValue).Within(5e38));
                Assert.That(-float.MaxValue, Is.Not.GreaterThan(float.MaxValue).Within(150).Percent);
                Assert.That(float.MaxValue, Is.Not.LessThan(-float.MaxValue).Within(150).Percent);
#if !NETFRAMEWORK
                // Half.MaxValue is 65504, the lower bounds are -10000 and -30000.
                Assert.That((Half)(-5000), Is.GreaterThan((Half)60000).Within(70000));
                Assert.That((Half)(-60000), Is.Not.GreaterThan((Half)60000).Within(70000));
                Assert.That((Half)(-60000), Is.Not.GreaterThan((Half)60000).Within(150).Percent);
                Assert.That((Half)60000, Is.Not.LessThan((Half)(-60000)).Within(150).Percent);
#endif
            }
        }

        [Test]
        public void FloatingPointComparisonRangeWithOffsetTooSmallForPrecision()
        {
            using (Assert.EnterMultipleScope())
            {
                // Floats around 1e8 are 8 apart, Halfs around 60000 are 32 apart: 1 does not change them.
                Assert.That(100000000f, Is.GreaterThan(100000000f).Within(1));
                Assert.That(100000000f, Is.LessThan(100000000f).Within(1));
#if !NETFRAMEWORK
                Assert.That((Half)60000, Is.GreaterThan((Half)60000).Within(1));
                Assert.That((Half)60000, Is.LessThan((Half)60000).Within(1));
#endif
            }
        }

        [Test]
        public void FloatingPointComparisonRangeIsNotRoundedToTheTypeOfTheValue()
        {
            using (Assert.EnterMultipleScope())
            {
                // The lower bound 0.99 rounds to 0.99f as a float.
                Assert.That(0.99f, Is.GreaterThan(1f).Within(1).Percent);
                Assert.That(0.99f, Is.GreaterThan(1f).Within(0.01));
#if !NETFRAMEWORK
                Assert.That((Half)0.99, Is.GreaterThan((Half)1).Within(1).Percent);
#endif

                // Infinity minus an offset that overflows to infinity in the type of the value is NaN.
                Assert.That(0f, Is.Not.GreaterThan(float.PositiveInfinity).Within(5e38));
                Assert.That(0f, Is.Not.LessThan(float.NegativeInfinity).Within(5e38));
#if !NETFRAMEWORK
                Assert.That((Half)0, Is.Not.GreaterThan(Half.PositiveInfinity).Within(70000d));
                Assert.That((Half)0, Is.Not.LessThan(Half.NegativeInfinity).Within(70000d));
#endif
            }
        }

        [Test]
        public void DecimalComparisonRangeWithOneBoundOutsideDecimalRange_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // The upper bound does not fit in a decimal, but the lower bound decimal.MaxValue - 1 does.
                Assert.That(decimal.MaxValue, Is.GreaterThan(decimal.MaxValue).Within(1m));
                Assert.That(decimal.MaxValue - 1, Is.Not.GreaterThan(decimal.MaxValue).Within(1m));
                Assert.That(decimal.MinValue, Is.LessThan(decimal.MinValue).Within(1m));
                Assert.That(decimal.MinValue + 1, Is.Not.LessThan(decimal.MinValue).Within(1m));
            }
        }

        [Test]
        public void ComparisonWithToleranceDoesNotOverflow_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // A negative value with an unsigned tolerance.
                Assert.That(-2, Is.GreaterThan(-1).Within(2u));
                Assert.That(-3, Is.Not.GreaterThan(-1).Within(2u));
                Assert.That(-5L, Is.LessThanOrEqualTo(-8L).Within(3ul));

                // Bounds outside the range of the type.
                Assert.That(int.MinValue, Is.GreaterThanOrEqualTo(int.MinValue).Within(1));
                Assert.That(long.MaxValue, Is.LessThan(long.MaxValue).Within(1L));
                Assert.That(ulong.MaxValue, Is.LessThanOrEqualTo(ulong.MaxValue - 3).Within(3ul));
                Assert.That(ulong.MaxValue, Is.Not.LessThanOrEqualTo(ulong.MaxValue - 4).Within(3ul));

                // Bounds of values that a double cannot represent exactly.
                Assert.That(long.MaxValue - 1, Is.GreaterThan(long.MaxValue).Within(2.0));
                Assert.That(long.MaxValue - 2, Is.Not.GreaterThan(long.MaxValue).Within(2.0));
                Assert.That(long.MaxValue - 2, Is.GreaterThanOrEqualTo(long.MaxValue).Within(2.0));

                // A percentage of an integer is usually fractional: 1% of 202 is 2.02.
                Assert.That(200, Is.GreaterThan(202).Within(1).Percent);
                Assert.That(199, Is.Not.GreaterThan(202).Within(1).Percent);
                Assert.That(200, Is.GreaterThanOrEqualTo(202).Within(1).Percent);
                Assert.That(199, Is.Not.GreaterThanOrEqualTo(202).Within(1).Percent);
                Assert.That(204, Is.LessThan(202).Within(1).Percent);
                Assert.That(205, Is.Not.LessThanOrEqualTo(202).Within(1).Percent);
            }
        }

#if !NETFRAMEWORK
        [Test]
        public void IntegerPercentageRangeOutsideDecimalRangeIsExact()
        {
            using (Assert.EnterMultipleScope())
            {
                // The upper bound is 2^127, one more than Int128.MaxValue; the lower bound is Int128.MinValue + 1.
                Assert.That(Int128.MaxValue, Is.LessThan(Int128.MinValue).Within(200).Percent);
                Assert.That(Int128.MinValue, Is.Not.GreaterThanOrEqualTo(Int128.MaxValue).Within(200).Percent);
                Assert.That(Int128.MinValue + 1, Is.GreaterThanOrEqualTo(Int128.MaxValue).Within(200).Percent);
            }
        }
#endif

        [Test]
        public void ComparisonOfIntegerWithFractionalToleranceThrows()
        {
            // There is no integer bound that is correct for both > and >=: a > 3.5 is a > 3, but a >= 3.5 is a >= 4.
            Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(5).Within(1.5).ApplyTo(4));
            Assert.Throws<InvalidOperationException>(() => Is.LessThanOrEqualTo(5L).Within(0.5m).ApplyTo(4));

            // Also for mixed signed and unsigned integers, which .NET Framework compares as decimal.
            Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(-1).Within(0.5).ApplyTo(0ul));
            Assert.Throws<InvalidOperationException>(() => Is.GreaterThan(0ul).Within(0.5).ApplyTo(-1L));
        }

        [Test]
        public void DecimalPercentageRangeIsComparedAsDecimal()
        {
            // The upper bound is 100000001m. As a decimal it is compared exactly with the float,
            // which as an Int128 bound would be rounded to 100000000f.
            Assert.That(100000000f, Is.LessThan(50000000.5m).Within(100).Percent);
        }

        [Test]
        public void ComparisonOfIntegerWithFloatingPointActualAllowsFractionalTolerance()
        {
            // The expected value is often an integer constant, while the actual value is a double, float or decimal.
            // The range is then calculated in the type in which both are compared.
            using (Assert.EnterMultipleScope())
            {
                Assert.That(0.9999999, Is.GreaterThanOrEqualTo(1).Within(1e-6));
                Assert.That(0.999998, Is.Not.GreaterThanOrEqualTo(1).Within(1e-6));
                Assert.That(0.3, Is.GreaterThan(0).Within(0.5));
                Assert.That(-0.6, Is.Not.GreaterThan(0).Within(0.5));
                Assert.That(0.5f, Is.LessThanOrEqualTo(0).Within(0.5));
                Assert.That(0.6f, Is.Not.LessThanOrEqualTo(0).Within(0.5));
                Assert.That(0.3m, Is.LessThan(0L).Within(0.5m));
                Assert.That(0.6m, Is.Not.LessThan(0L).Within(0.5m));
#if !NETFRAMEWORK
                // The analyzer does not know that Half and int are comparable, so apply the constraint directly.
                Assert.That(Is.LessThanOrEqualTo(0).Within(0.5).ApplyTo((Half)0.5).IsSuccess, Is.True);
#endif

                // A double actual is compared with a double bound, not with an exact decimal bound of 199.98.
                Assert.That(199.98000000000002, Is.GreaterThan(202).Within(1).Percent);
            }
        }

#if !NETFRAMEWORK
        [Test]
        public void IntegerThatDoesNotFitInHalfOrSingleIsComparedAsDouble()
        {
            // The analyzer does not know that Half and int are comparable, so apply the constraints directly.
            // As Half or float, 100000 and UInt128.MaxValue would become infinite.
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Is.GreaterThan(100000).Within(50000).ApplyTo((Half)60000).IsSuccess, Is.True);
                Assert.That(Is.EqualTo(100000).Within(50000).ApplyTo((Half)60000).IsSuccess, Is.True);
                Assert.That(Is.EqualTo(100000).ApplyTo(Half.PositiveInfinity).IsSuccess, Is.False);
                Assert.That(Numerics.Compare(100000, Half.PositiveInfinity), Is.LessThan(0));

                Assert.That(Is.GreaterThan(UInt128.MaxValue).Within(1e38).ApplyTo(3e38f).IsSuccess, Is.True);
                Assert.That(Is.EqualTo(UInt128.MaxValue).Within(1e38).ApplyTo(3e38f).IsSuccess, Is.True);
                Assert.That(Is.EqualTo(UInt128.MaxValue).ApplyTo(float.PositiveInfinity).IsSuccess, Is.False);
                Assert.That(Numerics.Compare(UInt128.MaxValue, float.PositiveInfinity), Is.LessThan(0));
            }
        }
#endif

        [TestCaseSource(nameof(ToleranceRangeTypeTestCases))]
        public void ToleranceRangeUsesTypeOfValue(object value, Tolerance tolerance, object expectedLowerBound, object expectedUpperBound)
        {
            Tolerance.Range range = tolerance.ApplyToValue(value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(range.LowerBound, Is.EqualTo(expectedLowerBound).And.TypeOf(expectedLowerBound.GetType()));
                Assert.That(range.UpperBound, Is.EqualTo(expectedUpperBound).And.TypeOf(expectedUpperBound.GetType()));
            }
        }

#if !NETFRAMEWORK
        private static readonly Int128 E20 = (Int128)10000000000 * 10000000000;
#endif

        private static TestCaseData[] ToleranceRangeTypeTestCases()
        {
            return [
                // The type of the amount does not matter.
                new TestCaseData(5, new Tolerance(2L), 3, 7),
                new TestCaseData(5, new Tolerance(2.0), 3, 7),
                new TestCaseData(5L, new Tolerance(2), 3L, 7L),
                new TestCaseData(5u, new Tolerance(2ul), 3u, 7u),
                new TestCaseData(5.0, new Tolerance(2), 3.0, 7.0),
                new TestCaseData(5.0f, new Tolerance(2.5), 2.5f, 7.5f),
                new TestCaseData(5m, new Tolerance(2.5), 2.5m, 7.5m),

                // A floating point bound that is not exact in the type of the value uses double.
                new TestCaseData(float.MaxValue, new Tolerance(1e38), (double)float.MaxValue - 1e38, (double)float.MaxValue + 1e38),
                new TestCaseData(1f, new Tolerance(0.01), 1 - 0.01, 1 + 0.01),

                // A binary floating point amount is the decimal value it is written as,
                // also on runtimes that round its conversion to decimal to 15 significant digits.
                new TestCaseData(10000000000000002L, new Tolerance(10000000000000002d), 0L, 20000000000000004L),
                new TestCaseData(1m, new Tolerance(10000000000000002d), -10000000000000001m, 10000000000000003m),
                new TestCaseData(0L, new Tolerance(1e11f), -100000000000L, 100000000000L),
#if !NETFRAMEWORK
                new TestCaseData(Int128.MaxValue, new Tolerance(1e20f), Int128.MaxValue - E20, (UInt128)Int128.MaxValue + (UInt128)E20),
#endif

                // An offset too small for the precision of the value would not move the bound, so it uses double.
                new TestCaseData(100000000f, new Tolerance(1), 99999999.0, 100000001.0),
#if !NETFRAMEWORK
                new TestCaseData((Half)60000, new Tolerance(1), 59999.0, 60001.0),
                new TestCaseData((Half)60000, new Tolerance(64), (Half)59936, (Half)60064),
#endif

                // A decimal bound that does not fit in a decimal uses double, the other bound stays exact.
                new TestCaseData(decimal.MaxValue, new Tolerance(1m), decimal.MaxValue - 1, (double)decimal.MaxValue + 1),
                new TestCaseData(decimal.MinValue, new Tolerance(1m), (double)decimal.MinValue - 1, decimal.MinValue + 1),

                // A whole bound that does not fit in the type of the value uses the next wider integer type, as C# promotes:
                // long for 32-bit values and Int128 for 64-bit values. The other bound keeps the type of the value.
                new TestCaseData(-1, new Tolerance(2u), -3, 1),
                new TestCaseData(0u, new Tolerance(1), -1L, 1u),
                new TestCaseData(1u, new Tolerance(2), -1L, 3u),
                new TestCaseData(int.MaxValue, new Tolerance(1), int.MaxValue - 1, (long)int.MaxValue + 1),
                new TestCaseData(int.MinValue, new Tolerance(1), (long)int.MinValue - 1, int.MinValue + 1),
#if !NETFRAMEWORK
                new TestCaseData(0ul, new Tolerance(1), (Int128)(-1), 1ul),
                new TestCaseData(ulong.MaxValue, new Tolerance(1), ulong.MaxValue - 1, (Int128)ulong.MaxValue + 1),
                new TestCaseData(long.MinValue, new Tolerance(1), (Int128)long.MinValue - 1, long.MinValue + 1),
                new TestCaseData((UInt128)0, new Tolerance(1), (Int128)(-1), (UInt128)1),

                // A bound that does not fit in long uses Int128.
                new TestCaseData(0u, new Tolerance(1e19), -(Int128)10000000000000000000ul, (Int128)10000000000000000000ul),
#else
                // .NET Framework has no Int128, so a bound of a 64-bit value that does not fit uses decimal.
                new TestCaseData(0ul, new Tolerance(1), -1m, 1ul),
                new TestCaseData(ulong.MaxValue, new Tolerance(1), ulong.MaxValue - 1, (decimal)ulong.MaxValue + 1),
                new TestCaseData(0u, new Tolerance(1e19), -10000000000000000000m, 10000000000000000000m),
#endif

#if !NETFRAMEWORK
                // A whole percentage offset of an integer value outside the decimal range is exact, as a linear one.
                new TestCaseData(Int128.MinValue, new Tolerance(200).Percent, -3 * Math.Pow(2, 127), (UInt128)1 << 127),
                new TestCaseData(Int128.MaxValue, new Tolerance(200).Percent, Int128.MinValue + 1, 3 * Math.Pow(2, 127)),
#endif

                // A percentage of a decimal value keeps decimal bounds, also whole ones.
                new TestCaseData(5m, new Tolerance(20).Percent, 4m, 6m),
                new TestCaseData(0.5m, new Tolerance(200).Percent, -0.5m, 1.5m),
                new TestCaseData(0.1m, new Tolerance(10).Percent, 0.09m, 0.11m),

                // A fractional offset of an integer value uses decimal.
                new TestCaseData(202, new Tolerance(1).Percent, 199.98m, 204.02m),
                new TestCaseData(200, new Tolerance(1).Percent, 198, 202),
            ];
        }

        [TestCaseSource(nameof(ToleranceTestCases))]
        public bool AreEqualWithTolerance(object expected, object actual, Tolerance tolerance)
        {
            return Numerics.AreEqual(expected, actual, ref tolerance);
        }

        private static TestCaseData[] ToleranceTestCases()
        {
            return [
                // The difference does not fit in the type, but does in the unsigned type of the same size.
                new TestCaseData(int.MaxValue, int.MinValue, new Tolerance(1)) { ExpectedResult = false },
                new TestCaseData(int.MaxValue, int.MinValue, new Tolerance(uint.MaxValue)) { ExpectedResult = true },
                new TestCaseData(long.MaxValue, long.MinValue, new Tolerance(1)) { ExpectedResult = false },
                new TestCaseData(long.MaxValue, long.MinValue, new Tolerance(ulong.MaxValue)) { ExpectedResult = true },
                new TestCaseData((nint)(-5), (nint)5, new Tolerance(10)) { ExpectedResult = true },
                new TestCaseData(int.MinValue, 0, new Tolerance(1)) { ExpectedResult = false },
                new TestCaseData(int.MinValue, int.MaxValue, new Tolerance(10).Percent) { ExpectedResult = false },
                new TestCaseData(long.MinValue, long.MaxValue, new Tolerance(10).Percent) { ExpectedResult = false },
                new TestCaseData(long.MinValue, long.MaxValue, new Tolerance(200).Percent) { ExpectedResult = true },

                // A tolerance larger than the type allows any difference.
                new TestCaseData(int.MaxValue, int.MinValue, new Tolerance(long.MaxValue)) { ExpectedResult = true },
                new TestCaseData(long.MaxValue, long.MinValue, new Tolerance(1e30)) { ExpectedResult = true },
                new TestCaseData(long.MaxValue, long.MinValue, new Tolerance(1e28).Percent) { ExpectedResult = true },

                // Also for mixed signed and unsigned integers, which .NET Framework compares as decimal.
                new TestCaseData(-1L, ulong.MaxValue, new Tolerance(1e30)) { ExpectedResult = true },
                new TestCaseData(-1L, ulong.MaxValue, new Tolerance(1)) { ExpectedResult = false },

                // A binary floating point amount is the decimal value it is written as, as for comparison ranges:
                // 1e11f is 10^11, not 99999997952, 1e20f is 10^20, not 100000002004087734272, and 0.1f is 0.1.
                new TestCaseData(0L, 100000000000L, new Tolerance(1e11f)) { ExpectedResult = true },
                new TestCaseData(0L, 100000000001L, new Tolerance(1e11f)) { ExpectedResult = false },
                new TestCaseData(1m, 1.1m, new Tolerance(0.1f)) { ExpectedResult = true },
                new TestCaseData(1m, 1.1000000001m, new Tolerance(0.1f)) { ExpectedResult = false },
#if !NETFRAMEWORK
                new TestCaseData(Int128.MaxValue, Int128.MaxValue - E20, new Tolerance(1e20f)) { ExpectedResult = true },
                new TestCaseData(Int128.MaxValue, Int128.MaxValue - E20 - 1, new Tolerance(1e20f)) { ExpectedResult = false },
                new TestCaseData((UInt128)E20 - 1, (Int128)(-1), new Tolerance(1e20f)) { ExpectedResult = true },
                new TestCaseData((UInt128)E20 - 1, (Int128)(-2), new Tolerance(1e20f)) { ExpectedResult = false },
#endif

                // A fractional tolerance is rounded down, as the difference between integers is a whole number.
                new TestCaseData(2, 4, new Tolerance(1.5)) { ExpectedResult = false },
                new TestCaseData(2, 3, new Tolerance(1.5)) { ExpectedResult = true },
                new TestCaseData(2, 4, new Tolerance(2.5m)) { ExpectedResult = true },
                new TestCaseData(2, 3, new Tolerance(0.1)) { ExpectedResult = false },

                // A percentage of an integer allows the difference rounded down: 1% of 202 is 2.02.
                new TestCaseData(202, 200, new Tolerance(1).Percent) { ExpectedResult = true },
                new TestCaseData(202, 199, new Tolerance(1).Percent) { ExpectedResult = false },
                new TestCaseData(2, 3, new Tolerance(1.5).Percent) { ExpectedResult = false },
                new TestCaseData(0, 0, new Tolerance(10).Percent) { ExpectedResult = true },
                new TestCaseData(0, 1, new Tolerance(10).Percent) { ExpectedResult = false },

                // A binary floating point percentage is the decimal value it is written as,
                // also on runtimes that round its conversion to decimal to 15 significant digits.
                new TestCaseData(100, 200, new Tolerance(99.99999999999999d).Percent) { ExpectedResult = false },
                new TestCaseData(3, 4, new Tolerance(100d / 3).Percent) { ExpectedResult = true },

#if !NETFRAMEWORK
                // 12 * 83.33333333333333333333333333 rounds to 1000 as decimal, but the allowed difference is below 10.
                // .NET Framework has no BigInteger to calculate it exactly.
                new TestCaseData(12, 2, new Tolerance(83.33333333333333333333333333m).Percent) { ExpectedResult = false },
                new TestCaseData(12, 3, new Tolerance(83.33333333333333333333333333m).Percent) { ExpectedResult = true },
#endif

                // decimal.MaxValue / 2 rounds up to 2^95, so the product of 2 and 2^95 must not be calculated.
                new TestCaseData(2, 1000, new Tolerance(39614081257132168796771975168m).Percent) { ExpectedResult = true },
                new TestCaseData(2ul, ulong.MaxValue, new Tolerance(39614081257132168796771975168m).Percent) { ExpectedResult = true },

                // The difference between decimals can be outside the decimal range.
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(1)) { ExpectedResult = false },
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(10).Percent) { ExpectedResult = false },
                new TestCaseData(decimal.MaxValue, decimal.MinValue, new Tolerance(300).Percent) { ExpectedResult = true },
                new TestCaseData(1.5m, 2.75m, new Tolerance(1.25m)) { ExpectedResult = true },
#if !NETFRAMEWORK
                new TestCaseData(Int128.MaxValue, Int128.MinValue, new Tolerance(1)) { ExpectedResult = false },
                new TestCaseData(Int128.MaxValue, Int128.MinValue, new Tolerance(UInt128.MaxValue)) { ExpectedResult = true },
                new TestCaseData(Int128.MinValue, Int128.MaxValue, new Tolerance(10).Percent) { ExpectedResult = false },
                new TestCaseData(Int128.MinValue, Int128.MaxValue, new Tolerance(200).Percent) { ExpectedResult = true },
                new TestCaseData(UInt128.MaxValue, UInt128.MinValue, new Tolerance(1e40)) { ExpectedResult = true },

                // Mixed sign UInt128 values are compared as BigInteger.
                new TestCaseData((Int128)(-5), (UInt128)5, new Tolerance(10)) { ExpectedResult = true },
                new TestCaseData((Int128)(-5), (UInt128)5, new Tolerance(9.5)) { ExpectedResult = false },
                new TestCaseData(Int128.MinValue, UInt128.MaxValue, new Tolerance(1e39)) { ExpectedResult = true },
                new TestCaseData(Int128.MinValue, UInt128.MaxValue, new Tolerance(1e38)) { ExpectedResult = false },
                new TestCaseData((Int128)(-100), (UInt128)50, new Tolerance(150).Percent) { ExpectedResult = true },
                new TestCaseData((Int128)(-100), (UInt128)51, new Tolerance(150).Percent) { ExpectedResult = false },

                // Percentages of 128-bit values outside the decimal range.
                new TestCaseData(Exp10UInt128(20), UInt128.MaxValue, new Tolerance(1e21).Percent) { ExpectedResult = true },
                new TestCaseData(Exp10UInt128(20), UInt128.MaxValue, new Tolerance(1e18).Percent) { ExpectedResult = false },
                new TestCaseData((Int128)(-1), UInt128.MaxValue, new Tolerance(decimal.MaxValue).Percent) { ExpectedResult = false },
                new TestCaseData((Int128)0, UInt128.MaxValue, new Tolerance(decimal.MaxValue).Percent) { ExpectedResult = false },

                // The smallest decimal percentage: 1e-28 percent of 10^38 is 10^8. A smaller percentage is rounded to a decimal.
                new TestCaseData(Exp10UInt128(38), Exp10UInt128(38) + 100000000, new Tolerance(1e-28).Percent) { ExpectedResult = true },
                new TestCaseData(Exp10UInt128(38), Exp10UInt128(38) + 100000001, new Tolerance(1e-28).Percent) { ExpectedResult = false },

                // A binary floating point percentage is the decimal value it is written as:
                // 1e-17f is not exactly 1e-17, but 1e-17 percent of 10^20 is 10.
                new TestCaseData((Int128)Exp10UInt128(20), (Int128)Exp10UInt128(20) + 10, new Tolerance(1e-17f).Percent) { ExpectedResult = true },
                new TestCaseData((Int128)Exp10UInt128(20), (Int128)Exp10UInt128(20) + 11, new Tolerance(1e-17f).Percent) { ExpectedResult = false },
#endif
            ];
        }

#if !NETFRAMEWORK
        private static UInt128 Exp10UInt128(int exponent) => (UInt128)BigInteger.Pow(10, exponent);
#endif

        [TestCaseSource(nameof(AbsoluteDifferenceTestCases))]
        public void AbsoluteDifferenceIsPositiveAndOfTheUnsignedType(object expected, object actual, object expectedDifference)
        {
            Assert.That(Numerics.Difference(expected, actual, ToleranceMode.Linear), Is.EqualTo(expectedDifference).And.TypeOf(expectedDifference.GetType()));
            Assert.That(Numerics.Difference(actual, expected, ToleranceMode.Linear), Is.EqualTo(expectedDifference).And.TypeOf(expectedDifference.GetType()));
        }

        private static TestCaseData[] AbsoluteDifferenceTestCases()
        {
            // The difference between two integers fits in the unsigned type of the same size.
            return [
                new TestCaseData(2, 5, 3u),
                new TestCaseData(int.MaxValue, int.MinValue, uint.MaxValue),
                new TestCaseData((short)-3, (byte)4, 7u),
                new TestCaseData(2u, 5u, 3u),
                new TestCaseData(long.MaxValue, long.MinValue, ulong.MaxValue),
                new TestCaseData(2ul, 5ul, 3ul),
                new TestCaseData(-1, 1u, 2ul),
                new TestCaseData((nint)(-5), (nint)5, (nuint)10),
                new TestCaseData(1.5, 2.25, 0.75),
                new TestCaseData(1.5m, 2.25m, 0.75m),
                new TestCaseData(decimal.MaxValue, decimal.MinValue, 2 * (double)decimal.MaxValue),
#if !NETFRAMEWORK
                new TestCaseData(Int128.MaxValue, Int128.MinValue, UInt128.MaxValue),
                new TestCaseData((Half)1.5, (Half)2.25, (Half)0.75),
#endif
            ];
        }

        [Test]
        public void PercentDifferenceIsPositive()
        {
            Assert.That(Numerics.Difference(-200, -150, ToleranceMode.Percent), Is.EqualTo(25.0));
            Assert.That(Numerics.Difference(200, 250, ToleranceMode.Percent), Is.EqualTo(25.0));
            Assert.That(Numerics.Difference(int.MinValue, int.MaxValue, ToleranceMode.Percent), Is.EqualTo((double)uint.MaxValue * 100 / 2147483648.0));
        }

        [Test]
        public void PercentDifferenceDoesNotOverflow_Issue5507()
        {
            using (Assert.EnterMultipleScope())
            {
                // The difference times 100 does not fit in a double.
                Assert.That(Numerics.Difference(1e307, 0d, ToleranceMode.Percent), Is.EqualTo(100.0));

                // The difference itself does not fit in a double.
                Assert.That(Numerics.Difference(double.MaxValue, -double.MaxValue, ToleranceMode.Percent), Is.EqualTo(200.0));
            }
        }

        [Test]
        public void DifferenceForNonNumericTypesReturnsNaN()
        {
            Assert.That(Numerics.Difference(new object(), new object(), _tenPercent.Mode), Is.EqualTo(double.NaN));
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanMatchNonConvertibleOperands_Issue5535(object a, object b)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.AreEqual(a, b, ref _zeroTolerance), Is.True);
                Assert.That(Numerics.AreEqual(b, a, ref _zeroTolerance), Is.True);
            }
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanCompareNonConvertibleOperands_Issue5535(object a, object b)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.Compare(a, b), Is.Zero);
                Assert.That(Numerics.Compare(b, a), Is.Zero);
            }
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanCalculateDifferenceBetweenNonConvertibleOperands_Issue5535(object a, object b)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Numerics.Difference(a, b, ToleranceMode.Linear), Is.Zero);
                Assert.That(Numerics.Difference(b, a, ToleranceMode.Linear), Is.Zero);
            }
        }

        // ulong and long are at least as wide as nuint and nint, so they must be used for the calculation.
        // Converting these values to nuint or nint would overflow in a 32-bit process.
        [TestCaseSource(nameof(WideAndNativeIntegerTestCases))]
        public void CanCompareWideIntegerWithNativeInteger(object wide, object native)
        {
            Assert.That(Numerics.Compare(wide, native), Is.GreaterThan(0));
            Assert.That(Numerics.Compare(native, wide), Is.LessThan(0));
        }

        private static TestCaseData[] WideAndNativeIntegerTestCases()
        {
            return [
                new TestCaseData(5_000_000_000ul, (nuint)1),
                new TestCaseData(5_000_000_000L, (nint)1),
            ];
        }

        [TestCaseSource(nameof(WideAndNativeIntegerTestCases))]
        public void CanCalculateDifferenceBetweenWideIntegerAndNativeInteger(object wide, object native)
        {
            Assert.That(Numerics.Difference(wide, native, ToleranceMode.Linear), Is.EqualTo(4_999_999_999L));
            Assert.That(Numerics.Difference(native, wide, ToleranceMode.Linear), Is.EqualTo(4_999_999_999L));
        }

        private static TestCaseData[] NonConvertibleOperandTestCases()
        {
            // nint and nuint are not IConvertible and char cannot be converted to floating point types.
            return [
                new TestCaseData(5ul, (nuint)5),
                new TestCaseData(5L, (nint)5),
                new TestCaseData('A', 65d),
                new TestCaseData('A', 65f),
                new TestCaseData('A', 65m),
                new TestCaseData('A', (nint)65),
                new TestCaseData('A', (nuint)65),
#if !NETFRAMEWORK
                new TestCaseData('A', (Half)65),
                new TestCaseData('A', (Int128)65),
                new TestCaseData('A', (UInt128)65),
#endif
            ];
        }

        [Test]
        public void TestFloatsAndDoubles()
        {
            object x = 0.0500000007f;
            Assert.That(x, Is.EqualTo(0.05).Within(0.0000001));
        }

        [Test]
        public void CanCompareDecimalsWithHighPrecision()
        {
            var expected = 95217168582.206969750145956m;
            var actual = 95217168582.20696975014595521m;

            var result = Numerics.Compare(expected, actual);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void CanCalculateDifferenceDecimalsWithHighPrecision()
        {
            var expected = 95217168582.206969750145956m;
            var actual = 95217168582.20696975014595521m;

            var result = Numerics.Difference(expected, actual, ToleranceMode.Linear);

            Assert.That(result, Is.EqualTo(0.00000000000000079M));
        }

        [Test]
        public void CanCompareDoublesWithHighMantissa()
        {
            var expected = Convert.ToDouble(decimal.MaxValue) * 1.1;
            var actual = Convert.ToDouble(decimal.MaxValue);

            var result = Numerics.Difference(expected, actual, ToleranceMode.Linear);

            Assert.That(result, Is.EqualTo(7.9228162514264408E+27));
        }

        [Test]
        public void CanCompareDecimalAndHighDouble()
        {
            var expected = Convert.ToDouble(decimal.MaxValue) * 1.1;
            var actual = decimal.MaxValue;

            var result = Numerics.Difference(expected, actual, ToleranceMode.Linear);

            Assert.That(result, Is.EqualTo(7.9228162514264408E+27));
        }

        [Test]
        public void CanCompareDoubleAndHighDouble()
        {
            const double maximum = 1e30;
            const double value = 0.0099999999999988987;

            Assert.That(value, Is.LessThan(maximum));
        }

        [Test]
        public void CanCompareHighResDouble()
        {
            const double value = 1.0000000000000038d;

            Assert.That(value, Is.GreaterThan(1.0));
        }

        [Test]
        public void CanCompareMidRangeDecimalAndDouble()
        {
            var expected = 3.14159m;
            var actual = 2.718281d;

            var result = Numerics.Difference(expected, actual, ToleranceMode.Linear);

            // Within 15 decimal places of precision as this is the minimum precision across our supported runtimes
            // NET11 makes this more granular, so we use a very small tolerance to account for that.
            Assert.That(result, Is.EqualTo(0.423309).Within(0.000000000000001));
        }

        [TestCase((int)8500)]
        [TestCase((int)11500)]
        [TestCase((uint)8500)]
        [TestCase((uint)11500)]
        [TestCase((long)8500)]
        [TestCase((long)11500)]
        [TestCase((ulong)8500)]
        [TestCase((ulong)11500)]
        public void FailsOnIntegralsOutsideOfPercentage<T>(T value)
            where T : struct, IEquatable<T>
        {
            Assert.Throws<AssertionException>(() => Assert.That(Numerics.AreEqual(10000, value, ref _tenPercent), Is.True));
        }

        [Test]
        public void FailsOnDecimalBelowPercentage()
        {
            Assert.Throws<AssertionException>(() => Assert.That(Numerics.AreEqual(10000m, 8500m, ref _tenPercent), Is.True));
        }

        [Test]
        public void FailsOnDecimalAbovePercentage()
        {
            Assert.Throws<AssertionException>(() => Assert.That(Numerics.AreEqual(10000m, 11500m, ref _tenPercent), Is.True));
        }

        [TestCaseSource(nameof(NegativeFloatingPointOutsideOfPercentageTestCases))]
        public void FailsOnNegativeFloatingPointOutsideOfPercentage_Issue5500(object expected, object actual)
        {
            Assert.That(Numerics.AreEqual(expected, actual, ref _tenPercent), Is.False);
        }

        private static TestCaseData[] NegativeFloatingPointOutsideOfPercentageTestCases()
        {
            return [
                new TestCaseData(-10000d, -8500d),
                new TestCaseData(-10000d, -11500d),
                new TestCaseData(-10000d, 10000d),
                new TestCaseData(-10000f, -8500f),
                new TestCaseData(-10000f, -11500f),
                new TestCaseData(-10000f, 10000f),
#if !NETFRAMEWORK
                new TestCaseData((Half)(-10000), (Half)(-8500)),
                new TestCaseData((Half)(-10000), (Half)(-11500)),
                new TestCaseData((Half)(-10000), (Half)10000),
#endif
            ];
        }

        [TestCaseSource(nameof(NegativeFloatingPointWithinPercentageTestCases))]
        public void CanMatchNegativeFloatingPointWithPercentage(object expected, object actual)
        {
            Assert.That(Numerics.AreEqual(expected, actual, ref _tenPercent), Is.True);
        }

        private static TestCaseData[] NegativeFloatingPointWithinPercentageTestCases()
        {
            return [
                new TestCaseData(-10000d, -9500d),
                new TestCaseData(-10000d, -10500d),
                new TestCaseData(-10000f, -9500f),
                new TestCaseData(-10000f, -10500f),
#if !NETFRAMEWORK
                new TestCaseData((Half)(-10000), (Half)(-9500)),
                new TestCaseData((Half)(-10000), (Half)(-10500)),
#endif
            ];
        }

        [Test]
        public void FailsOnDecimalIsPartOfIsFixedPointNumericMethod()
        {
            Assert.That(Numerics.IsFixedPointNumeric(1000m), Is.False);
        }
    }
}
