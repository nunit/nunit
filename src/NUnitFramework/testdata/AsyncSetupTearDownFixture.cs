#if NET_4_0 || NET_4_5 || PORTABLE
using System;
using System.Threading.Tasks;

#if NET_4_0
using Task = System.Threading.Tasks.TaskEx;
#endif

namespace NUnit.TestData
{
    public class AsyncSetupTearDownFixture
    {
        public int SuccessfulAsyncMethodRuns;
        public int FailingAsyncMethodRuns;

        public async System.Threading.Tasks.Task SuccessfulAsyncMethod()
        {
            await ReturnOne();
            SuccessfulAsyncMethodRuns++;
        }

        public async System.Threading.Tasks.Task FailingAsyncMethod()
        {
            FailingAsyncMethodRuns++;
            await ThrowException();
        }

        private static Task<int> ReturnOne()
        {
            return Task.Run(() => 1);
        }

        private static Task<int> ThrowException()
        {
            Func<int> throws = () => { throw new InvalidOperationException(); };
            return Task.Run(throws);
        }
    }
}
#endif