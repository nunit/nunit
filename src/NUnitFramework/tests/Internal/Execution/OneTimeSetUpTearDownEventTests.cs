using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Legacy;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.TestFixtureTests;

namespace NUnit.Framework.Tests.Internal.Execution
{
    [TestFixtureSource(nameof(GetOneTimeSetUpTearDownSuites))]
    [NonParallelizable]
    public class OneTimeSetUpTearDownEventTests : ITestListener
    {
        private readonly TestSuite _testSuite;

        private ConcurrentQueue<TestEvent> _events;

        private IEnumerable<TestEvent> AllEvents => _events.AsEnumerable();

        public OneTimeSetUpTearDownEventTests(TestSuite testSuite)
        {
            _testSuite = testSuite;
        }

        [OneTimeSetUp]
        public void RunTestSuite()
        {
            _events = new ConcurrentQueue<TestEvent>();

            var dispatcher = new ParallelWorkItemDispatcher(4);
            var context = new TestExecutionContext();
            context.Dispatcher = dispatcher;
            context.Listener = this;

            var workItem = TestBuilder.CreateWorkItem(_testSuite, context);

            dispatcher.Start(workItem);
            workItem.WaitForCompletion();
        }

        [Test]
        public void TestEventsCompleteAndInRightOrder()
        {
            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // TestStarting fake-assembly.dll
                new TestEvent() { Action = TestAction.TestStarting },               // TestStarting NUnit
                new TestEvent() { Action = TestAction.TestStarting },               // TestStarting Tests
                new TestEvent() { Action = TestAction.TestStarting },               // TestStarting TestFixtureWithOneTimeSetUpTearDown
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // From OneTimeSetUp in Baseclass
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // From OneTimeSetUp in Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // MyTest1
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // MyTest2
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // From OneTimeTearDown in Fixture
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestFinished },
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        #region Test Data

        private static IEnumerable<TestFixtureData> GetOneTimeSetUpTearDownSuites()
        {
            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixtureWithOneTimeSetUpTearDown))))));
        }

        #endregion

        #region ITestListener implementation

        void ITestListener.TestStarted(ITest test)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.TestStarting,
                TestName = test.Name,
                ThreadName = Thread.CurrentThread.Name ?? "NoThreadName",
            });
        }

        void ITestListener.TestFinished(ITestResult result)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.TestFinished,
                TestName = result.Name,
                Result = result.ResultState.ToString(),
                ThreadName = Thread.CurrentThread.Name ?? "NoThreadName",
            });
        }

        void ITestListener.TestOutput(TestOutput output)
        {
        }

        void ITestListener.SendMessage(TestMessage message)
        {
        }

        void ITestListener.OneTimeSetUpStarted(ITest test)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.OneTimeSetUpStarted,
            });
        }

        void ITestListener.OneTimeSetUpFinished(ITest test)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.OneTimeSetUpFinished,
            });
        }

        void ITestListener.OneTimeTearDownStarted(ITest test)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.OneTimeTearDownStarted,
            });
        }

        void ITestListener.OneTimeTearDownFinished(ITest test)
        {
            _events.Enqueue(new TestEvent()
            {
                Action = TestAction.OneTimeTearDownFinished,
            });
        }

        #endregion

        #region Helper Methods

        private static TestSuite Suite(string name)
        {
            return TestBuilder.MakeSuite(name);
        }

        private static TestSuite Fixture(Type type)
        {
            return TestBuilder.MakeFixture(type);
        }

        #endregion

        #region Nested Types

        public enum TestAction
        {
            TestStarting,
            TestFinished,
            OneTimeSetUpStarted,
            OneTimeSetUpFinished,
            OneTimeTearDownStarted,
            OneTimeTearDownFinished,
        }

        /// <summary>
        /// Compares TestEvents solely on Action Property
        /// </summary>
        public class TestEventActionComparer : System.Collections.IComparer
        {
            public int Compare(object? x, object? y)
            {
                return (x as TestEvent)!.Action.CompareTo((y as TestEvent)!.Action);
            }

            public int GetHashCode(TestEvent obj)
            {
                return obj.Action.GetHashCode();
            }
        }

        public class TestEvent
        {
            public TestAction Action;
            public string? TestName;
            public string? ThreadName;
            public string? ShiftName;
            public string? Result;
        }

        #endregion
    }
}
