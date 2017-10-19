// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.TestData.ParallelExecutionData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal.Execution
{
    [TestFixtureSource(nameof(GetParallelSuites))]
    public class ParallelExecutionTests : ITestListener
    {
        private readonly TestSuite _testSuite;
        private readonly int _numCases;
        private readonly string _expectedShifts;

        private List<string> _events;
        private TestResult _result;

        public ParallelExecutionTests(TestSuite testSuite, int numCases, string expectedShifts)
        {
            _testSuite = testSuite;
            _numCases = numCases;
            _expectedShifts = expectedShifts;
        }

        [OneTimeSetUp]
        public void RunTestSuite()
        {
            _events = new List<string>();

            var dispatcher = new ParallelWorkItemDispatcher(4);
            var context = new TestExecutionContext();
            context.Dispatcher = dispatcher;
            context.Listener = this;

            dispatcher.ShiftStarting += (shift) =>
            {
                lock (_events)
                    _events.Add("ShiftStarted " + shift.Name);
            };

            dispatcher.ShiftFinished += (shift) =>
            {
                lock (_events)
                    _events.Add("ShiftFinished " + shift.Name);
            };

            var workItem = TestBuilder.CreateWorkItem(_testSuite, context);

            dispatcher.Start(workItem);
            workItem.WaitForCompletion();

            _result = workItem.Result;
        }

        // NOTE: The following tests use Assert.Fail under control of an
        // if statement to avoid evaluating DumpEvents unnecessarily.
        // Unfortunately, we can't use the form of Assert that takes
        // a Func for a message because it's not present in .NET 2.0

        [Test]
        public void AllTestsPassed()
        {
            if (_result.ResultState != ResultState.Success)
                Assert.Fail(DumpEvents("Not all tests passed"));
        }

        [Test]
        public void AllTestsRan()
        {
            if (_result.PassCount != _numCases)
                Assert.Fail(DumpEvents("Incorrect number of test cases"));
        }

        // NOTE: The Shift events come directly to the test,
        // while the test listener events are added to a queue
        // and arrive a bit more slowly. SO it's possible for
        // the ShiftFinished event to arrive early, making it
        // appear that some other events ran outside of any
        // shift. For that reason, the following test is the
        // only one that uses the ShiftFinished event. 

        [Test]
        public void OnlyOneShiftMayBeActive()
        {
            int count = 0;
            lock (_events)
            {
                foreach (var @event in _events)
                {
                    if (@event.StartsWith("ShiftStarted") && ++count > 1)
                        Assert.Fail(DumpEvents("Shift started while another shift was active"));

                    if (@event.StartsWith("ShiftFinished"))
                        --count;
                }           
            }
        }

        [Test]
        public void ExpectedShiftsAreRun()
        {
            lock (_events)
            {
                var shifts = String.Join("+",
                    _events.FindAll(e => e.StartsWith("ShiftStarted")).Select(e => e.Substring(13)).ToArray());

                if (shifts != _expectedShifts)
                    Assert.Fail(DumpEvents("Expected " + _expectedShifts + " but was " + shifts));
            }
        }

        #region Test Data

        static IEnumerable<TestFixtureData> GetParallelSuites()
        {
            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                1,
                "NonParallel")
                .SetName("SingleFixture_Default");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1)).NonParallelizable()))),
                1,
                "NonParallel" )
                .SetName("SingleFixture_NonParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1)).Parallelizable()))),
                1, // Assert
                "NonParallel+Parallel" )
                .SetName("SingleFixture_Parallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll").NonParallelizable()
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                1,
                "NonParallel")
                .SetName("SingleFixture_AssemblyNonParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll").Parallelizable()
                    .Containing(Suite("NUnit")
                        .Containing(Suite("Tests")
                            .Containing(Fixture(typeof(TestFixture1))))),
                1,
                "Parallel+NonParallel" )
                .SetName("SingleFixture_AssemblyParallelizable");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Suite("ParallelExecutionData")
                                .Containing(Fixture(typeof(TestSetUpFixture))
                                    .Containing(
                                        Fixture(typeof(TestFixture1)),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3))))))),
                3,
                "NonParallel+NonParallel+NonParallel" ) // TODO: SHould just be one shift!
                .SetName("ThreeFixtures_SetUpFixture_Default");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Suite("ParallelExecutionData")
                                .Containing(Fixture(typeof(TestSetUpFixture))
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                3,
                "NonParallel+Parallel+NonParallel" ) // TODO: Sometimes get one or two extra NonParallelizable at end
                .SetName("ThreeFixtures_TwoParallelizable_SetUpFixture");

            yield return new TestFixtureData(
                Suite("fake-assembly.dll")
                    .Containing(Suite("NUnit")
                        .Containing(Suite("TestData")
                            .Containing(Suite("ParallelExecutionData")
                                .Containing(Fixture(typeof(TestSetUpFixture)).Parallelizable()
                                    .Containing(
                                        Fixture(typeof(TestFixture1)).Parallelizable(),
                                        Fixture(typeof(TestFixture2)),
                                        Fixture(typeof(TestFixture3)).Parallelizable()))))),
                3,
                "NonParallel+Parallel+NonParallel+Parallel" ) // TODO: Sometimes get P+P+NP+P
                .SetName("ThreeFixtures_TwoParallelizable_ParallelizableSetUpFixture");
        }

        #endregion

        #region ITestListener implementation

        public void TestStarted(ITest test)
        {
            lock (_events)
                _events.Add("TestStarted " + test.FullName);
        }

        public void TestFinished(ITestResult result)
        {
            lock (_events)
                _events.Add("TestFinished " + result.FullName + " " + result.ResultState);
        }

        public void TestOutput(TestOutput output)
        {
            lock (_events)
                _events.Add(output.Stream + " " + output.Text.TrimEnd('\r', '\n'));
        }

        #endregion

        #region Helper Methods

        private string DumpEvents(string message)
        {
            var sb = new StringBuilder().AppendLine(message);

            foreach (string @event in _events)
                sb.AppendLine(@event);

            return sb.ToString();
        }

        public static TestSuite Suite(string name) { return TestBuilder.MakeSuite(name); }
        public static TestSuite Fixture(Type type) { return TestBuilder.MakeFixture(type); }

        #endregion
    }
}
#endif
