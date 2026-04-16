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

        [Test]
        public void TestSingularTryRemove()
        {
            var collectionTally = _generator();
            List<string> strings = new List<string>(TestStrings);

            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(collectionTally.Result.ExtraItems, Is.Empty);

            collectionTally.TryRemove(strings[0]);
            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(2));
            Assert.That(collectionTally.Result.ExtraItems, Is.Empty);

            collectionTally.TryRemove(strings[1]);
            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(1));
            Assert.That(collectionTally.Result.ExtraItems, Is.Empty);

            collectionTally.TryRemove(strings[2]);
            Assert.That(collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveEntireCollection()
        {
            var collectionTally = _generator();
            List<string> strings = new List<string>(TestStrings);

            collectionTally.TryRemove(strings);
            Assert.That(collectionTally.Result.MissingItems, Is.Empty);
            Assert.That(collectionTally.Result.ExtraItems, Is.Empty);
        }

        [Test]
        public void TestRemoveNonExistingSingularElement()
        {
            var collectionTally = _generator();
            collectionTally.TryRemove("notFound");

            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(collectionTally.Result.ExtraItems, Has.Count.EqualTo(1));
            Assert.That(collectionTally.Result.ExtraItems.Contains("notFound"), Is.True);
        }

        [Test]
        public void TestRemoveMultipleNonExistingElements()
        {
            var collectionTally = _generator();
            List<string> nonExistingElems = new List<string>() { "notFound", "notFound2" };

            collectionTally.TryRemove(nonExistingElems);

            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(3));
            Assert.That(collectionTally.Result.ExtraItems, Has.Count.EqualTo(2));
            Assert.That(collectionTally.Result.ExtraItems.Contains("notFound"), Is.True);
            Assert.That(collectionTally.Result.ExtraItems.Contains("notFound2"), Is.True);
        }

        [Test]
        public void TestRemoveSomeExistingElements()
        {
            var collectionTally = _generator();
            List<string> someExistingElems = new List<string>() { "one", "notFound2" };

            collectionTally.TryRemove(someExistingElems);

            Assert.That(collectionTally.Result.MissingItems, Has.Count.EqualTo(2));
            Assert.That(collectionTally.Result.ExtraItems, Has.Count.EqualTo(1));
            Assert.That(collectionTally.Result.ExtraItems.Contains("notFound2"), Is.True);
        }
    }
}
