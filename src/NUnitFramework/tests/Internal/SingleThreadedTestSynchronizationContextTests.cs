// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal
{
    [Parallelizable(ParallelScope.Children)]
    public static class SingleThreadedTestSynchronizationContextTests
    {
        [Test]
        public static void RunWaitsWhenQueueIsEmptyUntilShutdown()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            {
                var thread = new Thread(context.Run);

                using (var queueRunning = new ManualResetEventSlim())
                {
                    context.Post(_ => queueRunning.Set(), null);
                    thread.Start();
                    Assert.That(queueRunning.Wait(1000), Is.True);
                }

                Assert.That(thread.Join(10), Is.False);

                context.ShutDown();

                Assert.That(thread.Join(1000), Is.True);
            }
        }

        [Test]
        public static void RunWaitsWhenQueueIsEmptyUntilPostAndExecutesPostedWork()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            {
                var thread = new Thread(context.Run);
                thread.Start();

                using (var workExecuted = new ManualResetEventSlim())
                {
                    Thread.Sleep(100);
                    context.Post(_ => workExecuted.Set(), null);

                    Assert.That(workExecuted.Wait(1000), Is.True);
                }
            }
        }

        [Test]
        public static void RunAfterShutdownThrowsInvalidOperationException()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            {
                context.ShutDown();

                Assert.That(context.Run, Throws.InvalidOperationException);
            }
        }

        [Test]
        public static void ReenteringRunThrowsInvalidOperationException()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            {
                context.Post(_ =>
                {
                    Assert.That(context.Run, Throws.InvalidOperationException);
                    context.ShutDown();
                }, null);

                context.Run();
            }
        }

        [Test]
        public static void RecursivelyPostedWorkIsStillExecutedIfStartedWithinTimeout()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = new CallbackWatcher();

                context.Post(state =>
                {
                    context.Post(_ => Thread.Sleep(TimeSpan.FromSeconds(0.5)), null);
                    context.Post(_ => wasExecuted.OnCallback(), null);

                    context.ShutDown();
                }, null);

                using (wasExecuted.ExpectCallback())
                    context.Run();
            }
        }

        [Test]
        public static void WorkInQueueAfterTimeoutIsDiscardedAndCausesRunToThrowAndError()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = new CallbackWatcher();

                context.Post(state =>
                {
                    context.Post(_ => Thread.Sleep(TimeSpan.FromSeconds(1.5)), null);
                    context.Post(_ => wasExecuted.OnCallback(), null);

                    context.ShutDown();
                }, null);

                TestResult testResult;
                Exception exception;

                using (wasExecuted.ExpectCallback(count: 0)) // Work is discarded
                using (new TestExecutionContext.IsolatedContext())
                {
                    try
                    {
                        context.Run();
                        exception = null;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }

                    testResult = TestExecutionContext.CurrentContext.CurrentResult;
                }

                Assert.That(exception, Is.InstanceOf<InvalidOperationException>()); // Run() throws

                Assert.That(testResult.WorstAssertionStatus, Is.EqualTo(AssertionStatus.Error)); // Run() errors
            }
        }

        [Test]
        public static void PostAfterTimeoutDiscardsWorkAndThrowsAndErrors()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = new CallbackWatcher();

                var testResult = (TestResult)null;
                var exception = (Exception)null;

                context.Post(state =>
                {
                    context.Post(_ =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1.5));

                        try
                        {
                            context.Post(__ => wasExecuted.OnCallback(), null);
                            exception = null;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }

                        testResult = TestExecutionContext.CurrentContext.CurrentResult;
                    }, null);

                    context.ShutDown();
                }, null);

                using (wasExecuted.ExpectCallback(count: 0)) // Work is discarded
                using (new TestExecutionContext.IsolatedContext())
                {
                    context.Run();
                }

                Assert.That(exception, Is.InstanceOf<InvalidOperationException>()); // Run() throws

                Assert.That(testResult.WorstAssertionStatus, Is.EqualTo(AssertionStatus.Error)); // Run() errors
            }
        }

        [Test]
        public static void RecursivelyPostedWorkIsStillExecutedWithinTimeout()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1)))
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = new CallbackWatcher();

                context.Post(_ =>
                {
                    ScheduleWorkRecursively(Stopwatch.StartNew(), until: TimeSpan.FromSeconds(0.5), wasExecuted: wasExecuted);

                    context.ShutDown();
                }, null);

                using (wasExecuted.ExpectCallback())
                    context.Run();
            }
        }

        private static void ScheduleWorkRecursively(Stopwatch stopwatch, TimeSpan until, CallbackWatcher wasExecuted)
        {
            if (stopwatch.Elapsed >= until)
            {
                wasExecuted.OnCallback();
                return;
            }

            SynchronizationContext.Current.Post(_ => ScheduleWorkRecursively(stopwatch, until, wasExecuted), null);
        }

        [Test]
        public static void Duplicate_shutdown_call_does_not_extend_shutdown_time()
        {
            using (var context = new SingleThreadedTestSynchronizationContext(shutdownTimeout: TimeSpan.FromSeconds(1.5)))
            using (TestUtils.TemporarySynchronizationContext(context))
            {
                var wasExecuted = new CallbackWatcher();

                context.Post(_ =>
                {
                    context.ShutDown();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    context.ShutDown();
                }, null);
                context.Post(_ => Thread.Sleep(TimeSpan.FromSeconds(1)), null);
                context.Post(_ => wasExecuted.OnCallback(), null);

                using (wasExecuted.ExpectCallback(count: 0))
                    Assert.That(context.Run, Throws.InvalidOperationException);
            }
        }
    }
}
