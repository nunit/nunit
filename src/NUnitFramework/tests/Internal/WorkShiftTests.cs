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
            q.Enqueue(Fakes.GetWorkItem(this, "Test1"));
            Assert.That(_shift.HasWork, "Should have work after enqueue");
            _shift.Start();
            Assert.That(_shift.HasWork, "Should have work after starting");
        }

        [Test]
        public void StartTwiceIsIgnored()
        {
            var q = CreateQueue("test");
            _shift.AddQueue(q);

            _shift.Start();
            q.Pause();
            _shift.Start();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(_shift.IsActive, Is.True, "Shift should still be active");
                Assert.That(q.State, Is.EqualTo(WorkItemQueueState.Paused), "Second Start should not restart the queues");
            }
        }

        [Test]
        public void EndShiftTwiceRaisesEndOfShiftOnce()
        {
            var q = CreateQueue("test");
            _shift.AddQueue(q);
            int endOfShiftCount = 0;
            _shift.EndOfShift += _ => endOfShiftCount++;

            _shift.Start();
            _shift.EndShift();
            q.Start();
            _shift.EndShift();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(endOfShiftCount, Is.EqualTo(1), "EndOfShift should be raised once");
                Assert.That(_shift.IsActive, Is.False, "Shift should not be active");
                Assert.That(q.State, Is.EqualTo(WorkItemQueueState.Running), "Second EndShift should not pause the queues");
            }
        }

        [Test]
        public void EndShiftWithoutStartIsIgnored()
        {
            int endOfShiftCount = 0;
            _shift.EndOfShift += _ => endOfShiftCount++;

            _shift.EndShift();

            Assert.That(endOfShiftCount, Is.Zero, "EndOfShift should not be raised for an inactive shift");
        }

        [Test]
        public void ShutDownActiveShift()
        {
            var q = CreateQueue("test");
            _shift.AddQueue(q);
            int endOfShiftCount = 0;
            _shift.EndOfShift += _ => endOfShiftCount++;

            _shift.Start();
            _shift.ShutDown();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(_shift.IsActive, Is.False, "Shift should not be active after shutdown");
                Assert.That(q.State, Is.EqualTo(WorkItemQueueState.Stopped), "Queues should be stopped");
                Assert.That(endOfShiftCount, Is.Zero, "ShutDown should not raise EndOfShift");
            }

            _shift.EndShift();
            Assert.That(endOfShiftCount, Is.Zero, "EndShift after ShutDown should be ignored");
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
                // Always release and stop the worker. TestWorker threads are foreground
                // threads, so a worker left waiting on its queue would keep the process alive.
                fixture.Finish.Set();
                _shift.ShutDown();
                _shift.EndOfShift -= OnEndOfShift;
            }
        }

        [Test]
        public void ShiftDoesNotEndWhileDequeuedItemIsWaitingForBusy()
        {
            // Regression test for #5447: a worker that has dequeued an item but not yet
            // been counted as busy by the shift must still count as work.
            const int timeout = BusyWorkerFixture.Timeout;

            using var blockedInBusy = new ManualResetEventSlim(initialState: false);
            using var releaseBusy = new ManualResetEventSlim(initialState: false);
            using var otherWorkerIdle = new ManualResetEventSlim(initialState: false);
            using var endOfShift = new ManualResetEventSlim(initialState: false);

            var q = CreateQueue("test");
            var workers = new[] { new TestWorker(q, "worker-1"), new TestWorker(q, "worker-2") };
            int blockOnce = 0;
            TestWorker? blockedWorker = null;

            void BlockFirstWorkerInBusy(TestWorker worker, WorkItem work)
            {
                if (Interlocked.Exchange(ref blockOnce, 1) == 0)
                {
                    Volatile.Write(ref blockedWorker, worker);
                    blockedInBusy.Set();
                    releaseBusy.Wait(timeout);
                }
            }

            void SignalOtherWorkerIdle(TestWorker worker, WorkItem work)
            {
                if (!ReferenceEquals(worker, Volatile.Read(ref blockedWorker)))
                    otherWorkerIdle.Set();
            }

            void OnEndOfShift(WorkShift shift) => endOfShift.Set();

            _shift.AddQueue(q);
            foreach (var worker in workers)
            {
                _shift.Assign(worker);

                // Subscribed before the shift subscribes its own handlers in Start(),
                // so this runs before the shift counts the worker as busy.
                worker.Busy += BlockFirstWorkerInBusy;
            }

            _shift.EndOfShift += OnEndOfShift;

            try
            {
                _shift.Start();

                // Subscribed after the shift's handlers, so this runs once the
                // shift has decided whether to end.
                foreach (var worker in workers)
                    worker.Idle += SignalOtherWorkerIdle;

                q.Enqueue(Fakes.GetWorkItem(this, "Test1"));
                q.Enqueue(Fakes.GetWorkItem(this, "Test1"));

                Assert.That(blockedInBusy.Wait(timeout), Is.True, "No worker got blocked in Busy");
                Assert.That(otherWorkerIdle.Wait(timeout), Is.True, "Other worker did not become idle");

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(q.IsEmpty, Is.True, "Queue should be drained");
                    Assert.That(endOfShift.IsSet, Is.False, "Shift ended while a dequeued item was not yet executed");
                    Assert.That(_shift.IsActive, Is.True, "Shift should still be active");
                }

                releaseBusy.Set();

                Assert.That(endOfShift.Wait(timeout), Is.True, "Shift did not end after the blocked worker completed");
            }
            finally
            {
                releaseBusy.Set();
                _shift.ShutDown();
                _shift.EndOfShift -= OnEndOfShift;
            }
        }
    }
}
