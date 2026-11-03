// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class EnumerablesComparerTests
    {
        [Test]
        public void HomogeneousIntCollectionsAreEqual()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3, 4, 5 };
            var actual = new List<int> { 1, 2, 3, 4, 5 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CollectionsRespectIgnoreCaseModifier()
        {
            // Arrange
            var expected = new[] { "apple", "banana", "cherry" };
            var actual = new[] { "APPLE", "BANANA", "CHERRY" };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected).IgnoreCase);
        }

        [Test]
        public void CollectionsWithDifferentElementsFail()
        {
            // Arrange
            var expected = new[] { 1, 2, 3 };
            var actual = new[] { 1, 2, 99 };

            // Act & Assert
            Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
        }

        [Test]
        public void HeterogeneousCollectionsAreEqual()
        {
            // Arrange
            var expected = new object[] { 1, "two", 3.0 };
            var actual = new object[] { 1, "two", 3.0 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CollectionsOfCustomStructsAreEqual()
        {
            // Arrange
            var expected = new List<SamplePoint> { new(10, 20), new(30, 40) };
            var actual = new List<SamplePoint> { new(10, 20), new(30, 40) };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsWithNullAtFirstPositionAreEqual()
        {
            // Arrange
            var expected = new int?[] { null, 2, 3 };
            var actual = new int?[] { null, 2, 3 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsWithNullInMiddleAreEqual()
        {
            // Arrange
            var expected = new string?[] { "alpha", null, "gamma" };
            var actual = new string?[] { "alpha", null, "gamma" };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsDifferingByNullFail()
        {
            // Arrange
            var expected = new int?[] { 1, null, 3 };
            var actual = new int?[] { 1, 2, 3 };

            // Act & Assert
            Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
        }

        [Test]
        public void NestedHomogeneousCollectionsAreEqual()
        {
            // Arrange
            var expected = new List<List<int>>
            {
                new() { 1, 2 },
                new() { 3, 4 }
            };
            var actual = new List<List<int>>
            {
                new() { 1, 2 },
                new() { 3, 4 }
            };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void EmptyHomogeneousCollectionsAreEqual()
        {
            // Arrange
            var expected = new List<int>();
            var actual = new List<int>();

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsWithDifferentLengthsFail()
        {
            // Arrange
            var expected = new[] { 1, 2, 3 };
            var actual = new[] { 1, 2 };

            // Act & Assert
            Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
        }

        [Test]
        public void HomogeneousCollectionsWithCustomUsingPredicateAreEqual()
        {
            // Arrange
            var expected = new[] { 10, 20, 30 };
            var actual = new[] { 11, 21, 31 };

            Assert.That(actual, Is.EqualTo(expected).Using<int>((x, y) => Math.Abs(x - y) <= 1));
        }

        [Test]
        public void CyclicHomogeneousCollectionsDoNotCauseStackOverflow()
        {
            var listA = new CyclicList();
            listA.Add(listA);

            var listB = new CyclicList();
            listB.Add(listB);

            Assert.That(listA, Is.Not.EqualTo(listB));
            Assert.That(listA, Is.EqualTo(listA));
        }

        [Test]
        public void MixedRuntimeTypesDoNotBypassSpecializedComparers()
        {
            // Arrange
            var expected = new List<IEnumerable>
            {
                new List<int> { 1, 2 },
                new int[1, 2] { { 1, 2 } }
            };

            var actual = new List<IEnumerable>
            {
                new List<int> { 1, 2 },
                new int[2, 1]
                {
                    { 1 },
                    { 2 }
                }
            };

            // Act & Assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public void CustomComparerInvokedEvenForIdenticalReferences()
        {
            // Arrange
            const string item = "same_instance";
            var expected = new[] { item };
            var actual = new[] { item };

            // Act & Assert
            Assert.That(actual, Is.Not.EqualTo(expected).Using<string>((x, y) => false));
        }

        [Test]
        public void HeterogeneousElementsUnderCommonGenericInterfaceFallBackSafely()
        {
            // Arrange
            var expected = new List<IComparable> { 1, "test" };
            var actual = new List<IComparable> { 1, "test" };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsRespectTolerance()
        {
            // Arrange
            var expected = new[] { 1.0, 2.0, 3.0 };
            var actual = new[] { 1.05, 1.95, 3.05 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected).Within(0.1));
            Assert.That(actual, Is.Not.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void HomogeneousCollectionsRespectToleranceAfterFirstElement()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 1, 2, 5 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected).Within(2));
            Assert.That(actual, Is.Not.EqualTo(expected).Within(1));
        }

        [Test]
        public void HeterogeneousCollectionsDetectDifferenceAfterTypeChange()
        {
            // Arrange
            var expected = new object[] { 1, "two", 3 };
            var actual = new object[] { 1, "two", 4 };

            // Act & Assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public void ToleranceWithCustomComparerOnElementsIsNotSupported()
        {
            // Arrange
            var expected = new[] { 1, 2 };
            var actual = new[] { 1, 2 };

            // Act & Assert
            Assert.That(
                () => Assert.That(actual, Is.EqualTo(expected).Using<int>((x, y) => x == y).Within(1)),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void CustomComparerIsInvokedForNullElements()
        {
            // Arrange
            var expected = new string?[] { null };
            var actual = new[] { "x" };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected).Using<string?>((a, b) => true));
        }

        [Test]
        public void HomogeneousCollectionsDifferingByNullInActualFail()
        {
            // Arrange
            var expected = new[] { "alpha", "beta" };
            var actual = new[] { "alpha", null };

            // Act & Assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public void ElementsOfDifferentTypesAtSamePositionAreComparedCorrectly()
        {
            // Arrange
            var expected = new object[] { 1, 2, 3 };
            var actual = new object[] { 1L, 2.0, 3 };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(new object[] { 1L, 2.0, 4 }, Is.Not.EqualTo(expected));
        }

        [Test]
        public void ElementsWithoutBuiltInComparerAreEqualWhenSameInstances()
        {
            // Arrange
            var first = new PlainItem(1);
            var second = new PlainItem(2);
            var expected = new List<PlainItem> { first, second };
            var actual = new List<PlainItem> { first, second };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ElementsWithoutBuiltInComparerAreNotEqualWhenDifferentInstances()
        {
            // Arrange
            var first = new PlainItem(1);
            var expected = new List<PlainItem> { first, new(2) };
            var actual = new List<PlainItem> { first, new(2) };

            // Act & Assert
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public void ToleranceOnElementsWithoutBuiltInComparerIsNotSupported()
        {
            // Arrange
            var expected = new List<PlainItem> { new(1), new(2) };
            var actual = new List<PlainItem> { new(1), new(2) };

            // Act & Assert
            Assert.That(
                () => Assert.That(actual, Is.EqualTo(expected).Within(1)),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void SameInstancesAreEqualEvenIfTheirEqualsReturnsFalse()
        {
            // Arrange
            var first = new NeverEqualItem();
            var second = new NeverEqualItem();
            var expected = new List<NeverEqualItem> { first, second };
            var actual = new List<NeverEqualItem> { first, second };

            // Act & Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        #region Helper Types

        private readonly record struct SamplePoint(int X, int Y);

        private sealed class NeverEqualItem
        {
            public override bool Equals(object? obj) => false;

            public override int GetHashCode() => 0;
        }

        private sealed class PlainItem
        {
            public PlainItem(int value) => Value = value;

            public int Value { get; }
        }

        private sealed class CyclicList : List<CyclicList>
        {
        }
        #endregion
    }
}
