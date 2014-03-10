#if NET_4_5
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class NUnitTestCaseBuilderTests
    {
        private static readonly System.Type fixtureType = typeof(AsyncDummyFixture);

        [TestCase("AsyncVoid", RunState.Runnable)]
        [TestCase("AsyncTask", RunState.Runnable)]
        [TestCase("AsyncGenericTask", RunState.NotRunnable)]
        [TestCase("NonAsyncTask", RunState.NotRunnable)]
        [TestCase("NonAsyncGenericTask", RunState.NotRunnable)]
        public void AsyncTests(string methodName, RunState expectedState)
        {
            var test = TestBuilder.MakeTestCase(fixtureType, methodName);
            Assert.That(test.RunState, Is.EqualTo(expectedState));
        }

        [TestCase("AsyncVoidTestCase", RunState.Runnable)]
        [TestCase("AsyncVoidTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("AsyncTaskTestCase", RunState.Runnable)]
        [TestCase("AsyncTaskTestCaseWithExpectedResult", RunState.NotRunnable)]
        [TestCase("AsyncGenericTaskTestCase", RunState.NotRunnable)]
        [TestCase("AsyncGenericTaskTestCaseWithExpectedResult", RunState.Runnable)]
        public void AsyncTestCases(string methodName, RunState expectedState)
        {
            var suite = TestBuilder.MakeParameterizedMethodSuite(fixtureType, methodName);
            var testCase = (Test)suite.Tests[0];
            Assert.That(testCase.RunState, Is.EqualTo(expectedState));
        }
    }
}
#endif