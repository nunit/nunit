// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    public class WorkItemTests
    {
        private WorkItem _workItem;
        private TestExecutionContext _context;

        [SetUp]
        public void CreateWorkItems()
        {
            IMethodInfo method = new MethodWrapper(typeof(DummyFixture), "DummyTest");
            ITest test = new TestMethod(method);
            _workItem = WorkItem.CreateWorkItem(test, TestFilter.Empty);

            _context = new TestExecutionContext();
            _workItem.InitializeContext(_context);
        }

        [Test]
        public void ConstructWorkItem()
        {
            Assert.That(_workItem, Is.TypeOf<SimpleWorkItem>());
            Assert.That(_workItem.Test.Name, Is.EqualTo("DummyTest"));
            Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Ready));
        }

        [Test]
        public void ExecuteWorkItem()
        {
            _workItem.Execute();

            Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Complete));
            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(_context.ExecutionStatus, Is.EqualTo(TestExecutionStatus.Running));
        }

        [Test]
        public void CanStopRun()
        {
            _context.ExecutionStatus = TestExecutionStatus.StopRequested;
            _workItem.Execute();
            Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Complete));
            Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(_context.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
        }

        Thread _thread;

        private void StartExecution()
        {
            _thread = new Thread(ThreadProc);
            _thread.Start();
        }

        private void ThreadProc()
        {
            _workItem.Execute();
        }

        // Use static for simplicity
        static class DummyFixture
        {
            public static int Delay = 0;

            public static void DummyTest()
            {
                if (Delay > 0)
                    Thread.Sleep(Delay);
            }
        }
    }
}
