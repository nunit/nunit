using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

#pragma warning disable NUnit2046 // Use Constraint for better assertion messages in case of failure
namespace NUnit.Framework.Tests.Constraints
{
    /// <summary>
    /// A test fixture that ensures constraints cast their associated property,
    /// collection item, etc. to its actual type instead of just <see cref="object"/>.
    /// </summary>
    [TestFixture]
    public class ConstraintCastingTests
    {
        private class NonGenericEnumerable : IEnumerable
        {
            public IEnumerator GetEnumerator() => throw new NotImplementedException();
        }

        private class SimpleEnumerable : IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class NestedEnumerable : IReadOnlyList<int>
        {
            public int this[int index] => throw new NotImplementedException();
            public int Count => throw new NotImplementedException();
            public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class MultipleNestedEnumerable : IReadOnlyList<int>, IReadOnlyCollection<int>
        {
            public int this[int index] => throw new NotImplementedException();
            public int Count => throw new NotImplementedException();
            public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class MultipleDifferentNestedEnumerable : IReadOnlyList<int>, IReadOnlyCollection<string>
        {
            public int this[int index] => throw new NotImplementedException();
            public int Count => throw new NotImplementedException();
            public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
            IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Test]
        public void NonGenericEnumerableHasNoTypeArgument()
        {
            var actual = typeof(NonGenericEnumerable).FindPrimaryEnumerableInterfaceGenericTypeArgument();
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void SimpleEnumerableHasIntTypeArgument()
        {
            var actual = typeof(SimpleEnumerable).FindPrimaryEnumerableInterfaceGenericTypeArgument();
            Assert.That(actual, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void NestedEnumerableHasIntTypeArgument()
        {
            var actual = typeof(NestedEnumerable).FindPrimaryEnumerableInterfaceGenericTypeArgument();
            Assert.That(actual, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void MultipleNestedEnumerableHasIntTypeArgument()
        {
            var actual = typeof(MultipleNestedEnumerable).FindPrimaryEnumerableInterfaceGenericTypeArgument();
            Assert.That(actual, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void MultipleDifferentNestedEnumerableHasNoTypeArgument()
        {
            var actual = typeof(MultipleDifferentNestedEnumerable).FindPrimaryEnumerableInterfaceGenericTypeArgument();
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void LengthPropertyConstraint()
        {
            var defaultLength = string.Empty;
            var nonDefaultLength = "1";

            using (Assert.EnterMultipleScope())
            {
                Assert.That(defaultLength.Length, Is.Zero);
                Assert.That(defaultLength.Length, Is.Default);
                Assert.That(defaultLength, Has.Length.Default);
                Assert.That(defaultLength, Has.Property("Length").Default);

                Assert.That(nonDefaultLength.Length, Is.Not.Zero);
                Assert.That(nonDefaultLength.Length, Is.Not.Default);
                Assert.That(nonDefaultLength, Has.Length.Not.Default);
                Assert.That(nonDefaultLength, Has.Property("Length").Not.Default);
            }
        }

        [Test]
        public void CountPropertyConstraint()
        {
            var defaultCount = new List<int>();
            var nonDefaultCount = new List<int>() { 0 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(defaultCount.Count, Is.Zero);
                Assert.That(defaultCount.Count, Is.Default);
                Assert.That(defaultCount, Has.Count.Default);
                Assert.That(defaultCount, Has.Property("Count").Default);

                Assert.That(nonDefaultCount.Count, Is.Not.Zero);
                Assert.That(nonDefaultCount.Count, Is.Not.Default);
                Assert.That(nonDefaultCount, Has.Count.Not.Default);
                Assert.That(nonDefaultCount, Has.Property("Count").Not.Default);
            }
        }

        [Test]
        public void ExactConstraint()
        {
            var oneDefaultItems = new List<int>() { default };
            var noDefaultItems = new List<int>() { 1 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(oneDefaultItems.Single(), Is.Zero);
                Assert.That(oneDefaultItems.Single(), Is.Default);
                Assert.That(oneDefaultItems, Has.One.Default);
                Assert.That(oneDefaultItems, Has.Exactly(1).Default);

                Assert.That(noDefaultItems.Single(), Is.Not.Zero);
                Assert.That(noDefaultItems.Single(), Is.Not.Default);
                Assert.That(noDefaultItems, Has.One.Not.Default);
                Assert.That(noDefaultItems, Has.Exactly(1).Not.Default);
            }
        }

        [Test]
        public void NullableExactConstraint()
        {
            var oneDefaultItems = new List<int?>() { default };
            var noDefaultItems = new List<int?>() { 0 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(oneDefaultItems.Single(), Is.Null);
                Assert.That(oneDefaultItems.Single(), Is.Default);
                Assert.That(oneDefaultItems, Has.One.Default);
                Assert.That(oneDefaultItems, Has.Exactly(1).Default);

                Assert.That(noDefaultItems.Single(), Is.Not.Null);
                Assert.That(noDefaultItems.Single(), Is.Not.Default);
                Assert.That(noDefaultItems, Has.One.Not.Default);
                Assert.That(noDefaultItems, Has.Exactly(1).Not.Default);
            }
        }

        [Test]
        public void SomeItemsConstraint()
        {
            var someDefaultItems = new List<int>() { default, 1, 2, 3 };
            var noDefaultItems = new List<int>() { 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(someDefaultItems.Any(x => x == default), Is.True);
                Assert.That(someDefaultItems, Has.Some.Zero);
                Assert.That(someDefaultItems, Has.Some.Default);

                Assert.That(noDefaultItems.Any(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.Some.Not.Zero);
                Assert.That(noDefaultItems, Has.Some.Not.Default);
            }
        }

        [Test]
        public void NullableSomeItemsConstraint()
        {
            var someDefaultItems = new List<int?>() { default, 0, 1, 2, 3 };
            var noDefaultItems = new List<int?>() { 0, 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(someDefaultItems.Any(x => x == default), Is.True);
                Assert.That(someDefaultItems, Has.Some.Null);
                Assert.That(someDefaultItems, Has.Some.Default);

                Assert.That(noDefaultItems.Any(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.Some.Not.Null);
                Assert.That(noDefaultItems, Has.Some.Not.Default);
            }
        }

        [Test]
        public void AllItemsConstraint()
        {
            var allDefaultItems = new List<int>() { default, default, default };
            var noDefaultItems = new List<int>() { 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(allDefaultItems.All(x => x == default), Is.True);
                Assert.That(allDefaultItems, Has.All.Zero);
                Assert.That(allDefaultItems, Has.All.Default);

                Assert.That(noDefaultItems.All(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.All.Not.Zero);
                Assert.That(noDefaultItems, Has.All.Not.Default);
            }
        }

        [Test]
        public void NullableAllItemsConstraint()
        {
            var allDefaultItems = new List<int?>() { default, default, default };
            var noDefaultItems = new List<int?>() { 0, 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(allDefaultItems.All(x => x == default), Is.True);
                Assert.That(allDefaultItems, Has.All.Null);
                Assert.That(allDefaultItems, Has.All.Default);

                Assert.That(noDefaultItems.All(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.All.Not.Null);
                Assert.That(noDefaultItems, Has.All.Not.Default);
            }
        }

        [Test]
        public void NoItemConstraint()
        {
            var allDefaultItems = new List<int>() { default, default, default };
            var noDefaultItems = new List<int>() { 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(allDefaultItems.All(x => x == default), Is.True);
                Assert.That(allDefaultItems, Has.None.Not.Zero);
                Assert.That(allDefaultItems, Has.None.Not.Default);

                Assert.That(noDefaultItems.All(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.None.Zero);
                Assert.That(noDefaultItems, Has.None.Default);
            }
        }

        [Test]
        public void NullableNoItemConstraint()
        {
            var allDefaultItems = new List<int?>() { default, default, default };
            var noDefaultItems = new List<int?>() { 0, 1, 2, 3 };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(allDefaultItems.All(x => x == default), Is.True);
                Assert.That(allDefaultItems, Has.None.Not.Null);
                Assert.That(allDefaultItems, Has.None.Not.Default);

                Assert.That(noDefaultItems.All(x => x == default), Is.False);
                Assert.That(noDefaultItems, Has.None.Null);
                Assert.That(noDefaultItems, Has.None.Default);
            }
        }
    }
}
#pragma warning restore NUnit2046 // Use Constraint for better assertion messages in case of failure
