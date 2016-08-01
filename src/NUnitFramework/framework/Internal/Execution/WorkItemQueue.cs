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

#if PARALLEL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
#if NET_2_0 || NET_3_5 || NETCF
using ManualResetEventSlim = System.Threading.ManualResetEvent;
#endif

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
        private const int spinCount = 5;

        private Logger log = InternalTrace.GetLogger("WorkItemQueue");

        private readonly ConcurrentQueue<WorkItem> _innerQueue = new ConcurrentQueue<WorkItem>();

        /* This event is used solely for the purpose of having an optimized sleep cycle when
         * we have to wait on an external event (Add or Remove for instance)
         */
        private readonly ManualResetEventSlim _mreAdd = new ManualResetEventSlim(false);

        /* The whole idea is to use these two values in a transactional
         * way to track and manage the actual data inside the underlying lock-free collection
         * instead of directly working with it or using external locking.
         *
         * They are manipulated with CAS and are guaranteed to increase over time and use
         * of the instance thus preventing ABA problems.
         */
        private int _addId = int.MinValue;
        private int _removeId = int.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueue"/> class.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        public WorkItemQueue(string name)
        {
            Name = name;
            State = WorkItemQueueState.Paused;
            MaxCount = 0;
            ItemsProcessed = 0;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the work item queue.
        /// </summary>
        public string Name { get; private set; }

        private int _itemsProcessed;
        /// <summary>
        /// Gets the total number of items processed so far
        /// </summary>
        public int ItemsProcessed
        {
            get { return _itemsProcessed; }
            private set { _itemsProcessed = value; }
        }

        private int _maxCount;

        /// <summary>
        /// Gets the maximum number of work items.
        /// </summary>
        public int MaxCount
        {
            get { return _maxCount; }
            private set { _maxCount = value; }
        }

        private int _state;
        /// <summary>
        /// Gets the current state of the queue
        /// </summary>
        public WorkItemQueueState State
        {
            get { return (WorkItemQueueState)_state; }
            private set { _state = (int)value; }
        }

        /// <summary>
        /// Get a bool indicating whether the queue is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _innerQueue.IsEmpty; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueue a WorkItem to be processed
        /// </summary>
        /// <param name="work">The WorkItem to process</param>
        public void Enqueue(WorkItem work)
        {
            do
            {
                int cachedAddId = _addId;

                // Validate that we have are the current enqueuer
                if (Interlocked.CompareExchange(ref _addId, cachedAddId + 1, cachedAddId) != cachedAddId)
                    continue;

                // Add to the collection
                _innerQueue.Enqueue(work);

                // Set MaxCount using CAS
                int i, j = _maxCount;
                do
                {
                    i = j;
                    j = Interlocked.CompareExchange(ref _maxCount, Math.Max(i, _innerQueue.Count), i);
                }
                while (i != j);

                // Wake up threads that may have been sleeping
                _mreAdd.Set();

                return;
            } while (true);
        }

        /// <summary>
        /// Dequeue a WorkItem for processing
        /// </summary>
        /// <returns>A WorkItem or null if the queue has stopped</returns>
        public WorkItem Dequeue()
        {
            SpinWait sw = new SpinWait();

            do
            {
                WorkItemQueueState cachedState = State;

                if (cachedState == WorkItemQueueState.Stopped)
                    return null; // Tell worker to terminate

                int cachedRemoveId = _removeId;
                int cachedAddId = _addId;

                // Empty case (or paused)
                if (cachedRemoveId == cachedAddId || cachedState == WorkItemQueueState.Paused)
                {
                    // Spin a few times to see if something changes
                    if (sw.Count <= spinCount)
                    {
                        sw.SpinOnce();
                    }
                    else
                    {
                        // Reset to wait for an enqueue
                        _mreAdd.Reset();

                        // Recheck for an enqueue to avoid a Wait
                        if ((cachedRemoveId != _removeId || cachedAddId != _addId) && cachedState != WorkItemQueueState.Paused)
                        {
                            // Queue is not empty, set the event
                            _mreAdd.Set();
                            continue;
                        }

                        // Wait for something to happen
                        _mreAdd.Wait(500);
                    }

                    continue;
                }

                // Validate that we are the current dequeuer
                if (Interlocked.CompareExchange(ref _removeId, cachedRemoveId + 1, cachedRemoveId) != cachedRemoveId)
                    continue;


                // Dequeue our work item
                WorkItem work;
                while (!_innerQueue.TryDequeue(out work)) { };

                // Add to items processed using CAS
                Interlocked.Increment(ref _itemsProcessed);

                return work;
            } while (true);
        }

        /// <summary>
        ///  Start or restart processing of items from the queue
        /// </summary>
        public void Start()
        {
            log.Info("{0} starting", Name);

            if (Interlocked.CompareExchange(ref _state, (int)WorkItemQueueState.Running, (int)WorkItemQueueState.Paused) == (int)WorkItemQueueState.Paused)
                _mreAdd.Set();
        }

        /// <summary>
        /// Signal the queue to stop
        /// </summary>
        public void Stop()
        {
            log.Info("{0} stopping - {1} WorkItems processed, max size {2}", Name, ItemsProcessed, MaxCount);

            if (Interlocked.Exchange(ref _state, (int)WorkItemQueueState.Stopped) != (int)WorkItemQueueState.Stopped)
                _mreAdd.Set();
        }

        /// <summary>
        /// Pause the queue for restarting later
        /// </summary>
        public void Pause()
        {
            log.Info("{0} pausing", Name);

            Interlocked.CompareExchange(ref _state, (int)WorkItemQueueState.Paused, (int)WorkItemQueueState.Running);
        }

        #endregion
    }

#if NET_2_0 || NET_3_5 || NETCF
    internal static class ManualResetEventExtensions
    {
        public static bool Wait (this ManualResetEvent mre, int millisecondsTimeout)
        {
            return mre.WaitOne(millisecondsTimeout, false);
        }
    }
#endif

}
#endif
