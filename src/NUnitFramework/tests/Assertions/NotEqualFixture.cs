// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class NotEqualFixture
    {
        [Test]
        public void NotEqual()
        {
            Assert.AreNotEqual( 5, 3 );
        }

        [Test]
        public void NotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.AreNotEqual( 5, 5 ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NullNotEqualToNonNull()
        {
            Assert.AreNotEqual( null, 3 );
        }

        [Test]
        public void NullEqualsNull()
        {
            var expectedMessage =
                "  Expected: not equal to null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.AreNotEqual( null, null ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void ArraysNotEqual()
        {
            Assert.AreNotEqual( new object[] { 1, 2, 3 }, new object[] { 1, 3, 2 } );
        }

        [Test]
        public void ArraysNotEqualFails()
        {
            var expectedMessage =
                "  Expected: not equal to < 1, 2, 3 >" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.AreNotEqual( new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 } ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void UInt()
        {
            uint u1 = 5;
            uint u2 = 8;
            Assert.AreNotEqual( u1, u2 );
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

            System.Byte b12 = 35;
            System.SByte sb13 = 35;
            System.Decimal d14 = 35;
            System.Double d15 = 35;
            System.Single s16 = 35;
            System.Int32 i17 = 35;
            System.UInt32 ui18 = 35;
            System.Int64 i19 = 35;
            System.UInt64 ui20 = 35;
            System.Int16 i21 = 35;
            System.UInt16 i22 = 35;

            Assert.AreNotEqual(23, b1);
            Assert.AreNotEqual(23, sb2);
            Assert.AreNotEqual(23, d4);
            Assert.AreNotEqual(23, d5);
            Assert.AreNotEqual(23, f6);
            Assert.AreNotEqual(23, i7);
            Assert.AreNotEqual(23, u8);
            Assert.AreNotEqual(23, l9);
            Assert.AreNotEqual(23, s10);
            Assert.AreNotEqual(23, us11);

            Assert.AreNotEqual(23, b12);
            Assert.AreNotEqual(23, sb13);
            Assert.AreNotEqual(23, d14);
            Assert.AreNotEqual(23, d15);
            Assert.AreNotEqual(23, s16);
            Assert.AreNotEqual(23, i17);
            Assert.AreNotEqual(23, ui18);
            Assert.AreNotEqual(23, i19);
            Assert.AreNotEqual(23, ui20);
            Assert.AreNotEqual(23, i21);
            Assert.AreNotEqual(23, i22);
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);
            var expected = new ThrowsIfToStringIsCalled(2);

            Assert.AreNotEqual(expected, actual);
        }
    }
}
