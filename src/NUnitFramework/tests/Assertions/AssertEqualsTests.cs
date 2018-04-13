// ***********************************************************************
// Copyright (c) 2004 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using NUnit.TestUtilities;
using System;
using System.IO;

namespace NUnit.Framework.Assertions
{
	[TestFixture]
    public class AssertEqualsTests
    {
        [Test]
        public void Equals()
        {
            string nunitString = "Hello NUnit";
            string expected = nunitString;
            string actual = nunitString;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EqualsNull() 
        {
            Assert.AreEqual(null, null);
        }
        
        [Test]
        public void Bug575936Int32Int64Comparison()
        {
            long l64 = 0;
            int i32 = 0;
            Assert.AreEqual(i32, l64);
        }

        [Test]
        public void Bug524CharIntComparison()
        {
            char c = '\u0000';
            Assert.AreEqual(0, c);
        }

        [Test]
        public void Bug524CharIntWithoutOverload()
        {
            char c = '\u0000';
            Assert.That(c, Is.EqualTo(0));
        }

        [Test]
        public void CharCharComparison()
        {
            char c = 'a';
            Assert.That(c, Is.EqualTo('a'));
        }

        [Test]
        public void IntegerLongComparison()
        {
            Assert.AreEqual(1, 1L);
            Assert.AreEqual(1L, 1);
        }

        [Test]
        public void IntegerEquals()
        {
            int val = 42;
            Assert.AreEqual(val, 42);
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

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(expected, junitString));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        
        [Test]
        public void EqualsNaNFails() 
        {
            var expectedMessage =
                "  Expected: 1.234d +/- 0.0d" + Environment.NewLine +
                "  But was:  " + Double.NaN + Environment.NewLine;
            
            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(1.234, Double.NaN, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }    


        [Test]
        public void NanEqualsFails() 
        {
            var expectedMessage =
                "  Expected: " + Double.NaN + Environment.NewLine +
                "  But was:  1.234d" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(Double.NaN, 1.234, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }     
        
        [Test]
        public void NanEqualsNaNSucceeds() 
        {
            Assert.AreEqual(Double.NaN, Double.NaN, 0.0);
        }     

        [Test]
        public void NegInfinityEqualsInfinity() 
        {
            Assert.AreEqual(Double.NegativeInfinity, Double.NegativeInfinity, 0.0);
        }

        [Test]
        public void PosInfinityEqualsInfinity() 
        {
            Assert.AreEqual(Double.PositiveInfinity, Double.PositiveInfinity, 0.0);
        }
        
        [Test]
        public void PosInfinityNotEquals() 
        {
            var expectedMessage =
                "  Expected: " + Double.PositiveInfinity + Environment.NewLine +
                "  But was:  1.23d" + Environment.NewLine;
            
            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(Double.PositiveInfinity, 1.23, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void PosInfinityNotEqualsNegInfinity() 
        {
            var expectedMessage =
                "  Expected: " + Double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + Double.NegativeInfinity + Environment.NewLine;
            
            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(Double.PositiveInfinity, Double.NegativeInfinity, 0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]	
        public void SinglePosInfinityNotEqualsNegInfinity() 
        {
            var expectedMessage =
                "  Expected: " + Double.PositiveInfinity + Environment.NewLine +
                "  But was:  " + Double.NegativeInfinity + Environment.NewLine;
            
            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(float.PositiveInfinity, float.NegativeInfinity, (float)0.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EqualsThrowsException()
        {
            object o = new object();
            Assert.Throws<InvalidOperationException>(() => Assert.Equals(o, o));
        }

        [Test]
        public void ReferenceEqualsThrowsException()
        {
            object o = new object();
            Assert.Throws<InvalidOperationException>(() => Assert.ReferenceEquals(o, o));
        }
        
        [Test]
        public void Float() 
        {
            float val = (float)1.0;
            float expected = val;
            float actual = val;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual, (float)0.0);
        }

        [Test]
        public void Byte() 
        {
            byte val = 1;
            byte expected = val;
            byte actual = val;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void String() 
        {
            string s1 = "test";
            string s2 = new System.Text.StringBuilder(s1).ToString();

            Assert.IsTrue(s1.Equals(s2));
            Assert.AreEqual(s1,s2);
        }

        [Test]
        public void Short() 
        {
            short val = 1;
            short expected = val;
            short actual = val;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Int() 
        {
            int val = 1;
            int expected = val;
            int actual = val;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UInt() 
        {
            uint val = 1;
            uint expected = val;
            uint actual = val;

            Assert.IsTrue(expected == actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Decimal() 
        {
            decimal expected = 100m;
            decimal actual = 100.0m;
            int integer = 100;

            Assert.IsTrue( expected == actual );
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(expected == integer);
            Assert.AreEqual(expected, integer);
            Assert.IsTrue(actual == integer);
            Assert.AreEqual(actual, integer);
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
        
            System.Byte    b12  = 35;  
            System.SByte   sb13 = 35; 
            System.Decimal d14  = 35; 
            System.Double  d15  = 35; 
            System.Single  s16  = 35; 
            System.Int32   i17  = 35; 
            System.UInt32  ui18 = 35; 
            System.Int64   i19  = 35; 
            System.UInt64  ui20 = 35; 
            System.Int16   i21  = 35; 
            System.UInt16  i22  = 35;
            System.Char    c12 = '3';
            System.Char    c22 = 'a';

            Assert.AreEqual(35, b1);
            Assert.AreEqual(35, sb2);
            Assert.AreEqual(35, d4);
            Assert.AreEqual(35, d5);
            Assert.AreEqual(35, f6);
            Assert.AreEqual(35, i7);
            Assert.AreEqual(35, u8);
            Assert.AreEqual(35, l9);
            Assert.AreEqual(35, s10);
            Assert.AreEqual(35, us11);
            Assert.AreEqual('3', c1);
            Assert.AreEqual('a', c2);
        
            Assert.AreEqual( 35, b12  );
            Assert.AreEqual( 35, sb13 );
            Assert.AreEqual( 35, d14  );
            Assert.AreEqual( 35, d15  );
            Assert.AreEqual( 35, s16  );
            Assert.AreEqual( 35, i17  );
            Assert.AreEqual( 35, ui18 );
            Assert.AreEqual( 35, i19  );
            Assert.AreEqual( 35, ui20 );
            Assert.AreEqual( 35, i21  );
            Assert.AreEqual( 35, i22  );
            Assert.AreEqual('3', c12);
            Assert.AreEqual('a', c22);

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

            Assert.AreEqual(35, b23);
            Assert.AreEqual(35, sb24);
            Assert.AreEqual(35, d25);
            Assert.AreEqual(35, d26);
            Assert.AreEqual(35, f27);
            Assert.AreEqual(35, i28);
            Assert.AreEqual(35, u29);
            Assert.AreEqual(35, l30);
            Assert.AreEqual(35, s31);
            Assert.AreEqual(35, us32);
            Assert.AreEqual('3', c3);
            Assert.AreEqual('a', c4);
        }

        [Test]
        public void EnumsEqual()
        {
            MyEnum actual = MyEnum.a;
            Assert.AreEqual( MyEnum.a, actual );
        }

        [Test]
        public void EnumsNotEqual()
        {
            MyEnum actual = MyEnum.a;
            var expectedMessage =
                "  Expected: c" + Environment.NewLine +
                "  But was:  a" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual( MyEnum.c, actual ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DateTimeEqual()
        {
            DateTime dt1 = new DateTime( 2005, 6, 1, 7, 0, 0 );
            DateTime dt2 = new DateTime( 2005, 6, 1, 0, 0, 0 ) + TimeSpan.FromHours( 7.0 );
            Assert.AreEqual( dt1, dt2 );
        }

        [Test]
        public void DateTimeNotEqual_DifferenceInHours()
        {
            DateTime dt1 = new DateTime( 2005, 6, 1, 7, 0, 0 );
            DateTime dt2 = new DateTime( 2005, 6, 1, 0, 0, 0 );
            var expectedMessage =
                "  Expected: 2005-06-01 07:00:00" + Environment.NewLine +
                "  But was:  2005-06-01 00:00:00" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(dt1, dt2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        [Test]
        public void DateTimeNotEqual_DifferenceInTicks()
        {
            DateTime dt1 = new DateTime(1914, 06, 28, 12, 00, 00);
            DateTime dt2 = dt1.AddTicks(666);
            var expectedMessage =
                "  Expected: 1914-06-28 12:00:00" + Environment.NewLine +
                "  But was:  1914-06-28 12:00:00.0000666" + Environment.NewLine;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(dt1, dt2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DirectoryInfoEqual()
        {
            using (var testDir = new TestDirectory())
            {
                var one = new DirectoryInfo(testDir.Directory.FullName);
                var two = new DirectoryInfo(testDir.Directory.FullName);
                Assert.AreEqual(one, two);
            }
        }

        [Test]
        public void DirectoryInfoNotEqual()
        {
            using (var one = new TestDirectory())
            using (var two = new TestDirectory())
            {
                Assert.Throws<AssertionException>(() => Assert.AreEqual(one.Directory, two.Directory));
            }
        }

        private enum MyEnum
        {
            a, b, c
        }

        [Test]
        public void DoubleNotEqualMessageDisplaysAllDigits()
        {
            double d1 = 36.1;
            double d2 = 36.099999999999994;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(d1, d2) );

            var message = ex.Message;
            int i = message.IndexOf('3');
            int j = message.IndexOf( 'd', i );
            string expected = message.Substring( i, j - i + 1 );
            i = message.IndexOf( '3', j );
            j = message.IndexOf( 'd', i );
            string actual = message.Substring( i , j - i + 1 );

            Assert.AreNotEqual( expected, actual );
        }

        [Test]
        public void FloatNotEqualMessageDisplaysAllDigits()
        {
            float f1 = 36.125F;
            float f2 = 36.125004F;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(f1, f2));

            var message = ex.Message;
            int i = message.IndexOf( '3' );
            int j = message.IndexOf( 'f', i );
            string expected = message.Substring( i, j - i + 1 );
            i = message.IndexOf( '3', j );
            j = message.IndexOf( 'f', i );
            string actual = message.Substring( i, j - i + 1 );

            Assert.AreNotEqual( expected, actual );
        }

        [Test]
        public void DoubleNotEqualMessageDisplaysTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;
            double tol = 0.005;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(d1, d2, tol));

            Assert.That(ex.Message, Does.Contain("+/- 0.005"));
        }

        [Test]
        public void FloatNotEqualMessageDisplaysTolerance()
        {
            float f1 = 0.15F;
            float f2 = 0.12F;
            float tol = 0.001F;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual( f1, f2, tol ));

            Assert.That(ex.Message, Does.Contain( "+/- 0.001"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualMessageDisplaysDefaultTolerance()
        {
            double d1 = 0.15;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(d1, d2));
            Assert.That(ex.Message, Does.Contain("+/- 0.005"));
        }

        [Test, DefaultFloatingPointTolerance(0.005)]
        public void DoubleNotEqualWithNanDoesNotDisplayDefaultTolerance()
        {
            double d1 = double.NaN;
            double d2 = 0.12;

            var ex = Assert.Throws<AssertionException>(() => Assert.AreEqual(d1, d2));
            Assert.That(ex.Message.IndexOf("+/-") == -1);
        }

        [Test]
        public void IEquatableSuccess_OldSyntax()
        {
            IntEquatable a = new IntEquatable(1);

            Assert.AreEqual(1, a);
            Assert.AreEqual(a, 1);
        }

        [Test]
        public void IEquatableSuccess_ConstraintSyntax()
        {
            IntEquatable a = new IntEquatable(1);

            Assert.That(a, Is.EqualTo(1));
            Assert.That(1, Is.EqualTo(a));
        }

        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assert.Equals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("Assert.Equals should not be used for Assertions"));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assert.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("Assert.ReferenceEquals should not be used for Assertions"));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);
            var expected = new ThrowsIfToStringIsCalled(1);

            Assert.AreEqual(expected, actual);
        }

        class IntEquatable : IEquatable<int>
        {
            readonly int i;

            public IntEquatable(int i)
            {
                this.i = i;
            }

            public bool Equals(int other)
            {
                return i.Equals(other);
            }
        }
    }

    /// <summary>
    /// This class is for testing issue #1301 where ToString() is called on
    /// a class to create the description of the constraint even where that
    /// description is not used because the test passes.
    /// </summary>
    internal class ThrowsIfToStringIsCalled
    {
        readonly int _x;

        public ThrowsIfToStringIsCalled(int x)
        {
            _x = x;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as ThrowsIfToStringIsCalled;
            if (other == null)
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
            return base.ToString();
        }
    }
}

