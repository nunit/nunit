// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    internal sealed partial class SingleThreadedTestSynchronizationContext : SynchronizationContext, IDisposable
    {
        private const string ShutdownTimeoutMessage =
            "Work posted to the synchronization context did not complete within ten seconds. Consider explicitly waiting for the work to complete.";

        private readonly TimeSpan _shutdownTimeout;
        private readonly Queue<ScheduledWork> _queue = new();
        private Status _status;
        private Stopwatch? _timeSinceShutdown;

        public SingleThreadedTestSynchronizationContext(TimeSpan shutdownTimeout)
        {
            _shutdownTimeout = shutdownTimeout;
        }

        private enum Status
        {
            NotStarted,
            Running,
            ShuttingDown,
            ShutDown
        }

        /// <summary>
        /// May be called from any thread.
        /// </summary>
        public override void Post(SendOrPostCallback d, object? state)
        {
            Guard.ArgumentNotNull(d, nameof(d));

            AddWork(new ScheduledWork(d, state, finished: null));
        }

        /// <summary>
        /// May be called from any thread.
        /// </summary>
        public override void Send(SendOrPostCallback d, object? state)
        {
            Guard.ArgumentNotNull(d, nameof(d));

            if (SynchronizationContext.Current == this)
            {
                d.Invoke(state);
            }
            else
            {
                using (var finished = new ManualResetEventSlim())
                {
                    AddWork(new ScheduledWork(d, state, finished));
                    finished.Wait();
                }
            }
        }

        private void AddWork(ScheduledWork work)
        {
            lock (_queue)
            {
                switch (_status)
                {
                    case Status.ShuttingDown:
                        if (_timeSinceShutdown!.Elapsed < _shutdownTimeout)
                            break;
                        goto case Status.ShutDown;

                    case Status.ShutDown:
                        throw ErrorAndGetExceptionForShutdownTimeout();
                }

                _queue.Enqueue(work);
                Monitor.Pulse(_queue);
            }
        }

        /// <summary>
        /// May be called from any thread.
        /// </summary>
        public void ShutDown()
        {
            lock (_queue)
            {
                switch (_status)
                {
                    case Status.ShuttingDown:
                    case Status.ShutDown:
                        return;
                }

                _timeSinceShutdown = Stopwatch.StartNew();
                _status = Status.ShuttingDown;
                Monitor.Pulse(_queue);
            }
        }

        /// <summary>
        /// May be called from any thread, but may only be called once.
        /// </summary>
        public void Run()
        {
            lock (_queue)
            {
                switch (_status)
                {
                    case Status.Running:
                        throw new InvalidOperationException("SingleThreadedTestSynchronizationContext.Run may not be reentered.");

                    case Status.ShuttingDown:
                    case Status.ShutDown:
                        throw new InvalidOperationException("This SingleThreadedTestSynchronizationContext has been shut down.");
                }

                _status = Status.Running;
            }

            while (TryTake(out var scheduledWork))
                scheduledWork.Execute();
        }

        private bool TryTake(out ScheduledWork scheduledWork)
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                {
                    if (_status == Status.ShuttingDown)
                    {
                        _status = Status.ShutDown;
                        scheduledWork = default(ScheduledWork);
                        return false;
                    }

                    Monitor.Wait(_queue);
                }

                if (_status == Status.ShuttingDown && _timeSinceShutdown!.Elapsed > _shutdownTimeout)
                {
                    _status = Status.ShutDown;
                    throw ErrorAndGetExceptionForShutdownTimeout();
                }

                scheduledWork = _queue.Dequeue();
            }

            return true;
        }

        private static Exception ErrorAndGetExceptionForShutdownTimeout()
        {
            var testExecutionContext = TestExecutionContext.CurrentContext;

            testExecutionContext?.CurrentResult.RecordAssertion(AssertionStatus.Error, ShutdownTimeoutMessage);

            return new InvalidOperationException(ShutdownTimeoutMessage);
        }

        public void Dispose()
        {
            ShutDown();
        }
    }
}
