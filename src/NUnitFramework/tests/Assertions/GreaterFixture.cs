// ***********************************************************************
// Copyright (c) 2005 Charlie Poole
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

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class GreaterFixture
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
        public void Greater()
        {
            Assert.Greater(i1,i2);
            Assert.Greater(u1,u2);
            Assert.Greater(l1,l2);
            Assert.Greater(ul1,ul2);
            Assert.Greater(d1,d2, "double");
            Assert.Greater(de1,de2, "{0}", "decimal");
            Assert.Greater(f1,f2, "float");
        }

        [Test]
        public void MixedTypes()
        {	
            Assert.Greater( 5, 3L, "int to long");
            Assert.Greater( 5, 3.5f, "int to float" );
            Assert.Greater( 5, 3.5d, "int to double" );
            Assert.Greater( 5, 3U, "int to uint" );
            Assert.Greater( 5, 3UL, "int to ulong" );
            Assert.Greater( 5, 3M, "int to decimal" );

            Assert.Greater( 5L, 3, "long to int");
            Assert.Greater( 5L, 3.5f, "long to float" );
            Assert.Greater( 5L, 3.5d, "long to double" );
            Assert.Greater( 5L, 3U, "long to uint" );
            Assert.Greater( 5L, 3UL, "long to ulong" );
            Assert.Greater( 5L, 3M, "long to decimal" );

            Assert.Greater( 8.2f, 5, "float to int" );
            Assert.Greater( 8.2f, 8L, "float to long" );
            Assert.Greater( 8.2f, 3.5d, "float to double" );
            Assert.Greater( 8.2f, 8U, "float to uint" );
            Assert.Greater( 8.2f, 8UL, "float to ulong" );
            Assert.Greater( 8.2f, 3.5M, "float to decimal" );

            Assert.Greater( 8.2d, 5, "double to int" );
            Assert.Greater( 8.2d, 5L, "double to long" );
            Assert.Greater( 8.2d, 3.5f, "double to float" );
            Assert.Greater( 8.2d, 8U, "double to uint" );
            Assert.Greater( 8.2d, 8UL, "double to ulong" );
            Assert.Greater( 8.2d, 3.5M, "double to decimal" );
            

            Assert.Greater( 5U, 3, "uint to int" );
            Assert.Greater( 5U, 3L, "uint to long" );
            Assert.Greater( 5U, 3.5f, "uint to float" );
            Assert.Greater( 5U, 3.5d, "uint to double" );
            Assert.Greater( 5U, 3UL, "uint to ulong" );
            Assert.Greater( 5U, 3M, "uint to decimal" );
            
            Assert.Greater( 5ul, 3, "ulong to int" );
            Assert.Greater( 5UL, 3L, "ulong to long" );
            Assert.Greater( 5UL, 3.5f, "ulong to float" );
            Assert.Greater( 5UL, 3.5d, "ulong to double" );
            Assert.Greater( 5UL, 3U, "ulong to uint" );
            Assert.Greater( 5UL, 3M, "ulong to decimal" );
            
            Assert.Greater( 5M, 3, "decimal to int" );
            Assert.Greater( 5M, 3L, "decimal to long" );
            Assert.Greater( 5M, 3.5f, "decimal to float" );
            Assert.Greater( 5M, 3.5d, "decimal to double" );
            Assert.Greater( 5M, 3U, "decimal to uint" );
            Assert.Greater( 5M, 3UL, "decimal to ulong" );
        }

        [Test]
        public void NotGreaterWhenEqual()
        {
            var expectedMessage =
                "  Expected: greater than 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Greater(i1,i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotGreater()
        {
            var expectedMessage =
                "  Expected: greater than 5" + Environment.NewLine +
                "  But was:  4" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Greater(i2,i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotGreaterIComparable()
        {
            var expectedMessage =
                "  Expected: greater than Ignored" + Environment.NewLine +
                "  But was:  Explicit" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Greater(e1,e2));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            var expectedMessage =
                "  Expected: greater than 99" + Environment.NewLine +
                "  But was:  7" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Greater( 7, 99 ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}

