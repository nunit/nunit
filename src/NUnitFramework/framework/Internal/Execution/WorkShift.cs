// ***********************************************************************
// Copyright (c) 2012-2014 Charlie Poole
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

#if PARALLEL
using System;
using System.Collections.Generic;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The dispatcher needs to do different things at different,
    /// non-overlapped times. For example, non-parallel tests may
    /// not be run at the same time as parallel tests. We model
    /// this using the metaphor of a working shift. The WorkShift
    /// class associates one or more WorkItemQueues with one or
    /// more TestWorkers.
    ///
    /// Work in the queues is processed until all queues are empty
    /// and all workers are idle. Both tests are needed because a
    /// worker that is busy may end up adding more work to one of
    /// the queues. At that point, the shift is over and another
    /// shift may begin. This cycle continues until all the tests
    /// have been run.
    /// </summary>
    public class WorkShift
    {
        private static Logger log = InternalTrace.GetLogger("WorkShift");

        private object _syncRoot = new object();
        private int _busyCount = 0;

        // Shift name - used for logging
        private string _name;

        /// <summary>
        /// Construct a WorkShift
        /// </summary>
        public WorkShift(string name)
        {
            _name = name;

            this.IsActive = false;
            this.Queues = new List<WorkItemQueue>();
            this.Workers = new List<TestWorker>();
        }

        #region Public Events and Properties

        /// <summary>
        /// Event that fires when the shift has ended
        /// </summary>
        public event EventHandler EndOfShift;

        /// <summary>
        /// Gets a flag indicating whether the shift is currently active
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets a list of the queues associated with this shift.
        /// </summary>
        /// <remarks>Used for testing</remarks>
        public IList<WorkItemQueue> Queues { get; private set; }

        /// <summary>
        /// Gets the list of workers associated with this shift.
        /// </summary>
        public IList<TestWorker> Workers { get; private set; }

        /// <summary>
        /// Gets a bool indicating whether this shift has any work to do
        /// </summary>
        public bool HasWork
        {
            get
            {
                foreach (var q in Queues)
                    if (!q.IsEmpty)
                        return true;

                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a WorkItemQueue to the shift, starting it if the
        /// shift is currently active.
        /// </summary>
        public void AddQueue(WorkItemQueue queue)
        {
            log.Debug("{0} shift adding queue {1}", _name, queue.Name);

            Queues.Add(queue);

            if (this.IsActive)
                queue.Start();
        }

        /// <summary>
        /// Assign a worker to the shift.
        /// </summary>
        /// <param name="worker"></param>
        public void Assign(TestWorker worker)
        {
            log.Debug("{0} shift assigned worker {1}", _name, worker.Name);

            Workers.Add(worker);

            worker.Busy += (s, ea) => Interlocked.Increment(ref _busyCount);
            worker.Idle += (s, ea) =>
            {
                // Quick check first using Interlocked.Decrement
                if (Interlocked.Decrement(ref _busyCount) == 0)
                    lock (_syncRoot)
                    {
                        // Check busy count again under the lock
                        if (_busyCount == 0 && !HasWork)
                            this.EndShift();
                    }
            };

            worker.Start();
        }

        /// <summary>
        /// Start or restart processing for the shift
        /// </summary>
        public void Start()
        {
            log.Info("{0} shift starting", _name);

            this.IsActive = true;

            foreach (var q in Queues)
                q.Start();
        }

        /// <summary>
        /// End the shift, pausing all queues and raising
        /// the EndOfShift event.
        /// </summary>
        public void EndShift()
        {
            log.Info("{0} shift ending", _name);

            this.IsActive = false;

            // Pause all queues
            foreach (var q in Queues)
                q.Pause();

            // Signal the dispatcher that shift ended
            if (EndOfShift != null)
                EndOfShift(this, EventArgs.Empty);
        }

        /// <summary>
        /// Shut down the shift.
        /// </summary>
        public void ShutDown()
        {
            this.IsActive = false;

            foreach (var q in Queues)
                q.Stop();
        }

        /// <summary>
        /// Cancel (abort or stop) the shift without completing all work
        /// </summary>
        /// <param name="force">true if the WorkShift should be aborted, false if it should allow its currently running tests to complete</param>
        public void Cancel(bool force)
        {
            if (force)
                this.IsActive = false;

            foreach (var w in Workers)
                w.Cancel(force);
        }

        #endregion
    }
}

#endif
