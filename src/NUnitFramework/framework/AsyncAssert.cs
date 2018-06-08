#if ASYNC
using System.Threading.Tasks;

namespace NUnit.Framework
{
    public static class AsyncAssert
    {
        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">A TestDelegate to be executed in Multiple Assertion mode.</param>
#if NET40
        public static Task Multiple(AsyncTestDelegate testDelegate)
        {
            Guard.ArgumentNotNull(testDelegate, nameof(TestDelegate));

            var helper = AssertMultipleHelper.Start();

            return testDelegate.Invoke().ContinueWith(task =>
            {
                helper.Finally();
                task.ThrowAwaitExceptionOnFailure();
                helper.OnSuccess();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
#else
        public static async Task Multiple(AsyncTestDelegate testDelegate)
        {
            Guard.ArgumentNotNull(testDelegate, nameof(TestDelegate));

            var helper = AssertMultipleHelper.Start();
            try
            {
                await testDelegate.Invoke();
            }
            finally
            {
                helper.Finally();
            }

            helper.OnSuccess();
        }
#endif
    }
}
#endif
