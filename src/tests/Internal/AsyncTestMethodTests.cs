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
                yield return new object[] { Method("AsyncVoid"), ResultState.NotRunnable, 0 };

                yield return new object[] { Method("AsyncTaskSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("AsyncTaskFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("AsyncTaskError"), ResultState.Error, 0 };

                yield return new object[] { Method("TaskSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("TaskFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("TaskError"), ResultState.Error, 0 };

                yield return new object[] { Method("AsyncTaskResult"), ResultState.NotRunnable, 0 };
                yield return new object[] { Method("TaskResult"), ResultState.NotRunnable, 0 };

                yield return new object[] { Method("AsyncTaskResultCheckSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("AsyncTaskResultCheckFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("AsyncTaskResultCheckError"), ResultState.Failure, 0 };

                yield return new object[] { Method("TaskResultCheckSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("TaskResultCheckFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("TaskResultCheckError"), ResultState.Failure, 0 };

                yield return new object[] { Method("AsyncTaskTestCaseWithParametersSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("AsyncTaskResultCheckSuccessReturningNull"), ResultState.Success, 1 };
                yield return new object[] { Method("TaskResultCheckSuccessReturningNull"), ResultState.Success, 1 };
                
                yield return new object[] { Method("NestedAsyncTaskSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("NestedAsyncTaskFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("NestedAsyncTaskError"), ResultState.Error, 0 };

                yield return new object[] { Method("AsyncTaskMultipleSuccess"), ResultState.Success, 1 };
                yield return new object[] { Method("AsyncTaskMultipleFailure"), ResultState.Failure, 1 };
                yield return new object[] { Method("AsyncTaskMultipleError"), ResultState.Error, 0 };

                yield return new object[] { Method("TaskCheckTestContextAcrossTasks"), ResultState.Success, 2 };
                yield return new object[] { Method("TaskCheckTestContextWithinTestBody"), ResultState.Success, 2 };
            }
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