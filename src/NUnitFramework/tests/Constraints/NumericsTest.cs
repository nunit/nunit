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
            Assert.That(Numerics.Difference(wide, 0m, ToleranceMode.Linear), Is.EqualTo(wideAsDouble));
            Assert.That(Numerics.Difference(0m, wide, ToleranceMode.Linear), Is.EqualTo(-wideAsDouble));
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
                new TestCaseData<ushort>(0, 500) { ExpectedResult = -500 },
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
                new TestCaseData<double, double, double>(0.1, 0.15, -0.05),
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
                new TestCaseData<decimal, decimal, object>(10000m, 11500m, -15),

                new TestCaseData<int, int, object>(10000, 8500, 15),
                new TestCaseData<int, int, object>(10000, 11500, -15),
                new TestCaseData<uint, uint, object>(10000u, 8500u, 15),
                new TestCaseData<uint, uint, object>(10000u, 11500u, 15),

                new TestCaseData<long, long, object>(10000, 8500, 15),
                new TestCaseData<long, long, object>(10000, 11500, -15),
                new TestCaseData<ulong, ulong, object>(10000u, 8500u, 15),
                new TestCaseData<ulong, ulong, object>(10000u, 11500u, 15),

                new TestCaseData<nint, nint, object>(10000, 8500, 15),
                new TestCaseData<nint, nint, object>(10000, 11500, -15),
                new TestCaseData<nuint, nuint, object>(10000u, 8500u, 15),
                new TestCaseData<nuint, nuint, object>(10000u, 11500u, 15),
#if !NETFRAMEWORK
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)8500, 15),
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)11500, -15),
                new TestCaseData<UInt128, UInt128, object>((UInt128)10000, (UInt128)8500, 15),

                new TestCaseData<Half, Half, object>((Half)10000, (Half)8500, (Half)15.04),
                new TestCaseData<Half, Half, object>((Half)10000, (Half)11500, (Half)(-15.04)),
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
                new TestCaseData(-1, 1u, ToleranceMode.Linear, -2),
                new TestCaseData(1u, -1, ToleranceMode.Linear, 2),
                new TestCaseData(-1L, 1ul, ToleranceMode.Linear, -2),
                new TestCaseData(ulong.MaxValue, -1L, ToleranceMode.Linear, (decimal)ulong.MaxValue + 1),
                new TestCaseData((nint)(-1), (nuint)1, ToleranceMode.Linear, -2),
                new TestCaseData(-10, 5u, ToleranceMode.Percent, 150),
                new TestCaseData(-10L, 5ul, ToleranceMode.Percent, 150),
                new TestCaseData(0L, 5ul, ToleranceMode.Percent, double.NegativeInfinity),
#if !NETFRAMEWORK
                new TestCaseData((Int128)(-1), (UInt128)1, ToleranceMode.Linear, (BigInteger)(-2)),
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

        [Test]
        public void DifferenceForNonNumericTypesReturnsNaN()
        {
            Assert.That(Numerics.Difference(new object(), new object(), _tenPercent.Mode), Is.EqualTo(double.NaN));
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanMatchNonConvertibleOperands_Issue5535(object a, object b)
        {
            Assert.That(Numerics.AreEqual(a, b, ref _zeroTolerance), Is.True);
            Assert.That(Numerics.AreEqual(b, a, ref _zeroTolerance), Is.True);
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanCompareNonConvertibleOperands_Issue5535(object a, object b)
        {
            Assert.That(Numerics.Compare(a, b), Is.Zero);
            Assert.That(Numerics.Compare(b, a), Is.Zero);
        }

        [TestCaseSource(nameof(NonConvertibleOperandTestCases))]
        public void CanCalculateDifferenceBetweenNonConvertibleOperands_Issue5535(object a, object b)
        {
            Assert.That(Numerics.Difference(a, b, ToleranceMode.Linear), Is.Zero);
            Assert.That(Numerics.Difference(b, a, ToleranceMode.Linear), Is.Zero);
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

            Assert.That(result, Is.EqualTo(0.423309));
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
