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

        private ITestResult _result = null;
        private int _case1 = -1;
        private int _case2 = -1;
        private int _simpleTest = -1;

        private readonly string[] _suiteSites = new string[]
        {
            "Assembly",
            "BaseSetupFixture",
            "SetupFixture",
            "BaseInterface",
            "BaseFixture",
            "Interface",
            "Fixture"
        };

        private readonly string[] _parameterizedTestOutput = new string[]
        {
            "SomeTest-Case1",
            "SomeTest-Case2"
        };

        private readonly string[] _testOutput = new string[]
        {
            "SomeTestNotParameterized",
        };

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
            options["LOAD"] = new string[] { typeof(ActionAttributeFixture).FullName };
            // No need for the overhead of parallel execution here
            options["NumberOfTestWorkers"] = 0;

            Assert.NotNull(runner.Load(AssemblyHelper.GetAssemblyPath(typeof(ActionAttributeFixture)), options), "Assembly not loaded");

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
//                "AssemblySuite.Before.false.false",
//                "AssemblySite.Before.false.false",
//                "BaseSetupFixtureSuite.Before.true.false",
//                "BaseSetupFixtureSite.Before.true.false",
//                "SetupFixtureSuite.Before.true.false",
//                "SetupFixtureSite.Before.true.false",
//                "BaseInterfaceSuite.Before.true.false",
//                "BaseInterfaceSite.Before.true.false",
//                "BaseFixtureSuite.Before.true.false",
//                "BaseFixtureSite.Before.true.false",
//                "InterfaceSuite.Before.true.false",
//                "InterfaceSite.Before.true.false",
//                "FixtureSuite.Before.true.false",
//                "FixtureSite.Before.true.false",
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
                "CaseOne Action=ParameterizedTest Phase=Before Target=Test",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseOne Action=ParameterizedSite Phase=Before Target=Default",
#endif
                "CaseOne",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseOne Action=ParameterizedSite Phase=After Target=Default",
#endif
                "CaseOne Action=ParameterizedTest Phase=After Target=Test",
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
                "CaseTwo Action=ParameterizedTest Phase=Before Target=Test",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseTwo Action=ParameterizedSite Phase=Before Target=Default",
#endif
                "CaseTwo",
#if !DEFAULT_APPLIES_TO_TESTCASE
                "CaseTwo Action=ParameterizedSite Phase=After Target=Default",
#endif
                "CaseTwo Action=ParameterizedTest Phase=After Target=Test",
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
                "SimpleTest Action=MethodTest Phase=Before Target=Test",
                "SimpleTest Action=MethodSite Phase=Before Target=Default",
                "SimpleTest",
                "SimpleTest Action=MethodSite Phase=After Target=Default",
                "SimpleTest Action=MethodTest Phase=After Target=Test"
                //"FixtureTest.After.true.true",
                //"InterfaceTest.After.true.true",
                //"BaseFixtureTest.After.true.true",
                //"BaseInterfaceTest.After.true.true",
                //"SetupFixtureTest.After.true.true",
                //"BaseSetupFixtureTest.After.true.true",
                //"AssemblyTest.After.true.true",
                //"FixtureSite.After.true.false",
                //"FixtureSuite.After.true.false",
                //"InterfaceSite.After.true.false",
                //"InterfaceSuite.After.true.false",
                //"BaseFixtureSite.After.true.false",
                //"BaseFixtureSuite.After.true.false",
                //"BaseInterfaceSite.After.true.false",
                //"BaseInterfaceSuite.After.true.false",
                //"SetupFixtureSite.After.true.false",
                //"SetupFixtureSuite.After.true.false",
                //"BaseSetupFixtureSite.After.true.false",
                //"BaseSetupFixtureSuite.After.true.false",
                //"AssemblySite.After.false.false",
                //"AssemblySuite.After.false.false"
            };

            foreach (var item in expectedResults)
                Assert.That(ActionAttributeFixture.Events, Contains.Item(item));
        }

        [Test]
        public void CaseOneIsWrappedByAttributesOnMethod()
        {
            CheckBeforeAfterPair(_case1 - 1, _case1 + 1, "CaseOne");
            CheckBeforeAfterPair(_case1 - 2, _case1 + 2, "CaseOne");
        }

        [Test]
        public void CaseTwoIsWrappedByAttributesOnMethod()
        {
            CheckBeforeAfterPair(_case2 - 1, _case2 + 1, "CaseTwo");
            CheckBeforeAfterPair(_case2 - 2, _case2 + 2, "CaseTwo");
        }

        [Test]
        public void SimpleTestIsWrappedByAttributesOnMethod()
        {
            CheckBeforeAfterPair(_simpleTest - 1, _simpleTest + 1, "SimpleTest");
            CheckBeforeAfterPair(_simpleTest - 2, _simpleTest + 2, "SimpleTest");
        }

        private void CheckBeforeAfterPair(int index1, int index2, string testName)
        {
            var event1 = ActionAttributeFixture.Events[index1];
            var event2 = ActionAttributeFixture.Events[index2];

            Assert.That(event1, Contains.Substring("Action="));
            int index = event1.IndexOf("Action=") + 7;
            var action1 = event1.Substring(index, event1.IndexOf(' ', index) - index);

            Assert.That(event1, Contains.Substring("Target="));
            index = event1.IndexOf("Target=") + 7;
            var target1 = event1.Substring(index); // Target is last in string

            Assert.That(event1, Contains.Substring("Phase=Before"));
            Assert.That(event2, Contains.Substring("Phase=After"));
            Assert.That(event1, Does.StartWith(testName));
            Assert.That(event2, Does.StartWith(testName));
            Assert.That(event2, Contains.Substring("Action=" + action1), "Event mismatch");
            Assert.That(event2, Contains.Substring("Target=" + target1), "Event mismatch");
        }
    }
}
