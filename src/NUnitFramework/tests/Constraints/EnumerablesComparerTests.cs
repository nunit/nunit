// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class EnumerablesComparerTests
    {
        [TestCaseSource(nameof(EqualCases))]
        public void CollectionsAreEqual(IEnumerable expected, IEnumerable actual)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(NotEqualCases))]
        public void CollectionsAreNotEqual(IEnumerable expected, IEnumerable actual)
        {
            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public void HomogeneousCollectionsWithCustomUsingPredicateAreEqual()
        {
            var expected = new[] { 10, 20, 30 };
            var actual = new[] { 11, 21, 31 };

            Assert.That(actual, Is.EqualTo(expected).Using<int>((x, y) => Math.Abs(x - y) <= 1));
        }

        [Test]
        public void HomogeneousCollectionsRespectTolerance()
        {
            var expected = new[] { 1.0, 2.0, 3.0 };
            var actual = new[] { 1.05, 1.95, 3.05 };

            Assert.That(actual, Is.EqualTo(expected).Within(0.1));
            Assert.That(actual, Is.Not.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void HomogeneousCollectionsRespectToleranceAfterFirstElement()
        {
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 1, 2, 5 };

            Assert.That(actual, Is.EqualTo(expected).Within(2));
            Assert.That(actual, Is.Not.EqualTo(expected).Within(1));
        }

        [Test]
        public void ToleranceOnElementsWithoutBuiltInComparerIsNotSupported()
        {
            var expected = new List<PlainItem> { new(1), new(2) };
            var actual = new List<PlainItem> { new(1), new(2) };

            Assert.That(
                () => Assert.That(actual, Is.EqualTo(expected).Within(1)),
                Throws.TypeOf<NotSupportedException>());
        }

        private static IEnumerable<TestCaseData> EqualCases()
        {
            yield return new TestCaseData(new List<int> { 1, 2, 3, 4, 5 }, new List<int> { 1, 2, 3, 4, 5 })
                .SetName("HomogeneousIntCollectionsAreEqual");
            yield return new TestCaseData(new object[] { 1, "two", 3.0 }, new object[] { 1, "two", 3.0 })
                .SetName("HeterogeneousCollectionsAreEqual");
            yield return new TestCaseData(new List<SamplePoint> { new(10, 20), new(30, 40) }, new List<SamplePoint> { new(10, 20), new(30, 40) })
                .SetName("CollectionsOfCustomStructsAreEqual");
            yield return new TestCaseData(new int?[] { null, 2, 3 }, new int?[] { null, 2, 3 })
                .SetName("HomogeneousCollectionsWithNullAtFirstPositionAreEqual");
            yield return new TestCaseData(new string?[] { "alpha", null, "gamma" }, new string?[] { "alpha", null, "gamma" })
                .SetName("HomogeneousCollectionsWithNullInMiddleAreEqual");
            yield return new TestCaseData(new List<List<int>> { new() { 1, 2 }, new() { 3, 4 } }, new List<List<int>> { new() { 1, 2 }, new() { 3, 4 } })
                .SetName("NestedHomogeneousCollectionsAreEqual");
            yield return new TestCaseData(new List<int>(), new List<int>())
                .SetName("EmptyHomogeneousCollectionsAreEqual");
            yield return new TestCaseData(new List<IComparable> { 1, "test" }, new List<IComparable> { 1, "test" })
                .SetName("HeterogeneousElementsUnderCommonGenericInterfaceFallBackSafely");
            yield return new TestCaseData(new object[] { 1, 2, 3 }, new object[] { 1L, 2.0, 3 })
                .SetName("ElementsOfDifferentTypesAtSamePositionAreComparedCorrectly");

            var first = new PlainItem(1);
            var second = new PlainItem(2);
            yield return new TestCaseData(new List<PlainItem> { first, second }, new List<PlainItem> { first, second })
                .SetName("ElementsWithoutBuiltInComparerAreEqualWhenSameInstances");

            var neverEqualFirst = new NeverEqualItem();
            var neverEqualSecond = new NeverEqualItem();
            yield return new TestCaseData(new List<NeverEqualItem> { neverEqualFirst, neverEqualSecond }, new List<NeverEqualItem> { neverEqualFirst, neverEqualSecond })
                .SetName("SameInstancesAreEqualEvenIfTheirEqualsReturnsFalse");

            var cyclicList = new CyclicList();
            cyclicList.Add(cyclicList);
            yield return new TestCaseData(cyclicList, cyclicList)
                .SetName("CyclicCollectionIsEqualToItself");
        }

        private static IEnumerable<TestCaseData> NotEqualCases()
        {
            yield return new TestCaseData(new[] { 1, 2, 3 }, new[] { 1, 2, 99 })
                .SetName("CollectionsWithDifferentElementsAreNotEqual");
            yield return new TestCaseData(new int?[] { 1, null, 3 }, new int?[] { 1, 2, 3 })
                .SetName("HomogeneousCollectionsDifferingByNullInExpectedAreNotEqual");
            yield return new TestCaseData(new[] { "alpha", "beta" }, new[] { "alpha", null })
                .SetName("HomogeneousCollectionsDifferingByNullInActualAreNotEqual");
            yield return new TestCaseData(new[] { 1, 2, 3 }, new[] { 1, 2 })
                .SetName("HomogeneousCollectionsWithDifferentLengthsAreNotEqual");
            var oneByTwo = new int[1, 2] { { 1, 2 } };
            var twoByOne = new int[2, 1]
            {
                { 1 },
                { 2 }
            };
            yield return new TestCaseData(new List<IEnumerable> { new List<int> { 1, 2 }, oneByTwo }, new List<IEnumerable> { new List<int> { 1, 2 }, twoByOne })
                .SetName("MixedRuntimeTypesDoNotBypassSpecializedComparers");
            yield return new TestCaseData(new object[] { 1, "two", 3 }, new object[] { 1, "two", 4 })
                .SetName("HeterogeneousCollectionsDetectDifferenceAfterTypeChange");
            yield return new TestCaseData(new object[] { 1, 2, 3 }, new object[] { 1L, 2.0, 4 })
                .SetName("ElementsOfDifferentTypesAtSamePositionDetectDifference");

            var first = new PlainItem(1);
            yield return new TestCaseData(new List<PlainItem> { first, new(2) }, new List<PlainItem> { first, new(2) })
                .SetName("ElementsWithoutBuiltInComparerAreNotEqualWhenDifferentInstances");

            var cyclicListA = new CyclicList();
            cyclicListA.Add(cyclicListA);
            var cyclicListB = new CyclicList();
            cyclicListB.Add(cyclicListB);
            yield return new TestCaseData(cyclicListA, cyclicListB)
                .SetName("CyclicCollectionsDoNotCauseStackOverflow");
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
