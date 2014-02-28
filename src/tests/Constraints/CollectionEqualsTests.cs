// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Collections;
using Env = NUnit.Env;

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

        public void CanMatchTwoLists()
        {
            //IList expected = new List<int>();
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
                "  Expected is <System.Int32[3]>, actual is <NUnit.TestUtilities.Collections.SimpleObjectCollection> with 3 elements" + Env.NewLine +
                "  Values differ at index [1]" + Env.NewLine +
                TextMessageWriter.Pfx_Expected + "2" + Env.NewLine +
                TextMessageWriter.Pfx_Actual + "5" + Env.NewLine));
        }

        [Test]
        [TestCaseSource( "IgnoreCaseData" )]
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
