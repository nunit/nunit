// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class AsyncTestMethodTests
    {
        private static readonly bool PLATFORM_IGNORE = OSPlatform.CurrentPlatform.IsUnix;

        private DefaultTestCaseBuilder _builder;
        private object _testObject;

        [SetUp]
        public void Setup()
        {
            _builder = new DefaultTestCaseBuilder();
            _testObject = new AsyncRealFixture();
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return GetTestCase(Method("AsyncVoid"), ResultState.NotRunnable, 0, false);

                yield return GetTestCase(Method("AsyncTaskSuccess"), ResultState.Success, 1, true);
                yield return GetTestCase(Method("AsyncTaskFailure"), ResultState.Failure, 1, true);
                yield return GetTestCase(Method("AsyncTaskError"), ResultState.Error, 0, false);
                yield return GetTestCase(Method("AsyncTaskPass"), ResultState.Success, 0, false);
                yield return GetTestCase(Method("AsyncTaskIgnore"), ResultState.Ignored, 0, false);
                yield return GetTestCase(Method("AsyncTaskInconclusive"), ResultState.Inconclusive, 0, false);

                yield return GetTestCase(Method("TaskSuccess"), ResultState.Success, 1, true);
                yield return GetTestCase(Method("TaskFailure"), ResultState.Failure, 1, true);
                yield return GetTestCase(Method("TaskError"), ResultState.Error, 0, false);
                yield return GetTestCase(Method("TaskPass"), ResultState.Success, 0, false);
                yield return GetTestCase(Method("TaskIgnore"), ResultState.Ignored, 0, false);
                yield return GetTestCase(Method("TaskInconclusive"), ResultState.Inconclusive, 0, false);

                yield return GetTestCase(Method("AsyncTaskResult"), ResultState.NotRunnable, 0, false);
                yield return GetTestCase(Method("TaskResult"), ResultState.NotRunnable, 0, false);

                yield return GetTestCase(Method("AsyncTaskResultCheckSuccess"), ResultState.Success, 1, false);
                yield return GetTestCase(Method("AsyncTaskResultCheckFailure"), ResultState.ChildFailure, 1, false);
                yield return GetTestCase(Method("AsyncTaskResultCheckError"), ResultState.ChildFailure, 0, false);

                yield return GetTestCase(Method("TaskResultCheckSuccess"), ResultState.Success, 1, false);
                yield return GetTestCase(Method("TaskResultCheckFailure"), ResultState.ChildFailure, 1, false);
                yield return GetTestCase(Method("TaskResultCheckError"), ResultState.ChildFailure, 0, false);

                yield return GetTestCase(Method("AsyncTaskTestCaseWithParametersSuccess"), ResultState.Success, 1, true);
                yield return GetTestCase(Method("AsyncTaskResultCheckSuccessReturningNull"), ResultState.Success, 1, false);
                yield return GetTestCase(Method("TaskResultCheckSuccessReturningNull"), ResultState.Success, 1, false);

                yield return GetTestCase(Method("NestedAsyncTaskSuccess"), ResultState.Success, 1, true);
                yield return GetTestCase(Method("NestedAsyncTaskFailure"), ResultState.Failure, 1, true);
                yield return GetTestCase(Method("NestedAsyncTaskError"), ResultState.Error, 0, false);

                yield return GetTestCase(Method("AsyncTaskMultipleSuccess"), ResultState.Success, 1, true);
                yield return GetTestCase(Method("AsyncTaskMultipleFailure"), ResultState.Failure, 1, true);
                yield return GetTestCase(Method("AsyncTaskMultipleError"), ResultState.Error, 0, false);

                yield return GetTestCase(Method("TaskCheckTestContextAcrossTasks"), ResultState.Success, 2, true);
                yield return GetTestCase(Method("TaskCheckTestContextWithinTestBody"), ResultState.Success, 2, true);
            }
        }

        /// <summary>
        /// Private method to return a test case, optionally ignored on the Linux platform.
        /// We use this since the Platform attribute is not supported on TestCaseData.
        /// </summary>
        private static TestCaseData GetTestCase(IMethodInfo method, ResultState resultState, int assertionCount, bool ignoreThis)
        {
            var data = new TestCaseData(method, resultState, assertionCount);
            if (PLATFORM_IGNORE && ignoreThis)
                data = data.Ignore("Intermittent failure on Linux");
            return data;
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void RunTests(IMethodInfo method, ResultState resultState, int assertionCount)
        {
            var test = _builder.BuildFrom(method);
            var result = TestBuilder.RunTest(test, _testObject);

            Assert.That(result.ResultState, Is.EqualTo(resultState), "Wrong result state");
            Assert.That(result.AssertCount, Is.EqualTo(assertionCount), "Wrong assertion count");
        }

        private static IMethodInfo Method(string name)
        {
            return new MethodWrapper(typeof(AsyncRealFixture), name);
        }
    }
}
