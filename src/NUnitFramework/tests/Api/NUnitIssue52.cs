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
            var actualResult = equalityComparer.AreEqual(x, y, ref tolerance);

            Assert.IsFalse(actualResult);
            Assert.Contains(x, y);
        }

        [TestCaseSource(nameof(GetTestCases))]
        public void SelfContainedItemDoesntRecurseForever<T>(T x, ICollection y)
        {
            var equalityComparer = new NUnitEqualityComparer();
            var tolerance = Tolerance.Default;
            equalityComparer.ExternalComparers.Add(new DetectRecursionComparer(30));

            Assert.DoesNotThrow(() =>
            {
                var equality = equalityComparer.AreEqual(x, y, ref tolerance);
                Assert.IsFalse(equality);
                Assert.Contains(x, y);
            });
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            var item = new SelfContainer();
            var items = new SelfContainer[] { new SelfContainer(), item };

            yield return new TestCaseData(item, items);
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
