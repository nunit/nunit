// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Assertions
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
            Assert.Zero(_i1);
            Assert.Zero(_u1);
            Assert.Zero(_l1);
            Assert.Zero(_ul1);
            Assert.Zero(_f1);
            Assert.Zero(_de1);
            Assert.Zero(_d1);
        }

        [Test]
        public void AssertZeroFailsWhenNumberIsNotAZero()
        {
            Assert.Throws<AssertionException>(() => Assert.Zero(_i2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_u2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_l2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_ul2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_f2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_de2));
            Assert.Throws<AssertionException>(() => Assert.Zero(_d2));
        }

        [Test]
        public void NotZeroIsNotZero()
        {
            Assert.NotZero(_i2);
            Assert.NotZero(_u2);
            Assert.NotZero(_l2);
            Assert.NotZero(_ul2);
            Assert.NotZero(_f2);
            Assert.NotZero(_de2);
            Assert.NotZero(_d2);
        }

        [Test]
        public void AssertNotZeroFailsWhenNumberIsZero()
        {
            Assert.Throws<AssertionException>(() => Assert.NotZero(_i1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_u1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_l1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_ul1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_f1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_de1));
            Assert.Throws<AssertionException>(() => Assert.NotZero(_d1));
        }

        [Test]
        public void ZeroWithMessagesOverload()
        {
            Assert.That(
                () => Assert.Zero(1, "MESSAGE"),
                Throws.TypeOf<AssertionException>().With.Message.Contains("MESSAGE"));
        }

        [Test]
        public void ZeroWithMessageOverloadPasses()
        {
            Assert.Zero(0, "Message");
        }

        [Test]
        public void ExpectedFailureMessageExists()
        {
            var expectedMessage =
                "  Expected: 0" + Environment.NewLine +
                "  But was:  1234" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Zero(_i2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}


