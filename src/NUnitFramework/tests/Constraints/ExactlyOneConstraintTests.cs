// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public class ExactlyOneConstraintTests
    {
        private static readonly IEnumerable<string> testCollectionLen0 = new List<string>();
        private static readonly IEnumerable<string> testCollectionLen1 = new List<string> { "item" };
        private static readonly IEnumerable<string> testCollectionLen2 = new List<string> { "item", "otherItem" };
        private static readonly IEnumerable<string> testCollectionLen3 = new List<string> { "item", "item", "otherItem" };

        [Test]
        public void ExactlyOneItemInCollection()
        {
            Assert.That(testCollectionLen1, Has.One.Items);
        }

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(testCollectionLen1, Has.One.EqualTo("item"));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatch()
        {
            Assert.That(testCollectionLen1, Has.One.Not.EqualTo("notItem"));
        }
        [Test]
        public void ExactlyOneItemMustMatchButNoneInCollection()
        {
            Assert.IsFalse(Has.One.EqualTo("item").ApplyTo(testCollectionLen0).IsSuccess);
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsTwo()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One.Items));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatchFromTwoElements()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One.EqualTo("notItem")));
        }

        [Test]
        public void ExactlyOneItemMustMatchButMultipleMatch()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen3, Has.One.EqualTo("item")));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsOne()
        {
            Assert.That(testCollectionLen1, Has.One.Items);
        }

        [Test]
        public void ExactlyOneConstraintMatchingWhereCollectionIsTwo()
        {
            Assert.That(testCollectionLen2, Has.One.EqualTo("item"));
        }

        [Test]
        public void ExactlyOneConstraintNotMatchingWhereCollectionIsTwo()
        {
           Assert.Throws<AssertionException>(() =>
                Assert.That(testCollectionLen2, Has.One.EqualTo("blah")));
        }
    }
}
