// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class AssertEqualsTests
    {
        // Here we use real comparison to validate NUnit Assert.AreEquals behaviour matches.
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void Equals()
        {
            string nunitString = "Hello NUnit";
            string expected = nunitString;
            string actual = nunitString;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public void EqualsNull()
        {
            ClassicAssert.AreEqual(null, null);
        }

        [Test]
        public void Bug575936Int32Int64Comparison()
        {
            long l64 = 0;
            int i32 = 0;
            ClassicAssert.AreEqual(i32, l64);
        }

        [Test]
        public void Bug524CharIntComparison()
        {
            char c = '\u0000';
            ClassicAssert.AreEqual(0, c);
        }

        [Test]
        public void CharCharComparison()
        {
            char c = 'a';
            ClassicAssert.AreEqual('a', c);
        }

        [Test]
        public void IntegerLongComparison()
        {
            ClassicAssert.AreEqual(1, 1L);
            ClassicAssert.AreEqual(1L, 1);
        }

        [Test]
        public void IntegerEquals()
        {
            int val = 42;
            ClassicAssert.AreEqual(val, 42);
        }

        [Test]
        public void EqualsFail()
        {
            string junitString = "Goodbye JUnit";
            string expected = "Hello NUnit";

            var expectedMessage =
                "  Expected string length 11 but was 13. Strings differ at index 0." + Environment.NewLine +
                "  Expected: \"Hello NUnit\"" + Environment.NewLine +
                "  But was:  \"Goodbye JUnit\"" + Environment.NewLine +
                "  -----------^" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(expected, junitString));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void EqualsNaNFails()
        {
            var expectedMessage =
                "  Expected: 1.234d +/- 0.0d" + Environment.NewLine +
                "  But was:  " + double.NaN + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(1.234, double.NaN, 0.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NanEqualsFails()
        {
            var expectedMessage =
                "  Expected: " + double.NaN + Environment.NewLine +
                "  But was:  1.234d" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(double.NaN, 1.234, 0.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void NanEqualsNaNSucceeds()
        {
            ClassicAssert.AreEqual(double.NaN, double.NaN, 0.0);
        }

        [Test]
        public void NegInfinityEqualsInfinity()
        {
            ClassicAssert.AreEqual(double.NegativeInfinity, double.NegativeInfinity, 0.0);
        }

        [Test]
        public void PosInfinityEqualsInfinity()
        {
            ClassicAssert.AreEqual(double.PositiveInfinity, double.PositiveInfinity, 0.0);
        }

        [Test]
        public void PosInfinityNotEquals()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  1.23d" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(double.PositiveInfinity, 1.23, 0.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void PosInfinityNotEqualsNegInfinity()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + double.NegativeInfinity + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(double.PositiveInfinity, double.NegativeInfinity, 0.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void SinglePosInfinityNotEqualsNegInfinity()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + double.NegativeInfinity + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(float.PositiveInfinity, float.NegativeInfinity, (float)0.0));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void EqualsThrowsException()
        {
            var o = new object();
            Assert.Throws<InvalidOperationException>(() => Assert.Equals(o, o));
        }

        [Test]
        public void ReferenceEqualsThrowsException()
        {
            var o = new object();
            Assert.Throws<InvalidOperationException>(() => Assert.ReferenceEquals(o, o));
        }

        [Test]
        public void Double()
        {
            double val = (double)1.0;
            double expected = val;
            double actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual, (double)0.0, "double");
        }

        [Test]
        public void Float()
        {
            float val = (float)1.0;
            float expected = val;
            float actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual, (float)0.0);
        }

        [Test]
        public void Byte()
        {
            byte val = 1;
            byte expected = val;
            byte actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public void String()
        {
            string s1 = "test";
            string s2 = new StringBuilder(s1).ToString();

            ClassicAssert.IsTrue(s1.Equals(s2));
            ClassicAssert.AreEqual(s1, s2);
        }

        [Test]
        public void Short()
        {
            short val = 1;
            short expected = val;
            short actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Int()
        {
            int val = 1;
            int expected = val;
            int actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public void UInt()
        {
            uint val = 1;
            uint expected = val;
            uint actual = val;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Decimal()
        {
            decimal expected = 100m;
            decimal actual = 100.0m;
            int integer = 100;

            ClassicAssert.IsTrue(expected == actual);
            ClassicAssert.AreEqual(expected, actual);
            ClassicAssert.IsTrue(expected == integer);
            ClassicAssert.AreEqual(expected, integer);
            ClassicAssert.IsTrue(actual == integer);
            ClassicAssert.AreEqual(actual, integer);
        }

        /// <summary>
        /// Checks to see that a value comparison works with all types.
        /// Current version has problems when value is the same but the
        /// types are different...C# is not like Java, and doesn't automatically
        /// perform value type conversion to simplify this type of comparison.
        ///
        /// Related to Bug575936Int32Int64Comparison, but covers all numeric
        /// types.
        /// </summary>
        [Test]
        public void EqualsSameTypes()
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

            sbyte sb2 = 35;
            byte b2 = 35;
            short s2 = 35;
            ushort us2 = 35;
            int i2 = 35;
            uint u2 = 35;
            long l2 = 35;
            ulong ul2 = 35;
            decimal dec2 = 35;
            float f2 = 35;
            double d2 = 35;
            char c2 = '3';

            ClassicAssert.AreEqual(sb1, sb2);
            ClassicAssert.AreEqual(b1, b2);
            ClassicAssert.AreEqual(s1, s2);
            ClassicAssert.AreEqual(us1, us2);
            ClassicAssert.AreEqual(i1, i2);
            ClassicAssert.AreEqual(u1, u2);
            ClassicAssert.AreEqual(l1, l2);
            ClassicAssert.AreEqual(ul1, ul2);
            ClassicAssert.AreEqual(dec1, dec2);
            ClassicAssert.AreEqual(f1, f2);
            ClassicAssert.AreEqual(d1, d2);
            ClassicAssert.AreEqual(c1, c2);

            sbyte? sb3 = 35;
            byte? b3 = 35;
            short? s3 = 35;
            ushort? us3 = 35;
            int? i3 = 35;
            uint? u3 = 35;
            long? l3 = 35;
            ulong? ul3 = 35;
            decimal? dec3 = 35;
            float? f3 = 35;
            double? d3 = 35;
            char? c3 = '3';

            ClassicAssert.AreEqual(sb1, sb3);
            ClassicAssert.AreEqual(b1, b3);
            ClassicAssert.AreEqual(s1, s3);
            ClassicAssert.AreEqual(us1, us3);
            ClassicAssert.AreEqual(i1, i3);
            ClassicAssert.AreEqual(u1, u3);
            ClassicAssert.AreEqual(l1, l3);
            ClassicAssert.AreEqual(ul1, ul3);
            ClassicAssert.AreEqual(dec1, dec3);
            ClassicAssert.AreEqual(f1, f3);
            ClassicAssert.AreEqual(d1, d3);
            ClassicAssert.AreEqual(c1, c3);
        }

        [Test]
        public void EnumsEqual()
        {
            MyEnum actual = MyEnum.A;
            ClassicAssert.AreEqual(MyEnum.A, actual);
        }

        [Test]
        public void EnumsNotEqual()
        {
            MyEnum actual = MyEnum.A;
            var expectedMessage =
                "  Expected: " + nameof(MyEnum.C) + Environment.NewLine +
                "  But was:  " + nameof(MyEnum.A) + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(MyEnum.C, actual));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DateTimeEqual()
        {
            var dt1 = new DateTime(2005, 6, 1, 7, 0, 0);
            var dt2 = new DateTime(2005, 6, 1, 0, 0, 0) + TimeSpan.FromHours(7.0);
            ClassicAssert.AreEqual(dt1, dt2);
        }

        [Test]
        public void DateTimeNotEqual_DifferenceInHours()
        {
            var dt1 = new DateTime(2005, 6, 1, 7, 0, 0);
            var dt2 = new DateTime(2005, 6, 1, 0, 0, 0);
            var expectedMessage =
                "  Expected: 2005-06-01 07:00:00" + Environment.NewLine +
                "  But was:  2005-06-01 00:00:00" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(dt1, dt2));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void DateTimeNotEqual_DifferenceInTicks()
        {
            var dt1 = new DateTime(1914, 06, 28, 12, 00, 00);
            var dt2 = dt1.AddTicks(666);
            var expectedMessage =
                "  Expected: 1914-06-28 12:00:00" + Environment.NewLine +
                "  But was:  1914-06-28 12:00:00.0000666" + Environment.NewLine;

            Assert.That(() => ClassicAssert.AreEqual(dt1, dt2),
                Throws.InstanceOf<AssertionException>().With.Message.Contain(expectedMessage));
        }

        [Test]
        public void DirectoryInfoEqual()
        {
            using var testDir = new TestDirectory();
            var one = new DirectoryInfo(testDir.Directory.FullName);
            var two = new DirectoryInfo(testDir.Directory.FullName);
            ClassicAssert.AreEqual(one, two);
        }

        [Test]
        public void DirectoryInfoNotEqual()
        {
            using var one = new TestDirectory();
            using var two = new TestDirectory();
            Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(one.Directory, two.Directory));
        }

        private enum MyEnum
        {
            A, B, C
        }

        [Test]
        public void DoubleNotEqualMessageDisplaysAllDigits()
        {
            double d1 = 36.1;
            double d2 = 36.099999999999994;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(d1, d2));

            var message = ex?.Message;
            ClassicAssert.NotNull(message);
            int i = message!.IndexOf('3');
            int j = message.IndexOf('d', i);
            string expected = message.Substring(i, j - i + 1);
            i = message.IndexOf('3', j);
            j = message.IndexOf('d', i);
            string actual = message.Substring(i, j - i + 1);

            ClassicAssert.AreNotEqual(expected, actual);
        }

        [Test]
        public void FloatNotEqualMessageDisplaysAllDigits()
        {
            float f1 = 36.125F;
            float f2 = 36.125004F;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(f1, f2));

            var message = ex?.Message;
            ClassicAssert.NotNull(message);
            int i = message!.IndexOf('3');
            int j = message.IndexOf('f', i);
            string expected = message.Substring(i, j - i + 1);
            i = message.IndexOf('3', j);
            j = message.IndexOf('f', i);
            string actual = message.Substring(i, j - i + 1);

            ClassicAssert.AreNotEqual(expected, actual);
        }

        [Test]
        public void DoubleNotEqualMessageDisplaysTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;
            double tol = 0.005;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(d1, d2, tol));

            Assert.That(ex?.Message, Does.Contain("+/- 0.005"));
        }

        [Test]
        public void FloatNotEqualMessageDisplaysTolerance()
        {
            float f1 = 0.15F;
            float f2 = 0.12F;
            float tol = 0.001F;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(f1, f2, tol));

            Assert.That(ex?.Message, Does.Contain("+/- 0.001"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualMessageDisplaysDefaultTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(d1, d2));
            Assert.That(ex?.Message, Does.Contain("+/- 0.005"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualWithNanDoesNotDisplayDefaultTolerance()
        {
            double d1 = double.NaN;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => ClassicAssert.AreEqual(d1, d2));
            Assert.That(ex?.Message.IndexOf("+/-") == -1);
        }

        [Test]
        public void IEquatableSuccess_OldSyntax()
        {
            var a = new IntEquatable(1);

            ClassicAssert.AreEqual(1, a);
            ClassicAssert.AreEqual(a, 1);
        }

        [Test]
        public void IEquatableSuccess_ConstraintSyntax()
        {
            var a = new IntEquatable(1);

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.EqualTo(1));
                Assert.That(1, Is.EqualTo(a));
            });
        }

        [Test]
        public void EqualsFailsWhenUsed()
        {
            Assert.That(() => Assert.Equals(string.Empty, string.Empty),
                Throws.InvalidOperationException.With.Message.StartWith("Assert.Equals should not be used."));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            Assert.That(() => Assert.ReferenceEquals(string.Empty, string.Empty),
                Throws.InvalidOperationException.With.Message.StartWith("Assert.ReferenceEquals should not be used."));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new NUnit.Framework.Tests.Assertions.ThrowsIfToStringIsCalled(1);
            var expected = new NUnit.Framework.Tests.Assertions.ThrowsIfToStringIsCalled(1);

            ClassicAssert.AreEqual(expected, actual);
        }

#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        private sealed class IntEquatable : IEquatable<int>
        {
            private readonly int _i;

            public IntEquatable(int i)
            {
                _i = i;
            }

            public bool Equals(int other)
            {
                return _i.Equals(other);
            }
        }
    }
}
