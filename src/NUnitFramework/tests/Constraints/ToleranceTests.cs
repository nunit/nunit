// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ToleranceTests
    {
        [Test, DefaultFloatingPointTolerance(0.1)]
        public void DefaultTolerance_Success()
        {
            Assert.That(2.05d, Is.EqualTo(2.0d));
        }

        [Test, DefaultFloatingPointTolerance(0.01)]
        public void DefaultTolerance_Failure()
        {
            Assert.That(2.05d, Is.Not.EqualTo(2.0d));
        }

        [Test, DefaultFloatingPointTolerance(0.5)]
        public void TestToleranceDefault()
        {
            var defaultTolerance = Tolerance.Default;
            Assert.That(defaultTolerance.IsUnsetOrDefault, Is.True);

            var comparer = new NUnitEqualityComparer();
            Assert.That(comparer.AreEqual(2.0d, 2.1d, ref defaultTolerance), Is.True);
        }

        [Test, DefaultFloatingPointTolerance(0.5)]
        public void TestToleranceExact()
        {
            var noneTolerance = Tolerance.Exact;
            Assert.That(noneTolerance.IsUnsetOrDefault, Is.False);

            var comparer = new NUnitEqualityComparer();
            Assert.That(comparer.AreEqual(2.0d, 2.1d, ref noneTolerance), Is.False);
        }

        [Test]
        public void TestToleranceVarianceExact()
        {
            var noneTolerance = Tolerance.Exact;
            Assert.That(noneTolerance.HasVariance, Is.False);
        }

        [Test]
        public void TestToleranceVarianceDefault()
        {
            var noneTolerance = Tolerance.Default;
            Assert.That(noneTolerance.HasVariance, Is.False);
        }

        [Test]
        public void TestWithinCanOnlyBeUsedOnce()
        {
            Assert.That(() => Is.EqualTo(1.1d).Within(0.5d).Within(0.2d),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("modifier may appear only once"));
        }

        [Test]
        public void TestModesCanOnlyBeUsedOnce()
        {
            var tolerance = new Tolerance(5);
            Assert.That(() => tolerance.Percent.Ulps,
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("multiple tolerance modes"));
        }

        [Test]
        public void TestNumericToleranceRequired()
        {
            var tolerance = new Tolerance("Five");
            Assert.That(() => tolerance.Percent,
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("numeric tolerance is required"));
        }

        [Test]
        public void TestModeMustFollowTolerance()
        {
            var tolerance = Tolerance.Default; // which is new Tolerance(0, ToleranceMode.Unset)
            Assert.That(() => tolerance.Percent,
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Tolerance amount must be specified"));
        }

        [Test]
        public void TestToleranceDefaultIsSameAs()
        {
            Assert.That(Tolerance.Default, Is.SameAs(Tolerance.Default));
        }

        [Test]
        public void TestToleranceExactIsSameAs()
        {
            Assert.That(Tolerance.Exact, Is.SameAs(Tolerance.Exact));
        }

        [Test]
        public void ToStringTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Tolerance.Default.ToString(), Is.EqualTo("Unset"));
                Assert.That(Tolerance.Exact.ToString(), Is.EqualTo("Exact"));
                Assert.That(Is.EqualTo(5).Within(2).Tolerance.ToString(), Is.EqualTo("2"));
                Assert.That(Is.EqualTo(5).Within(2).Ulps.Tolerance.ToString(), Is.EqualTo("2 Ulps"));
                Assert.That(Is.EqualTo(5).Within(2).Percent.Tolerance.ToString(), Is.EqualTo("2 Percent"));
                Assert.That(Is.EqualTo(5).Within(2).Seconds.Tolerance.ToString(), Is.EqualTo("00:00:02"));
                Assert.That(Is.EqualTo(5).Within(2).Minutes.Tolerance.ToString(), Is.EqualTo("00:02:00"));
            });
        }

        [Test]
        public void EqualityTests()
        {
            var tol1 = Tolerance.Default;
            var tol2 = Tolerance.Default;
            Assert.That(tol1, Is.EqualTo(tol2));
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(tol1 == tol2, Is.True);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
        }
    }
}
