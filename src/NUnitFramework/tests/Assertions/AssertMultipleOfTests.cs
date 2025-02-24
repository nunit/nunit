// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertMultipleOfTests
    {
        private const int Number = 3;

        private readonly byte _b1 = 3;
        private readonly byte _b2 = 4;
        private readonly int _i1 = 6;
        private readonly int _i2 = 7;
        private readonly uint _u1 = 9;
        private readonly uint _u2 = 10;
        private readonly long _l1 = 12;
        private readonly long _l2 = 13;
        private readonly ulong _ul1 = 15;
        private readonly ulong _ul2 = 16;
        private readonly float _f1 = 18F;
        private readonly float _f2 = 19F;
        private readonly decimal _de1 = 21M;
        private readonly decimal _de2 = 22M;
        private readonly double _d1 = 24D;
        private readonly double _d2 = 25D;

        [Test]
        public void SucceedsIfMultipleOfNumber()
        {
            Assert.That(_b1, Is.MultipleOf(Number));
            Assert.That(_i1, Is.MultipleOf(Number));
            Assert.That(_u1, Is.MultipleOf(Number));
            Assert.That(_l1, Is.MultipleOf(Number));
            Assert.That(_ul1, Is.MultipleOf(Number));
        }

        [Test]
        public void SucceedsIfOddOrEven()
        {
            Assert.That(_b1, Is.Odd);
            Assert.That(_i1, Is.Even);
            Assert.That(_u1, Is.Odd);
            Assert.That(_l1, Is.Even);
            Assert.That(_ul1, Is.Odd);
        }

        [Test]
        public void FailsIfMultipleOfFloatingPoint()
        {
            // Should this throw an exception instead?
            Assert.Throws<AssertionException>(() => Assert.That(_f1, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_de1, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_d1, Is.MultipleOf(Number)));

            Assert.Throws<AssertionException>(() => Assert.That(_f2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_de2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_d2, Is.MultipleOf(Number)));
        }

        [Test]
        public void FailsIfNotMultipleOfNumber()
        {
            Assert.Throws<AssertionException>(() => Assert.That(_b2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_i2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_u2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_l2, Is.MultipleOf(Number)));
            Assert.Throws<AssertionException>(() => Assert.That(_ul2, Is.MultipleOf(Number)));
        }

        [Test]
        public void Combines()
        {
            Assert.That(2 * 3, Is.MultipleOf(3).And.Even);
            Assert.That(3 * 3, Is.MultipleOf(3).And.Odd);
            Assert.That(2 * 4, Is.Not.MultipleOf(3).And.Even);
        }
    }
}
