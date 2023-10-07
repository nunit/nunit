// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertZeroTests
    {
        private readonly int _i1 = 0;
        private readonly int _i2 = 1234;
        private readonly uint _u1 = 0;
        private readonly uint _u2 = 12345879;
        private readonly long _l1 = 0;
        private readonly long _l2 = 12345879;
        private readonly ulong _ul1 = 0;
        private readonly ulong _ul2 = 12345879;
        private readonly float _f1 = 0F;
        private readonly float _f2 = 8.543F;
        private readonly decimal _de1 = 0M;
        private readonly decimal _de2 = 83.4M;
        private readonly double _d1 = 0.0;
        private readonly double _d2 = 8.0;

        [Test]
        public void ZeroIsZero()
        {
            Assert.That(_i1, Is.Zero);
            Assert.That(_u1, Is.Zero);
            Assert.That(_l1, Is.Zero);
            Assert.That(_ul1, Is.Zero);
            Assert.That(_f1, Is.Zero);
            Assert.That(_de1, Is.Zero);
            Assert.That(_d1, Is.Zero);
        }

        [Test]
        public void AssertZeroFailsWhenNumberIsNotAZero()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_i2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_u2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_l2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_ul2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_f2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_de2, Is.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_d2, Is.Zero));
        }

        [Test]
        public void NotZeroIsNotZero()
        {
            Assert.That(_i2, Is.Not.Zero);
            Assert.That(_u2, Is.Not.Zero);
            Assert.That(_l2, Is.Not.Zero);
            Assert.That(_ul2, Is.Not.Zero);
            Assert.That(_f2, Is.Not.Zero);
            Assert.That(_de2, Is.Not.Zero);
            Assert.That(_d2, Is.Not.Zero);
        }

        [Test]
        public void AssertNotZeroFailsWhenNumberIsZero()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_i1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_u1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_l1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_ul1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_f1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_de1, Is.Not.Zero));
            Assert.Throws<AssertionException>(() => Assert.That(_d1, Is.Not.Zero));
        }

        [Test]
        public void ZeroWithMessagesOverload()
        {
            Assert.That(
                () => Assert.That(1, Is.Zero, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void ZeroWithMessageOverloadPasses()
        {
            Assert.That(0, Is.Zero, "Message");
        }

        [Test]
        public void ExpectedFailureMessageExists()
        {
            var expectedMessage =
                "  Expected: 0" + Environment.NewLine +
                "  But was:  1234" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(_i2, Is.Zero));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }
    }
}
