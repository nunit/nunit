// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertPolarityTests
    {
        private readonly int _i1 = 1;
        private readonly int _i2 = -1;
        private readonly uint _u1 = 123456789;
        private readonly long _l1 = 123456789;
        private readonly long _l2 = -12345879;
        private readonly ulong _ul1 = 123456890;
        private readonly float _f1 = 8.543F;
        private readonly float _f2 = -8.543F;
        private readonly decimal _de1 = 83.4M;
        private readonly decimal _de2 = -83.4M;
        private readonly double _d1 = 8.0;
        private readonly double _d2 = -8.0;

        [Test]
        public void PositiveNumbersPassPositiveAssertion()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_i1, Is.Positive);
                Assert.That(_l1, Is.Positive);
                Assert.That(_f1, Is.Positive);
                Assert.That(_de1, Is.Positive);
                Assert.That(_d1, Is.Positive);
            });
        }

        [Test]
        public void AssertNegativeNumbersFailPositiveAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_i2, Is.Positive));
            Assert.Throws<AssertionException>(() => Assert.That(_l2, Is.Positive));
            Assert.Throws<AssertionException>(() => Assert.That(_f2, Is.Positive));
            Assert.Throws<AssertionException>(() => Assert.That(_de2, Is.Positive));
            Assert.Throws<AssertionException>(() => Assert.That(_d2, Is.Positive));
        }

        [Test]
        public void NegativeNumbersPassNegativeAssertion()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_i2, Is.Negative);
                Assert.That(_l2, Is.Negative);
                Assert.That(_f2, Is.Negative);
                Assert.That(_de2, Is.Negative);
                Assert.That(_d2, Is.Negative);
            });
        }

        [Test]
        public void AssertPositiveNumbersFailNegativeAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_i1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_u1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_l1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_ul1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_f1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_de1, Is.Negative));
            Assert.Throws<AssertionException>(() => Assert.That(_d1, Is.Negative));
        }

        [Test]
        public void IsNegativeWithMessageOverload()
        {
            Assert.That(
                () => Assert.That(1, Is.Negative, "MESSAGE"),
                Throws.TypeOf<SingleAssertException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverload()
        {
            Assert.That(
                () => Assert.That(-1, Is.Positive, "MESSAGE"),
                Throws.TypeOf<SingleAssertException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverloadPasses()
        {
            Assert.That(1, Is.Positive, "Message");
        }

        [Test]
        public void IsNegativeWithMessageOverloadPasses()
        {
            Assert.That(-1, Is.Negative, "Message");
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsPositive()
        {
            var expectedMessage =
                "  Expected: greater than 0" + Environment.NewLine +
                "  But was:  -1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(_i2, Is.Positive));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsNegative()
        {
            var expectedMessage =
                "  Expected: less than 0" + Environment.NewLine +
                "  But was:  1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(_i1, Is.Negative));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
