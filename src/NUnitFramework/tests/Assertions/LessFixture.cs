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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class LessFixture
    {
        private readonly int i1 = 5;
        private readonly int i2 = 8;
        private readonly uint u1 = 12345678;
        private readonly uint u2 = 12345879;
        private readonly long l1 = 12345678;
        private readonly long l2 = 12345879;
        private readonly ulong ul1 = 12345678;
        private readonly ulong ul2 = 12345879;
        private readonly float f1 = 3.543F;
        private readonly float f2 = 8.543F;
        private readonly decimal de1 = 53.4M;
        private readonly decimal de2 = 83.4M;
        private readonly double d1 = 4.85948654;
        private readonly double d2 = 8.0;
        private readonly System.Enum e1 = RunState.Explicit;
        private readonly System.Enum e2 = RunState.Ignored;

        [Test]
        public void Less()
        {
            // Testing all forms after seeing some bugs. CFP
            Assert.Less(i1,i2);
            Assert.Less(i1,i2,"int");
            Assert.Less(i1,i2,"{0}","int");
            Assert.Less(u1,u2,"uint");
            Assert.Less(u1,u2,"{0}","uint");
            Assert.Less(l1,l2,"long");
            Assert.Less(l1,l2,"{0}","long");
            Assert.Less(ul1,ul2,"ulong");
            Assert.Less(ul1,ul2,"{0}","ulong");
            Assert.Less(d1,d2);
            Assert.Less(d1,d2, "double");
            Assert.Less(d1,d2, "{0}", "double");
            Assert.Less(de1,de2);
            Assert.Less(de1,de2, "decimal");
            Assert.Less(de1,de2, "{0}", "decimal");
            Assert.Less(f1,f2);
            Assert.Less(f1,f2, "float");
            Assert.Less(f1,f2, "{0}", "float");
        }

        [Test]
        public void MixedTypes()
        {	
            Assert.Less( 5, 8L, "int to long");
            Assert.Less( 5, 8.2f, "int to float" );
            Assert.Less( 5, 8.2d, "int to double" );
            Assert.Less( 5, 8U, "int to uint" );
            Assert.Less( 5, 8UL, "int to ulong" );
            Assert.Less( 5, 8M, "int to decimal" );

            Assert.Less( 5L, 8, "long to int");
            Assert.Less( 5L, 8.2f, "long to float" );
            Assert.Less( 5L, 8.2d, "long to double" );
            Assert.Less( 5L, 8U, "long to uint" );
            Assert.Less( 5L, 8UL, "long to ulong" );
            Assert.Less( 5L, 8M, "long to decimal" );

            Assert.Less( 3.5f, 5, "float to int" );
            Assert.Less( 3.5f, 8L, "float to long" );
            Assert.Less( 3.5f, 8.2d, "float to double" );
            Assert.Less( 3.5f, 8U, "float to uint" );
            Assert.Less( 3.5f, 8UL, "float to ulong" );
            Assert.Less( 3.5f, 8.2M, "float to decimal" );

            Assert.Less( 3.5d, 5, "double to int" );
            Assert.Less( 3.5d, 5L, "double to long" );
            Assert.Less( 3.5d, 8.2f, "double to float" );
            Assert.Less( 3.5d, 8U, "double to uint" );
            Assert.Less( 3.5d, 8UL, "double to ulong" );
            Assert.Less( 3.5d, 8.2M, "double to decimal" );
            

            Assert.Less( 5U, 8, "uint to int" );
            Assert.Less( 5U, 8L, "uint to long" );
            Assert.Less( 5U, 8.2f, "uint to float" );
            Assert.Less( 5U, 8.2d, "uint to double" );
            Assert.Less( 5U, 8UL, "uint to ulong" );
            Assert.Less( 5U, 8M, "uint to decimal" );
            
            Assert.Less( 5ul, 8, "ulong to int" );
            Assert.Less( 5UL, 8L, "ulong to long" );
            Assert.Less( 5UL, 8.2f, "ulong to float" );
            Assert.Less( 5UL, 8.2d, "ulong to double" );
            Assert.Less( 5UL, 8U, "ulong to uint" );
            Assert.Less( 5UL, 8M, "ulong to decimal" );
            
            Assert.Less( 5M, 8, "decimal to int" );
            Assert.Less( 5M, 8L, "decimal to long" );
            Assert.Less( 5M, 8.2f, "decimal to float" );
            Assert.Less( 5M, 8.2d, "decimal to double" );
            Assert.Less( 5M, 8U, "decimal to uint" );
            Assert.Less( 5M, 8UL, "decimal to ulong" );
        }

        [Test]
        public void NotLessWhenEqual()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  5" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Less(i1,i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotLess()
        {
            var expectedMessage =
                "  Expected: less than 5" + Environment.NewLine +
                "  But was:  8" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Less(i2,i1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NotLessIComparable()
        {
            var expectedMessage =
                "  Expected: less than Explicit" + Environment.NewLine +
                "  But was:  Ignored" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Less(e2,e1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void FailureMessage()
        {
            string msg = null;

            try
            {
                Assert.Less( 9, 4 );
            }
            catch( AssertionException ex )
            {
                msg = ex.Message;
            }

            StringAssert.Contains( TextMessageWriter.Pfx_Expected + "less than 4", msg );
            StringAssert.Contains( TextMessageWriter.Pfx_Actual + "9", msg );
        }
    }
}
