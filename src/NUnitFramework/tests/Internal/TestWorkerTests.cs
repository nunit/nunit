// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text;
using System.Threading;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
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

            _worker.Busy += (s, ea) => sb.Append("Busy");
            work.Executed += (s, ea) => sb.Append("Exec");
            _worker.Idle += (s, ea) => sb.Append("Idle");

            _queue.Enqueue(work);
            _worker.Start();
            _queue.Start();

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            // TODO: Remove when https://github.com/nunit/nunit.analyzers/issues/982 is released
            Assert.That(() => sb.ToString(), Is.EqualTo("BusyExecIdle").After(
                delayInMilliseconds: 10_000, pollingInterval: 200));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
        }

        private void FakeMethod()
        {
        }
    }
}
