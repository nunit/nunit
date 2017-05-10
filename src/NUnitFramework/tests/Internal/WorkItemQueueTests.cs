// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Threading;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Execution
{
    public class WorkItemQueueTests
    {
        private WorkItemQueue _queue;

        [SetUp]
        public void CreateQueue()
        {
            _queue = new WorkItemQueue("TestQ", true, ApartmentState.MTA);
        }

        [Test]
        public void InitialState()
        {
            Assert.That(_queue.Name, Is.EqualTo("TestQ"));
            Assert.That(_queue.IsEmpty, "Queue is not empty");
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Paused));
        }

        [Test]
        public void StartQueue()
        {
            _queue.Start();
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void StopQueue_NoWorkers()
        {
            _queue.Start();
            _queue.Stop();
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Stopped));
        }

        [Test]
        public void StopQueue_WithWorkers()
        {
            var dispatcher = new ParallelWorkItemDispatcher(2);
            var workers = new TestWorker[]
            {
                new TestWorker(dispatcher, _queue, "1"),
                new TestWorker(dispatcher, _queue, "2"),
                new TestWorker(dispatcher, _queue, "3")
            };

            foreach (var worker in workers)
            {
                worker.Start();
                Assert.That(worker.IsAlive, "Worker thread {0} did not start", worker.Name);
            }

            _queue.Start();
            _queue.Stop();
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Stopped));

            int iters = 10;
            int alive = workers.Length;

            while (iters-- > 0 && alive > 0)
            {
                Thread.Sleep(60);  // Allow time for workers to stop

                alive = 0;
                foreach (var worker in workers)
                    if (worker.IsAlive)
                        alive++;
            }

            if (alive > 0)
                foreach (var worker in workers)
                    Assert.False(worker.IsAlive, "Worker thread {0} did not stop", worker.Name);
        }

        [Test]
        public void PauseQueue()
        {
            _queue.Start();
            _queue.Pause();
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Paused));
        }

        [Test]
        public void EnqueueBeforeDequeue()
        {
            EnqueueWorkItems();
            _queue.Start();
            VerifyQueueContents();
        }

        [Test]
        public void DequeueBeforeEnqueue()
        {
            _queue.Start();
            new Thread(new ThreadStart(EnqueueWorkItemsAfterWait)).Start();
            VerifyQueueContents();
        }

        [Test]
        public void EnqueueAndDequeueWhilePaused()
        {
            EnqueueWorkItems();
            new Thread(new ThreadStart(ReleasePauseAfterWait)).Start();
            VerifyQueueContents();
        }

        private void EnqueueWorkItems()
        {
            _queue.Enqueue(Fakes.GetWorkItem(this, "Test1"));
            _queue.Enqueue(Fakes.GetWorkItem(this, "Test2"));
            _queue.Enqueue(Fakes.GetWorkItem(this, "Test3"));
        }

        private void EnqueueWorkItemsAfterWait()
        {
            Thread.Sleep(10);
            EnqueueWorkItems();
        }

        private void ReleasePauseAfterWait()
        {
            Thread.Sleep(10);
            _queue.Start();
        }

        private void VerifyQueueContents()
        {
            Assert.That(_queue.Dequeue().Test.Name, Is.EqualTo("Test1"));
            Assert.That(_queue.Dequeue().Test.Name, Is.EqualTo("Test2"));
            Assert.That(_queue.Dequeue().Test.Name, Is.EqualTo("Test3"));
        }

        private void Test1()
        {
        }

        private void Test2()
        {
        }

        private void Test3()
        {
        }
    }
}

#endif
