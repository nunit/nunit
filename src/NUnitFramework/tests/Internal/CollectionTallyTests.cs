// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixtureSource(nameof(GetCollectionTallyTestCases))]
    internal class CollectionTallyTests
    {
        private static readonly List<string> TestStrings = new List<string> { "one", "two", "three" };

        private readonly Func<CollectionTally<string>> _generator;
        private CollectionTally<string> _collectionTally;

        private static IEnumerable<TestFixtureParameters> GetCollectionTallyTestCases()
        {
            yield return new TestFixtureParameters(() => new CollectionTally<string>(TestStrings, new NUnitEqualityComparer()))
            {
                ArgDisplayNames = ["DefaultEqualityComparer"]
            };

            var modifiedComparer = new NUnitEqualityComparer();
            modifiedComparer.IgnoreCase = true;
            yield return new TestFixtureParameters(() => new CollectionTally<string>(TestStrings, modifiedComparer))
            {
                ArgDisplayNames = ["NUnitEqualityComparer"]
            };
        }

        public CollectionTallyTests(Func<CollectionTally<string>> generator)
        {
            _generator = generator;
        }

        [SetUp]
        public void TestSetup()
        {
            _collectionTally = _generator();
        }

        [Test]
        public void TestSingularTryRemove()
        {
            List<string> strings = new List<string>(TestStrings);

            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove(strings[0]);
            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(2));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove(strings[1]);
            Assert.That(_collectionTally.Result.MissingItems, Has.Count.EqualTo(1));
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);

            _collectionTally.TryRemove(strings[2]);
            Assert.That(_collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveEntireCollection()
        {
            List<string> strings = new List<string>(TestStrings);

            _collectionTally.TryRemove(strings);
            Assert.That(_collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(_collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveNonExistingSingularElement()
        {
            _collectionTally.TryRemove("notFound");

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
