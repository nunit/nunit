// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class NumericsTests
    {
        private Tolerance _tenPercent, _zeroTolerance, _absoluteTolerance;

        [SetUp]
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

        private static TestCaseData[] FloatingPointEqualsTestCases
        {
            get
            {
                return [
                    new TestCaseData(123m),
#if !NETFRAMEWORK
                    BuildTestCaseData((Half)124),
#endif
                ];
            }
        }

        private static TestCaseData[] FixedPointEqualsTestCases
        {
            get
            {
                return [
                    BuildTestCaseData((nint)130),
                    BuildTestCaseData((nuint)131),
#if !NETFRAMEWORK
                    BuildTestCaseData((Int128)132),
                    BuildTestCaseData((UInt128)133),
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
                    BuildTestCaseData((nint)10500),
                    BuildTestCaseData((nint)9500),
                    BuildTestCaseData((nuint)10500),
                    BuildTestCaseData((nuint)9500),
#if !NETFRAMEWORK
                    BuildTestCaseData((Int128)10500),
                    BuildTestCaseData((Int128)9500),
                    BuildTestCaseData((UInt128)10500),
                    BuildTestCaseData((UInt128)9500),
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
                    new TestCaseData(9500m),
                    new TestCaseData(10000m),
                    new TestCaseData(10500m),
#if !NETFRAMEWORK
                    BuildTestCaseData((Half)9500m),
                    BuildTestCaseData((Half)10000m),
                    BuildTestCaseData((Half)10500m)
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
                    new TestCaseData(double.MaxValue),
                    new TestCaseData(double.MinValue),
#if !NETFRAMEWORK
                    new TestCaseData(Half.MaxValue),
                    new TestCaseData(Half.MinValue),
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
            => Numerics.Difference(a, b, _absoluteTolerance.Mode);

        [TestCaseSource(nameof(CalculateAbsoluteDifferenceFloatingPointTestCases))]
        public void CanCalculateAbsoluteDifferenceFloatingPoint<T1, T2, T3>(T1 a, T2 b, T3 expected)
        {
#pragma warning disable NUnit2047 // Incompatible types for Within constraint
            Assert.That(Numerics.Difference(a, b, _absoluteTolerance.Mode), Is.EqualTo(expected).Within(0.00001));
#pragma warning restore NUnit2047 // Incompatible types for Within constraint
        }

        private static TestCaseData[] CalculateAbsoluteDifferenceFixedPointTestCases()
        {
            return
            [
                new TestCaseData<decimal>(10000m, 9500m) { ExpectedResult = 500m },
                new TestCaseData<int>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<nint>(10000, 9500) { ExpectedResult = 500 },
                new TestCaseData<nuint>(10000, 9500) { ExpectedResult = 500 },
#if !NETFRAMEWORK
                new TestCaseData<Int128>(Int128.MaxValue, Int128.MaxValue - 500) { ExpectedResult = 500 },
                new TestCaseData<UInt128>(UInt128.MaxValue, UInt128.MaxValue - 500) { ExpectedResult = 500 },
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
                new TestCaseData<uint, uint, object>(10000u, 11500u, -15),
                new TestCaseData<uint, uint, object>(0u, 11500u, double.NegativeInfinity),

                new TestCaseData<long, long, object>(10000, 8500, 15),
                new TestCaseData<long, long, object>(10000, 11500, -15),
                new TestCaseData<ulong, ulong, object>(10000u, 8500u, 15),
                new TestCaseData<ulong, ulong, object>(10000u, 11500u, -15),
                new TestCaseData<ulong, ulong, object>(0u, 11500u, double.NegativeInfinity),

                new TestCaseData<nint, nint, object>(10000, 8500, 15),
                new TestCaseData<nint, nint, object>(10000, 11500, -15),
                new TestCaseData<nuint, nuint, object>(10000u, 8500u, 15),
                new TestCaseData<nuint, nuint, object>(10000u, 11500u, -15),
                new TestCaseData<nuint, nuint, object>(0u, 11500u, double.NegativeInfinity),
#if !NETFRAMEWORK
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)8500, 15),
                new TestCaseData<Int128, Int128, object>((Int128)10000, (Int128)11500, -15),
                new TestCaseData<UInt128, UInt128, object>((UInt128)10000, (UInt128)8500, 15),
                new TestCaseData<UInt128, UInt128, object>((UInt128)10000, (UInt128)11500, -15),
                new TestCaseData<UInt128, UInt128, object>((UInt128)0, (UInt128)11500, double.NegativeInfinity),

                new TestCaseData<Half, Half, object>((Half)10000, (Half)8500, (Half)15.040000000000001d),
                new TestCaseData<Half, Half, object>((Half)10000, (Half)11500, (Half)(-15.040000000000001d)),
                new TestCaseData<Half, Half, object>((Half)0, (Half)5, double.NegativeInfinity),
                new TestCaseData<Half, Half, object>((Half)5, (Half)0, 100),
#endif
            ];
        }

        [Test]
        public void DifferenceForNonNumericTypesReturnsNaN()
        {
            Assert.That(Numerics.Difference(new object(), new object(), _tenPercent.Mode), Is.EqualTo(double.NaN));
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

            Assert.That(expected, Is.GreaterThan(actual));
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

        [Test]
        public void FailsOnDecimalIsPartOfIsFixedPointNumericMethod()
        {
            Assert.That(Numerics.IsFixedPointNumeric(1000m), Is.False);
        }

        private static TestCaseData BuildTestCaseData<T>(T value)
            => new TestCaseData<T>(value);
    }
}
