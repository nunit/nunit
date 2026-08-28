// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
        [TestCaseSource(nameof(IntegerEqualsTestCases))]
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

        private static TestCaseData[] IntegerEqualsTestCases
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
        [TestCaseSource(nameof(IntegerToleranceTestCases))]
        public void CanMatchIntegralsWithPercentage(object value)
        {
            Assert.That(Numerics.AreEqual(10000, value, ref _tenPercent), Is.True);
        }

        private static TestCaseData[] IntegerToleranceTestCases
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

        [Test]
        public void CanCalculateAbsoluteDifference()
        {
            Assert.That(Numerics.Difference(10000m, 9500m, _absoluteTolerance.Mode), Is.EqualTo(500m));
            Assert.That(Convert.ToDouble(Numerics.Difference(0.1, 0.05, _absoluteTolerance.Mode)), Is.EqualTo(0.05).Within(0.00001));
            Assert.That(Convert.ToDouble(Numerics.Difference(0.1, 0.15, _absoluteTolerance.Mode)), Is.EqualTo(-0.05).Within(0.00001));
        }

        [Test]
        public void CanCalculatePercentDifference()
        {
            Assert.That(Numerics.Difference(10000m, 8500m, _tenPercent.Mode), Is.EqualTo(15));
            Assert.That(Numerics.Difference(10000m, 11500m, _tenPercent.Mode), Is.EqualTo(-15));
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

        private static TestCaseData BuildTestCaseData<T>(T value, [CallerArgumentExpression(nameof(value))] string argDisplayName = null!)
        {
            var data = new TestCaseData<T>(value);

            if (value is null)
            {
                return data.SetArgDisplayNames("null");
            }
            else
            {
                // Workaround for:
                // There is no primitive literal for some types (ex: nint)
                // We must construct these with a cast (ex: (nint)1) but not 'as' since they are not reference types
                // Yet parentheses within arg names have issues displaying in Test Explorer
                // So we extract the 'cast' type and represent it as 'as' in the display name to avoid parentheses
                var castType = Regex.Match(argDisplayName, @"^(\(.+\)).+$").Groups[1].Value.ToString().AsSpan();
                return data.SetArgDisplayNames($"{value} as {castType.Slice(1, castType.Length - 2).ToString()}");
            }
        }
    }
}
