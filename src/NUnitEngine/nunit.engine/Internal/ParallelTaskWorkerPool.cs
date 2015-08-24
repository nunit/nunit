// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtainingn
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NUnit.Engine.Internal
{
    public class ParallelTaskWorkerPool
    {
        private readonly object _taskLock = new object();
        private readonly Queue<ITask> _tasks;
        private readonly List<Thread> _threads;
        private readonly int _maxThreads;

        private bool _isRunning = false;
        
        public ParallelTaskWorkerPool(int maxThreads)
        {
            if (maxThreads < 1)
            {
                throw new ArgumentException("ParallelTaskWorkerPool requires maxThreads be greater than 0");
            }

            _maxThreads = maxThreads;
            _tasks = new Queue<ITask>();
            _threads = new List<Thread>();
        }

        public void Enqueue(ITask task)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Can only enqueue tasks before starting the worker pool");
            }

            _tasks.Enqueue(task);
        }

        public void Start()
        {
            _isRunning = true;

            var numThreads = Math.Min(_tasks.Count, _maxThreads);
            for (var i = 0; i < numThreads; i++)
            {
                _threads.Add(new Thread(ProcessTasksProc));
            }

            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        private void ProcessTasksProc()
        {
            while (true)
            {
                ITask task = null;
                lock (_taskLock)
                {
                    if (_tasks.Count > 0)
                    {
                        task = _tasks.Dequeue();
                    }
                    else
                    {
                        return;
                    }
                }

                task.Execute();
            }
        }

        /// <summary>
        /// Blocks the calling thread until all worker threads terminate or the specified time elapses
        /// </summary>
        public bool WaitAll(TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            var remainingTimeout = timeout;

            foreach (var thread in _threads)
            {
                if (!thread.Join(remainingTimeout))
                {
                    return false;
                }
                
                remainingTimeout = timeout - sw.Elapsed;
                if (remainingTimeout.TotalMilliseconds <= 0)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Blocks the calling thread until all worker threads terminate
        /// </summary>
        public void WaitAll()
        {
            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }
}