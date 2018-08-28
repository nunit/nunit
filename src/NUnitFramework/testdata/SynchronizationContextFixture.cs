using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class SynchronizationContextFixture
    {
        public static IEnumerable<TestCaseData> TestCaseSourceThatSetsSynchronizationContext1()
        {
            if (SynchronizationContext.Current is DummySynchronizationContext)
            {
                return new[] { new TestCaseData(false) };
            }

            SynchronizationContext.SetSynchronizationContext(new DummySynchronizationContext());
            return new[] { new TestCaseData(true) };
        }

        public static IEnumerable<TestCaseData> TestCaseSourceThatSetsSynchronizationContext2()
        {
            return TestCaseSourceThatSetsSynchronizationContext1();
        }

        [TestCaseSource(nameof(TestCaseSourceThatSetsSynchronizationContext1))]
        public static void TestMethodWithSourceThatSetsSynchronizationContext1(bool succeed)
        {
            Assert.That(succeed);
        }

        [TestCaseSource(nameof(TestCaseSourceThatSetsSynchronizationContext2))]
        public static void TestMethodWithSourceThatSetsSynchronizationContext2(bool succeed)
        {
            Assert.That(succeed);
        }

        public static void TestCasesThatSetSynchronizationContext([Range(1, 2)] int testCase)
        {
            if (SynchronizationContext.Current is DummySynchronizationContext)
            {
                Assert.Fail();
            }

            SynchronizationContext.SetSynchronizationContext(new DummySynchronizationContext());
        }

        private sealed class DummySynchronizationContext : SynchronizationContext
        {
            public override SynchronizationContext CreateCopy()
            {
                return new DummySynchronizationContext();
            }
        }
    }
}
