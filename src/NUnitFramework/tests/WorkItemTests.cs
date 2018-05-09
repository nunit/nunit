// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;
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
            FixtureMethod method = typeof(DummyFixture).GetFixtureMethod("DummyTest");
            ITest test = new TestMethod(method);
            _workItem = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty);

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
            public static readonly int Delay = 0;

            public static void DummyTest()
            {
                if (Delay > 0)
                    Thread.Sleep(Delay);
            }
        }


#if APARTMENT_STATE

        [TestCaseSource(nameof(GetTargetApartmentTestData))]
        public void GetsTargetApartmentFromParentTests(Test test, ApartmentState expected)
        {
            var work = new FakeWorkItem(test, TestFilter.Empty);

            Assert.That(work.TargetApartment, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(GetTargetApartmentTestData))]
        public void GetsTargetApartmentFromParentTestsInWrappedTests(Test test, ApartmentState expected)
        {
            var work = new FakeWorkItem(test, TestFilter.Empty);
            var wrapped = new FakeWorkItem(work);

            Assert.That(wrapped.TargetApartment, Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> GetTargetApartmentTestData()
        {
            yield return new TestCaseData(CreateFakeTests(ApartmentState.Unknown, ApartmentState.Unknown, ApartmentState.Unknown), ApartmentState.Unknown);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.Unknown, ApartmentState.Unknown, ApartmentState.STA), ApartmentState.STA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.Unknown, ApartmentState.STA, ApartmentState.Unknown), ApartmentState.STA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.STA, ApartmentState.Unknown, ApartmentState.Unknown), ApartmentState.STA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.Unknown, ApartmentState.Unknown, ApartmentState.MTA), ApartmentState.MTA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.Unknown, ApartmentState.MTA, ApartmentState.Unknown), ApartmentState.MTA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.MTA, ApartmentState.Unknown, ApartmentState.Unknown), ApartmentState.MTA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.STA, ApartmentState.MTA, ApartmentState.Unknown), ApartmentState.MTA);
            yield return new TestCaseData(CreateFakeTests(ApartmentState.MTA, ApartmentState.STA, ApartmentState.Unknown), ApartmentState.STA);
        }

        static ITest CreateFakeTests(ApartmentState assemblyApartment, ApartmentState fixtureApartment, ApartmentState methodApartment) =>
            new FakeTest("Method", methodApartment)
            {
                Parent = new FakeTest("Fixture", fixtureApartment)
                {
                    Parent  = new FakeTest("Assembly", assemblyApartment)
                }
            };

        class FakeTest : Test
        {
            public FakeTest(string name, ApartmentState apartmentState) : base(name)
            {
                if (apartmentState != ApartmentState.Unknown)
                    Properties.Add(PropertyNames.ApartmentState, apartmentState);
            }

            public override object[] Arguments
            {
                get { throw new System.NotImplementedException(); }
            }

            public override string XmlElementName => "MockTest";

            public override bool HasChildren => Tests.Count > 0;

            public override IList<ITest> Tests => new List<ITest>();

            public override TNode AddToXml(TNode parentNode, bool recursive)
            {
                throw new System.NotImplementedException();
            }

            public override TestResult MakeTestResult() => new FakeTestResult(this);
        }

        class FakeTestResult : TestResult
        {
            public FakeTestResult(ITest test) : base(test)
            {
            }

            public override int FailCount => 0;

            public override int WarningCount => 0;

            public override int PassCount => 1;

            public override int SkipCount => 0;

            public override int InconclusiveCount => 0;

            public override bool HasChildren => false;

            public override IEnumerable<ITestResult> Children => null;
        }

        class FakeWorkItem : WorkItem
        {
            public FakeWorkItem(WorkItem wrappedItem) : base(wrappedItem)
            {
            }

            public FakeWorkItem(Test test, ITestFilter filter) : base(test, filter)
            {
            }

            protected override void PerformWork()
            {
                throw new System.NotImplementedException();
            }
        }
#endif
    }
}
