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

// If STA_QUEUE is defined, a separate queue is used for WorkItems
// that must run in the STA. Otherwise, these items run in the
// main queue and a separate thread is kicked off for each item.
// Even if the STA queue is used, we currently run all test cases
// in a fixture together, so individual methods that need the STA
// must still have a separate thread started to run them. This
// design should be re-visited when we are running each test in
// a fixture as a separate work item, possibly in parallel.

//#define STA_QUEUE

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

        private List<TestWorker> _workers = new List<TestWorker>();
        private WorkItemQueue _readyQueue = new WorkItemQueue("ReadyQueue");
#if STA_QUEUE
        private WorkItemQueue _staQueue = new WorkItemQueue("STAQueue");
#endif

        private int _itemsDispatched;

        /// <summary>
        /// Construct a WorkItemDispatcher
        /// </summary>
        /// <param name="numWorkers">Number of workers to use</param>
        public WorkItemDispatcher(int numWorkers)
        {
            for (int i = 1; i <= numWorkers; i++)
            {
                string name = string.Format("Worker#" + i.ToString());
                _workers.Add(new TestWorker(_readyQueue, name, ApartmentState.MTA));
            }

#if STA_QUEUE
            _workers.Add(new TestWorker(_staQueue, "STAWorker", ApartmentState.STA));
#endif
        }

        /// <summary>
        /// Start the dispatcher.
        /// </summary>
        public void Start()
        {
            log.Info("Starting {0} TestWorkers", _workers.Count);

            foreach (TestWorker worker in _workers)
                worker.Start();
        }

        /// <summary>
        /// Stop the dispatcher.
        /// </summary>
        public void Stop()
        {
            log.Info(
                "WorkItemDispatcher stopping\r\n\tDispatched {0} items\r\n\tMax Queue Length: {1}",
                _itemsDispatched, 
                _readyQueue.MaxCount);

            _readyQueue.Stop();
#if STA_QUEUE
            _staQueue.Stop();
#endif
        }

        /// <summary>
        /// Dispatch a single work item.
        /// </summary>
        /// <param name="work">The item to dispatch</param>
        public void Dispatch(WorkItem work)
        {
            log.Debug("Enqueuing WorkItem for {0}", work.Test.FullName);
#if STA_QUEUE
            if (work.TargetApartment == ApartmentState.STA)
                _staQueue.Enqueue(work);
            else
#endif
                _readyQueue.Enqueue(work);

            ++_itemsDispatched;
        }
    }
}
#endif
