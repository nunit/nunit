// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

#if !NETFRAMEWORK
using System;
#endif

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class FloatingPointNumericsTests
    {
        /// <summary>Tests the floating point value comparison helper</summary>
        [Test]
        public void FloatEqualityWithUlps()
        {
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000008f, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000017f, 1),
                Is.False);

            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.06f, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.13f, 1),
                Is.False);
            Assert.That(// Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0f, -2.0f, 1),
                Is.False);
        }

        /// <summary>Tests the double precision floating point value comparison helper</summary>
        [Test]
        public void DoubleEqualityWithUlps()
        {
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000002, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000004, 1),
                Is.False);

            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000001, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000002, 1),
                Is.False);
            Assert.That(// Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0, -2.0, 1),
                Is.False);
        }

#if !NETFRAMEWORK
        [Test]
        public void HalfEqualityWithUlps()
        {
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps((Half)1.0f, (Half)1.0009765625f, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps((Half)1.0f, (Half)1.001953125, 1),
                Is.False);

            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps((Half)1000.00f, (Half)1000.5, 1),
                Is.True);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps((Half)1000.00f, (Half)1001.0f, 1),
                Is.False);
            Assert.That(// Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps((Half)2.0f, (Half)(-2.0f), 1),
                Is.False);
            Assert.That(
                FloatingPointNumerics.AreAlmostEqualUlps((Half)65500, (Half)65472, 1),
                Is.True);
            Assert.That(// Ensure we don't overflow on large ulps values
                FloatingPointNumerics.AreAlmostEqualUlps((Half)65500, (Half)(-495.75f), 40_000L),
                Is.True);
        }
#endif
    }
}
