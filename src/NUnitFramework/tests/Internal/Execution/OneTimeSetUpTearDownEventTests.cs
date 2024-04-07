using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Legacy;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.OneTimeSetUpTearDownData;

namespace NUnit.Framework.Tests.Internal.Execution
{
    [TestFixture]
    [NonParallelizable]
    public class OneTimeSetUpTearDownEventTests : ITestListener
    {
        private ConcurrentQueue<TestEvent> _events = new();

        private IEnumerable<TestEvent> AllEvents => _events.AsEnumerable();

        private void RunTestSuite(TestSuite testSuite)
        {
            _events = new ConcurrentQueue<TestEvent>();

            var dispatcher = new ParallelWorkItemDispatcher(1);
            var context = new TestExecutionContext();
            context.Dispatcher = dispatcher;
            context.Listener = this;

            var workItem = TestBuilder.CreateWorkItem(testSuite, context);

            dispatcher.Start(workItem);
            workItem.WaitForCompletion();
        }

        private void RunTestFixture(object testFixture)
        {
            TestSuite suite = TestBuilder.MakeFixture(testFixture);
            suite.Fixture = testFixture;
            RunTestSuite(suite);
        }

        [Test]
        public void RegularTestFixture_AllPassing()
        {
            var fixture = new SetUpAndTearDownFixture();
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp in Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown in Fixture
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void RegularTestFixture_FailingOneTimeSetUp()
        {
            var fixture = new SetUpAndTearDownFixture
            {
                ThrowInBaseSetUp = true
            };
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp in Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Tests are not started since OTS failed
                new TestEvent() { Action = TestAction.TestFinished },               // Tests are not started since OTS failed
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown in Fixture
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void OverriddenOneTimeSetUpOneTimeTearDown_AllPassing()
        {
            var fixture = new OverrideSetUpAndTearDown();
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // Overridden OneTimeSetUp
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // Overridden OneTimeTearDown
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void OverriddenOneTimeSetUpOneTimeTearDown_FailingBaseOneTimeSetUp()
        {
            var fixture = new OverrideSetUpAndTearDown()
            {
                // Base OneTimeSetUp would fail, but since derived class overrides, the failure never occurs
                ThrowInBaseSetUp = true
            };
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // Overridden OneTimeSetUp
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // Overridden OneTimeTearDown
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void RegularTestFixture_FailingOneTimeTearDown()
        {
            var fixture = new MisbehavingFixture()
            {
                BlowUpInTearDown = true,
            };
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown, both events should come although OTT fails
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void DerivedTestFixture_EventsForEachOneTimeSetUpOneTimeTearDown()
        {
            var fixture = new DerivedSetUpAndTearDownFixture();
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp from base class
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp from derived class
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from base class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test1 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test2 from derived class
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown from derived class
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown from base class
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };

            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

        [Test]
        public void TestFixtureWithMultipleSetUpTearDown_EventsForEachOneTimeSetUpOneTimeTearDown()
        {
            var fixture = new FixtureWithMultipleSetUpTearDown();
            RunTestFixture(fixture);

            List<TestEvent> expectedEventsInTheRightOrder = new List<TestEvent>()
            {
                new TestEvent() { Action = TestAction.TestStarting },               // Fixture
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp1
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.OneTimeSetUpStarted },        // OneTimeSetUp2
                new TestEvent() { Action = TestAction.OneTimeSetUpFinished },
                new TestEvent() { Action = TestAction.TestStarting },               // Test
                new TestEvent() { Action = TestAction.TestFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown1
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.OneTimeTearDownStarted },     // OneTimeTearDown2
                new TestEvent() { Action = TestAction.OneTimeTearDownFinished },
                new TestEvent() { Action = TestAction.TestFinished },               // Fixture
            };
            //Debugger.Launch();
            CollectionAssert.AreEqual(expectedEventsInTheRightOrder, AllEvents, new TestEventActionComparer());
        }

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
