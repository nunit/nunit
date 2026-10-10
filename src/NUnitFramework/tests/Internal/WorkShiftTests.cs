// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    public class WorkShiftTests
    {
        private WorkShift _shift;

        [SetUp]
        public void CreateShift()
        {
            _shift = new WorkShift("dummy");
        }

        [Test]
        public void InitialState()
        {
            Assert.That(_shift.IsActive, Is.False, "Should not be active");
            Assert.That(_shift.Queues, Is.Empty);
        }

        [Test]
        public void StartShift()
        {
            _shift.Start();
            Assert.That(_shift.IsActive, Is.True, "Should be active");
        }

        private static WorkItemQueue CreateQueue(string name)
        {
            return new WorkItemQueue(name, true, ApartmentState.MTA);
        }

        [Test]
        public void AddQueue()
        {
            _shift.AddQueue(CreateQueue("test"));
            Assert.That(_shift.IsActive, Is.False, "Should not be active");
            Assert.That(_shift.Queues, Has.Count.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Paused));
        }

        [Test]
        public void AddQueueThenStart()
        {
            _shift.AddQueue(CreateQueue("test"));
            _shift.Start();
            Assert.That(_shift.IsActive, Is.True, "Should be active");
            Assert.That(_shift.Queues, Has.Count.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void StartShiftThenAddQueue()
        {
            _shift.Start();
            _shift.AddQueue(CreateQueue("test"));
            Assert.That(_shift.IsActive, Is.True, "Should be active");
            Assert.That(_shift.Queues, Has.Count.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void AddQueueThenStartThenAddQueue()
        {
            _shift.AddQueue(CreateQueue("test"));
            _shift.Start();
            _shift.AddQueue(CreateQueue("test"));
            Assert.That(_shift.IsActive, Is.True, "Should be active");
            Assert.That(_shift.Queues, Has.Count.EqualTo(2));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
            Assert.That(_shift.Queues[1].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void HasWorkTest()
        {
            var q = CreateQueue("test");
            _shift.AddQueue(q);
            Assert.That(_shift.HasWork, Is.False, "Should not have work initially");
            q.Enqueue(Fakes.GetWorkItem(this, nameof(Test1)));
            Assert.That(_shift.HasWork, "Should have work after enqueue");
            _shift.Start();
            Assert.That(_shift.HasWork, "Should have work after starting");
        }

        private void Test1()
        {
        }

        private class BusyWorkerFixture : IDisposable
        {
            public const int Timeout = 10_000;

            public ManualResetEventSlim Started { get; } = new ManualResetEventSlim(initialState: false);
            public ManualResetEventSlim Finish { get; } = new ManualResetEventSlim(initialState: false);

            public void Dispose()
            {
                Started.Dispose();
                Finish.Dispose();
            }

            public void RunUntilToldToFinish()
            {
                Started.Set();
                Finish.Wait(Timeout);
            }
        }

        [Test]
        public void HasWorkWhenWorkerIsBusy()
        {
            using var fixture = new BusyWorkerFixture();
            using var endOfShift = new ManualResetEventSlim(initialState: false);

            var q = CreateQueue("test");
            var w = new TestWorker(q, "test-worker");

            _shift.EndOfShift += OnEndOfShift;

            void OnEndOfShift(WorkShift shift) => endOfShift.Set();

            _shift.AddQueue(q);
            _shift.Assign(w);

            try
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(q.IsEmpty, Is.True, "Queue should be empty initially");
                    Assert.That(w.IsBusy, Is.False, "Worker should not be busy initially");
                    Assert.That(_shift.HasWork, Is.False, "Shift should not have work initially");
                }

                q.Enqueue(Fakes.GetWorkItem(fixture.RunUntilToldToFinish));
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(q.IsEmpty, Is.False, "Queue has work");
                    Assert.That(w.IsBusy, Is.False, "Worker has not started");
                    Assert.That(_shift.HasWork, Is.True, "Shift should have work after enqueue");
                }

                _shift.Start();

                // Wait for worker to start and pick up the work item
                Assert.That(fixture.Started.Wait(BusyWorkerFixture.Timeout), Is.True, "Worker did not start in time");

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(q.IsEmpty, Is.True, "Queue should be empty again");
                    Assert.That(w.IsBusy, Is.True, "Worker has started");
                    Assert.That(_shift.HasWork, Is.True, "Shift has work, despite the empty queue");
                }

                // Tell the worker to finish
                fixture.Finish.Set();

                // Wait for the shift to end
                Assert.That(endOfShift.Wait(BusyWorkerFixture.Timeout), Is.True, "Shift did not end in time");
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(q.IsEmpty, Is.True, "Queue should still be empty");
                    Assert.That(w.IsBusy, Is.False, "Worker has finished");
                    Assert.That(_shift.HasWork, Is.False, "Shift should not have work after worker finishes");
                }

                _shift.ShutDown();
                Assert.That(() => w.IsAlive, Is.False.After(BusyWorkerFixture.Timeout, 100), "Worker should be done after shutdown");
            }
            finally
            {
                _shift.EndOfShift -= OnEndOfShift;
            }
        }
    }
}
