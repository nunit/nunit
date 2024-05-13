// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Internal
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

            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove((object)strings[0]);
            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(2));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove((object)strings[1]);
            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(1));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove((object)strings[2]);
            Assert.That(_collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveEntireCollection()
        {
            List<string> strings = new List<string>(_testStrings);

            _collectionTally.TryRemove(strings);
            Assert.That(_collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveNonExistingSingularElement()
        {
            _collectionTally.TryRemove((object)"notFound");

            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(_collectionTally.Result.ExtraItems, Has.Count.EqualTo(1));
            Assert.That(_collectionTally.Result.ExtraItems.Contains("notFound"), Is.True);
        }

        [Test]
        public void TestRemoveMultipleNonExistingElements()
        {
            List<string> nonExistingElems = new List<string>() { "notFound", "notFound2" };

            _collectionTally.TryRemove(nonExistingElems);

            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(_collectionTally.Result.ExtraItems, Has.Count.EqualTo(2));
            Assert.That(_collectionTally.Result.ExtraItems.Contains("notFound"), Is.True);
            Assert.That(_collectionTally.Result.ExtraItems.Contains("notFound2"), Is.True);
        }

        [Test]
        public void TestRemoveSomeExistingElements()
        {
            List<string> someExistingElems = new List<string>() { "one", "notFound2" };

            _collectionTally.TryRemove(someExistingElems);

            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(2));
            Assert.That(_collectionTally.Result.ExtraItems, Has.Count.EqualTo(1));
            Assert.That(_collectionTally.Result.ExtraItems.Contains("notFound2"), Is.True);
        }
    }
}
