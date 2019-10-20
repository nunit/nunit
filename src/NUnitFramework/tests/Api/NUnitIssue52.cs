using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Api
{
    [TestFixture]
    public class NUnitIssue52
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void SelfContainedItemFoundInCollection<T>(T x, ICollection y)
        {
            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            var equality = equalityComparer.AreEqual(x, y, ref tolerance);

            Assert.IsFalse(equality);
            Assert.Contains(x, y);
            Assert.That(y, Contains.Item(x));
            Assert.That(y, Does.Contain(x));
        }

        [TestCaseSource(nameof(GetTestCases))]
        public void SelfContainedItemDoesntRecurseForever<T>(T x, ICollection y)
        {
            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            equalityComparer.ExternalComparers.Add(new DetectRecursionComparer(30));

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => equalityComparer.AreEqual(x, y, ref tolerance));

                var equality = equalityComparer.AreEqual(x, y, ref tolerance);
                Assert.IsFalse(equality);

                Assert.Contains(x, y);
                Assert.That(y, Contains.Item(x));
                Assert.That(y, Does.Contain(x));
            });
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            var enumerable = new SelfContainer();
            var enumerableContainer = new SelfContainer[] { new SelfContainer(), enumerable };

            yield return new TestCaseData(enumerable, enumerableContainer);

            object itemB = 1;
            object[] itemBSet = new object[2];
            itemBSet[0] = itemB;
            itemBSet[1] = itemBSet;

            yield return new TestCaseData(itemB, itemBSet);

            var dict = new Dictionary<object, object>();
            var dictItem = new Dictionary<object, object>();

            dict[dictItem] = dictItem;
            dictItem[dict] = dict;

            yield return new TestCaseData(dictItem, dict);
        }

        private class DetectRecursionComparer : EqualityAdapter
        {
            private readonly int maxRecursion;

            [MethodImpl(MethodImplOptions.NoInlining)]
            public DetectRecursionComparer(int maxRecursion)
            {
                var callerDepth = new StackTrace().FrameCount - 1;
                this.maxRecursion = callerDepth + maxRecursion;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public override bool CanCompare(object x, object y)
            {
                var currentDepth = new StackTrace().FrameCount - 1;
                return currentDepth >= maxRecursion;
            }

            public override bool AreEqual(object x, object y)
            {
                throw new InvalidOperationException("Recurses");
            }
        }

        private class SelfContainer : IEnumerable
        {
            public IEnumerator GetEnumerator() { yield return this; }
        }
    }
}
