// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData
{
    [TestFixture]
    public class UnhandledExceptionFixture
    {
        private Task? _task;
        private bool _exceptionCaught = true;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _exceptionCaught = false;

#if NET10_0_OR_GREATER
            System.Runtime.ExceptionServices.ExceptionHandling.SetUnhandledExceptionHandler(CaughtIt);
#endif
            AppDomain.CurrentDomain.UnhandledException += CaughtIt;
        }

        [SetUp]
        public void Setup()
        {
            _task = null;
            _exceptionCaught = false;
        }

        [Test]
        public void TestExceptionThrownInSpawnedThread()
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(50);
                throw new InvalidOperationException("Raised from non-nunit Thread");
            });
            thread.Start();
        }

        [Test]
        [UnhandledExceptionHandling(UnhandledExceptionHandling.Ignore)]
        public void TestExceptionThrownInSpawnedThreadDirectedToBeIgnored() => TestExceptionThrownInSpawnedThread();

        [Test]
        public void TestExceptionThrownInTask()
        {
            TaskScheduler.UnobservedTaskException += CaughtIt;

            _task = Task.Run(async () =>
            {
                await Task.Delay(50);
                throw new InvalidOperationException("Raised from non-nunit Task");
            });
        }

        private void CaughtIt(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _exceptionCaught = true;
        }

        private void CaughtIt(object sender, UnhandledExceptionEventArgs e)
        {
            _exceptionCaught = true;
        }

        private bool CaughtIt(Exception exception)
        {
            _exceptionCaught = true;
            return true;
        }

        [Test]
        [UnhandledExceptionHandling(UnhandledExceptionHandling.Ignore)]
        public void TestExceptionThrownInTaskDirectedToBeIgnored() => TestExceptionThrownInTask();

        [TearDown]
        public async Task TearDown()
        {
            await Task.Delay(100);

            if (_task is not null)
            {
                await Task.Yield();

                // Ensure the task runs to completion using a backdoor,
                // as standard awaiting it would cause the exception to be observed and
                // thus not trigger the unobserved task exception behavior.
                ((IAsyncResult)_task).AsyncWaitHandle.WaitOne();
                Assume.That(_task.IsCompleted, Is.True);
                _task = null;
            }

            for (int i = 0; i < 10 && !_exceptionCaught; i++)
            {
                // Force finalizers to run, which should cause unobserved task exceptions to be raised.
                GC.Collect();
                GC.WaitForPendingFinalizers();

                await Task.Delay(10);
            }

            TaskScheduler.UnobservedTaskException -= CaughtIt;
            Assert.That(_exceptionCaught, Is.True, "Expected exception thrown was not caught");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            AppDomain.CurrentDomain.UnhandledException -= CaughtIt;
        }

#if THREAD_ABORT
        [Test]
        public void TestThreadAbort()
        {
            LimitRunningTime(LongRunningOperations);
        }

        [Test]
        [UnhandledExceptionHandling(UnhandledExceptionHandling.Ignore)]
        public void TestThreadAbortDirectedToBeIgnored() => TestThreadAbort();

        [Test]
        public void TestThreadAbortCaughtAndReset()
        {
            LimitRunningTime(() => AbortSafe(LongRunningOperations));
            // If we got here without an exception, the ThreadAbortException was successfully caught and reset.
            _exceptionCaught = true;
        }

        [Test]
        [UnhandledExceptionHandling(UnhandledExceptionHandling.Ignore)]
        public void TestThreadAbortCaughtAndResetDirectedToBeIgnored() => TestThreadAbortCaughtAndReset();

        private static void LimitRunningTime(Action action)
        {
            var thread = new Thread(_ => action());
            thread.Start();
            if (!thread.Join(500))
            {
                thread.Abort();
                thread.Join(); // ensure it’s finished before returning
            }
        }

        private static void AbortSafe(Action action)
        {
            try
            {
                action();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort(); // optional: prevents rethrow at end of catch/finally
            }
        }

        private static void LongRunningOperations() => Thread.Sleep(1000);
#endif
    }

    [UnhandledExceptionHandling(UnhandledExceptionHandling.Ignore)]
    public class IgnoringUnhandledExceptionFixture : UnhandledExceptionFixture
    {
    }

    [UnhandledExceptionHandling(UnhandledExceptionHandling.Error)]
    public class FailUnhandledExceptionFixture : UnhandledExceptionFixture
    {
    }
}
