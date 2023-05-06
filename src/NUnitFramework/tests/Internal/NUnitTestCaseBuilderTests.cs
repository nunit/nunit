// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class NUnitTestCaseBuilderTests
    {
#pragma warning disable IDE1006 // Naming Styles
        private readonly Type fixtureType = typeof(AsyncDummyFixture);
#pragma warning restore IDE1006 // Naming Styles

        [TestCase(nameof(AsyncDummyFixture.AsyncVoid), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncTask), RunState.Runnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncGenericTask), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.NonAsyncTask), RunState.Runnable)]
        [TestCase(nameof(AsyncDummyFixture.NonAsyncGenericTask), RunState.NotRunnable)]
        public void AsyncTests(string methodName, RunState expectedState)
        {
            var test = TestBuilder.MakeTestCase(fixtureType, methodName);
            Assert.That(test.RunState, Is.EqualTo(expectedState));
        }

        [TestCase(nameof(AsyncDummyFixture.AsyncVoidTestCase), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncVoidTestCaseWithExpectedResult), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncTaskTestCase), RunState.Runnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncTaskTestCaseWithExpectedResult), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncGenericTaskTestCase), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.AsyncGenericTaskTestCaseWithExpectedResult), RunState.Runnable)]
        [TestCase(nameof(AsyncDummyFixture.TaskTestCase), RunState.Runnable)]
        [TestCase(nameof(AsyncDummyFixture.TaskTestCaseWithExpectedResult), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.GenericTaskTestCase), RunState.NotRunnable)]
        [TestCase(nameof(AsyncDummyFixture.GenericTaskTestCaseWithExpectedResult), RunState.Runnable)]
        public void AsyncTestCases(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(fixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }

#pragma warning disable IDE1006 // Naming Styles
        private readonly Type optionalTestParametersFixtureType = typeof(OptionalTestParametersFixture);
#pragma warning restore IDE1006 // Naming Styles

        [TestCase(nameof(OptionalTestParametersFixture.MethodWithOptionalParams0), RunState.NotRunnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithOptionalParams1), RunState.Runnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithOptionalParams2), RunState.Runnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithOptionalParams3), RunState.NotRunnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithAllOptionalParams0), RunState.Runnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithAllOptionalParams1), RunState.Runnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithAllOptionalParams2), RunState.Runnable)]
        [TestCase(nameof(OptionalTestParametersFixture.MethodWithAllOptionalParams3), RunState.NotRunnable)]
        public void ParametrizedTestCaseTests(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(optionalTestParametersFixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }

        private readonly Type _testNameFixtureType = typeof(TestNameFixture);

        [TestCase(nameof(TestNameFixture.ImplicitNull), RunState.Runnable)]
        [TestCase(nameof(TestNameFixture.ExplicitNull), RunState.Runnable)]
        [TestCase(nameof(TestNameFixture.EmptyTest), RunState.NotRunnable)]
        [TestCase(nameof(TestNameFixture.WhiteSpaceTest), RunState.NotRunnable)]
        [TestCase(nameof(TestNameFixture.ProperNameTest), RunState.Runnable)]
        public void TestNameTests(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(_testNameFixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }
    }
}
