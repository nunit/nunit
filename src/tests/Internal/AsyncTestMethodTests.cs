#if NET_4_0 || NET_4_5
using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class AsyncTestMethodTests
    {
        private static readonly bool ON_LINUX = OSPlatform.CurrentPlatform.IsUnix;

        private DefaultTestCaseBuilder _builder;
        private object _testObject;

        [SetUp]
        public void Setup()
        {
            _builder = new DefaultTestCaseBuilder();
            _testObject = new AsyncRealFixture();
        }

        public IEnumerable TestCases
        {
            get
            {
				yield return GetTestCase(Method("AsyncVoid"), ResultState.NotRunnable, 0, false);

				yield return GetTestCase(Method("AsyncTaskSuccess"), ResultState.Success, 1, true);
				yield return GetTestCase(Method("AsyncTaskFailure"), ResultState.Failure, 1, true);
				yield return GetTestCase(Method("AsyncTaskError"), ResultState.Error, 0, false);

				yield return GetTestCase(Method("TaskSuccess"), ResultState.Success, 1, true);
				yield return GetTestCase(Method("TaskFailure"), ResultState.Failure, 1, true);
				yield return GetTestCase(Method("TaskError"), ResultState.Error, 0, false);

				yield return GetTestCase(Method("AsyncTaskResult"), ResultState.NotRunnable, 0, false);
				yield return GetTestCase(Method("TaskResult"), ResultState.NotRunnable, 0, false);

				yield return GetTestCase(Method("AsyncTaskResultCheckSuccess"), ResultState.Success, 1, false);
				yield return GetTestCase(Method("AsyncTaskResultCheckFailure"), ResultState.Failure, 1, false);
				yield return GetTestCase(Method("AsyncTaskResultCheckError"), ResultState.Failure, 0, false);

				yield return GetTestCase(Method("TaskResultCheckSuccess"), ResultState.Success, 1, false);
				yield return GetTestCase(Method("TaskResultCheckFailure"), ResultState.Failure, 1, false);
				yield return GetTestCase(Method("TaskResultCheckError"), ResultState.Failure, 0, false);

				yield return GetTestCase(Method("AsyncTaskTestCaseWithParametersSuccess"), ResultState.Success, 1, true);
				yield return GetTestCase(Method("AsyncTaskResultCheckSuccessReturningNull"), ResultState.Success, 1, false);
				yield return GetTestCase(Method("TaskResultCheckSuccessReturningNull"), ResultState.Success, 1, false);
                
				yield return GetTestCase(Method("NestedAsyncTaskSuccess"), ResultState.Success, 1, false);
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
		private TestCaseData GetTestCase(MethodInfo method, ResultState resultState, int assertionCount, bool ignoreOnLinux)
		{
			var data = new TestCaseData(method, resultState, assertionCount);
			if (ON_LINUX && ignoreOnLinux)
				data = data.Ignore("Intermittent failure on Linux");
			return data;
		}

        [Test]
        [TestCaseSource("TestCases")]
        public void RunTests(MethodInfo method, ResultState resultState, int assertionCount)
        {
            var test = _builder.BuildFrom(method);
            var result = TestBuilder.RunTest(test, _testObject);

            Assert.That(result.ResultState, Is.EqualTo(resultState), "Wrong result state");
            Assert.That(result.AssertCount, Is.EqualTo(assertionCount), "Wrong assertion count");
        }

        private static MethodInfo Method(string name)
        {
            return typeof(AsyncRealFixture).GetMethod(name);
        }
    }
}
#endif