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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    class CollectionEqualsTests
    {
        [Test]
        public void CanMatchTwoCollections()
        {
            ICollection expected = new SimpleObjectCollection(1, 2, 3);
            ICollection actual = new SimpleObjectCollection(1, 2, 3);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanMatchTwoLists()
        {
            IList expected = new List<int> { 1, 2, 3 };
            IList actual = new List<int> { 1, 2, 3 };

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanMatchAnArrayWithACollection()
        {
            ICollection collection = new SimpleObjectCollection(1, 2, 3);
            int[] array = new int[] { 1, 2, 3 };

            Assert.That(collection, Is.EqualTo(array));
            Assert.That(array, Is.EqualTo(collection));
        }

        [Test]
        public void FailureMatchingArrayAndCollection()
        {
            int[] expected = new int[] { 1, 2, 3 };
            ICollection actual = new SimpleObjectCollection(1, 5, 3);

            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex.Message, Is.EqualTo(
                "  Expected is <System.Int32[3]>, actual is <NUnit.TestUtilities.Collections.SimpleObjectCollection> with 3 elements" + Environment.NewLine +
                "  Values differ at index [1]" + Environment.NewLine +
                TextMessageWriter.Pfx_Expected + "2" + Environment.NewLine +
                TextMessageWriter.Pfx_Actual + "5" + Environment.NewLine));
        }

        [TestCaseSource( nameof(IgnoreCaseData) )]
        public void HonorsIgnoreCase( IEnumerable expected, IEnumerable actual )
        {
            Assert.That( expected, Is.EqualTo( actual ).IgnoreCase );
        }

        private static readonly object[] IgnoreCaseData =
        {
            new object[] {new SimpleObjectCollection("x", "y", "z"),new SimpleObjectCollection("x", "Y", "Z")},
            new object[] {new[] {'A', 'B', 'C'}, new object[] {'a', 'b', 'c'}},
            new object[] {new[] {"a", "b", "c"}, new object[] {"A", "B", "C"}},
            new object[] {new Dictionary<int, string> {{ 1, "a" }}, new Dictionary<int, string> {{ 1, "A" }}},
            new object[] {new Dictionary<int, char> {{ 1, 'A' }}, new Dictionary<int, char> {{ 1, 'a' }}},
            new object[] {new List<char> {'A', 'B', 'C'}, new List<char> {'a', 'b', 'c'}},
            new object[] {new List<string> {"a", "b", "c"}, new List<string> {"A", "B", "C"}},
        };
    }
}
