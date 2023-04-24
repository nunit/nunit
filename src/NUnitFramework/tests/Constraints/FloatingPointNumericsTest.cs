// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class FloatingPointNumericsTests
    {

        /// <summary>Tests the floating point value comparison helper</summary>
        [Test]
        public void FloatEqualityWithUlps()
        {
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000008f, 1)
, Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000017f, 1)
, Is.False);

            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.06f, 1)
, Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.13f, 1)
, Is.False);
            Assert.That( // Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0f, -2.0f, 1)
, Is.False);
        }

        /// <summary>Tests the double precision floating point value comparison helper</summary>
        [Test]
        public void DoubleEqualityWithUlps()
        {
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000002, 1)
, Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000004, 1)
, Is.False);

            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000001, 1)
, Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000002, 1)
, Is.False);
            Assert.That( // Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0, -2.0, 1)
, Is.False);
        }
    }
}
