// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
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

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EqualsNull()
        {
            Classic.Assert.AreEqual(null, null);
        }

        [Test]
        public void Bug575936Int32Int64Comparison()
        {
            long l64 = 0;
            int i32 = 0;
            Classic.Assert.AreEqual(i32, l64);
        }

        [Test]
        public void Bug524CharIntComparison()
        {
            char c = '\u0000';
            Classic.Assert.AreEqual(0, c);
        }

        [Test]
        public void CharCharComparison()
        {
            char c = 'a';
            Classic.Assert.AreEqual('a', c);
        }

        [Test]
        public void IntegerLongComparison()
        {
            Classic.Assert.AreEqual(1, 1L);
            Classic.Assert.AreEqual(1L, 1);
        }

        [Test]
        public void IntegerEquals()
        {
            int val = 42;
            Classic.Assert.AreEqual(val, 42);
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

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(expected, junitString));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EqualsNaNFails()
        {
            var expectedMessage =
                "  Expected: 1.234d +/- 0.0d" + Environment.NewLine +
                "  But was:  " + double.NaN + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(1.234, double.NaN, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }


        [Test]
        public void NanEqualsFails()
        {
            var expectedMessage =
                "  Expected: " + double.NaN + Environment.NewLine +
                "  But was:  1.234d" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(double.NaN, 1.234, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NanEqualsNaNSucceeds()
        {
            Classic.Assert.AreEqual(double.NaN, double.NaN, 0.0);
        }

        [Test]
        public void NegInfinityEqualsInfinity()
        {
            Classic.Assert.AreEqual(double.NegativeInfinity, double.NegativeInfinity, 0.0);
        }

        [Test]
        public void PosInfinityEqualsInfinity()
        {
            Classic.Assert.AreEqual(double.PositiveInfinity, double.PositiveInfinity, 0.0);
        }

        [Test]
        public void PosInfinityNotEquals()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  1.23d" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(double.PositiveInfinity, 1.23, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void PosInfinityNotEqualsNegInfinity()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + double.NegativeInfinity + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(double.PositiveInfinity, double.NegativeInfinity, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void SinglePosInfinityNotEqualsNegInfinity()
        {
            var expectedMessage =
                "  Expected: " + double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + double.NegativeInfinity + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(float.PositiveInfinity, float.NegativeInfinity, (float)0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
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
        public void Float()
        {
            float val = (float)1.0;
            float expected = val;
            float actual = val;

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual, (float)0.0);
        }

        [Test]
        public void Byte()
        {
            byte val = 1;
            byte expected = val;
            byte actual = val;

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void String()
        {
            string s1 = "test";
            string s2 = new StringBuilder(s1).ToString();

            Classic.Assert.IsTrue(s1.Equals(s2));
            Classic.Assert.AreEqual(s1,s2);
        }

        [Test]
        public void Short()
        {
            short val = 1;
            short expected = val;
            short actual = val;

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Int()
        {
            int val = 1;
            int expected = val;
            int actual = val;

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UInt()
        {
            uint val = 1;
            uint expected = val;
            uint actual = val;

            Classic.Assert.IsTrue(expected == actual);
            Classic.Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Decimal()
        {
            decimal expected = 100m;
            decimal actual = 100.0m;
            int integer = 100;

            Classic.Assert.IsTrue( expected == actual );
            Classic.Assert.AreEqual(expected, actual);
            Classic.Assert.IsTrue(expected == integer);
            Classic.Assert.AreEqual(expected, integer);
            Classic.Assert.IsTrue(actual == integer);
            Classic.Assert.AreEqual(actual, integer);
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
            byte      b1 = 35;
            sbyte    sb2 = 35;
            decimal   d4 = 35;
            double    d5 = 35;
            float     f6 = 35;
            int       i7 = 35;
            uint      u8 = 35;
            long      l9 = 35;
            short    s10 = 35;
            ushort  us11 = 35;
            char      c1 = '3';
            char      c2 = 'a';

            byte b12  = 35;
            sbyte sb13 = 35;
            decimal d14  = 35;
            double d15  = 35;
            float s16  = 35;
            int i17  = 35;
            uint ui18 = 35;
            long i19  = 35;
            ulong ui20 = 35;
            short i21  = 35;
            ushort i22  = 35;
            char c12 = '3';
            char c22 = 'a';

            Classic.Assert.AreEqual(35, b1);
            Classic.Assert.AreEqual(35, sb2);
            Classic.Assert.AreEqual(35, d4);
            Classic.Assert.AreEqual(35, d5);
            Classic.Assert.AreEqual(35, f6);
            Classic.Assert.AreEqual(35, i7);
            Classic.Assert.AreEqual(35, u8);
            Classic.Assert.AreEqual(35, l9);
            Classic.Assert.AreEqual(35, s10);
            Classic.Assert.AreEqual(35, us11);
            Classic.Assert.AreEqual('3', c1);
            Classic.Assert.AreEqual('a', c2);

            Classic.Assert.AreEqual( 35, b12  );
            Classic.Assert.AreEqual( 35, sb13 );
            Classic.Assert.AreEqual( 35, d14  );
            Classic.Assert.AreEqual( 35, d15  );
            Classic.Assert.AreEqual( 35, s16  );
            Classic.Assert.AreEqual( 35, i17  );
            Classic.Assert.AreEqual( 35, ui18 );
            Classic.Assert.AreEqual( 35, i19  );
            Classic.Assert.AreEqual( 35, ui20 );
            Classic.Assert.AreEqual( 35, i21  );
            Classic.Assert.AreEqual( 35, i22  );
            Classic.Assert.AreEqual('3', c12);
            Classic.Assert.AreEqual('a', c22);

            byte? b23 = 35;
            sbyte? sb24 = 35;
            decimal? d25 = 35;
            double? d26 = 35;
            float? f27 = 35;
            int? i28 = 35;
            uint? u29 = 35;
            long? l30 = 35;
            short? s31 = 35;
            ushort? us32 = 35;
            char? c3 = '3';
            char? c4 = 'a';

            Classic.Assert.AreEqual(35, b23);
            Classic.Assert.AreEqual(35, sb24);
            Classic.Assert.AreEqual(35, d25);
            Classic.Assert.AreEqual(35, d26);
            Classic.Assert.AreEqual(35, f27);
            Classic.Assert.AreEqual(35, i28);
            Classic.Assert.AreEqual(35, u29);
            Classic.Assert.AreEqual(35, l30);
            Classic.Assert.AreEqual(35, s31);
            Classic.Assert.AreEqual(35, us32);
            Classic.Assert.AreEqual('3', c3);
            Classic.Assert.AreEqual('a', c4);
        }

        [Test]
        public void EnumsEqual()
        {
            MyEnum actual = MyEnum.A;
            Classic.Assert.AreEqual(MyEnum.A, actual);
        }

        [Test]
        public void EnumsNotEqual()
        {
            MyEnum actual = MyEnum.A;
            var expectedMessage =
                "  Expected: " + nameof(MyEnum.C) + Environment.NewLine +
                "  But was:  " + nameof(MyEnum.A) + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(MyEnum.C, actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DateTimeEqual()
        {
            var dt1 = new DateTime( 2005, 6, 1, 7, 0, 0 );
            var dt2 = new DateTime( 2005, 6, 1, 0, 0, 0 ) + TimeSpan.FromHours( 7.0 );
            Classic.Assert.AreEqual( dt1, dt2 );
        }

        [Test]
        public void DateTimeNotEqual_DifferenceInHours()
        {
            var dt1 = new DateTime( 2005, 6, 1, 7, 0, 0 );
            var dt2 = new DateTime( 2005, 6, 1, 0, 0, 0 );
            var expectedMessage =
                "  Expected: 2005-06-01 07:00:00" + Environment.NewLine +
                "  But was:  2005-06-01 00:00:00" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(dt1, dt2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DateTimeNotEqual_DifferenceInTicks()
        {
            var dt1 = new DateTime(1914, 06, 28, 12, 00, 00);
            var dt2 = dt1.AddTicks(666);
            var expectedMessage =
                "  Expected: 1914-06-28 12:00:00" + Environment.NewLine +
                "  But was:  1914-06-28 12:00:00.0000666" + Environment.NewLine;

            Assert.That(() => Classic.Assert.AreEqual(dt1, dt2),
                Throws.InstanceOf<AssertionException>().With.Message.EqualTo(expectedMessage));
        }

        [Test]
        public void DirectoryInfoEqual()
        {
            using (var testDir = new TestDirectory())
            {
                var one = new DirectoryInfo(testDir.Directory.FullName);
                var two = new DirectoryInfo(testDir.Directory.FullName);
                Classic.Assert.AreEqual(one, two);
            }
        }

        [Test]
        public void DirectoryInfoNotEqual()
        {
            using (var one = new TestDirectory())
            using (var two = new TestDirectory())
            {
                Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(one.Directory, two.Directory));
            }
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

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(d1, d2) );

            var message = ex.Message;
            int i = message.IndexOf('3');
            int j = message.IndexOf( 'd', i );
            string expected = message.Substring( i, j - i + 1 );
            i = message.IndexOf( '3', j );
            j = message.IndexOf( 'd', i );
            string actual = message.Substring( i , j - i + 1 );

            Classic.Assert.AreNotEqual( expected, actual );
        }

        [Test]
        public void FloatNotEqualMessageDisplaysAllDigits()
        {
            float f1 = 36.125F;
            float f2 = 36.125004F;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(f1, f2));

            var message = ex.Message;
            int i = message.IndexOf( '3' );
            int j = message.IndexOf( 'f', i );
            string expected = message.Substring( i, j - i + 1 );
            i = message.IndexOf( '3', j );
            j = message.IndexOf( 'f', i );
            string actual = message.Substring( i, j - i + 1 );

            Classic.Assert.AreNotEqual( expected, actual );
        }

        [Test]
        public void DoubleNotEqualMessageDisplaysTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;
            double tol = 0.005;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(d1, d2, tol));

            Assert.That(ex.Message, Does.Contain("+/- 0.005"));
        }

        [Test]
        public void FloatNotEqualMessageDisplaysTolerance()
        {
            float f1 = 0.15F;
            float f2 = 0.12F;
            float tol = 0.001F;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual( f1, f2, tol ));

            Assert.That(ex.Message, Does.Contain( "+/- 0.001"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualMessageDisplaysDefaultTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(d1, d2));
            Assert.That(ex.Message, Does.Contain("+/- 0.005"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualWithNanDoesNotDisplayDefaultTolerance()
        {
            double d1 = double.NaN;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => Classic.Assert.AreEqual(d1, d2));
            Assert.That(ex.Message.IndexOf("+/-") == -1);
        }

        [Test]
        public void IEquatableSuccess_OldSyntax()
        {
            var a = new IntEquatable(1);

            Classic.Assert.AreEqual(1, a);
            Classic.Assert.AreEqual(a, 1);
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
            var actual = new ThrowsIfToStringIsCalled(1);
            var expected = new ThrowsIfToStringIsCalled(1);

            Classic.Assert.AreEqual(expected, actual);
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

    /// <summary>
    /// This class is for testing issue #1301 where ToString() is called on
    /// a class to create the description of the constraint even where that
    /// description is not used because the test passes.
    /// </summary>
    internal sealed class ThrowsIfToStringIsCalled
    {
        private readonly int _x;

        public ThrowsIfToStringIsCalled(int x)
        {
            _x = x;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ThrowsIfToStringIsCalled other)
                return false;

            return _x == other._x;
        }

        public override int GetHashCode()
        {
            return _x;
        }

        public override string ToString()
        {
            Assert.Fail("Should not call ToString() if Assert does not fail");
            return base.ToString()!;
        }
    }
}
