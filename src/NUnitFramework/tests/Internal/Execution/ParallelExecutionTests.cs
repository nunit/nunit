// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.TestData.ParallelExecutionData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal.Execution
{
    [TestFixtureSource(nameof(GetParallelSuites))]
    [NonParallelizable]
    public class ParallelExecutionTests : ITestListener
    {
        private readonly TestSuite _testSuite;
        private readonly Expectations _expectations;

        private ConcurrentQueue<TestEvent> _events;
        private TestResult _result;

        private IEnumerable<TestEvent> AllEvents => _events.AsEnumerable();
        private IEnumerable<TestEvent> ShiftEvents => AllEvents.Where(e => e.Action == TestAction.ShiftStarted || e.Action == TestAction.ShiftFinished);
        private IEnumerable<TestEvent> TestEvents => AllEvents.Where(e => e.Action == TestAction.TestStarting || e.Action == TestAction.TestFinished);

        public ParallelExecutionTests(TestSuite testSuite, Expectations expectations)
        {
            _testSuite = testSuite;
            _expectations = expectations;
        }

        [OneTimeSetUp]
        public void RunTestSuite()
        {
            _events = new ConcurrentQueue<TestEvent>();

            var dispatcher = new ParallelWorkItemDispatcher(4);
            var context = new TestExecutionContext
            {
                Dispatcher = dispatcher,
                Listener = this
            };

            dispatcher.ShiftStarting += (shift) =>
            {
                _events.Enqueue(new TestEvent()
                {
                    Action = TestAction.ShiftStarted,
                    ShiftName = shift.Name
                });
            };

            dispatcher.ShiftFinished += (shift) =>
            {
                _events.Enqueue(new TestEvent()
                {
                    Action = TestAction.ShiftFinished,
                    ShiftName = shift.Name
                });
            };

            var workItem = TestBuilder.CreateWorkItem(_testSuite, context);

            dispatcher.Start(workItem);
            workItem.WaitForCompletion();

            _result = workItem.Result;
        }

        [Test]
        public void AllTestsPassed()
        {
            if (_result.ResultState != ResultState.Success || _result.PassCount != _testSuite.TestCaseCount)
                Assert.Fail(DumpEvents("Not all tests passed"));
        }

        [Test]
        public void OnlyOneShiftIsActiveAtSameTime()
        {
            int count = 0;
            foreach (var e in _events)
            {
                if (e.Action == TestAction.ShiftStarted && ++count > 1)
                    Assert.Fail(DumpEvents("Shift started while another shift was active"));

                if (e.Action == TestAction.ShiftFinished)
                    --count;
            }
        }

        [Test]
        public void CorrectInitialShift()
        {
            string expected = "NonParallel";
            if (_testSuite.Properties.ContainsKey(PropertyNames.ParallelScope))
            {
                var scope = (ParallelScope)_testSuite.Properties.Get(PropertyNames.ParallelScope)!;
                if ((scope & ParallelScope.Self) != 0)
                    expected = "Parallel";
            }

            var e = _events.First();
            Assert.That(e.Action, Is.EqualTo(TestAction.ShiftStarted));
            Assert.That(e.ShiftName, Is.EqualTo(expected));
        }

        [Test]
        public void TestsRunOnExpectedWorkers()
        {
            Assert.Multiple(() =>
            {
                foreach (var e in TestEvents)
                    _expectations.Verify(e);
            });
        }

        #region Test Data

        private static IEnumerable<TestFixtureData> GetParallelSuites()
        {
            const string nonParallelWorker = "NUnit.Fw.NonParallelWorker";
            const string parallelWorker = "NUnit.Fw.ParallelWorker";
            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("Tests").RunsOn(nonParallelWorker),
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker)))
                .SetName("SingleFixture_Default");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1)).NonParallelizable()))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("Tests").RunsOn(nonParallelWorker),
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker)))
                .SetName("SingleFixture_NonParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1)).Parallelizable()))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(nonParallelWorker).FinishesOn(parallelWorker),
                    That("NUnit").StartsOn(nonParallelWorker).FinishesOn(parallelWorker),
                    That("Tests").StartsOn(nonParallelWorker).FinishesOn(parallelWorker),
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker)))
                .SetName("SingleFixture_Parallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll").NonParallelizable()
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("Tests").RunsOn(nonParallelWorker),
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker)))
                .SetName("SingleFixture_AssemblyNonParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll").Parallelizable()
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(parallelWorker).FinishesOn(nonParallelWorker),
                    That("NUnit").StartsOn(parallelWorker).FinishesOn(nonParallelWorker),
                    That("Tests").StartsOn(parallelWorker).FinishesOn(nonParallelWorker),
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker)))
                .SetName("SingleFixture_AssemblyParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll").Parallelizable()
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1)).Parallelizable()))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(parallelWorker),
                    That("NUnit").RunsOn(parallelWorker),
                    That("Tests").RunsOn(parallelWorker),
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker)))
                .SetName("SingleFixture_AssemblyAndFixtureParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixtureWithParallelParameterizedTest))))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("Tests").RunsOn(nonParallelWorker),
                    That("TestFixtureWithParallelParameterizedTest").RunsOn(nonParallelWorker),
                    That("ParameterizedTest").StartsOn(nonParallelWorker).FinishesOn(parallelWorker),
                    That("ParameterizedTest(1)").RunsOn(parallelWorker),
                    That("ParameterizedTest(2)").RunsOn(parallelWorker),
                    That("ParameterizedTest(3)").RunsOn(parallelWorker)))
                .SetName("SingleFixture_ParameterizedTest");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1))
                                .Containing(
                                    Fixture(typeof(TestFixture1)),
                                    Fixture(typeof(TestFixture2)),
                                    Fixture(typeof(TestFixture3)))))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("TestData").RunsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn(nonParallelWorker), // SetUpFixture1
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(nonParallelWorker),
                    That("TestFixture3_Test").RunsOn(nonParallelWorker)))
                .SetName("ThreeFixtures_SetUpFixture_Default");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1))
                                .Containing(
                                    Fixture(typeof(TestFixture1)).Parallelizable(),
                                    Fixture(typeof(TestFixture2)),
                                    Fixture(typeof(TestFixture3)).Parallelizable())))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("TestData").RunsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn(nonParallelWorker), // SetUpFixture1
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoParallelizable_SetUpFixture");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1)).Parallelizable()
                                .Containing(
                                    Fixture(typeof(TestFixture1)).Parallelizable(),
                                    Fixture(typeof(TestFixture2)),
                                    Fixture(typeof(TestFixture3)).Parallelizable())))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(nonParallelWorker),
                    That("NUnit").StartsOn(nonParallelWorker),
                    That("TestData").StartsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn(parallelWorker), // SetUpFixture1
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoParallelizable_ParallelizableSetUpFixture");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1)).Parallelizable()
                                .Containing(Fixture(typeof(SetUpFixture2)).Parallelizable()
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(nonParallelWorker),
                    That("NUnit").StartsOn(nonParallelWorker),
                    That("TestData").StartsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn(parallelWorker), // SetUpFixture1 && SetUpFixture2
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoSetUpFixturesInSameNamespace_BothParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1))
                                .Containing(Fixture(typeof(SetUpFixture2))
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("NUnit").RunsOn(nonParallelWorker),
                    That("TestData").RunsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn("*"), // SetUpFixture1 && SetUpFixture2
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoSetUpFixturesInSameNamespace_NeitherParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1)).Parallelizable()
                                .Containing(Fixture(typeof(SetUpFixture2))
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(nonParallelWorker),
                    That("NUnit").StartsOn(nonParallelWorker),
                    That("TestData").StartsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn("*"), // SetUpFixture1 && SetUpFixture2 (we can't distinguish the two)
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoSetUpFixturesInSameNamespace_FirstOneParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Fixture(typeof(SetUpFixture1))
                                .Containing(Fixture(typeof(SetUpFixture2)).Parallelizable()
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                Expecting(
                    That("fake-assembly.dll").StartsOn(nonParallelWorker),
                    That("NUnit").StartsOn(nonParallelWorker),
                    That("TestData").StartsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn("*"), // SetUpFixture1 && SetUpFixture2 (we can't distinguish the two)
                    That("TestFixture1").RunsOn(parallelWorker),
                    That("TestFixture1_Test").RunsOn(parallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker),
                    That("TestFixture3").RunsOn(parallelWorker),
                    That("TestFixture3_Test").RunsOn(parallelWorker)))
                .SetName("ThreeFixtures_TwoSetUpFixturesInSameNamespace_SecondOneParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("SomeNamespace")
                        .Containing(Fixture(typeof(SetUpFixture1)))
                            .Containing(Fixture(typeof(TestFixture1))))
                    .Containing(Suite("OtherNamespace")
                        .Containing(Fixture(typeof(TestFixture2)))),
                Expecting(
                    That("fake-assembly.dll").RunsOn(nonParallelWorker),
                    That("SomeNamespace").RunsOn(nonParallelWorker),
                    That("ParallelExecutionData").RunsOn(nonParallelWorker), // SetUpFixture1
                    That("TestFixture1").RunsOn(nonParallelWorker),
                    That("TestFixture1_Test").RunsOn(nonParallelWorker),
                    That("OtherNamespace").RunsOn(nonParallelWorker),
                    That("TestFixture2").RunsOn(nonParallelWorker),
                    That("TestFixture2_Test").RunsOn(nonParallelWorker)))
                .SetName("Issue-2464");

            if (new PlatformHelper().IsPlatformSupported(new PlatformAttribute { Include = "Win, Mono" }))
            {
                const string nunitFwParallelSTAWorker = "NUnit.Fw.ParallelSTAWorker";
                yield return new TestFixtureData(
                        Suite("fake-assembly.dll").Parallelizable()
                                                  .Containing(Fixture(typeof(STAFixture)).Parallelizable()),
                        Expecting(
                            That("fake-assembly.dll").StartsOn(parallelWorker),
                            That("STAFixture").RunsOn(nunitFwParallelSTAWorker),
                            That("STAFixture_Test").RunsOn(nunitFwParallelSTAWorker)))
                    .SetName("Issue-2467");
            }
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

        private static Expectations Expecting(params Expectation[] expectations)
        {
            return new Expectations(expectations);
        }

        private static Expectation That(string testName)
        {
            return new Expectation(testName);
        }

        private string DumpEvents(string message)
        {
            var sb = new StringBuilder().AppendLine(message);

            foreach (var e in _events)
                sb.AppendLine(e.ToString());

            return sb.ToString();
        }

        #endregion

        #region Nested Types

        public enum TestAction
        {
            ShiftStarted,
            ShiftFinished,
            TestStarting,
            TestFinished
        }

        public class TestEvent
        {
            public TestAction Action;
            public string? TestName;
            public string? ThreadName;
            public string? ShiftName;
            public string? Result;

            public override string ToString()
            {
                switch (Action)
                {
                    case TestAction.ShiftStarted:
                        return $"{Action} {ShiftName}";

                    default:
                    case TestAction.TestStarting:
                        return $"{Action} {TestName} [{ThreadName}]";

                    case TestAction.TestFinished:
                        return $"{Action} {TestName} {Result} [{ThreadName}]";
                }
            }
        }

        public class Expectation
        {
            public string TestName { get; }
            public string StartWorker { get; private set; }
            public string FinishWorker { get; private set; }

            public Expectation(string testName)
            {
                TestName = testName;
                StartWorker = FinishWorker = "*";
            }

            // THe RunsOn method specifies that the work item will
            // run entirely on a particular worker. In the case of
            // a composite work item, this doesn't include its
            // children, which may run on a different thread.
            public Expectation RunsOn(string worker)
            {
                StartWorker = FinishWorker = worker;
                return this;
            }

            // Sometimes, the thread used to complete a composite work item
            // is unimportant because there is no actual user code to run.
            // In that case, we can just specify which worker is used to
            // initially run the item using StartsOn
            public Expectation StartsOn(string worker)
            {
                StartWorker = worker;
                return this;
            }

            // FinishesOn may be used if we know that the work item
            // starts and finishes on different threads and we want
            // to specify both of them. In most cases, this amounts
            // to over-specification since we usually don't care.
            public Expectation FinishesOn(string worker)
            {
                FinishWorker = worker;
                return this;
            }

            // Verify that a particular TestEvent meets all expectations
            public void Verify(TestEvent e)
            {
                var worker = e.Action == TestAction.TestStarting ? StartWorker : FinishWorker;
                if (worker != "*")
                    Assert.That(e.ThreadName, Does.StartWith(worker), $"{e.Action} {e.TestName} running on wrong type of worker thread.");
            }
        }

        public class Expectations
        {
            private readonly Dictionary<string, Expectation> _expectations = new Dictionary<string, Expectation>();

            public Expectations(params Expectation[] expectations)
            {
                foreach (var expectation in expectations)
                    _expectations.Add(expectation.TestName, expectation);
            }

            public void Verify(TestEvent e)
            {
                string? testName = e.TestName;
                Assert.That(testName, Is.Not.Null);
                Assert.That(_expectations, Does.ContainKey(testName), $"The test {e.TestName} is not in the dictionary.");
                _expectations[testName].Verify(e);
            }
        }

        #endregion
    }
}
