// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
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
        [Test]
        public void CanMatchWithoutToleranceMode(object value)
        {
            Assert.That(Numerics.AreEqual(value, value, ref _zeroTolerance), Is.True);
        }

        // Separate test case because you can't use decimal in an attribute (24.1.3)
        [Test]
        public void CanMatchDecimalWithoutToleranceMode()
        {
            Assert.That(Numerics.AreEqual(123m, 123m, ref _zeroTolerance), Is.True);
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
        [Test]
        public void CanMatchIntegralsWithPercentage(object value)
        {
            Assert.That(Numerics.AreEqual(10000, value, ref _tenPercent), Is.True);
        }

        [Test]
        public void CanMatchDecimalWithPercentage()
        {
            Assert.That(Numerics.AreEqual(10000m, 9500m, ref _tenPercent), Is.True);
            Assert.That(Numerics.AreEqual(10000m, 10000m, ref _tenPercent), Is.True);
            Assert.That(Numerics.AreEqual(10000m, 10500m, ref _tenPercent), Is.True);
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
        public void CanCompareDecimalsWithHighPrecision()
        {
            var expected = 95217168582.206969750145956m;
            var actual =   95217168582.20696975014595521m;

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
            var actual =   2.718281d;

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
        public void FailsOnIntegralsOutsideOfPercentage(object value)
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
    }
}
