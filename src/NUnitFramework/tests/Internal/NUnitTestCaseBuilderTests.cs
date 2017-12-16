// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class NUnitTestCaseBuilderTests
    {
#if ASYNC
        private readonly Type fixtureType = typeof(AsyncDummyFixture);

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
#endif

        private readonly Type optionalTestParametersFixtureType = typeof(OptionalTestParametersFixture);

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
    }
}
