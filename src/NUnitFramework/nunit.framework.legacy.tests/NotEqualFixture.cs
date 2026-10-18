// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.Assertions;

namespace NUnit.Framework.Legacy.Tests
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
            sbyte sb1 = 35;
            byte b1 = 35;
            short s1 = 35;
            ushort us1 = 35;
            int i1 = 35;
            uint u1 = 35;
            long l1 = 35;
            ulong ul1 = 35;
            decimal dec1 = 35;
            float f1 = 35;
            double d1 = 35;
            char c1 = '3';

            sbyte sb2 = 23;
            byte b2 = 23;
            short s2 = 23;
            ushort us2 = 23;
            int i2 = 23;
            uint u2 = 23;
            long l2 = 23;
            ulong ul2 = 23;
            decimal dec2 = 23;
            float f2 = 23;
            double d2 = 23;
            char c2 = '2';

            ClassicAssert.AreNotEqual(sb1, sb2);
            ClassicAssert.AreNotEqual(b1, b2);
            ClassicAssert.AreNotEqual(s1, s2);
            ClassicAssert.AreNotEqual(us1, us2);
            ClassicAssert.AreNotEqual(i1, i2);
            ClassicAssert.AreNotEqual(u1, u2);
            ClassicAssert.AreNotEqual(l1, l2);
            ClassicAssert.AreNotEqual(ul1, ul2);
            ClassicAssert.AreNotEqual(dec1, dec2);
            ClassicAssert.AreNotEqual(f1, f2);
            ClassicAssert.AreNotEqual(d1, d2);
            ClassicAssert.AreNotEqual(c1, c2);
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
