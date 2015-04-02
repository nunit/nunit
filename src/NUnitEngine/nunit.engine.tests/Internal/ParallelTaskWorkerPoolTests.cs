using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Engine.Internal;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
    class ParallelTaskWorkerPoolTests
    {
        [TestCase(-1)]
        [TestCase(0)]
        public void RequiresAtleastOneThread(int numThreads)
        {
            Assert.Throws<ArgumentException>(() => new ParallelTaskWorkerPool(numThreads));
        }

        [Test]
        public void EnqueueCannotBeCalledAfterWorkHasStarted()
        {
            var workerPool = new ParallelTaskWorkerPool(1);
            workerPool.Start();

            Assert.Throws<InvalidOperationException>(() => workerPool.Enqueue(new NoOpTask()));
        }

        [Test]
        public void WaitAll_SingleTask()
        {
            var workerPool = new ParallelTaskWorkerPool(1);
            var task = new BusyTask();
            workerPool.Enqueue(task);
            workerPool.Start();

            Assert.IsFalse(workerPool.WaitAll(TimeSpan.FromMilliseconds(10)),
                "Threads should not have exited, work is in progress");

            task.MarkTaskAsCompleted();
            
            Assert.IsTrue(workerPool.WaitAll(TimeSpan.FromMilliseconds(100)),
                "Threads should have exited, all work is complete");
        }

        [Test]
        public void WaitAll_SingleThread_MultipleTasks()
        {
            var workerPool = new ParallelTaskWorkerPool(1);
            var task1 = new BusyTask();
            var task2 = new BusyTask();
            workerPool.Enqueue(task1);
            workerPool.Enqueue(task2);
            workerPool.Start();

            Assert.IsFalse(workerPool.WaitAll(TimeSpan.FromMilliseconds(10)),
                "Threads should not have exited, 2 tasks are in progress");

            Assert.AreEqual(BusyTaskState.Executing, task1.State);
            Assert.AreEqual(BusyTaskState.Queued, task2.State);

            task1.MarkTaskAsCompleted();

            Assert.IsFalse(workerPool.WaitAll(TimeSpan.FromMilliseconds(10)),
                "Threads should not have exited, 1 task is in progress");

            Assert.AreEqual(BusyTaskState.Completed, task1.State);
            Assert.AreEqual(BusyTaskState.Executing, task2.State);

            task2.MarkTaskAsCompleted();

            Assert.IsTrue(workerPool.WaitAll(TimeSpan.FromMilliseconds(100)),
                "Threads should have exited, all work is complete");

            Assert.AreEqual(BusyTaskState.Completed, task1.State);
            Assert.AreEqual(BusyTaskState.Completed, task2.State);
        }

        [Test]
        public void WaitAll_TwoThreads_MultipleTasks()
        {
            var workerPool = new ParallelTaskWorkerPool(2);
            var task1 = new BusyTask();
            var task2 = new BusyTask();
            workerPool.Enqueue(task1);
            workerPool.Enqueue(task2);
            workerPool.Start();

            Assert.IsFalse(workerPool.WaitAll(TimeSpan.FromMilliseconds(10)),
                "Threads should not have exited, 2 tasks are in progress");

            Assert.AreEqual(BusyTaskState.Executing, task1.State);
            Assert.AreEqual(BusyTaskState.Executing, task2.State);

            task1.MarkTaskAsCompleted();

            Assert.IsFalse(workerPool.WaitAll(TimeSpan.FromMilliseconds(10)),
                "Threads should not have exited, 1 task is in progress");

            Assert.AreEqual(BusyTaskState.Completed, task1.State);
            Assert.AreEqual(BusyTaskState.Executing, task2.State);

            task2.MarkTaskAsCompleted();

            Assert.IsTrue(workerPool.WaitAll(TimeSpan.FromMilliseconds(100)),
                "Threads should have exited, all work is complete");

            Assert.AreEqual(BusyTaskState.Completed, task1.State);
            Assert.AreEqual(BusyTaskState.Completed, task2.State);
        }

        private class NoOpTask : ITask 
        {
            public void Execute() { }
        }

        private enum BusyTaskState
        {
            Queued,
            Executing,
            Completed,
        }

        private class BusyTask : ITask
        {
            private readonly Semaphore _semaphore;
            public BusyTaskState State { get; private set; }

            public BusyTask()
            {
                _semaphore = new Semaphore(0, 1);
                State = BusyTaskState.Queued;
            }

            public void Execute()
            {
                State = BusyTaskState.Executing;
                _semaphore.WaitOne();
                State = BusyTaskState.Completed;
            }

            public void MarkTaskAsCompleted()
            {
                _semaphore.Release(1);
            }
        }
    }
}
