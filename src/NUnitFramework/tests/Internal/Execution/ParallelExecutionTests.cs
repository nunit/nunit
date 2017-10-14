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

        private List<string> _events;
        private TestResult _result;

        public ParallelExecutionTests(TestSuite testSuite, int numCases)
        {
            _testSuite = testSuite;
            _numCases = numCases;
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
                AddEvent("ShiftStarted " + shift.Name);
            };

            dispatcher.ShiftFinished += (shift) =>
            {
                AddEvent("ShiftFinished " + shift.Name);
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

        [Test]
        public void OnlyOneShiftMayBeActive()
        {
            int count = 0;
            foreach (var @event in _events)
            {
                if (@event.StartsWith("ShiftStarted") && ++count > 1)
                    Assert.Fail(DumpEvents("Shift started while another shift was active"));

                if (@event.StartsWith("ShiftFinished"))
                    --count;               
            }
        }

        [Test]
        public void AllTestsAreWithinShifts()
        {
            bool inShift = false;
            foreach (var @event in _events)
            {
                if (@event.StartsWith("ShiftStarted"))
                    inShift = true;
                else if (@event.StartsWith("ShiftFinished"))
                    inShift = false;
                else
                    if(!inShift)
                        Assert.Fail(DumpEvents("Test event received outside of any shift"));
            }
        }

        [Test]
        public void ListEvents()
        {
            Console.WriteLine(DumpEvents("Events Received:"));
        }

        #region Test Data

        static IEnumerable<TestFixtureData> GetParallelSuites()
        {
            yield return new TestFixtureData(
                TestBuilder.MakeSuite("fake-assembly.dll")
                    .Containing(TestBuilder.MakeSuite("NUnit")
                        .Containing(TestBuilder.MakeSuite("TestData")
                            .Containing(TestBuilder.MakeSuite("ParallelExecutionData")
                                .Containing(TestBuilder.MakeFixture(typeof(TestSetUpFixture)).Parallelizable()
                                    .Containing(
                                        TestBuilder.MakeFixture(typeof(TestFixture1)).Parallelizable(), 
                                        TestBuilder.MakeFixture(typeof(TestFixture2)), 
                                        TestBuilder.MakeFixture(typeof(TestFixture3)).Parallelizable()
                                    )
                                )
                            )
                        )
                    ),
                3);
        }

        #endregion

        #region ITestListener implementation

        public void TestStarted(ITest test)
        {
            AddEvent("TestStarted " + test.FullName);
        }

        public void TestFinished(ITestResult result)
        {
            AddEvent("TestFinished " + result.FullName + " " + result.ResultState);
        }

        public void TestOutput(TestOutput output)
        {
            AddEvent(output.Stream + " " + output.Text.TrimEnd('\r', '\n'));
        }

        #endregion

        #region Helper Methods

        private void AddEvent(string @event)
        {
            lock (_events)
                _events.Add(@event);
        }

        private string DumpEvents(string message)
        {
            var sb = new StringBuilder().AppendLine(message);

            foreach (string @event in _events)
                sb.AppendLine(@event);

            return sb.ToString();
        }

        #endregion
    }
}
#endif
