// ***********************************************************************
// Copyright (c) 2012 Charlie Poole, Rob Prouse
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
#if NET_2_0 || NET_3_5
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
        private const int SPIN_COUNT = 5;

        // Although the code makes the number of levels relatively
        // easy to change, it is still baked in as a constant at
        // this time. If we wanted to make it variable, that would
        // be a bit more work, which does not now seem necessary.
        private const int HIGH_PRIORITY = 0;
        private const int NORMAL_PRIORITY = 1;
        private const int PRIORITY_LEVELS = 2;

        private Logger log = InternalTrace.GetLogger("WorkItemQueue");

        private ConcurrentQueue<WorkItem>[] _innerQueues;

        private class SavedState
        {
            public ConcurrentQueue<WorkItem>[] InnerQueues;

            public SavedState(WorkItemQueue queue)
            {
                InnerQueues = queue._innerQueues;
            }
        }

        private Stack<SavedState> _savedState = new Stack<SavedState>();
        private object saveLock = new object();

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
        /// <param name="isParallel">Flag indicating whether this is a parallel queue</param>
        /// <param name="apartment">ApartmentState to use for items on this queue</param>
        public WorkItemQueue(string name, bool isParallel, ApartmentState apartment)
        {
            Name = name;
            IsParallelQueue = isParallel;
            TargetApartment = apartment;
            State = WorkItemQueueState.Paused;
            ItemsProcessed = 0;

            InitializeQueues();
        }

        private void InitializeQueues()
        {
            _innerQueues = new ConcurrentQueue<WorkItem>[PRIORITY_LEVELS];

            for (int i = 0; i < PRIORITY_LEVELS; i++)
                _innerQueues[i] = new ConcurrentQueue<WorkItem>();
        }

        #region Properties

        /// <summary>
        /// Gets the name of the work item queue.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a flag indicating whether this queue is used for parallel execution
        /// </summary>
        public bool IsParallelQueue { get; private set; }

        /// <summary>
        /// Gets the target ApartmentState for work items on this queue
        /// </summary>
        public ApartmentState TargetApartment { get; private set; }

        private int _itemsProcessed;
        /// <summary>
        /// Gets the total number of items processed so far
        /// </summary>
        public int ItemsProcessed
        {
            get { return _itemsProcessed; }
            private set { _itemsProcessed = value; }
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
            get
            {
                foreach (var q in _innerQueues)
                    if (!q.IsEmpty)
                        return false;

                return true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueue a WorkItem to be processed
        /// </summary>
        /// <param name="work">The WorkItem to process</param>
        public void Enqueue(WorkItem work)
        {
            Enqueue(work, work is CompositeWorkItem.OneTimeTearDownWorkItem ? HIGH_PRIORITY : NORMAL_PRIORITY);
        }

        /// <summary>
        /// Enqueue a WorkItem to be processed - internal for testing
        /// </summary>
        /// <param name="work">The WorkItem to process</param>
        /// <param name="priority">The priority at which to process the item</param>
        internal void Enqueue(WorkItem work, int priority)
        {
            Guard.ArgumentInRange(priority >= 0 && priority < PRIORITY_LEVELS,
                "Invalid priority specified", "priority");

            do
            {
                int cachedAddId = _addId;

                // Validate that we have are the current enqueuer
                if (Interlocked.CompareExchange(ref _addId, cachedAddId + 1, cachedAddId) != cachedAddId)
                    continue;

                // Add to the collection
                _innerQueues[priority].Enqueue(work);

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
                    if (sw.Count <= SPIN_COUNT)
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
                WorkItem work = null;
                while (work == null)
                    foreach (var q in _innerQueues)
                        if (q.TryDequeue(out work))
                            break;

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
            log.Info("{0}.{1} starting", Name, _savedState.Count);

            if (Interlocked.CompareExchange(ref _state, (int)WorkItemQueueState.Running, (int)WorkItemQueueState.Paused) == (int)WorkItemQueueState.Paused)
                _mreAdd.Set();
        }

        /// <summary>
        /// Signal the queue to stop
        /// </summary>
        public void Stop()
        {
            log.Info("{0}.{1} stopping - {2} WorkItems processed", Name, _savedState.Count, ItemsProcessed);

            if (Interlocked.Exchange(ref _state, (int)WorkItemQueueState.Stopped) != (int)WorkItemQueueState.Stopped)
                _mreAdd.Set();
        }

        /// <summary>
        /// Pause the queue for restarting later
        /// </summary>
        public void Pause()
        {
            log.Debug("{0}.{1} pausing", Name, _savedState.Count);

            Interlocked.CompareExchange(ref _state, (int)WorkItemQueueState.Paused, (int)WorkItemQueueState.Running);
        }

        /// <summary>
        /// Save the current inner queue and create new ones for use by
        /// a non-parallel fixture with parallel children.
        /// </summary>
        internal void Save()
        {
            lock (saveLock)
            {
                Pause();
                _savedState.Push(new SavedState(this));

                InitializeQueues();
                Start();
            }
        }

        /// <summary>
        /// Restore the inner queue that was previously saved
        /// </summary>
        internal void Restore()
        {
            Pause();

            _innerQueues = _savedState.Pop().InnerQueues;

            Start();
        }

        #endregion
    }

#if NET_2_0 || NET_3_5
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
