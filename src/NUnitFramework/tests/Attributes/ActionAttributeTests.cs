// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.ActionAttributeTests;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public class ActionAttributeTests
    {
        // NOTE: An earlier version of this fixture attempted to test
        // the exact order of all actions. However, the order of execution 
        // of the individual tests cannot be predicted reliably across
        // different runtimes, so we now look only at the relative position
        // of before and after actions with respect to the test.

        private static readonly string ASSEMBLY_PATH = AssemblyHelper.GetAssemblyPath(typeof(ActionAttributeFixture));
        private static readonly string ASSEMBLY_NAME = System.IO.Path.GetFileName(ASSEMBLY_PATH);

        private ITestResult _result = null;
        private int index = -1;
        private int _case2 = -1;
        private int _simpleTest = -1;

        [OneTimeSetUp]
        public void Setup()
        {
#if NUNITLITE
            var runner = new NUnitLiteTestAssemblyRunner(new DefaultTestAssemblyBuilder());
#else
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
#endif

            ActionAttributeFixture.ClearResults();

            IDictionary options = new Hashtable();
            options["LOAD"] = new string[] { "NUnit.TestData.ActionAttributeTests" };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            Assert.NotNull(runner.Load(ASSEMBLY_PATH, options), "Assembly not loaded");

            _result = runner.Run(TestListener.NULL, TestFilter.Empty);
        }

        [Test]
        public void TestsRunSuccessfully()
        {
            Assert.That(_result.ResultState, Is.EqualTo(ResultState.Success));

            foreach(string message in ActionAttributeFixture.Events)
                Console.WriteLine(message);
        }

        [Test]
        public void ExpectedOutput_InCorrectOrder()
        {
            var expectedEvents = new List<string>( new string[] {
                ASSEMBLY_NAME + ".OnAssembly.Before.Suite",
                ASSEMBLY_NAME + ".OnAssembly.Before.Default",
                "ActionAttributeTests.OnBaseSetupFixture.Before.Suite",
                "ActionAttributeTests.OnBaseSetupFixture.Before.Default",
                "ActionAttributeTests.OnSetupFixture.Before.Suite",
                "ActionAttributeTests.OnSetupFixture.Before.Default",
                "ActionAttributeFixture.OnBaseInterface.Before.Suite",
                "ActionAttributeFixture.OnBaseInterface.Before.Default",
                "ActionAttributeFixture.OnBaseFixture.Before.Suite",
                "ActionAttributeFixture.OnBaseFixture.Before.Default",
                "ActionAttributeFixture.OnInterface.Before.Suite",
                "ActionAttributeFixture.OnInterface.Before.Default",
                "ActionAttributeFixture.OnFixture.Before.Suite",
                "ActionAttributeFixture.OnFixture.Before.Default",
                "ParameterizedTest.OnMethod.Before.Suite",
                "CaseOne.OnAssembly.Before.Test",
                "CaseOne.OnBaseSetupFixture.Before.Test",
                "CaseOne.OnSetupFixture.Before.Test",
                "CaseOne.OnBaseInterface.Before.Test",
                "CaseOne.OnBaseFixture.Before.Test",
                "CaseOne.OnInterface.Before.Test",
                "CaseOne.OnFixture.Before.Test",
                "CaseOne.OnMethod.Before.Test",
                "CaseOne.OnMethod.Before.Default",
                "CaseOne",
                "CaseOne.OnMethod.After.Default",
                "CaseOne.OnMethod.After.Test",
                "CaseOne.OnFixture.After.Test",
                "CaseOne.OnInterface.After.Test",
                "CaseOne.OnBaseFixture.After.Test",
                "CaseOne.OnBaseInterface.After.Test",
                "CaseOne.OnSetupFixture.After.Test",
                "CaseOne.OnBaseSetupFixture.After.Test",
                "CaseOne.OnAssembly.After.Test",
                "CaseTwo.OnAssembly.Before.Test",
                "CaseTwo.OnBaseSetupFixture.Before.Test",
                "CaseTwo.OnSetupFixture.Before.Test",
                "CaseTwo.OnBaseInterface.Before.Test",
                "CaseTwo.OnBaseFixture.Before.Test",
                "CaseTwo.OnInterface.Before.Test",
                "CaseTwo.OnFixture.Before.Test",
                "CaseTwo.OnMethod.Before.Test",
                "CaseTwo.OnMethod.Before.Default",
                "CaseTwo",
                "CaseTwo.OnMethod.After.Default",
                "CaseTwo.OnMethod.After.Test",
                "CaseTwo.OnFixture.After.Test",
                "CaseTwo.OnInterface.After.Test",
                "CaseTwo.OnBaseFixture.After.Test",
                "CaseTwo.OnBaseInterface.After.Test",
                "CaseTwo.OnSetupFixture.After.Test",
                "CaseTwo.OnBaseSetupFixture.After.Test",
                "CaseTwo.OnAssembly.After.Test",
                "ParameterizedTest.OnMethod.After.Suite",
                "SimpleTest.OnAssembly.Before.Test",
                "SimpleTest.OnBaseSetupFixture.Before.Test",
                "SimpleTest.OnSetupFixture.Before.Test",
                "SimpleTest.OnBaseInterface.Before.Test",
                "SimpleTest.OnBaseFixture.Before.Test",
                "SimpleTest.OnInterface.Before.Test",
                "SimpleTest.OnFixture.Before.Test",
                "SimpleTest.OnMethod.Before.Test",
                "SimpleTest.OnMethod.Before.Default",
                "SimpleTest",
                "SimpleTest.OnMethod.After.Default",
                "SimpleTest.OnMethod.After.Test",
                "SimpleTest.OnFixture.After.Test",
                "SimpleTest.OnInterface.After.Test",
                "SimpleTest.OnBaseFixture.After.Test",
                "SimpleTest.OnBaseInterface.After.Test",
                "SimpleTest.OnSetupFixture.After.Test",
                "SimpleTest.OnBaseSetupFixture.After.Test",
                "SimpleTest.OnAssembly.After.Test",
                "ActionAttributeFixture.OnFixture.After.Default",
                "ActionAttributeFixture.OnFixture.After.Suite",
                "ActionAttributeFixture.OnInterface.After.Default",
                "ActionAttributeFixture.OnInterface.After.Suite",
                "ActionAttributeFixture.OnBaseFixture.After.Default",
                "ActionAttributeFixture.OnBaseFixture.After.Suite",
                "ActionAttributeFixture.OnBaseInterface.After.Default",
                "ActionAttributeFixture.OnBaseInterface.After.Suite",
                "ActionAttributeTests.OnSetupFixture.After.Default",
                "ActionAttributeTests.OnSetupFixture.After.Suite",
                "ActionAttributeTests.OnBaseSetupFixture.After.Default",
                "ActionAttributeTests.OnBaseSetupFixture.After.Suite",
                ASSEMBLY_NAME + ".OnAssembly.After.Default",
                ASSEMBLY_NAME + ".OnAssembly.After.Suite"
            } );

            var notFound = new List<string>();
            var notExpected = new List<string>();

            foreach (var item in expectedEvents)
                if (!ActionAttributeFixture.Events.Contains(item))
                    notFound.Add(item);

            foreach (var item in ActionAttributeFixture.Events)
                if (!expectedEvents.Contains(item))
                    notExpected.Add(item);

            if (notFound.Count > 0 || notExpected.Count > 0)
            {
                var sb = new StringBuilder("Expected and actual events do not match.");

                if (notFound.Count > 0)
                {
                    sb.Append(Env.NewLine + "   Missing:");
                    foreach (var item in notFound)
                        sb.Append(Env.NewLine + "     " + item);
                }

                if (notExpected.Count > 0)
                {
                    sb.Append(Env.NewLine + "   Extra:");
                    foreach (var item in notExpected)
                        sb.Append(Env.NewLine + "     " + item);
                }

                Assert.Fail(sb.ToString());
            }
        }

        [Test]
        public void AttributesOnAssembly()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(0, numEvents - 1, ASSEMBLY_NAME, "OnAssembly");
            CheckBeforeAfterPair(1, numEvents - 2, ASSEMBLY_NAME, "OnAssembly");
        }

        [Test]
        public void AttributesOnBaseSetUpFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(2, numEvents - 3, "ActionAttributeTests", "OnBaseSetupFixture");
            CheckBeforeAfterPair(3, numEvents - 4, "ActionAttributeTests", "OnBaseSetupFixture");
        }

        [Test]
        public void AttributesOnSetUpFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(4, numEvents - 5, "ActionAttributeTests", "OnSetupFixture");
            CheckBeforeAfterPair(5, numEvents - 6, "ActionAttributeTests", "OnSetupFixture");
        }

        [Test]
        public void AttributesOnBaseInterface()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(6, numEvents - 7, "ActionAttributeFixture", "OnBaseInterface");
            CheckBeforeAfterPair(7, numEvents - 8, "ActionAttributeFixture", "OnBaseInterface");
        }

        [Test]
        public void AttributesOnBaseFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(8, numEvents - 9, "ActionAttributeFixture", "OnBaseFixture");
            CheckBeforeAfterPair(9, numEvents - 10, "ActionAttributeFixture", "OnBaseFixture");
        }

        [Test]
        public void AttributesOnInterface()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(10, numEvents - 11, "ActionAttributeFixture", "OnInterface");
            CheckBeforeAfterPair(11, numEvents - 12, "ActionAttributeFixture", "OnInterface");
        }

        [Test]
        public void AttributesOnFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(12, numEvents - 13, "ActionAttributeFixture", "OnFixture");
            CheckBeforeAfterPair(13, numEvents - 14, "ActionAttributeFixture", "OnFixture");
        }

        [TestCase("CaseOne")]
        [TestCase("CaseTwo")]
        public void AttributesWrappingParameterizedTestMethod(string testName)
        {
            index = ActionAttributeFixture.Events.IndexOf(testName);
            Assert.That(index, Is.GreaterThanOrEqualTo(0), testName + " did not execute");

            CheckBeforeAfterPair(index - 1, index + 1, testName, "OnMethod");
            CheckBeforeAfterPair(index - 2, index + 2, testName, "OnMethod");
            CheckBeforeAfterPair(index - 3, index + 3, testName, "OnMethod", "Test");
            CheckBeforeAfterPair(index - 4, index + 4, testName, "OnFixture", "Test");
            CheckBeforeAfterPair(index - 5, index + 5, testName, "OnInterface", "Test");
            CheckBeforeAfterPair(index - 6, index + 6, testName, "OnBaseFixture", "Test");
            CheckBeforeAfterPair(index - 7, index + 7, testName, "OnBaseInterface", "Test");
            CheckBeforeAfterPair(index - 8, index + 8, testName, "OnSetupFixture", "Test");
            CheckBeforeAfterPair(index - 9, index + 9, testName, "OnBaseSetupFixture", "Test");
        }

        [Test]
        public void AttributesWrappingSimpleTestMethod()
        {
            index = ActionAttributeFixture.Events.IndexOf("SimpleTest");
            Assert.That(index, Is.GreaterThanOrEqualTo(0), "SimpleTest did not execute");

            CheckBeforeAfterPair(index - 1, index + 1, "SimpleTest", "OnMethod");
            CheckBeforeAfterPair(index - 2, index + 2, "SimpleTest", "OnMethod");
            CheckBeforeAfterPair(index - 3, index + 3, "SimpleTest", "OnFixture", "Test");
            CheckBeforeAfterPair(index - 4, index + 4, "SimpleTest", "OnInterface", "Test");
            CheckBeforeAfterPair(index - 5, index + 5, "SimpleTest", "OnBaseFixture", "Test");
            CheckBeforeAfterPair(index - 6, index + 6, "SimpleTest", "OnBaseInterface", "Test");
            CheckBeforeAfterPair(index - 7, index + 7, "SimpleTest", "OnSetupFixture", "Test");
            CheckBeforeAfterPair(index - 8, index + 8, "SimpleTest", "OnBaseSetupFixture", "Test");
        }

        private void CheckBeforeAfterPair(int index1, int index2, string testName, string tag, string target)
        {
            var event1 = ActionAttributeFixture.Events[index1];
            var event2 = ActionAttributeFixture.Events[index2];

            Assert.That(event1, Does.StartWith(testName + "." + tag + ".Before"));
            Assert.That(event2, Does.StartWith(testName + "." + tag + ".After"));

            int index = event1.LastIndexOf('.');
            var target1 = event1.Substring(index); // Target is last in string
            if (target != null)
                Assert.That(target1, Is.EqualTo("." + target));
            Assert.That(event2, Does.EndWith(target1), "Event mismatch");
        }

        private void CheckBeforeAfterPair(int index1, int index2, string testName, string tag)
        {
            CheckBeforeAfterPair(index1, index2, testName, tag, null);
        }
    }
}
