// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.TestUtilities;

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
            Test test = new TestMethod(method);

            _context = new TestExecutionContext();

            _workItem = TestBuilder.CreateWorkItem(test, _context);
        }

        [Test]
        public void ConstructWorkItem()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_workItem, Is.TypeOf<SimpleWorkItem>());
                Assert.That(_workItem.Test.Name, Is.EqualTo("DummyTest"));
                Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Ready));
            });
        }

        [Test]
        public void ExecuteWorkItem()
        {
            _workItem.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Complete));
                Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(_context.ExecutionStatus, Is.EqualTo(TestExecutionStatus.Running));
            });
        }

        [Test]
        public void CanStopRun()
        {
            _context.ExecutionStatus = TestExecutionStatus.StopRequested;
            _workItem.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(_workItem.State, Is.EqualTo(WorkItemState.Complete));
                Assert.That(_context.CurrentResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(_context.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
            });
        }

        // Use static for simplicity
        private static class DummyFixture
        {
            public static readonly int Delay = 0;

            public static void DummyTest()
            {
                if (Delay > 0)
                    Thread.Sleep(Delay);
            }
        }


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

        private static IEnumerable<TestCaseData> GetTargetApartmentTestData()
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

        private static ITest CreateFakeTests(ApartmentState assemblyApartment, ApartmentState fixtureApartment, ApartmentState methodApartment) =>
            new FakeTest("Method", methodApartment)
            {
                Parent = new FakeTest("Fixture", fixtureApartment)
                {
                    Parent = new FakeTest("Assembly", assemblyApartment)
                }
            };

        private class FakeTest : Test
        {
            public FakeTest(string name, ApartmentState apartmentState) : base(name)
            {
                if (apartmentState != ApartmentState.Unknown)
                    Properties.Add(PropertyNames.ApartmentState, apartmentState);
            }

            public override object[] Arguments => throw new System.NotImplementedException();

            public override string XmlElementName => "MockTest";

            public override bool HasChildren => Tests.Count > 0;

            public override IList<ITest> Tests => new List<ITest>();

            public override TNode AddToXml(TNode parentNode, bool recursive)
            {
                throw new System.NotImplementedException();
            }

            public override TestResult MakeTestResult() => new FakeTestResult(this);
        }

        private class FakeTestResult : TestResult
        {
            public FakeTestResult(ITest test) : base(test)
            {
            }

            public override int TotalCount => 1;

            public override int FailCount => 0;

            public override int WarningCount => 0;

            public override int PassCount => 1;

            public override int SkipCount => 0;

            public override int InconclusiveCount => 0;

            public override bool HasChildren => false;

            public override IEnumerable<ITestResult> Children => Enumerable.Empty<ITestResult>();
        }

        private class FakeWorkItem : WorkItem
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
    }
}
