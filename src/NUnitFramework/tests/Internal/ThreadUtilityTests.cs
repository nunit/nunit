// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if PARALLEL && PLATFORM_DETECTION
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
            using (var isThreadAboutToWait = new ManualResetEventSlim())
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

                isThreadAboutToWait.Wait();
                Thread.Sleep(1);

                if (kill)
                    ThreadUtility.Kill(thread, nativeId);
                else
                    ThreadUtility.Abort(thread, nativeId);

                // When not under CPU load, we expect the thread to take between 100 and 200 ms to abort.
                // However we get spurious failures in CI if we only wait 1000 ms.
                // Using AbortOrKillThreadWithMessagePump_StressTest (times: 1000, maxParallelism: 20),
                // I measured timer callbacks to be firing around ten times later than the 100 ms requested.
                // All this number does is manage how long we can tolerate waiting for a build if a bug
                // is introduced and the thread never aborts.
                const int waitTime = 15000;

                Assert.That(thread.Join(waitTime), "Native message pump was not able to be interrupted to enable a managed thread abort.");
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
