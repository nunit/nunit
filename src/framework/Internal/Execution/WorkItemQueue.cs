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

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// WorkItemQueueState indicates the current state of a WorkItemQueue
    /// </summary>
    public enum WorkItemQueueState
    {
        /// <summary>
        /// The queue is paused
        /// </summary>
        Paused,
        /// <summary>
        /// The queue is running
        /// </summary>
        Running,
        /// <summary>
        /// The queue is stopped
        /// </summary>
        Stopped
    }

    /// <summary>
    /// A WorkItemQueue holds work items that are ready to
    /// be run, either initially or after some dependency
    /// has been satisfied.
    /// </summary>
    public class WorkItemQueue
    {
        Logger log = InternalTrace.GetLogger("WorkItemQueue");

        private Queue<WorkItem> _innerQueue = new Queue<WorkItem>();
        private object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueue"/> class.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        public WorkItemQueue(string name)
        {
            this.Name = name;
            this.State = WorkItemQueueState.Paused;
            this.MaxCount = 0;
            this.ItemsProcessed = 0;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the work item queue.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the total number of items processed so far
        /// </summary>
        public int ItemsProcessed { get; private set; }

        /// <summary>
        /// Gets the maximum number of work items.
        /// </summary>
        public int MaxCount { get; private set; }

        /// <summary>
        /// Gets the current state of the queue
        /// </summary>
        public WorkItemQueueState State { get; private set; }

        /// <summary>
        /// Get a bool indicating whether the queue is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _innerQueue.Count == 0; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueue a WorkItem to be processed
        /// </summary>
        /// <param name="work">The WorkItem to process</param>
        public void Enqueue(WorkItem work)
        {
            lock (_syncRoot)
            {
                _innerQueue.Enqueue(work);
                if (_innerQueue.Count > MaxCount)
                    MaxCount = _innerQueue.Count;
                Monitor.PulseAll(_syncRoot);
            }
        }

        /// <summary>
        /// Dequeue a WorkItem for processing
        /// </summary>
        /// <returns>A WorkItem or null if the queue has stopped</returns>
        public WorkItem Dequeue()
        {
            lock (_syncRoot)
            {
                while (this.IsEmpty || this.State != WorkItemQueueState.Running)
                {
                    if (State == WorkItemQueueState.Stopped)
                        return null; // Tell worker to terminate
                    else // We are either paused or empty, so wait for something to change
                        Monitor.Wait(_syncRoot);
                }

                // Queue is running and non-empty
                ItemsProcessed++;
                return _innerQueue.Dequeue();
            }
        }

        /// <summary>
        ///  Start or restart processing of items from the queue
        /// </summary>
        public void Start()
        {
            lock (_syncRoot)
            {
                log.Info("{0} starting", Name);
                State = WorkItemQueueState.Running;
                Monitor.PulseAll(_syncRoot);
            }
        }

        /// <summary>
        /// Signal the queue to stop
        /// </summary>
        public void Stop()
        {
            lock (_syncRoot)
            {
                log.Info("{0} stopping - {1} WorkItems processed, max size {2}", Name, ItemsProcessed, MaxCount);

                State = WorkItemQueueState.Stopped;
                Monitor.PulseAll(_syncRoot);
            }
        }

        /// <summary>
        /// Pause the queue for restarting later
        /// </summary>
        public void Pause()
        {
            lock (_syncRoot)
            {
                State = WorkItemQueueState.Paused;
            }
        }

        #endregion
    }
}
#endif
