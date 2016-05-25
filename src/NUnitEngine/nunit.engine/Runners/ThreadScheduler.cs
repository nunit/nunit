// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Engine.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class ThreadScheduler: IScheduler
    {
        private readonly object _lockObject = new object();
        private readonly Thread[] _threads;
        private readonly Queue<Action> _actions = new Queue<Action>();
        private bool _disposed;

        public ThreadScheduler(int threadsCount)
        {
            if (threadsCount < 1) throw new ArgumentOutOfRangeException("threadsCount");

            _threads = new Thread[threadsCount];
            for (var i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(ThreadEntry) { IsBackground = true };
                _threads[i].Start();
            }
        }

        public void Schedule(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            lock (_lockObject)
            {
                _actions.Enqueue(action);
                Monitor.Pulse(_lockObject);
            }            
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            for (var i = 0; i < _threads.Length; i++)
            {
                lock (_lockObject)
                {
                    _actions.Enqueue(null);
                    Monitor.Pulse(_lockObject);
                }
            }

            for (var i = 0; i < _threads.Length; i++)
            {
                try
                {
                    _threads[i].Join();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void ThreadEntry()
        {
            do
            {
                Action action;
                lock (_lockObject)
                {
                    while (_actions.Count == 0)
                    {
                        Monitor.Wait(_lockObject);
                    }

                    action = _actions.Dequeue();
                }
                
                try
                {
                    if (action != null)
                    {
                        action();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            while (true);
        }
    }
}
