// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
#if NET_4_0 || NET_4_5 || PORTABLE
        private readonly Type fixtureType = typeof(AsyncDummyFixture);

        [TestCase("AsyncVoid", RunState.NotRunnable)]
        [TestCase("AsyncTask", RunState.Runnable)]
        [TestCase("AsyncGenericTask", RunState.NotRunnable)]
        [TestCase("NonAsyncTask", RunState.Runnable)]
        [TestCase("NonAsyncGenericTask", RunState.NotRunnable)]
        public void AsyncTests(string methodName, RunState expectedState)
        {
            var test = TestBuilder.MakeTestCase(fixtureType, methodName);
            Assert.That(test.RunState, Is.EqualTo(expectedState));
        }

        [TestCase("AsyncVoidTestCase", RunState.NotRunnable)]
        [TestCase("AsyncVoidTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("AsyncTaskTestCase", RunState.Runnable)]
        [TestCase("AsyncTaskTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("AsyncGenericTaskTestCase", RunState.NotRunnable)]
        [TestCase("AsyncGenericTaskTestCaseWithExpectedResult", RunState.Runnable)]
        [TestCase("TaskTestCase", RunState.Runnable)]
        [TestCase("TaskTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("GenericTaskTestCase", RunState.NotRunnable)]
        [TestCase("GenericTaskTestCaseWithExpectedResult", RunState.Runnable)]
        public void AsyncTestCases(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(fixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }
#endif

#if !NETCF
        private readonly Type optionalTestParametersFixtureType = typeof(OptionalTestParametersFixture);

        [TestCase("MethodWithOptionalParams0", RunState.NotRunnable)]
        [TestCase("MethodWithOptionalParams1", RunState.Runnable)]
        [TestCase("MethodWithOptionalParams2", RunState.Runnable)]
        [TestCase("MethodWithOptionalParams3", RunState.NotRunnable)]
        [TestCase("MethodWithAllOptionalParams0", RunState.Runnable)]
        [TestCase("MethodWithAllOptionalParams1", RunState.Runnable)]
        [TestCase("MethodWithAllOptionalParams2", RunState.Runnable)]
        [TestCase("MethodWithAllOptionalParams3", RunState.NotRunnable)]
        public void ParametrizedTestCaseTests(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(optionalTestParametersFixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }
#endif
    }
}