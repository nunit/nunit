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
using System.Linq;
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

        private WorkItem _topLevelWorkItem;

        #region Constructor

        /// <summary>
        /// Construct a ParallelWorkItemDispatcher
        /// </summary>
        /// <param name="levelOfParallelism">Number of workers to use</param>
        public ParallelWorkItemDispatcher(int levelOfParallelism)
        {
            // Create Shifts
            ParallelShift = new WorkShift("Parallel");
            NonParallelShift = new WorkShift("NonParallel");
            NonParallelSTAShift = new WorkShift("NonParallelSTA");

            foreach (var shift in Shifts)
                shift.EndOfShift += OnEndOfShift;

            // Assign queues to shifts
            ParallelShift.AddQueue(ParallelQueue);
            ParallelShift.AddQueue(ParallelSTAQueue);
            NonParallelShift.AddQueue(NonParallelQueue);
            NonParallelSTAShift.AddQueue(NonParallelSTAQueue);

            // Create workers and assign to shifts and queues
            // TODO: Avoid creating all the workers till needed
            for (int i = 1; i <= levelOfParallelism; i++)
            {
                string name = string.Format("Worker#" + i.ToString());
                ParallelShift.Assign(new TestWorker(ParallelQueue, name));
            }

            ParallelShift.Assign(new TestWorker(ParallelSTAQueue, "Worker#STA"));

            var worker = new TestWorker(NonParallelQueue, "Worker#STA_NP");
            worker.Busy += OnStartNonParallelWorkItem;
            NonParallelShift.Assign(worker);

            worker = new TestWorker(NonParallelSTAQueue, "Worker#NP_STA");
            worker.Busy += OnStartNonParallelWorkItem;
            NonParallelSTAShift.Assign(worker);
        }

        private void OnStartNonParallelWorkItem(TestWorker worker, WorkItem work)
        {
            // This captures the startup of TestFixtures and SetUpFixtures,
            // but not their teardown items, which are not composite items
            if (work is CompositeWorkItem && work.Test.TypeInfo != null)
                IsolateQueues(work);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enumerates all the shifts supported by the dispatcher
        /// </summary>
        public IEnumerable<WorkShift> Shifts
        {
            get
            {
                yield return ParallelShift;
                yield return NonParallelShift;
                yield return NonParallelSTAShift;
            }
        }

        /// <summary>
        /// Enumerates all the Queues supported by the dispatcher
        /// </summary>
        public IEnumerable<WorkItemQueue> Queues
        {
            get
            {
                yield return ParallelQueue;
                yield return ParallelSTAQueue;
                yield return NonParallelQueue;
                yield return NonParallelSTAQueue;
            }
        }

        // WorkShifts - Dispatcher processes tests in three non-overlapping shifts.
        // See comment in Workshift.cs for a more detailed explanation.
        private WorkShift ParallelShift { get; }
        private WorkShift NonParallelShift { get; }
        private WorkShift NonParallelSTAShift { get; }

        // WorkItemQueues
        private WorkItemQueue ParallelQueue { get; } = new WorkItemQueue("ParallelQueue", true, ApartmentState.MTA);
        private WorkItemQueue ParallelSTAQueue { get; } = new WorkItemQueue("ParallelSTAQueue", true, ApartmentState.STA);
        private WorkItemQueue NonParallelQueue { get; } = new WorkItemQueue("NonParallelQueue", false, ApartmentState.MTA);
        private WorkItemQueue NonParallelSTAQueue { get; } = new WorkItemQueue("NonParallelSTAQueue", false, ApartmentState.STA);

        #endregion

        #region IWorkItemDispatcher Members

        /// <summary>
        /// Start execution, setting the top level work,
        /// enqueuing it and starting a shift to execute it.
        /// </summary>
        public void Start(WorkItem topLevelWorkItem)
        {
            _topLevelWorkItem = topLevelWorkItem;

            var strategy = topLevelWorkItem.ParallelScope.HasFlag(ParallelScope.None)
                ? ParallelExecutionStrategy.NonParallel
                : ParallelExecutionStrategy.Parallel;

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
            Dispatch(work, work.ExecutionStrategy);
        }

        // Separate method so it can be used by Start
        private void Dispatch(WorkItem work, ParallelExecutionStrategy strategy)
        {
            log.Debug("Using {0} strategy for {1}", strategy, work.Name);

            switch (strategy)
            {
                default:
                case ParallelExecutionStrategy.Direct:
                    work.Execute();
                    break;
                case ParallelExecutionStrategy.Parallel:
                    if (work.TargetApartment == ApartmentState.STA)
                        ParallelSTAQueue.Enqueue(work);
                    else
                        ParallelQueue.Enqueue(work);
                    break;
                case ParallelExecutionStrategy.NonParallel:
                    if (work.TargetApartment == ApartmentState.STA)
                        NonParallelSTAQueue.Enqueue(work);
                    else
                        NonParallelQueue.Enqueue(work);
                    break;
            }
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

        private object _queueLock = new object();
        private int _isolationLevel = 0;

        /// <summary>
        /// Save the state of the queues and create a new isolated set
        /// </summary>
        internal void IsolateQueues(WorkItem work)
        {
            log.Info("Saving Queue State for {0}", work.Name);
            lock (_queueLock)
            {
                foreach (WorkItemQueue queue in Queues)
                    queue.Save();

                _isolationLevel++;
            }
        }

        /// <summary>
        /// Remove isolated queues and restore old ones
        /// </summary>
        private void RestoreQueues()
        {
            Guard.OperationValid(_isolationLevel > 0, $"Internal Error: Called {nameof(RestoreQueues)} with no saved queues.");
            Guard.OperationValid(Queues.All(q => q.IsEmpty), $"Internal Error: Called {nameof(RestoreQueues)} with non-empty queues.");

            // Keep lock until we can remove for both methods
            lock (_queueLock)
            {
                log.Info("Restoring Queue State");

                foreach (WorkItemQueue queue in Queues)
                    queue.Restore();

                _isolationLevel--;
            }
        }

        #endregion

        #region Helper Methods

        private void OnEndOfShift(object sender, EventArgs ea)
        {
            // This shift ended, so see if there is work in any other
            // TODO: Temp fix - revisit later when object model is redesigned.
            if (StartNextShift())
                return;

            if (_isolationLevel > 0)
                RestoreQueues();

            // Shift has ended but all work may not yet be done
            while (_topLevelWorkItem.State != WorkItemState.Complete)
            {
                // This will fail if there is no work - all queues empty.
                // In that case, we just continue the loop until either
                // a shift is started or all the work is complete.
                if (StartNextShift())
                    return;
            }

            // All work is complete, so shutdown.
            foreach (var shift in Shifts)
                shift.ShutDown();
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

    #region ParallelScopeHelper Class

#if NET20 || NET35
    static class ParallelScopeHelper
    {
        public static bool HasFlag(this ParallelScope scope, ParallelScope value)
        {
            return (scope & value) != 0;
        }
    }
#endif

    #endregion
}
#endif
