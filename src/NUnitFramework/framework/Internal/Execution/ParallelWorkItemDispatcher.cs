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
    /// ParallelWorkItemDispatcher handles execution of work items by
    /// queuing them for worker threads to process.
    /// </summary>
    public class ParallelWorkItemDispatcher : IWorkItemDispatcher
    {
        private static Logger log = InternalTrace.GetLogger("WorkItemDispatcher");

        private int _levelOfParallelism;
        private int _itemsDispatched;

        // Non-STA
        private WorkShift _parallelShift = new WorkShift("Parallel");
        private WorkShift _nonParallelShift = new WorkShift("NonParallel");
        private WorkItemQueue _parallelQueue;
        private WorkItemQueue _nonParallelQueue;

        // STA
#if !NETCF
        private WorkShift _nonParallelSTAShift = new WorkShift("NonParallelSTA");
        private WorkItemQueue _parallelSTAQueue;
        private WorkItemQueue _nonParallelSTAQueue;
#endif

        // The first WorkItem to be dispatched, assumed to be top-level item
        private WorkItem _topLevelWorkItem;

        /// <summary>
        /// Construct a ParallelWorkItemDispatcher
        /// </summary>
        /// <param name="levelOfParallelism">Number of workers to use</param>
        public ParallelWorkItemDispatcher(int levelOfParallelism)
        {
            _levelOfParallelism = levelOfParallelism;

            Shifts = new WorkShift[]
            {
                _parallelShift,
                _nonParallelShift,
#if !NETCF
                _nonParallelSTAShift
#endif
            };
            foreach (var shift in Shifts)
                shift.EndOfShift += OnEndOfShift;
        }

        /// <summary>
        /// Enumerates all the shifts supported by the dispatcher
        /// </summary>
        public IEnumerable<WorkShift> Shifts { get; private set; }

        #region IWorkItemDispatcher Members

        /// <summary>
        /// Dispatch a single work item for execution. The first
        /// work item dispatched is saved as the top-level
        /// work item and used when stopping the run.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
            // Special handling of the top-level item
            if (_topLevelWorkItem == null)
            {
                _topLevelWorkItem = work;
                Enqueue(work);
                StartNextShift();
            }
            // We run child items directly, rather than enqueuing them...
            // 1. If the context is single threaded.
            // 2. If there is no fixture, and so nothing to do but dispatch grandchildren.
            // 3. For now, if this represents a test case. This avoids issues of
            // tests that access the fixture state and allows handling ApartmentState
            // preferences set on the fixture.
            else if (work.Context.IsSingleThreaded
                  || work.Test.TypeInfo == null
                  || work is SimpleWorkItem)
                Execute(work);
            else
                Enqueue(work);

            ++_itemsDispatched;
        }

        private void Execute(WorkItem work)
        {
            log.Debug("Directly executing {0}", work.Test.Name);
            work.Execute();
        }

        private void Enqueue(WorkItem work)
        {
            log.Debug("Enqueuing {0}", work.Test.Name);

            if (work.IsParallelizable)
            {
#if !NETCF
                if (work.TargetApartment == ApartmentState.STA)
                    ParallelSTAQueue.Enqueue(work);
                else
#endif
                ParallelQueue.Enqueue(work);
            }
#if !NETCF
            else if (work.TargetApartment == ApartmentState.STA)
                NonParallelSTAQueue.Enqueue(work);
#endif
            else
                NonParallelQueue.Enqueue(work);
        }

        /// <summary>
        /// Cancel the ongoing run completely.
        /// If no run is in process, the call has no effect.
        /// </summary>
        public void CancelRun(bool force)
        {
            foreach (var shift in Shifts)
                shift.Cancel(force);
        }

        #endregion

        #region Private Queue Properties

        // Queues are not actually created until the first time the property
        // is referenced by the Dispatch method adding a WorkItem to it.

        private WorkItemQueue ParallelQueue
        {
            get
            {
                if (_parallelQueue == null)
                {
                    _parallelQueue = new WorkItemQueue("ParallelQueue");
                    _parallelShift.AddQueue(_parallelQueue);

                    for (int i = 1; i <= _levelOfParallelism; i++)
                    {
                        string name = string.Format("Worker#" + i.ToString());
#if NETCF
                        _parallelShift.Assign(new TestWorker(_parallelQueue, name));
#else
                        _parallelShift.Assign(new TestWorker(_parallelQueue, name, ApartmentState.MTA));
#endif
                    }
                }

                return _parallelQueue;
            }
        }

#if !NETCF
        private WorkItemQueue ParallelSTAQueue
        {
            get
            {
                if (_parallelSTAQueue == null)
                {
                    _parallelSTAQueue = new WorkItemQueue("ParallelSTAQueue");
                    _parallelShift.AddQueue(_parallelSTAQueue);
                    _parallelShift.Assign(new TestWorker(_parallelSTAQueue, "Worker#STA", ApartmentState.STA));
                }

                return _parallelSTAQueue;
            }
        }
#endif

        private WorkItemQueue NonParallelQueue
        {
            get
            {
                if (_nonParallelQueue == null)
                {
                    _nonParallelQueue = new WorkItemQueue("NonParallelQueue");
                    _nonParallelShift.AddQueue(_nonParallelQueue);
#if NETCF
                    _nonParallelShift.Assign(new TestWorker(_nonParallelQueue, "Worker#NP"));
#else
                    _nonParallelShift.Assign(new TestWorker(_nonParallelQueue, "Worker#STA_NP", ApartmentState.MTA));
#endif
                }

                return _nonParallelQueue;
            }
        }

#if !NETCF
        private WorkItemQueue NonParallelSTAQueue
        {
            get
            {
                if (_nonParallelSTAQueue == null)
                {
                    _nonParallelSTAQueue = new WorkItemQueue("NonParallelSTAQueue");
                    _nonParallelSTAShift.AddQueue(_nonParallelSTAQueue);
                    _nonParallelSTAShift.Assign(new TestWorker(_nonParallelSTAQueue, "Worker#NP_STA", ApartmentState.STA));
                }

                return _nonParallelSTAQueue;
            }
        }
#endif
        #endregion

        #region Helper Methods

        private void OnEndOfShift(object sender, EventArgs ea)
        {
            if (!StartNextShift())
            {
                foreach (var shift in Shifts)
                    shift.ShutDown();
            }
        }

        private bool StartNextShift()
        {
            foreach (var shift in Shifts)
            {
                if (shift.HasWork)
                {
                    shift.Start();
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}

#endif
