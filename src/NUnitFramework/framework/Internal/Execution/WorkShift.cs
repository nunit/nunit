// ***********************************************************************
// Copyright (c) 2012-2014 Charlie Poole, Rob Prouse
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
    /// Handler for ShiftChange events.
    /// </summary>
    /// <param name="shift">The shift that is starting or ending.</param>
    public delegate void ShiftChangeEventHandler(WorkShift shift);

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
        private static readonly Logger log = InternalTrace.GetLogger("WorkShift");

        private readonly object _syncRoot = new object();
        private int _busyCount = 0;

        /// <summary>
        /// Construct a WorkShift
        /// </summary>
        public WorkShift(string name)
        {
            Name = name;
            IsActive = false;
        }

        #region Public Events and Properties

        /// <summary>
        /// Event that fires when the shift has ended
        /// </summary>
        public event ShiftChangeEventHandler EndOfShift;
        
        /// <summary>
        /// The Name of this shift
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a flag indicating whether the shift is currently active
        /// </summary>
        public bool IsActive { get; private set; }

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

        #region Internal Properties

        /// <summary>
        /// Gets a list of the queues associated with this shift.
        /// </summary>
        /// <remarks>Internal for testing - immutable once initialized</remarks>
        internal IList<WorkItemQueue> Queues { get; } = new List<WorkItemQueue>();

        /// <summary>
        /// Gets the list of workers associated with this shift.
        /// </summary>
        /// <remarks>Internal for testing - immutable once initialized</remarks>
        internal IList<TestWorker> Workers { get; } = new List<TestWorker>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a WorkItemQueue to the shift, starting it if the
        /// shift is currently active.
        /// </summary>
        public void AddQueue(WorkItemQueue queue)
        {
            log.Debug("{0} shift adding queue {1}", Name, queue.Name);

            Queues.Add(queue);

            if (IsActive)
                queue.Start();
        }

        /// <summary>
        /// Assign a worker to the shift.
        /// </summary>
        /// <param name="worker"></param>
        public void Assign(TestWorker worker)
        {
            log.Debug("{0} shift assigned worker {1}", Name, worker.Name);

            Workers.Add(worker);
        }

        bool _firstStart = true;

        /// <summary>
        /// Start or restart processing for the shift
        /// </summary>
        public void Start()
        {
            log.Info("{0} shift starting", Name);

            IsActive = true;

            if (_firstStart)
                StartWorkers();

            foreach (var q in Queues)
                q.Start();

            _firstStart = false;
        }
    
        private void StartWorkers()
        {
            foreach (var worker in Workers)
            {
                worker.Busy += (s, ea) => Interlocked.Increment(ref _busyCount);
                worker.Idle += (s, ea) =>
                {
                    // Quick check first using Interlocked.Decrement
                    if (Interlocked.Decrement(ref _busyCount) == 0 && !HasWork)
                    {
                        lock (_syncRoot)
                        {
                            // Check again under the lock. If there is no work
                            // we can end the shift.
                            if (_busyCount == 0 && !HasWork)
                            {
                                EndShift();
                            }
                        }
                    }
                };

                worker.Start();
            }
        }

        /// <summary>
        /// End the shift, pausing all queues and raising
        /// the EndOfShift event.
        /// </summary>
        public void EndShift()
        {
            log.Info("{0} shift ending", Name);

            IsActive = false;

            // Pause all queues for this shift
            foreach (var q in Queues)
                q.Pause();

            // Signal the dispatcher that shift ended
            EndOfShift?.Invoke(this);
        }

        /// <summary>
        /// Shut down the shift.
        /// </summary>
        public void ShutDown()
        {
            IsActive = false;

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
                IsActive = false;

            foreach (var w in Workers)
                w.Cancel(force);
        }

        #endregion
    }
}

#endif
