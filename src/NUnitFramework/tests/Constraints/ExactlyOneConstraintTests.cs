// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    public class ExactlyOneConstraintTests
    {
        private static readonly IEnumerable<string> TestCollectionLen0 = new List<string>();
        private static readonly IEnumerable<string> TestCollectionLen1 = new List<string> { "item" };
        private static readonly IEnumerable<string> TestCollectionLen2 = new List<string> { "item", "otherItem" };
        private static readonly IEnumerable<string> TestCollectionLen3 = new List<string> { "item", "item", "otherItem" };

        [Test]
        public void ExactlyOneItemInCollection()
        {
            Assert.That(TestCollectionLen1, Has.One.Items);
        }

        [Test]
        public void ExactlyOneItemMatches()
        {
            Assert.That(TestCollectionLen1, Has.One.EqualTo("item"));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatch()
        {
            Assert.That(TestCollectionLen1, Has.One.Not.EqualTo("notItem"));
        }
        [Test]
        public void ExactlyOneItemMustMatchButNoneInCollection()
        {
            Assert.That(Has.One.EqualTo("item").ApplyTo(TestCollectionLen0).IsSuccess, Is.False);
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsTwo()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(TestCollectionLen2, Has.One.Items));
        }

        [Test]
        public void ExactlyOneItemDoesNotMatchFromTwoElements()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(TestCollectionLen2, Has.One.EqualTo("notItem")));
        }

        [Test]
        public void ExactlyOneItemMustMatchButMultipleMatch()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(TestCollectionLen3, Has.One.EqualTo("item")));
        }

        [Test]
        public void ExactlyOneBlankConstraintWhereCollectionIsOne()
        {
            Assert.That(TestCollectionLen1, Has.One.Items);
        }

        [Test]
        public void ExactlyOneConstraintMatchingWhereCollectionIsTwo()
        {
            Assert.That(TestCollectionLen2, Has.One.EqualTo("item"));
        }

        [Test]
        public void ExactlyOneConstraintNotMatchingWhereCollectionIsTwo()
        {
           Assert.Throws<AssertionException>(() =>
                Assert.That(TestCollectionLen2, Has.One.EqualTo("blah")));
        }
    }
}
