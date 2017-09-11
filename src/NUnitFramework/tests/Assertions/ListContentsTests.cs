// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
            Assert.Contains("abc", testArray);
            Assert.Contains(123, testArray);
            Assert.Contains("xyz", testArray, "expected array containing '{0}'", "xyz");
        }

        [Test]
        public void ArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains("def", testArray));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void EmptyArrayFails()
        {
            var expectedMessage =
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
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
                "  Expected: some item equal to \"def\"" + Environment.NewLine +
                "  But was:  < \"abc\", 123, \"xyz\" >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.Contains( "def", new SimpleObjectList( testArray ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DifferentTypesMayBeEqual()
        {
            Assert.Contains( 123.0, new SimpleObjectList( testArray ) );
        }

        [Test]
        public void DoesContainMultipleItemsString()
        {
            var collection = new[] { "test1", "test2", "test3" };
            Assert.That(collection, Does.Contain("test1").And.Contains("test2").And.Contains("test3"));
        }

        [Test]
        public void ContainsMultipleItemsString()
        {
            var collection = new[] { "test1", "test2", "test3" };
            Assert.That(collection, Contains.Item("test1").And.Contains("test2").And.Contains("test3"));
        }

        [Test]
        public void DoesContainMultipleItemsInt()
        {
            var collection = new[] { 1, 2, 3 };
            Assert.That(collection, Does.Contain(1).And.Contains(2).And.Contains(3));
        }

        [Test]
        public void DoesContainAnd()
        {
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Does.Contain(7).And.Member(2);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void DoesContainUsing()
        {
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Does.Contain(7).Using(myIntComparer);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ContainsMultipleItemsInt()
        {
            var collection = new[] { 1, 2, 3 };
            Assert.That(collection, Contains.Item(1).And.Contains(2).And.Contains(3));
        }

        [Test]
        public void ContainsItemAnd()
        {
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Contains.Item(7).And.Member(3);
            Assert.That(collection1, constraint);
        }

        [Test]
        public void ContainsItemUsing()
        {
            Func<int, int, bool> myIntComparer = (x, y) => x == y;
            var collection1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var constraint = Contains.Item(7).Using(myIntComparer);
            Assert.That(collection1, constraint);
        }
    }
}
