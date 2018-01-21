// ***********************************************************************
// Copyright (c) 2012 Charlie Poole, Rob Prouse
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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.ActionAttributeTests;

namespace NUnit.Framework
{
    [TestFixture, NonParallelizable]
    public class ActionAttributeTests
    {
        // NOTE: An earlier version of this fixture attempted to test
        // the exact order of all actions. However, the order of execution 
        // of the individual tests cannot be predicted reliably across
        // different runtimes, so we now look only at the relative position
        // of before and after actions with respect to the test.

        private static readonly string ASSEMBLY_PATH = AssemblyHelper.GetAssemblyPath(typeof(ActionAttributeFixture).GetTypeInfo().Assembly);
        private static readonly string ASSEMBLY_NAME = System.IO.Path.GetFileName(ASSEMBLY_PATH);

        private ITestResult _result = null;
        private int _numEvents = -1;

        [OneTimeSetUp]
        public void Setup()
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            ActionAttributeFixture.ClearResults();

            IDictionary<string, object> options = new Dictionary<string, object>();
            options["LOAD"] = new string[] { "NUnit.TestData.ActionAttributeTests" };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            Assert.NotNull(runner.Load(ASSEMBLY_PATH, options), "Assembly not loaded");
            Assert.That(runner.LoadedTest.RunState, Is.EqualTo(RunState.Runnable));

            _result = runner.Run(TestListener.NULL, TestFilter.Empty);

            _numEvents = ActionAttributeFixture.Events.Count;
        }

        [Test]
        public void TestsRunSuccessfully()
        {
            Assert.That(_result.ResultState, Is.EqualTo(ResultState.Success));

            //foreach(string message in ActionAttributeFixture.Events)
            //    Console.WriteLine(message);
        }

        [Test]
        public void ExpectedOutput_InCorrectOrder()
        {
            var notFound = new List<string>();
            var notExpected = new List<string>();

            foreach (var item in ExpectedEvents)
                if (!ActionAttributeFixture.Events.Contains(item))
                    notFound.Add(item);

            foreach (var item in ActionAttributeFixture.Events)
                if (!ExpectedEvents.Contains(item))
                    notExpected.Add(item);

            if (notFound.Count > 0 || notExpected.Count > 0)
            {
                var sb = new StringBuilder("Expected and actual events do not match.");

                if (notFound.Count > 0)
                {
                    sb.Append(Environment.NewLine + "   Missing:");
                    foreach (var item in notFound)
                        sb.Append(Environment.NewLine + "     " + item);
                }

                if (notExpected.Count > 0)
                {
                    sb.Append(Environment.NewLine + "   Extra:");
                    foreach (var item in notExpected)
                        sb.Append(Environment.NewLine + "     " + item);
                }

                Assert.Fail(sb.ToString());
            }
        }

        [Test]
        public void ActionsWrappingAssembly()
        {
            CheckActionsOnSuite(ASSEMBLY_NAME, 0, _numEvents - 1, ExpectedAssemblyActions);
        }

        [Test]
        public void ActionsWrappingSetUpFixture()
        {
            int firstAction = NumAssemblyActions;
            int lastAction = _numEvents - firstAction - 1;
            CheckActionsOnSuite("ActionAttributeTests", firstAction, lastAction, ExpectedSetUpFixtureActions);
        }

        [Test]
        public void ActionsWrappingTestFixture()
        {
            int firstAction = NumAssemblyActions + NumSetUpFixtureActions;
            int lastAction = _numEvents - firstAction - 1;
            CheckActionsOnSuite("ActionAttributeFixture", firstAction, lastAction, ExpectedTestFixtureActions);
        }

        [Test]
        public void ActionsWrappingParameterizedMethodSuite()
        {
            int case1 = ActionAttributeFixture.Events.IndexOf("CaseOne");
            int case2 = ActionAttributeFixture.Events.IndexOf("CaseTwo");
            Assume.That(case1, Is.GreaterThanOrEqualTo(0));
            Assume.That(case2, Is.GreaterThanOrEqualTo(0));

            int firstAction = Math.Min(case1, case2) - NumTestCaseActions - NumParameterizedTestActions;
            int lastAction = Math.Max(case1, case2) + NumTestCaseActions + NumParameterizedTestActions;
            CheckActionsOnSuite("ParameterizedTest", firstAction, lastAction, "OnMethod", "OnMethod");
        }

        [Test]
        public void CorrectNumberOfEventsReceived()
        {
            Assert.That(ActionAttributeFixture.Events.Count, Is.EqualTo(
                NumTestCaseEvents + 2 * (NumParameterizedTestActions + NumTestFixtureActions + NumSetUpFixtureActions + NumAssemblyActions)));
        }

        [TestCase("CaseOne")]
        [TestCase("CaseTwo")]
        [TestCase("SimpleTest")]
        public void ActionsWrappingTestMethod(string testName)
        {
            CheckActionsOnTestCase(testName);
        }

        #region Helper Methods

        private void CheckActionsOnSuite(string suiteName, int firstEvent, int lastEvent, params string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
                CheckBeforeAfterActionPair(firstEvent + i, lastEvent - i, suiteName, tags[i]);

            if (firstEvent > 0)
            {
                var beforeEvent = ActionAttributeFixture.Events[firstEvent - 1];
                Assert.That(beforeEvent, Does.Not.StartWith(suiteName), "Extra ActionAttribute Before: {0}", beforeEvent);
            }

            if (lastEvent < ActionAttributeFixture.Events.Count - 1)
            {
                var afterEvent = ActionAttributeFixture.Events[lastEvent + 1];
                Assert.That(afterEvent, Does.Not.StartWith(suiteName), "Extra ActionAttribute After: {0}", afterEvent);
            }
        }

        private void CheckActionsOnTestCase(string testName)
        {
            var index = ActionAttributeFixture.Events.IndexOf(testName);
            Assert.That(index, Is.GreaterThanOrEqualTo(0), "{0} did not execute", testName);
            var numActions = ExpectedTestCaseActions.Length;

            for (int i = 0; i < numActions; i++)
                CheckBeforeAfterActionPair(index - i - 1, index + i + 1, testName, ExpectedTestCaseActions[i]);

            Assert.That(ActionAttributeFixture.Events[index - numActions - 1], Does.Not.StartWith(testName), "Extra ActionAttribute Before");
            Assert.That(ActionAttributeFixture.Events[index + numActions + 1], Does.Not.StartWith(testName), "Extra ActionAttribute After");
        }

        private void CheckBeforeAfterActionPair(int index1, int index2, string testName, string tag)
        {
            var event1 = ActionAttributeFixture.Events[index1];
            var event2 = ActionAttributeFixture.Events[index2];

            Assert.That(event1, Does.StartWith(testName + "." + tag + ".Before"));
            Assert.That(event2, Does.StartWith(testName + "." + tag + ".After"));

            int index = event1.LastIndexOf('.');
            var target1 = event1.Substring(index); // Target is last in string

            Assert.That(event2, Does.EndWith(target1), "Event mismatch");
        }

        #endregion

        #region Expected Attributes and Events

        private static readonly string[] ExpectedAssemblyActions = new string[] {
                        "OnAssembly", "OnAssembly", "OnAssembly" };

        private static readonly string[] ExpectedSetUpFixtureActions = new string[] {
                        "OnBaseSetupFixture", "OnBaseSetupFixture", "OnBaseSetupFixture",
                        "OnSetupFixture", "OnSetupFixture", "OnSetupFixture"
        };

        private static readonly string[] ExpectedTestFixtureActions = new string[] {
                        "OnBaseInterface", "OnBaseInterface", "OnBaseInterface",
                        "OnBaseFixture", "OnBaseFixture", "OnBaseFixture",
                        "OnInterface", "OnInterface", "OnInterface",
                        "OnFixture", "OnFixture", "OnFixture"
        };

        private static readonly string[] ExpectedParameterizedTestActions = new string[] {
                        "OnMethod", "OnMethod"
        };

        private static readonly string[] ExpectedTestCaseActions = new string[] {
                        "OnMethod", "OnMethod", "OnMethod",
                        "SetUpTearDown",
                        "OnFixture", "OnFixture",
                        "OnInterface", "OnInterface",
                        "OnBaseFixture", "OnBaseFixture",
                        "OnBaseInterface", "OnBaseInterface",
                        "OnSetupFixture", "OnSetupFixture",
                        "OnBaseSetupFixture", "OnBaseSetupFixture",
                        "OnAssembly", "OnAssembly"
        };

        // The exact order of events may vary, depending on the runtime framework
        // in use. Consequently, we test heuristically. The following list is
        // only one possible ordering of events.
        private static readonly List<string> ExpectedEvents = new List<string>(new string[] {
                ASSEMBLY_NAME + ".OnAssembly.Before.Test, Suite",
                ASSEMBLY_NAME + ".OnAssembly.Before.Suite",
                ASSEMBLY_NAME + ".OnAssembly.Before.Default",
                "ActionAttributeTests.OnBaseSetupFixture.Before.Test, Suite",
                "ActionAttributeTests.OnBaseSetupFixture.Before.Suite",
                "ActionAttributeTests.OnBaseSetupFixture.Before.Default",
                "ActionAttributeTests.OnSetupFixture.Before.Test, Suite",
                "ActionAttributeTests.OnSetupFixture.Before.Suite",
                "ActionAttributeTests.OnSetupFixture.Before.Default",
                "ActionAttributeFixture.OnBaseInterface.Before.Test, Suite",
                "ActionAttributeFixture.OnBaseInterface.Before.Suite",
                "ActionAttributeFixture.OnBaseInterface.Before.Default",
                "ActionAttributeFixture.OnBaseFixture.Before.Test, Suite",
                "ActionAttributeFixture.OnBaseFixture.Before.Suite",
                "ActionAttributeFixture.OnBaseFixture.Before.Default",
                "ActionAttributeFixture.OnInterface.Before.Test, Suite",
                "ActionAttributeFixture.OnInterface.Before.Suite",
                "ActionAttributeFixture.OnInterface.Before.Default",
                "ActionAttributeFixture.OnFixture.Before.Test, Suite",
                "ActionAttributeFixture.OnFixture.Before.Suite",
                "ActionAttributeFixture.OnFixture.Before.Default",
                "ParameterizedTest.OnMethod.Before.Test, Suite",
                "ParameterizedTest.OnMethod.Before.Suite",
                "CaseOne.OnAssembly.Before.Test, Suite",
                "CaseOne.OnAssembly.Before.Test",
                "CaseOne.OnBaseSetupFixture.Before.Test, Suite",
                "CaseOne.OnBaseSetupFixture.Before.Test",
                "CaseOne.OnSetupFixture.Before.Test, Suite",
                "CaseOne.OnSetupFixture.Before.Test",
                "CaseOne.OnBaseInterface.Before.Test, Suite",
                "CaseOne.OnBaseInterface.Before.Test",
                "CaseOne.OnBaseFixture.Before.Test, Suite",
                "CaseOne.OnBaseFixture.Before.Test",
                "CaseOne.OnInterface.Before.Test, Suite",
                "CaseOne.OnInterface.Before.Test",
                "CaseOne.OnFixture.Before.Test, Suite",
                "CaseOne.OnFixture.Before.Test",
                "CaseOne.SetUpTearDown.Before.Test",
                "CaseOne.OnMethod.Before.Test, Suite",
                "CaseOne.OnMethod.Before.Test",
                "CaseOne.OnMethod.Before.Default",
                "CaseOne",
                "CaseOne.OnMethod.After.Default",
                "CaseOne.OnMethod.After.Test",
                "CaseOne.OnMethod.After.Test, Suite",
                "CaseOne.SetUpTearDown.After.Test",
                "CaseOne.OnFixture.After.Test",
                "CaseOne.OnFixture.After.Test, Suite",
                "CaseOne.OnInterface.After.Test",
                "CaseOne.OnInterface.After.Test, Suite",
                "CaseOne.OnBaseFixture.After.Test",
                "CaseOne.OnBaseFixture.After.Test, Suite",
                "CaseOne.OnBaseInterface.After.Test",
                "CaseOne.OnBaseInterface.After.Test, Suite",
                "CaseOne.OnSetupFixture.After.Test",
                "CaseOne.OnSetupFixture.After.Test, Suite",
                "CaseOne.OnBaseSetupFixture.After.Test",
                "CaseOne.OnBaseSetupFixture.After.Test, Suite",
                "CaseOne.OnAssembly.After.Test",
                "CaseOne.OnAssembly.After.Test, Suite",
                "CaseTwo.OnAssembly.Before.Test, Suite",
                "CaseTwo.OnAssembly.Before.Test",
                "CaseTwo.OnBaseSetupFixture.Before.Test, Suite",
                "CaseTwo.OnBaseSetupFixture.Before.Test",
                "CaseTwo.OnSetupFixture.Before.Test, Suite",
                "CaseTwo.OnSetupFixture.Before.Test",
                "CaseTwo.OnBaseInterface.Before.Test, Suite",
                "CaseTwo.OnBaseInterface.Before.Test",
                "CaseTwo.OnBaseFixture.Before.Test",
                "CaseTwo.OnBaseFixture.Before.Test, Suite",
                "CaseTwo.OnInterface.Before.Test, Suite",
                "CaseTwo.OnInterface.Before.Test",
                "CaseTwo.OnFixture.Before.Test, Suite",
                "CaseTwo.OnFixture.Before.Test",
                "CaseTwo.SetUpTearDown.Before.Test",
                "CaseTwo.OnMethod.Before.Test, Suite",
                "CaseTwo.OnMethod.Before.Test",
                "CaseTwo.OnMethod.Before.Default",
                "CaseTwo",
                "CaseTwo.OnMethod.After.Default",
                "CaseTwo.OnMethod.After.Test",
                "CaseTwo.OnMethod.After.Test, Suite",
                "CaseTwo.SetUpTearDown.After.Test",
                "CaseTwo.OnFixture.After.Test",
                "CaseTwo.OnFixture.After.Test, Suite",
                "CaseTwo.OnInterface.After.Test",
                "CaseTwo.OnInterface.After.Test, Suite",
                "CaseTwo.OnBaseFixture.After.Test",
                "CaseTwo.OnBaseFixture.After.Test, Suite",
                "CaseTwo.OnBaseInterface.After.Test",
                "CaseTwo.OnBaseInterface.After.Test, Suite",
                "CaseTwo.OnSetupFixture.After.Test",
                "CaseTwo.OnSetupFixture.After.Test, Suite",
                "CaseTwo.OnBaseSetupFixture.After.Test",
                "CaseTwo.OnBaseSetupFixture.After.Test, Suite",
                "CaseTwo.OnAssembly.After.Test",
                "CaseTwo.OnAssembly.After.Test, Suite",
                "ParameterizedTest.OnMethod.After.Suite",
                "ParameterizedTest.OnMethod.After.Test, Suite",
                "SimpleTest.OnAssembly.Before.Test, Suite",
                "SimpleTest.OnAssembly.Before.Test",
                "SimpleTest.OnBaseSetupFixture.Before.Test, Suite",
                "SimpleTest.OnBaseSetupFixture.Before.Test",
                "SimpleTest.OnSetupFixture.Before.Test, Suite",
                "SimpleTest.OnSetupFixture.Before.Test",
                "SimpleTest.OnBaseInterface.Before.Test, Suite",
                "SimpleTest.OnBaseInterface.Before.Test",
                "SimpleTest.OnBaseFixture.Before.Test, Suite",
                "SimpleTest.OnBaseFixture.Before.Test",
                "SimpleTest.OnInterface.Before.Test, Suite",
                "SimpleTest.OnInterface.Before.Test",
                "SimpleTest.OnFixture.Before.Test, Suite",
                "SimpleTest.OnFixture.Before.Test",
                "SimpleTest.SetUpTearDown.Before.Test",
                "SimpleTest.OnMethod.Before.Test, Suite",
                "SimpleTest.OnMethod.Before.Test",
                "SimpleTest.OnMethod.Before.Default",
                "SimpleTest",
                "SimpleTest.OnMethod.After.Default",
                "SimpleTest.OnMethod.After.Test",
                "SimpleTest.OnMethod.After.Test, Suite",
                "SimpleTest.SetUpTearDown.After.Test",
                "SimpleTest.OnFixture.After.Test",
                "SimpleTest.OnFixture.After.Test, Suite",
                "SimpleTest.OnInterface.After.Test",
                "SimpleTest.OnInterface.After.Test, Suite",
                "SimpleTest.OnBaseFixture.After.Test",
                "SimpleTest.OnBaseFixture.After.Test, Suite",
                "SimpleTest.OnBaseInterface.After.Test",
                "SimpleTest.OnBaseInterface.After.Test, Suite",
                "SimpleTest.OnSetupFixture.After.Test",
                "SimpleTest.OnSetupFixture.After.Test, Suite",
                "SimpleTest.OnBaseSetupFixture.After.Test",
                "SimpleTest.OnBaseSetupFixture.After.Test, Suite",
                "SimpleTest.OnAssembly.After.Test",
                "SimpleTest.OnAssembly.After.Test, Suite",
                "ActionAttributeFixture.OnFixture.After.Default",
                "ActionAttributeFixture.OnFixture.After.Suite",
                "ActionAttributeFixture.OnFixture.After.Test, Suite",
                "ActionAttributeFixture.OnInterface.After.Default",
                "ActionAttributeFixture.OnInterface.After.Suite",
                "ActionAttributeFixture.OnInterface.After.Test, Suite",
                "ActionAttributeFixture.OnBaseFixture.After.Default",
                "ActionAttributeFixture.OnBaseFixture.After.Suite",
                "ActionAttributeFixture.OnBaseFixture.After.Test, Suite",
                "ActionAttributeFixture.OnBaseInterface.After.Default",
                "ActionAttributeFixture.OnBaseInterface.After.Suite",
                "ActionAttributeFixture.OnBaseInterface.After.Test, Suite",
                "ActionAttributeTests.OnSetupFixture.After.Default",
                "ActionAttributeTests.OnSetupFixture.After.Suite",
                "ActionAttributeTests.OnSetupFixture.After.Test, Suite",
                "ActionAttributeTests.OnBaseSetupFixture.After.Default",
                "ActionAttributeTests.OnBaseSetupFixture.After.Suite",
                "ActionAttributeTests.OnBaseSetupFixture.After.Test, Suite",
                ASSEMBLY_NAME + ".OnAssembly.After.Default",
                ASSEMBLY_NAME + ".OnAssembly.After.Suite",
                ASSEMBLY_NAME + ".OnAssembly.After.Test, Suite"
            });

        private static readonly int NumTestCaseActions = ExpectedTestCaseActions.Length;
        private static readonly int EventsPerTestCase = 2 * NumTestCaseActions + 1;
        private static readonly int NumTestCaseEvents = 3 * EventsPerTestCase;
        private static readonly int NumParameterizedTestActions = ExpectedParameterizedTestActions.Length;
        private static readonly int NumTestFixtureActions = ExpectedTestFixtureActions.Length;
        private static readonly int NumSetUpFixtureActions = ExpectedSetUpFixtureActions.Length;
        private static readonly int NumAssemblyActions = ExpectedAssemblyActions.Length;

        #endregion
    }
}
