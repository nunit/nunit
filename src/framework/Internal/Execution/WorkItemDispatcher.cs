// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

#if !NUNITLITE
using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// WorkItemDispatcher handles execution of work items by
    /// queuing them for worker threads to process.
    /// </summary>
    public class WorkItemDispatcher
    {
        static Logger log = InternalTrace.GetLogger("WorkItemDispatcher");

        private int _levelOfParallelism;
        private int _itemsDispatched;

        // List of shifts that have been created
        private List<WorkShift> _shifts = new List<WorkShift>();

        /// <summary>
        /// Construct a WorkItemDispatcher
        /// </summary>
        /// <param name="levelOfParallelism">Number of workers to use</param>
        public WorkItemDispatcher(int levelOfParallelism)
        {
            _levelOfParallelism = levelOfParallelism;
        }

        /// <summary>
        /// Start the dispatcher.
        /// </summary>
        public void Start()
        {
            StartNextShift();
        }

        /// <summary>
        /// Stop the dispatcher.
        /// </summary>
        public void Stop()
        {
            foreach (var shift in _shifts)
                shift.Stop();
        }

        /// <summary>
        /// Dispatch a single work item.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
            log.Debug("Enqueuing {0}", work.Test.Name);

            if (work.IsParallelizable)
                FirstShift.Enqueue(work, work.TargetApartment == ApartmentState.STA ? 1 : 0);
            else if (work.TargetApartment == ApartmentState.STA)
                ThirdShift.Enqueue(work);
            else
                SecondShift.Enqueue(work);

            ++_itemsDispatched;
        }

        #region Private Properties and Methods

        private WorkShift _firstShift;
        private WorkShift FirstShift
        {
            get
            {
                if (_firstShift == null)
                {
                    var queue1 = new WorkItemQueue("ReadyQueue");
                    var queue2 = new WorkItemQueue("STAQueue");
                    _firstShift = new WorkShift(queue1, queue2);

                    for (int i = 1; i <= _levelOfParallelism; i++)
                    {
                        string name = string.Format("Worker#" + i.ToString());
                        _firstShift.Assign(new TestWorker(queue1, name, ApartmentState.MTA));
                    }

                    _firstShift.Assign(new TestWorker(queue2, "Worker#STA", ApartmentState.STA));

                    _shifts.Add(_firstShift);
                    _firstShift.EndOfShift += OnEndOfShift;
                }

                return _firstShift;
            }
        }

        private WorkShift _secondShift;
        private WorkShift SecondShift
        {
            get
            {
                if (_secondShift == null)
                {
                    var queue = new WorkItemQueue("NP_Queue");
                    _secondShift = new WorkShift(queue);

                    _secondShift.Assign(new TestWorker(queue, "Worker#NP", ApartmentState.MTA));

                    _shifts.Add(_secondShift);
                    _secondShift.EndOfShift += OnEndOfShift;
                }

                return _secondShift;
            }
        }

        private WorkShift _thirdShift;
        private WorkShift ThirdShift
        {
            get
            {
                if (_thirdShift == null)
                {
                    var queue = new WorkItemQueue("NP_STA_Queue");
                    _thirdShift = new WorkShift(queue);

                    _thirdShift.Assign(new TestWorker(queue, "Worker#NP+STA", ApartmentState.STA));

                    _shifts.Add(_thirdShift);
                    _thirdShift.EndOfShift += OnEndOfShift;
                }

                return _thirdShift;
            }
        }

        private void OnEndOfShift(object sender, EventArgs ea)
        {
            StartNextShift();
        }

        private void StartNextShift()
        {
            foreach (var shift in _shifts)
            {
                if (shift.HasWork)
                {
                    shift.Start();
                    break;
                }
            }
        }

        #endregion

        /// <summary>
        /// The dispatcher needs to do different things at different, 
        /// non-overlapped times. For example, non-parallel tests may
        /// not be run at the same time as parallel tests. We model
        /// this using the metaphor of a working shift. The WorkShift
        /// class associates one or more WorkItemQueues with one or 
        /// more TestWorkers. Work in the queues is processed until
        /// all queues are empty and all workers are idle. At that
        /// point, the shift is over and another shift may begin.
        /// This cycle continues until all the tests have been run.
        /// </summary>
        private class WorkShift
        {
            private List<WorkItemQueue> _queues = new List<WorkItemQueue>();
            private List<TestWorker> _workers = new List<TestWorker>();

            private object _syncRoot = new object();
            private int _busyCount = 0;

            /// <summary>
            /// Event that fires when the shift has ended
            /// </summary>
            public event EventHandler EndOfShift;

            /// <summary>
            /// Construct a WorkShift object serving one or more queues.
            /// </summary>
            /// <param name="queues"></param>
            public WorkShift(params WorkItemQueue[] queues)
            {
                foreach (var q in queues)
                    _queues.Add(q);
            }

            /// <summary>
            /// Gets a bool indicating whether this shift has any work to do
            /// </summary>
            public bool HasWork
            {
                get
                {
                    foreach (var q in _queues)
                        if (!q.IsEmpty)
                            return true;

                    return false;
                }
            }

            /// <summary>
            /// Assign a worker to this shift. The worker is already
            /// associated with a queue, which must be one of those
            /// owned by the shift.
            /// </summary>
            /// <param name="worker"></param>
            public void Assign(TestWorker worker)
            {
                _workers.Add(worker);
                worker.Busy += (s, ea) => { Interlocked.Increment(ref _busyCount); };
                worker.Idle += (s, ea) =>
                {
                    // Quick check first
                    if (Interlocked.Decrement(ref _busyCount) == 0)
                        lock (_syncRoot)
                        {
                            // Double-check that all workers are idle
                            if (_busyCount > 0)
                                return;
                            // Check that the queues are empty
                            foreach (var q in _queues)
                                if (!q.IsEmpty)
                                    return;
                            // End the shift
                            this.Pause();
                            // Signal the dispatcher
                            if (EndOfShift != null)
                                EndOfShift(this, EventArgs.Empty);
                        }
                };

                worker.Start();
            }

            /// <summary>
            /// Add some work to this shift, using the first or only queue
            /// </summary>
            /// <param name="work">The WorkItem to perform</param>
            public void Enqueue(WorkItem work)
            {
                Enqueue(work, 0);
            }

            /// <summary>
            /// Add some work to this shift, specifying the queue index
            /// </summary>
            /// <param name="work">The WorkItem to perform</param>
            /// <param name="qNum">The index of the queue to use</param>
            public void Enqueue(WorkItem work, int qNum)
            {
                _queues[qNum].Enqueue(work);
            }

            /// <summary>
            /// Start or restart processing for the shift
            /// </summary>
            public void Start()
            {
                foreach (var q in _queues)
                    q.Start();
            }

            /// <summary>
            /// Stop processing for the shift completely - called at exit only
            /// </summary>
            public void Stop()
            {
                foreach (var q in _queues)
                    q.Stop();
            }

            /// <summary>
            /// Pause work of this queue, for possible later restart - called at end of shift.
            /// </summary>
            public void Pause()
            {
                foreach (var q in _queues)
                    q.Pause();
            }
        }
    }
}
#endif
