// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
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
            var workers = new[]
            {
                new TestWorker(_queue, "1"),
                new TestWorker(_queue, "2"),
                new TestWorker(_queue, "3")
            };

            foreach (var worker in workers)
            {
                worker.Start();
                Assert.That(worker.IsAlive, $"Worker thread {worker.Name} did not start");
            }

            _queue.Start();
            _queue.Stop();
            Assert.That(_queue.State, Is.EqualTo(WorkItemQueueState.Stopped));

            int iters = 10;
            int alive = workers.Length;

            while (iters-- > 0 && alive > 0)
            {
                Thread.Sleep(60);  // Allow time for workers to stop

                alive = workers.Count(worker => worker.IsAlive);
            }

            if (alive > 0)
            {
                foreach (var worker in workers)
                    Assert.That(worker.IsAlive, Is.False, $"Worker thread {worker.Name} did not stop");
            }
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
            string[] names = new[] { "Test1", "Test2", "Test3" };

            EnqueueWorkItems(names);
            _queue.Start();
            VerifyQueueContents(names);
        }

        [Test]
        public void DequeueBeforeEnqueue()
        {
            _queue.Start();
            var names = new[] { "Test1", "Test2", "Test3" };

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(10);
                EnqueueWorkItems(names);
            })).Start();

            VerifyQueueContents(names);
        }

        [Test]
        public void EnqueueAndDequeueWhilePaused()
        {
            string[] names = new[] { "Test1", "Test2", "Test3" };
            EnqueueWorkItems(names);

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(10);
                _queue.Start();
            })).Start();

            VerifyQueueContents(names);
        }

        private const int HIGH_PRIORITY = 0;
        private const int NORMAL_PRIORITY = 1;

        [Test]
        public void PriorityIsHonored()
        {
            EnqueueWorkItem("Test1", NORMAL_PRIORITY);
            EnqueueWorkItem("Test2", HIGH_PRIORITY);
            EnqueueWorkItem("Test3", NORMAL_PRIORITY);
            _queue.Start();
            VerifyQueueContents("Test2", "Test1", "Test3");
        }

        [Test]
        public void OneTimeTearDownGetsPriority()
        {
            var testFixture = new TestFixture(new TypeWrapper(typeof(MyFixture)));

            var fixtureItem = WorkItemBuilder.CreateWorkItem(testFixture, TestFilter.Empty, new DebuggerProxy());
            Assert.That(fixtureItem, Is.Not.Null);
            // TODO: Remove '!' when https://github.com/nunit/nunit.analyzers/issues/535 is released
            var tearDown = new CompositeWorkItem.OneTimeTearDownWorkItem((CompositeWorkItem)fixtureItem!);
            EnqueueWorkItem("Test1");
            _queue.Enqueue(tearDown);
            EnqueueWorkItem("Test2");
            _queue.Start();
            VerifyQueueContents("WorkItemQueueTests+MyFixture", "Test1", "Test2");
        }

        private void EnqueueWorkItems(params string[] names)
        {
            foreach (string name in names)
                EnqueueWorkItem(name);
        }

        private void EnqueueWorkItem(string name)
        {
            _queue.Enqueue(Fakes.GetWorkItem(this, name));
        }

        private void EnqueueWorkItem(string name, int priority)
        {
            _queue.Enqueue(Fakes.GetWorkItem(this, name), priority);
        }

        private void VerifyQueueContents(params string[] names)
        {
            foreach (string name in names)
                Assert.That(_queue.Dequeue()?.Test.Name, Is.EqualTo(name));
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

        private class MyFixture
        {
        }
    }
}
