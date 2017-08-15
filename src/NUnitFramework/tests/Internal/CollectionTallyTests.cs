using NUnit.Framework.Internal;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class CollectionTallyTests
    {
        private List<string> _testStrings = new List<string>() { "one", "two", "three" };

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
