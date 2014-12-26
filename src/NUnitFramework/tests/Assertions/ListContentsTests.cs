// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Summary description for ListContentsTests.
    /// </summary>
    [TestFixture]
    public class ListContentsTests
    {
        private static readonly object[] testArray = { "abc", 123, "xyz" };

        [Test]
        public void ArraySucceeds()
        {
            Assert.Contains( "abc", testArray );
            Assert.Contains( 123, testArray );
            Assert.Contains( "xyz", testArray );
        }

        [Test]
        public void ArrayFails()
        {
            var expectedMessage =
                "  Expected: collection containing \"def\"" + Environment.NewLine + 
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;	
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains("def", testArray));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EmptyArrayFails()
        {
            var expectedMessage =
                "  Expected: collection containing \"def\"" + Environment.NewLine + 
                "  But was:  <empty>" + Environment.NewLine;	
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains( "def", new object[0] ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void NullArrayIsError()
        {
            Assert.Throws<ArgumentException>(() => Assert.Contains( "def", null ));
        }

        [Test]
        public void ArrayListSucceeds()
        {
            var list = new SimpleObjectList( testArray );

            Assert.Contains( "abc", list );
            Assert.Contains( 123, list );
            Assert.Contains( "xyz", list );
        }

        [Test]
        public void ArrayListFails()
        {
            var expectedMessage =
                "  Expected: collection containing \"def\"" + Environment.NewLine + 
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains( "def", new SimpleObjectList( testArray ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DifferentTypesMayBeEqual()
        {
            Assert.Contains( 123.0, new SimpleObjectList( testArray ) );
        }
    }
}
