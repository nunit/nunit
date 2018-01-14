// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using System;
using System.Collections.Generic;
using System.Text;
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
#if APARTMENT_STATE
            return new WorkItemQueue(name, true, ApartmentState.MTA);
#else
            return new WorkItemQueue(name, true);
#endif
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
#endif
