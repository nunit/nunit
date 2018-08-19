using System.Threading;
using NUnit.Framework;

namespace NUnit.TestData
{
    public static class SynchronizationContextFixture
    {
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
