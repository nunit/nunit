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
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class CollectionTallyTests
    {
        private readonly List<string> _testStrings = new List<string> { "one", "two", "three" };

        private CollectionTally _collectionTally;

        [SetUp]
        public void TestSetup()
        {
            _collectionTally = new CollectionTally(new NUnitEqualityComparer(), _testStrings);
        }

        [Test]
        public void TestSingularTryRemove()
        {
            List<string> strings = new List<string>(_testStrings);

            Assert.AreEqual(3, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(0, _collectionTally.Result.ExtraItems.Count);

            _collectionTally.TryRemove((object)strings[0]);
            Assert.AreEqual(2, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(0, _collectionTally.Result.ExtraItems.Count);

            _collectionTally.TryRemove((object)strings[1]);
            Assert.AreEqual(1, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(0, _collectionTally.Result.ExtraItems.Count);

            _collectionTally.TryRemove((object)strings[2]);
            Assert.AreEqual(0, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(0, _collectionTally.Result.ExtraItems.Count);
        }

        [Test]
        public void TestRemoveEntireCollection()
        {
            List<string> strings = new List<string>(_testStrings);

            _collectionTally.TryRemove(strings);
            Assert.AreEqual(0, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(0, _collectionTally.Result.ExtraItems.Count);
        }

        [Test]
        public void TestRemoveNonExistingSingularElement()
        {
            _collectionTally.TryRemove((object)"notFound");

            Assert.AreEqual(3, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(1, _collectionTally.Result.ExtraItems.Count);
            Assert.IsTrue(_collectionTally.Result.ExtraItems.Contains("notFound"));
        }

        [Test]
        public void TestRemoveMultipleNonExistingElements()
        {
            List<string> nonExistingElems = new List<string>() { "notFound", "notFound2" };

            _collectionTally.TryRemove(nonExistingElems);

            Assert.AreEqual(3, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(2, _collectionTally.Result.ExtraItems.Count);
            Assert.IsTrue(_collectionTally.Result.ExtraItems.Contains("notFound"));
            Assert.IsTrue(_collectionTally.Result.ExtraItems.Contains("notFound2"));
        }

        [Test]
        public void TestRemoveSomeExistingElements()
        {
            List<string> someExistingElems = new List<string>() { "one", "notFound2" };

            _collectionTally.TryRemove(someExistingElems);

            Assert.AreEqual(2, _collectionTally.Result.MissingItems.Count);
            Assert.AreEqual(1, _collectionTally.Result.ExtraItems.Count);
            Assert.IsTrue(_collectionTally.Result.ExtraItems.Contains("notFound2"));
        }
    }
}
