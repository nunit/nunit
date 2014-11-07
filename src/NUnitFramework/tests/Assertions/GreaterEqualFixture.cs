// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class GreaterEqualFixture
    {
        private readonly int i1 = 5;
        private readonly int i2 = 4;
        private readonly uint u1 = 12345879;
        private readonly uint u2 = 12345678;
        private readonly long l1 = 12345879;
        private readonly long l2 = 12345678;
        private readonly ulong ul1 = 12345879;
        private readonly ulong ul2 = 12345678;
        private readonly float f1 = 3.543F;
        private readonly float f2 = 2.543F;
        private readonly decimal de1 = 53.4M;
        private readonly decimal de2 = 33.4M;
        private readonly double d1 = 4.85948654;
        private readonly double d2 = 1.0;
        private readonly System.Enum e1 = RunState.Explicit;
        private readonly System.Enum e2 = RunState.Ignored;

        [Test]
        public void GreaterOrEqual_Int32()
        {
            Assert.GreaterOrEqual(i1, i1);           
            Assert.GreaterOrEqual(i1, i2);
        }

        [Test]
        public void GreaterOrEqual_UInt32()
        {
            Assert.GreaterOrEqual(u1, u1);
            Assert.GreaterOrEqual(u1, u2);
        }

        [Test]
        public void GreaterOrEqual_Long()
        {
            Assert.GreaterOrEqual(l1, l1);
            Assert.GreaterOrEqual(l1, l2);
        }

        [Test]
        public void GreaterOrEqual_ULong()
        {
            Assert.GreaterOrEqual(ul1, ul1);
            Assert.GreaterOrEqual(ul1, ul2);
        }

        [Test]
        public void GreaterOrEqual_Double()
        {
            Assert.GreaterOrEqual(d1, d1, "double");
            Assert.GreaterOrEqual(d1, d2, "double");
        }

        [Test]
        public void GreaterOrEqual_Decimal()
        {
            Assert.GreaterOrEqual(de1, de1, "{0}", "decimal");
            Assert.GreaterOrEqual(de1, de2, "{0}", "decimal");
        }

        [Test]
        public void GreaterOrEqual_Float()
        {
            Assert.GreaterOrEqual(f1, f1, "float");
            Assert.GreaterOrEqual(f1, f2, "float");
        }

        [Test]
        public void MixedTypes()
        {	
            Assert.GreaterOrEqual( 5, 3L, "int to long");
            Assert.GreaterOrEqual( 5, 3.5f, "int to float" );
            Assert.GreaterOrEqual( 5, 3.5d, "int to double" );
            Assert.GreaterOrEqual( 5, 3U, "int to uint" );
            Assert.GreaterOrEqual( 5, 3UL, "int to ulong" );
            Assert.GreaterOrEqual( 5, 3M, "int to decimal" );

            Assert.GreaterOrEqual( 5L, 3, "long to int");
            Assert.GreaterOrEqual( 5L, 3.5f, "long to float" );
            Assert.GreaterOrEqual( 5L, 3.5d, "long to double" );
            Assert.GreaterOrEqual( 5L, 3U, "long to uint" );
            Assert.GreaterOrEqual( 5L, 3UL, "long to ulong" );
            Assert.GreaterOrEqual( 5L, 3M, "long to decimal" );

            Assert.GreaterOrEqual( 8.2f, 5, "float to int" );
            Assert.GreaterOrEqual( 8.2f, 8L, "float to long" );
            Assert.GreaterOrEqual( 8.2f, 3.5d, "float to double" );
            Assert.GreaterOrEqual( 8.2f, 8U, "float to uint" );
            Assert.GreaterOrEqual( 8.2f, 8UL, "float to ulong" );
            Assert.GreaterOrEqual( 8.2f, 3.5M, "float to decimal" );

            Assert.GreaterOrEqual( 8.2d, 5, "double to int" );
            Assert.GreaterOrEqual( 8.2d, 5L, "double to long" );
            Assert.GreaterOrEqual( 8.2d, 3.5f, "double to float" );
            Assert.GreaterOrEqual( 8.2d, 8U, "double to uint" );
            Assert.GreaterOrEqual( 8.2d, 8UL, "double to ulong" );
            Assert.GreaterOrEqual( 8.2d, 3.5M, "double to decimal" );
            

            Assert.GreaterOrEqual( 5U, 3, "uint to int" );
            Assert.GreaterOrEqual( 5U, 3L, "uint to long" );
            Assert.GreaterOrEqual( 5U, 3.5f, "uint to float" );
            Assert.GreaterOrEqual( 5U, 3.5d, "uint to double" );
            Assert.GreaterOrEqual( 5U, 3UL, "uint to ulong" );
            Assert.GreaterOrEqual( 5U, 3M, "uint to decimal" );
            
            Assert.GreaterOrEqual( 5ul, 3, "ulong to int" );
            Assert.GreaterOrEqual( 5UL, 3L, "ulong to long" );
            Assert.GreaterOrEqual( 5UL, 3.5f, "ulong to float" );
            Assert.GreaterOrEqual( 5UL, 3.5d, "ulong to double" );
            Assert.GreaterOrEqual( 5UL, 3U, "ulong to uint" );
            Assert.GreaterOrEqual( 5UL, 3M, "ulong to decimal" );
            
            Assert.GreaterOrEqual( 5M, 3, "decimal to int" );
            Assert.GreaterOrEqual( 5M, 3L, "decimal to long" );
            Assert.GreaterOrEqual( 5M, 3.5f, "decimal to float" );
            Assert.GreaterOrEqual( 5M, 3.5d, "decimal to double" );
            Assert.GreaterOrEqual( 5M, 3U, "decimal to uint" );
            Assert.GreaterOrEqual( 5M, 3UL, "decimal to ulong" );
        }

        [Test]
        public void NotGreaterOrEqual()
        {
            var expectedMessage =
                "  Expected: greater than or equal to 5" + Environment.NewLine +
                "  But was:  4" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.GreaterOrEqual(i2, i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotGreaterEqualIComparable()
        {
            var expectedMessage =
                "  Expected: greater than or equal to Ignored" + Environment.NewLine +
                "  But was:  Explicit" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.GreaterOrEqual(e1, e2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            string msg = null;

            try
            {
                Assert.GreaterOrEqual(7, 99);
            }
            catch (AssertionException ex)
            {
                msg = ex.Message;
            }

            StringAssert.Contains( TextMessageWriter.Pfx_Expected + "greater than or equal to 99", msg);
            StringAssert.Contains( TextMessageWriter.Pfx_Actual + "7", msg);
        }
    }
}
