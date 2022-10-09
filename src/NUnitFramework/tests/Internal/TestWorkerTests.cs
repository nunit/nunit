// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    public class TestWorkerTests
    {
        private WorkItemQueue _queue;
        private TestWorker _worker;

        [SetUp]
        public void SetUp()
        {
            _queue = new WorkItemQueue("TestQ", true, ApartmentState.MTA);
            _worker = new TestWorker(_queue, "TestQ_Worker");
        }

        [TearDown]
        public void TearDown()
        {
            _queue.Stop();
        }

        [Test]
        public void BusyExecuteIdleEventsCalledInSequence()
        {
            StringBuilder sb = new StringBuilder();
            FakeWorkItem work = Fakes.GetWorkItem(this, "FakeMethod");

            _worker.Busy += (s, ea) => { sb.Append("Busy"); };
            work.Executed += (s, ea) => { sb.Append("Exec"); };
            _worker.Idle += (s, ea) => { sb.Append ("Idle"); };

            _queue.Enqueue(work);
            _worker.Start();
            _queue.Start();

            Assert.That(() => sb.ToString(), Is.EqualTo("BusyExecIdle").After(
                delayInMilliseconds: 10_000, pollingInterval: 200));
        }

        private void FakeMethod()
        {
        }
    }
}
