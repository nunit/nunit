// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    public class WorkShiftTests
    {
        private WorkShift? _shift;

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

        private void Test1()
        {
        }
    }
}
