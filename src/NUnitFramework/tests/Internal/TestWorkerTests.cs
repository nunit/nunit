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
using System.Text;
using System.Threading;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Execution
{
    public class TestWorkerTests
    {
        private WorkItemQueue _queue;
        private TestWorker _worker;

        [SetUp]
        public void SetUp()
        {
#if APARTMENT_STATE
            _queue = new WorkItemQueue("TestQ", true, ApartmentState.MTA);
#else
            _queue = new WorkItemQueue("TestQ", true);
#endif
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
                delayInMilliseconds: 10000, pollingInterval: 200));
        }

        private void FakeMethod()
        {
        }
    }
}

#endif
