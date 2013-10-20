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
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// A TestWorker pulls work items from a queue
    /// and executes them.
    /// </summary>
    public class TestWorker
    {
        static Logger log = InternalTrace.GetLogger("TestWorker");

        private WorkItemQueue _readyQueue;
        private Thread _workerThread;
        private ApartmentState _apartmentState;

        private int _workItemCount = 0;

        /// <summary>
        /// Construct a new TestWorker.
        /// </summary>
        /// <param name="queue">The queue from which to pull work items</param>
        /// <param name="name">The name of this worker</param>
        /// <param name="apartmentState">The apartment state to use for running tests</param>
        public TestWorker(WorkItemQueue queue, string name, ApartmentState apartmentState)
        {
            _readyQueue = queue;
            _apartmentState = apartmentState;

            _workerThread = new Thread(new ThreadStart(TestWorkerThreadProc));
            _workerThread.Name = name;
            _workerThread.SetApartmentState(apartmentState);
        }

        /// <summary>
        /// Our ThreadProc, which pulls and runs tests in a loop
        /// </summary>
        void TestWorkerThreadProc()
        {
            for(;;)
            {
                var workItem = _readyQueue.Dequeue();
                if (workItem == null)
                    break;

                log.Debug("Processing WorkItem for {0}", workItem.Test.FullName);
                workItem.Execute();
                ++_workItemCount;
            }

            log.Info("Stopping - {0} WorkItems processed.", _workItemCount);
        }

        /// <summary>
        /// Start processing work items.
        /// </summary>
        public void Start()
        {
            _workerThread.Start();
        }
    }
}
#endif
