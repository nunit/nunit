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

//#define DEFAULT_APPLIES_TO_TESTCASE
using System;
using System.Collections;
using System.Collections.Generic;
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
        private int _case1 = -1;
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
            _case1 = ActionAttributeFixture.Events.IndexOf("CaseOne");
            _case2 = ActionAttributeFixture.Events.IndexOf("CaseTwo");
            _simpleTest = ActionAttributeFixture.Events.IndexOf("SimpleTest");

            Assert.That(_case1, Is.GreaterThanOrEqualTo(0), "CaseOne did not execute");
            Assert.That(_case2, Is.GreaterThanOrEqualTo(0), "CaseTwo did not execute");
            Assert.That(_simpleTest, Is.GreaterThanOrEqualTo(0), "SimpleTest did not execute");
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
            string[] expectedResults = new string[] {
                ASSEMBLY_NAME + ".OnAssembly.Before.Suite",
                ASSEMBLY_NAME + ".OnAssembly.Before.Default",
//                "BaseSetupFixtureSuite.Before.true.false",
//                "BaseSetupFixtureSite.Before.true.false",
//                "SetupFixtureSuite.Before.true.false",
//                "SetupFixtureSite.Before.true.false",
//                "BaseInterfaceSuite.Before.true.false",
//                "BaseInterfaceSite.Before.true.false",
                "ActionAttributeFixture.OnBaseFixture.Before.Suite",
                "ActionAttributeFixture.OnBaseFixture.Before.Default",
//                "InterfaceSuite.Before.true.false",
//                "InterfaceSite.Before.true.false",
                "ActionAttributeFixture.OnFixture.Before.Suite",
                "ActionAttributeFixture.OnFixture.Before.Default",
//                "ParameterizedSuite.Before.false",
//#if DEFAULT_APPLIES_TO_TESTCASE
//                "ParameterizedSite.Before.false",
//#endif
//                "AssemblyTest.Before.true.true",
//                "BaseSetupFixtureTest.Before.true.true",
//                "SetupFixtureTest.Before.true.true",
//                "BaseInterfaceTest.Before.true.true",
//                "BaseFixtureTest.Before.true.true",
//                "InterfaceTest.Before.true.true",
//                "FixtureTest.Before.true.true",
                "CaseOne.OnMethod.Before.Test",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseOne.OnMethod.Before.Default",
#endif
                "CaseOne",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseOne.OnMethod.After.Default",
#endif
                "CaseOne.OnMethod.After.Test",
//                "FixtureTest.After.true.true",
//                "InterfaceTest.After.true.true",
//                "BaseFixtureTest.After.true.true",
//                "BaseInterfaceTest.After.true.true",
//                "SetupFixtureTest.After.true.true",
//                "BaseSetupFixtureTest.After.true.true",
//                "AssemblyTest.After.true.true",
//                "AssemblyTest.Before.true.true",
//                "BaseSetupFixtureTest.Before.true.true",
//                "SetupFixtureTest.Before.true.true",
//                "BaseInterfaceTest.Before.true.true",
//                "BaseFixtureTest.Before.true.true",
//                "InterfaceTest.Before.true.true",
//                "FixtureTest.Before.true.true",
                "CaseTwo.OnMethod.Before.Test",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseTwo.OnMethod.Before.Default",
#endif
                "CaseTwo",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseTwo.OnMethod.After.Default",
#endif
                "CaseTwo.OnMethod.After.Test",
//                "FixtureTest.After.true.true",
//                "InterfaceTest.After.true.true",
//                "BaseFixtureTest.After.true.true",
//                "BaseInterfaceTest.After.true.true",
//                "SetupFixtureTest.After.true.true",
//                "BaseSetupFixtureTest.After.true.true",
//                "AssemblyTest.After.true.true",
//#if DEFAULT_APPLIES_TO_TESTCASE
//                "ParameterizedSite.After.true.false",
//#endif
//                "ParameterizedSuite.After.true.false",
//                "AssemblyTest.Before.true.true",
//                "BaseSetupFixtureTest.Before.true.true",
//                "SetupFixtureTest.Before.true.true",
//                "BaseInterfaceTest.Before.true.true",
//                "BaseFixtureTest.Before.true.true",
//                "InterfaceTest.Before.true.true",
//                "FixtureTest.Before.true.true",
                "SimpleTest.OnMethod.Before.Test",
                "SimpleTest.OnMethod.Before.Default",
                "SimpleTest",
                "SimpleTest.OnMethod.After.Default",
                "SimpleTest.OnMethod.After.Test",
                //"FixtureTest.After.true.true",
                //"InterfaceTest.After.true.true",
                //"BaseFixtureTest.After.true.true",
                //"BaseInterfaceTest.After.true.true",
                //"SetupFixtureTest.After.true.true",
                //"BaseSetupFixtureTest.After.true.true",
                //"AssemblyTest.After.true.true",
                "ActionAttributeFixture.OnFixture.After.Default",
                "ActionAttributeFixture.OnFixture.After.Suite",
                //"InterfaceSite.After.true.false",
                //"InterfaceSuite.After.true.false",
                "ActionAttributeFixture.OnBaseFixture.After.Default",
                "ActionAttributeFixture.OnBaseFixture.After.Suite",
                //"BaseInterfaceSite.After.true.false",
                //"BaseInterfaceSuite.After.true.false",
                //"SetupFixtureSite.After.true.false",
                //"SetupFixtureSuite.After.true.false",
                //"BaseSetupFixtureSite.After.true.false",
                //"BaseSetupFixtureSuite.After.true.false",
                ASSEMBLY_NAME + ".OnAssembly.After.Default",
                ASSEMBLY_NAME + ".OnAssembly.After.Suite"
            };

            foreach (var item in expectedResults)
                Assert.That(ActionAttributeFixture.Events, Contains.Item(item));
            //Assert.That(ActionAttributeFixture.Events, Is.EquivalentTo(expectedResults));
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
        public void AttributesOnBaseFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(6, numEvents - 7, "ActionAttributeFixture", "OnBaseFixture");
            CheckBeforeAfterPair(7, numEvents - 8, "ActionAttributeFixture", "OnBaseFixture");
        }

        [Test]
        public void AttributesOnFixture()
        {
            var numEvents = ActionAttributeFixture.Events.Count;
            CheckBeforeAfterPair(8, numEvents - 9, "ActionAttributeFixture", "OnFixture");
            CheckBeforeAfterPair(9, numEvents - 10, "ActionAttributeFixture", "OnFixture");
        }

        [Test]
        public void AttributesOnMethod_CaseOne()
        {
            CheckBeforeAfterPair(_case1 - 1, _case1 + 1, "CaseOne", "OnMethod");
            CheckBeforeAfterPair(_case1 - 2, _case1 + 2, "CaseOne", "OnMethod");
        }

        [Test]
        public void AttributesOnMethod_CaseTwo()
        {
            CheckBeforeAfterPair(_case2 - 1, _case2 + 1, "CaseTwo", "OnMethod");
            CheckBeforeAfterPair(_case2 - 2, _case2 + 2, "CaseTwo", "OnMethod");
        }

        [Test]
        public void AttributesOnMethod_SimpleTest()
        {
            CheckBeforeAfterPair(_simpleTest - 1, _simpleTest + 1, "SimpleTest", "OnMethod");
            CheckBeforeAfterPair(_simpleTest - 2, _simpleTest + 2, "SimpleTest", "OnMethod");
        }

        private void CheckBeforeAfterPair(int index1, int index2, string testName, string tag)
        {
            var event1 = ActionAttributeFixture.Events[index1];
            var event2 = ActionAttributeFixture.Events[index2];

            Assert.That(event1, Does.StartWith(testName + "." + tag + ".Before"));
            Assert.That(event2, Does.StartWith(testName + "." + tag + ".After"));

            int index = event1.LastIndexOf('.');
            var target1 = event1.Substring(index); // Target is last in string
            Assert.That(event2, Does.EndWith(target1), "Event mismatch");
        }
    }
}
