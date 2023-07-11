// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class NotEqualFixture
    {
        [Test]
        public void NotEqual()
        {
            Classic.Assert.AreNotEqual(5, 3);
        }

        [Test]
        public void NotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreNotEqual(5, 5));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NullNotEqualToNonNull()
        {
            Classic.Assert.AreNotEqual(null, 3);
        }

        [Test]
        public void NullEqualsNull()
        {
            var expectedMessage =
                "  Expected: not equal to null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreNotEqual(null, null));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ArraysNotEqual()
        {
            Classic.Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 3, 2 });
        }

        [Test]
        public void ArraysNotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to < 1, 2, 3 >" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreNotEqual(new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 }));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void UInt()
        {
            uint u1 = 5;
            uint u2 = 8;
            Classic.Assert.AreNotEqual(u1, u2);
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

            Classic.Assert.AreNotEqual(23, b1);
            Classic.Assert.AreNotEqual(23, sb2);
            Classic.Assert.AreNotEqual(23, d4);
            Classic.Assert.AreNotEqual(23, d5);
            Classic.Assert.AreNotEqual(23, f6);
            Classic.Assert.AreNotEqual(23, i7);
            Classic.Assert.AreNotEqual(23, u8);
            Classic.Assert.AreNotEqual(23, l9);
            Classic.Assert.AreNotEqual(23, s10);
            Classic.Assert.AreNotEqual(23, us11);

            Classic.Assert.AreNotEqual(23, b12);
            Classic.Assert.AreNotEqual(23, sb13);
            Classic.Assert.AreNotEqual(23, d14);
            Classic.Assert.AreNotEqual(23, d15);
            Classic.Assert.AreNotEqual(23, s16);
            Classic.Assert.AreNotEqual(23, i17);
            Classic.Assert.AreNotEqual(23, ui18);
            Classic.Assert.AreNotEqual(23, i19);
            Classic.Assert.AreNotEqual(23, ui20);
            Classic.Assert.AreNotEqual(23, i21);
            Classic.Assert.AreNotEqual(23, i22);
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);
            var expected = new ThrowsIfToStringIsCalled(2);

            Classic.Assert.AreNotEqual(expected, actual);
        }
    }
}
