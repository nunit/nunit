// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.Assertions;

namespace NUnit.Framework.Tests.ClassicAssertions
{
    [TestFixture]
    public class NotEqualFixture
    {
        [Test]
        public void NotEqual()
        {
            Legacy.ClassicAssert.AreNotEqual(5, 3);
        }

        [Test]
        public void NotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.AreNotEqual(5, 5));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NullNotEqualToNonNull()
        {
            Legacy.ClassicAssert.AreNotEqual(null, 3);
        }

        [Test]
        public void NullEqualsNull()
        {
            var expectedMessage =
                "  Expected: not equal to null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.AreNotEqual(null, null));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void ArraysNotEqual()
        {
            Legacy.ClassicAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 3, 2 });
        }

        [Test]
        public void ArraysNotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to < 1, 2, 3 >" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Legacy.ClassicAssert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void UInt()
        {
            uint u1 = 5;
            uint u2 = 8;
            Legacy.ClassicAssert.AreNotEqual(u1, u2);
        }

        [Test]
        public void NotEqualSameTypes()
        {
            byte b1 = 35;
            sbyte sb2 = 35;
            decimal d4 = 35;
            double d5 = 35;
            float f6 = 35;
            int i7 = 35;
            uint u8 = 35;
            long l9 = 35;
            short s10 = 35;
            ushort us11 = 35;

            byte b12 = 35;
            sbyte sb13 = 35;
            decimal d14 = 35;
            double d15 = 35;
            float s16 = 35;
            int i17 = 35;
            uint ui18 = 35;
            long i19 = 35;
            ulong ui20 = 35;
            short i21 = 35;
            ushort i22 = 35;

            Legacy.ClassicAssert.AreNotEqual(23, b1);
            Legacy.ClassicAssert.AreNotEqual(23, sb2);
            Legacy.ClassicAssert.AreNotEqual(23, d4);
            Legacy.ClassicAssert.AreNotEqual(23, d5);
            Legacy.ClassicAssert.AreNotEqual(23, f6);
            Legacy.ClassicAssert.AreNotEqual(23, i7);
            Legacy.ClassicAssert.AreNotEqual(23, u8);
            Legacy.ClassicAssert.AreNotEqual(23, l9);
            Legacy.ClassicAssert.AreNotEqual(23, s10);
            Legacy.ClassicAssert.AreNotEqual(23, us11);

            Legacy.ClassicAssert.AreNotEqual(23, b12);
            Legacy.ClassicAssert.AreNotEqual(23, sb13);
            Legacy.ClassicAssert.AreNotEqual(23, d14);
            Legacy.ClassicAssert.AreNotEqual(23, d15);
            Legacy.ClassicAssert.AreNotEqual(23, s16);
            Legacy.ClassicAssert.AreNotEqual(23, i17);
            Legacy.ClassicAssert.AreNotEqual(23, ui18);
            Legacy.ClassicAssert.AreNotEqual(23, i19);
            Legacy.ClassicAssert.AreNotEqual(23, ui20);
            Legacy.ClassicAssert.AreNotEqual(23, i21);
            Legacy.ClassicAssert.AreNotEqual(23, i22);
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);
            var expected = new ThrowsIfToStringIsCalled(2);

            Legacy.ClassicAssert.AreNotEqual(expected, actual);
        }
    }
}
