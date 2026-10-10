// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private static readonly Logger Log = InternalTrace.GetLogger("WorkShift");

        private readonly Lock _syncRoot = new();
        private int _busyCount = 0;
        private int _active = 0;

        /// <summary>
        /// Construct a WorkShift
        /// </summary>
        public WorkShift(string name)
        {
            Name = name;
        }

        #region Public Events and Properties

        /// <summary>
        /// Event that fires when the shift has ended
        /// </summary>
        public event ShiftChangeEventHandler? EndOfShift;

        /// <summary>
        /// The Name of this shift
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a flag indicating whether the shift is currently active
        /// </summary>
        public bool IsActive => _active == 1;

        /// <summary>
        /// Gets a bool indicating whether this shift has any work to do
        /// </summary>
        public bool HasWork
        {
            get
            {
                foreach (var q in Queues)
                {
                    if (!q.IsEmpty)
                        return true;
                }

                foreach (var w in Workers)
                {
                    if (w.IsBusy)
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets a list of the queues associated with this shift.
        /// </summary>
        /// <remarks>Internal for testing - immutable once initialized</remarks>
        internal List<WorkItemQueue> Queues { get; } = new();

        /// <summary>
        /// Gets the list of workers associated with this shift.
        /// </summary>
        /// <remarks>Internal for testing - immutable once initialized</remarks>
        internal List<TestWorker> Workers { get; } = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a WorkItemQueue to the shift, starting it if the
        /// shift is currently active.
        /// </summary>
        public void AddQueue(WorkItemQueue queue)
        {
            Log.Debug("{0} shift adding queue {1}", Name, queue.Name);

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
            Log.Debug("{0} shift assigned worker {1}", Name, worker.Name);

            Workers.Add(worker);
        }

        private bool _firstStart = true;

        /// <summary>
        /// Start or restart processing for the shift
        /// </summary>
        public void Start()
        {
            lock (_syncRoot)
            {
                if (Interlocked.Exchange(ref _active, 1) == 1)
                {
                    Log.Info("{0} shift already started", Name);
                    return;
                }

                Log.Info("{0} shift starting", Name);

                if (_firstStart)
                {
                    _firstStart = false;
                    StartWorkers();
                }

                foreach (var q in Queues)
                    q.Start();
            }
        }

        private void StartWorkers()
        {
            foreach (var worker in Workers)
            {
                worker.Busy += (_, _) => Interlocked.Increment(ref _busyCount);
                worker.Idle += (_, _) =>
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
            lock (_syncRoot)
            {
                if (Interlocked.Exchange(ref _active, 0) == 0)
                {
                    Log.Info("{0} shift already ended", Name);
                    return;
                }

                Log.Info("{0} shift ending", Name);

                // Pause all queues for this shift
                foreach (var q in Queues)
                    q.Pause();
            }

            // Signal the dispatcher that shift ended
            EndOfShift?.Invoke(this);
        }

        /// <summary>
        /// Shut down the shift.
        /// </summary>
        public void ShutDown()
        {
            if (Interlocked.Exchange(ref _active, 0) == 1)
            {
                Log.Info("{0} shutdown with active shift", Name);
            }

            foreach (var q in Queues)
                q.Stop();
        }

        /// <summary>
        /// Cancel the shift without completing all work
        /// </summary>
        public void Cancel()
        {
            foreach (var w in Workers)
                w.Cancel();
        }

        #endregion
    }
}
