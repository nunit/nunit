// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Execution
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
            Assert.False(_shift.IsActive, "Should not be active");
            Assert.That(_shift.Queues.Count, Is.EqualTo(0));
        }

        [Test]
        public void StartShift()
        {
            _shift.Start();
            Assert.True(_shift.IsActive, "Should be active");
        }

        private static WorkItemQueue CreateQueue(string name)
        {
            return new WorkItemQueue(name, true, ApartmentState.MTA);
        }

        [Test]
        public void AddQueue()
        {
            _shift.AddQueue(CreateQueue("test"));
            Assert.False(_shift.IsActive, "Should not be active");
            Assert.That(_shift.Queues.Count, Is.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Paused));
        }

        [Test]
        public void AddQueueThenStart()
        {
            _shift.AddQueue(CreateQueue("test"));
            _shift.Start();
            Assert.True(_shift.IsActive, "Should be active");
            Assert.That(_shift.Queues.Count, Is.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void StartShiftThenAddQueue()
        {
            _shift.Start();
            _shift.AddQueue(CreateQueue("test"));
            Assert.True(_shift.IsActive, "Should be active");
            Assert.That(_shift.Queues.Count, Is.EqualTo(1));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void AddQueueThenStartThenAddQueue()
        {
            _shift.AddQueue(CreateQueue("test"));
            _shift.Start();
            _shift.AddQueue(CreateQueue("test"));
            Assert.True(_shift.IsActive, "Should be active");
            Assert.That(_shift.Queues.Count, Is.EqualTo(2));
            Assert.That(_shift.Queues[0].State, Is.EqualTo(WorkItemQueueState.Running));
            Assert.That(_shift.Queues[1].State, Is.EqualTo(WorkItemQueueState.Running));
        }

        [Test]
        public void HasWorkTest()
        {
            var q = CreateQueue("test");
            _shift.AddQueue(q);
            Assert.False(_shift.HasWork, "Should not have work initially");
            q.Enqueue(Fakes.GetWorkItem(this, "Test1"));
            Assert.That(_shift.HasWork, "Should have work after enqueue");
            _shift.Start();
            Assert.That(_shift.HasWork, "Should have work after starting");
        }

        private void Test1() { }
    }
}
