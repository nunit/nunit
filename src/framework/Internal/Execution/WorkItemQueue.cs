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
using System.Collections.Generic;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// A WorkItemQueue holds work items that are ready to
    /// be run, either initially or after some dependency
    /// has been satisfied.
    /// </summary>
    public class WorkItemQueue
    {
        private Queue<WorkItem> _innerQueue = new Queue<WorkItem>();
        private object _syncRoot = new object();
        private bool _stopping;
        private int _maxCount = 0;

        /// <summary>
        /// Gets the name of the work item queue.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the maximum number of work items.
        /// </summary>
        public int MaxCount
        {
            get { return _maxCount; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueue"/> class.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        public WorkItemQueue(string name)
        {
            this.Name = name;
        }

        // Not currently Used
        //public void Clear()
        //{
        //    lock (this.syncRoot)
        //    {
        //        _innerQueue.Clear();
        //    }
        //}

        /// <summary>
        /// Enqueue a WorkItem to be processed
        /// </summary>
        /// <param name="work">The WorkItem to process</param>
        public void Enqueue(WorkItem work)
        {
            lock (_syncRoot)
            {
                _innerQueue.Enqueue(work);
                if (_innerQueue.Count > _maxCount)
                    _maxCount = _innerQueue.Count;
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
                while (_innerQueue.Count == 0)
                {
                    if (_stopping)
                        return null;
                    else
                        Monitor.Wait(_syncRoot);
                }

                return _innerQueue.Dequeue();
            }
        }

        /// <summary>
        /// Signal the queue to stop. It will not
        /// actually stop until all items are processed.
        /// </summary>
        public void Stop()
        {
            lock (_syncRoot)
            {
                _stopping = true;
                Monitor.PulseAll(_syncRoot);
            }
        }
    }
}
#endif
