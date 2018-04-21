// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using System;
using System.Collections.Generic;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal sealed partial class SingleThreadedTestSynchronizationContext : SynchronizationContext, IDisposable
    {
        private readonly Queue<ScheduledWork> _queue = new Queue<ScheduledWork>();
        private Status status;

        private enum Status
        {
            NotStarted,
            Running,
            ShutDown
        }

        /// <summary>
        /// May be called from any thread.
        /// </summary>
        public override void Post(SendOrPostCallback d, object state)
        {
            Guard.ArgumentNotNull(d, nameof(d));

            AddWork(new ScheduledWork(d, state, finished: null));
        }

        /// <summary>
        /// May be called from any thread.
        /// </summary>
        public override void Send(SendOrPostCallback d, object state)
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
                if (status == Status.ShutDown) throw CreateInvalidWhenShutDownException();
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
                status = Status.ShutDown;
                Monitor.Pulse(_queue);

                if (_queue.Count != 0)
                    throw new InvalidOperationException("Shutting down SingleThreadedTestSynchronizationContext with work still in the queue.");
            }
        }

        private static InvalidOperationException CreateInvalidWhenShutDownException()
        {
            return new InvalidOperationException("This SingleThreadedTestSynchronizationContext has been shut down.");
        }

        /// <summary>
        /// May be called from any thread, but may only be called once.
        /// </summary>
        public void Run()
        {
            lock (_queue)
            {
                switch (status)
                {
                    case Status.Running:
                        throw new InvalidOperationException("SingleThreadedTestSynchronizationContext.Run may not be reentered.");
                    case Status.ShutDown:
                        throw CreateInvalidWhenShutDownException();
                }

                status = Status.Running;
            }

            ScheduledWork scheduledWork;
            while (TryTake(out scheduledWork))
                scheduledWork.Execute();
        }

        private bool TryTake(out ScheduledWork scheduledWork)
        {
            lock (_queue)
            {
                for (;;)
                {
                    if (status == Status.ShutDown)
                    {
                        scheduledWork = default(ScheduledWork);
                        return false;
                    }

                    if (_queue.Count != 0) break;
                    Monitor.Wait(_queue);
                }

                scheduledWork = _queue.Dequeue();
            }

            return true;
        }

        public void Dispose()
        {
            ShutDown();
        }
    }
}
