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

        #region Helper Types

        private readonly record struct SamplePoint(int X, int Y);

        private sealed class CyclicList : List<CyclicList>
        {
        }
        #endregion
    }
}
