// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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

#if NET_4_0 || NET_4_5
using System;
using System.Collections;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal class AsyncSynchronizationContext : SynchronizationContext
    {
        private int _operationCount;
        private readonly AsyncOperationQueue _operations = new AsyncOperationQueue();

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new InvalidOperationException("Sending to this synchronization context is not supported");
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _operations.Enqueue(new AsyncOperation(d, state));
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref _operationCount);
            base.OperationStarted();
        }

        public override void OperationCompleted()
        {
            if (Interlocked.Decrement(ref _operationCount) == 0)
                _operations.MarkAsComplete();

            base.OperationCompleted();
        }

        public void WaitForPendingOperationsToComplete()
        {
            _operations.InvokeAll();
        }

        private class AsyncOperationQueue
        {
            private bool _run = true;
            private readonly Queue _operations = Queue.Synchronized(new Queue());
            private readonly AutoResetEvent _operationsAvailable = new AutoResetEvent(false);

            public void Enqueue(AsyncOperation asyncOperation)
            {
                _operations.Enqueue(asyncOperation);
                _operationsAvailable.Set();
            }

            public void MarkAsComplete()
            {
                _run = false;
                _operationsAvailable.Set();
            }

            public void InvokeAll()
            {
                while (_run)
                {
                    InvokePendingOperations();
                    _operationsAvailable.WaitOne();
                }

                InvokePendingOperations();
            }

            private void InvokePendingOperations()
            {
                while (_operations.Count > 0)
                {
                    AsyncOperation operation = (AsyncOperation)_operations.Dequeue();
                    operation.Invoke();
                }
            }
        }

        private class AsyncOperation
        {
            private readonly SendOrPostCallback _action;
            private readonly object _state;

            public AsyncOperation(SendOrPostCallback action, object state)
            {
                _action = action;
                _state = state;
            }

            public void Invoke()
            {
                _action(_state);
            }
        }
    }
}
#endif