// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Assertions
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
            Assert.Positive(_i1);
            Assert.Positive(_l1);
            Assert.Positive(_f1);
            Assert.Positive(_de1);
            Assert.Positive(_d1);
        }

        [Test]
        public void AssertNegativeNumbersFailPositiveAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.Positive(_i2));
            Assert.Throws<AssertionException>(() => Assert.Positive(_l2));
            Assert.Throws<AssertionException>(() => Assert.Positive(_f2));
            Assert.Throws<AssertionException>(() => Assert.Positive(_de2));
            Assert.Throws<AssertionException>(() => Assert.Positive(_d2));
        }

        [Test]
        public void NegativeNumbersPassNegativeAssertion()
        {
            Assert.Negative(_i2);
            Assert.Negative(_l2);
            Assert.Negative(_f2);
            Assert.Negative(_de2);
            Assert.Negative(_d2);
        }

        [Test]
        public void AssertPositiveNumbersFailNegativeAssertion()
        {
            Assert.Throws<AssertionException>(() => Assert.Negative(_i1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_u1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_l1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_ul1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_f1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_de1));
            Assert.Throws<AssertionException>(() => Assert.Negative(_d1));
        }

        [Test]
        public void IsNegativeWithMessageOverload()
        {
            Assert.That(
                () => Assert.Negative(1, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverload()
        {
            Assert.That(
                () => Assert.Positive(-1, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void IsPositiveWithMessageOverloadPasses()
        {
            Assert.Positive(1, "Message");
        }

        [Test]
        public void IsNegativeWithMessageOverloadPasses()
        {
            Assert.Negative(-1, "Message");
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsPositive()
        {
            var expectedMessage =
                "  Expected: greater than 0" + Environment.NewLine +
                "  But was:  -1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Positive(_i2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ExpectedFailureMessageExistsForIsNegative()
        {
            var expectedMessage =
                "  Expected: less than 0" + Environment.NewLine +
                "  But was:  1" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Negative(_i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}


