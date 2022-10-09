// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
