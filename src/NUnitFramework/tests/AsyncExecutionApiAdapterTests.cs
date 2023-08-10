// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;

namespace NUnit.Framework.Tests
{
    // Make sure other tests are testing what we think they’re testing
    public static class AsyncExecutionApiAdapterTests
    {
        [TestCaseSource(typeof(AsyncExecutionApiAdapter), nameof(AsyncExecutionApiAdapter.All))]
        public static void ExecutesAsyncUserCode(AsyncExecutionApiAdapter adapter)
        {
            var didExecute = false;

            adapter.Execute(async () =>
            {
                await Task.Yield();
                didExecute = true;
            });

            Assert.That(didExecute);
        }
    }
}
