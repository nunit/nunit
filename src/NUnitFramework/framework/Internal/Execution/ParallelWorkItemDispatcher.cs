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

//#define NO_PARALLEL_CASES

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
        private static readonly Logger log = InternalTrace.GetLogger("Dispatcher");

        private readonly int _levelOfParallelism;
        private int _itemsDispatched;

        // WorkShifts - Dispatcher processes tests in three non-overlapping shifts.
        // See comment in Workshift.cs for a more detailed explanation.
        private readonly WorkShift _parallelShift = new WorkShift("Parallel");
        private readonly WorkShift _nonParallelShift = new WorkShift("NonParallel");
        private readonly WorkShift _nonParallelSTAShift = new WorkShift("NonParallelSTA");

        /// <summary>
        /// Enumerates all the shifts supported by the dispatcher
        /// </summary>
        public IEnumerable<WorkShift> Shifts { get; private set; }

        // Queues used by WorkShifts
        private readonly Lazy<WorkItemQueue> _parallelQueue;
        private readonly Lazy<WorkItemQueue> _nonParallelQueue;
        private readonly Lazy<WorkItemQueue> _parallelSTAQueue;
        private readonly Lazy<WorkItemQueue> _nonParallelSTAQueue;

        #region Constructor

        /// <summary>
        /// Construct a ParallelWorkItemDispatcher
        /// </summary>
        /// <param name="levelOfParallelism">Number of workers to use</param>
        public ParallelWorkItemDispatcher(int levelOfParallelism)
        {
            _levelOfParallelism = levelOfParallelism;

            // Initialize WorkShifts
            Shifts = new WorkShift[]
            {
                _parallelShift,
                _nonParallelShift,
                _nonParallelSTAShift
            };

            foreach (var shift in Shifts)
                shift.EndOfShift += OnEndOfShift;

            // Set up queues for lazy initialization
            _parallelQueue = new Lazy<WorkItemQueue>(() =>
            {
                var parallelQueue = new WorkItemQueue("ParallelQueue");
                _parallelShift.AddQueue(parallelQueue);

                for (int i = 1; i <= _levelOfParallelism; i++)
                {
                    string name = string.Format("Worker#" + i.ToString());
                    _parallelShift.Assign(new TestWorker(parallelQueue, name, ApartmentState.MTA));
                }

                return parallelQueue;
            });

            _parallelSTAQueue = new Lazy<WorkItemQueue>(() =>
            {
                var parallelSTAQueue = new WorkItemQueue("ParallelSTAQueue");
                _parallelShift.AddQueue(parallelSTAQueue);
                _parallelShift.Assign(new TestWorker(parallelSTAQueue, "Worker#STA", ApartmentState.STA));

                return parallelSTAQueue;
            });

            _nonParallelQueue = new Lazy<WorkItemQueue>(() =>
            {
                var nonParallelQueue = new WorkItemQueue("NonParallelQueue");
                _nonParallelShift.AddQueue(nonParallelQueue);
                _nonParallelShift.Assign(new TestWorker(nonParallelQueue, "Worker#STA_NP", ApartmentState.MTA));

                return nonParallelQueue;
            });

            _nonParallelSTAQueue = new Lazy<WorkItemQueue>(() =>
            {
                var nonParallelSTAQueue = new WorkItemQueue("NonParallelSTAQueue");
                _nonParallelSTAShift.AddQueue(nonParallelSTAQueue);
                _nonParallelSTAShift.Assign(new TestWorker(nonParallelSTAQueue, "Worker#NP_STA", ApartmentState.STA));

                return nonParallelSTAQueue;
            });
        }

        #endregion

        #region IWorkItemDispatcher Members

        /// <summary>
        /// Start execution, setting the top level work,
        /// enqueuing it and starting a shift to execute it.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            var strategy = topLevelWorkItem.ParallelScope.HasFlag(ParallelScope.None)
                ? ExecutionStrategy.NonParallel
                : ExecutionStrategy.Parallel;

            Dispatch(topLevelWorkItem, strategy);
          
            StartNextShift();
        }

        /// <summary>
        /// Dispatch a single work item for execution. The first
        /// work item dispatched is saved as the top-level
        /// work item and used when stopping the run.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
            Dispatch(work, GetExecutionStrategy(work));
        }

        private void Dispatch(WorkItem work, ExecutionStrategy strategy)
        {
            log.Debug("Using {0} strategy for {1}", strategy, work.Name);

            switch (strategy)
            {
                default:
                case ExecutionStrategy.Direct:
                    work.Execute();
                    break;
                case ExecutionStrategy.Parallel:
                    if (work.TargetApartment == ApartmentState.STA)
                        ParallelSTAQueue.Enqueue(work);
                    else
                        ParallelQueue.Enqueue(work);
                    break;
                case ExecutionStrategy.NonParallel:
                    if (work.TargetApartment == ApartmentState.STA)
                        NonParallelSTAQueue.Enqueue(work);
                    else
                        NonParallelQueue.Enqueue(work);
                    break;
            }

            Interlocked.Increment(ref _itemsDispatched);
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
                return _parallelQueue.Value;
            }
        }

        private WorkItemQueue ParallelSTAQueue
        {
            get
            {
                return _parallelSTAQueue.Value;
            }
        }

        private WorkItemQueue NonParallelQueue
        {
            get
            {
                return _nonParallelQueue.Value;
            }
        }

        private WorkItemQueue NonParallelSTAQueue
        {
            get
            {
                return _nonParallelSTAQueue.Value;
            }
        }
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

        private enum ExecutionStrategy
        {
            Direct,
            Parallel,
            NonParallel,
        }
        
        private static ExecutionStrategy GetExecutionStrategy(WorkItem work)
        {
            // If there is no fixture and so nothing to do but dispatch 
            // grandchildren we run directly. This saves time that would 
            // otherwise be spent enqueuing and dequeing items.
            // TODO: It would be even better if we could avoid creating 
            // these "do-nothing" work items in the first place.
            if (work.Test.TypeInfo == null)
                return ExecutionStrategy.Direct;

            // If the context is single-threaded we are required to run
            // the tests one by one on the same thread as the fixture.
            if (work.Context.IsSingleThreaded)
                return ExecutionStrategy.Direct;

#if NO_PARALLEL_CASES
            // For now, if this represents a test case, run directly. 
            // This avoids issues caused by tests that access the fixture 
            // state and allows handling ApartmentState preferences set on 
            // the fixture more easily.
            if (work is SimpleWorkItem)
                return ExecutionStrategy.Direct;
#endif

            if (work.ParallelScope.HasFlag(ParallelScope.Self) ||
                work.Context.ParallelScope.HasFlag(ParallelScope.Children) ||
                work.Test is TestFixture && work.Context.ParallelScope.HasFlag(ParallelScope.Fixtures))
            {
                return ExecutionStrategy.Parallel;
            }
            else
            if (work.ParallelScope.HasFlag(ParallelScope.None))
            {
                return ExecutionStrategy.NonParallel;
            }
            else
            {
                return ExecutionStrategy.Direct;
            }
        }

#endregion
    }

#if NET_2_0 || NET_3_5
    static class ParallelScopeHelper
    {
        public static bool HasFlag(this ParallelScope scope, ParallelScope value)
        {
            return (scope & value) != 0;
        }
    }
#endif
}
#endif