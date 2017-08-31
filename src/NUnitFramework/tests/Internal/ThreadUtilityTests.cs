#if !NETSTANDARD1_3 && !NETSTANDARD1_6

using System.Runtime.InteropServices;
using System.Threading;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class ThreadUtilityTests
    {
        [Platform("Win")]
        [TestCase(false, TestName = "Abort")]
        [TestCase(true, TestName = "Kill")]
        public void AbortOrKillThreadWithMessagePump(bool kill)
        {
            using (var isThreadAboutToWait = new ManualResetEvent(false))
            {
                var nativeId = 0;
                var thread = new Thread(() =>
                {
                    nativeId = ThreadUtility.GetCurrentThreadNativeId();
                    isThreadAboutToWait.Set();
                    while (true)
                        WaitMessage();
                });
                thread.Start();

                isThreadAboutToWait.WaitOne();
                Thread.Sleep(1);

                if (kill)
                    ThreadUtility.Kill(thread, nativeId);
                else
                    ThreadUtility.Abort(thread, nativeId);

                Assert.That(thread.Join(1000), "Native message pump was not able to be interrupted to enable a managed thread abort.");
            }
        }

        [Platform("Win")]
        [Test, Explicit("For diagnostic purposes; slow")]
        public void AbortOrKillThreadWithMessagePump_StressTest()
        {
            StressUtility.RunParallel(
                () => AbortOrKillThreadWithMessagePump(kill: true),
                times: 1000,
                maxParallelism: 20);
        }

        [DllImport("user32.dll")]
        private static extern bool WaitMessage();
    }
}

#endif
