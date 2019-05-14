using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;
using F = NUnit.TestData.AwaitableReturnTypeFixture;

namespace NUnit.Framework
{
#if TASK_PARALLEL_LIBRARY_API
    [TestFixture(nameof(F.ReturnsTask))]
    [TestFixture(nameof(F.ReturnsCustomTask))]
#endif
    [TestFixture(nameof(F.ReturnsCustomAwaitable))]
    [TestFixture(nameof(F.ReturnsCustomAwaitableWithImplicitOnCompleted))]
    [TestFixture(nameof(F.ReturnsCustomAwaitableWithImplicitUnsafeOnCompleted))]
    public class AwaitableReturnTypeTests
    {
        private readonly string _methodName;

        public AwaitableReturnTypeTests(string methodName)
        {
            _methodName = methodName;
        }

        protected ITestResult RunCurrentTestMethod(AsyncWorkload workload)
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(F), _methodName);

            return TestBuilder.RunTest(test, new F(workload));
        }

        [Test]
        public void GetResultIsCalledSynchronouslyIfIsCompleteIsFalse()
        {
            var wasCalled = false;

            RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: true,
                onCompleted: continuation => Assert.Fail("OnCompleted should not be called when IsCompleted is true."),
                getResult: () => { wasCalled = true; return 42; })
            ).AssertPassed();

            Assert.That(wasCalled);
        }

        [Test]
        public void GetResultIsCalledSynchronouslyWhenContinued()
        {
            var wasCalled = false;

            RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: false,
                onCompleted: continuation => continuation.Invoke(),
                getResult: () => { wasCalled = true; return 42; })
            ).AssertPassed();

            Assert.That(wasCalled);
        }

        [Test]
        public void GetResultIsNotCalledUntilContinued()
        {
            var wasCalled = false;
            var continuation = (Action)null;
            var result = (ITestResult)null;

            ThreadPool.QueueUserWorkItem(state =>
            {
                result = RunCurrentTestMethod(new AsyncWorkload(
                    isCompleted: false,
                    onCompleted: action => continuation = action,
                    getResult: () => { wasCalled = true; return 42; })
                );
            });

            SpinWait.SpinUntil(() => continuation != null);

            Assert.That(wasCalled, Is.False, "GetResult was called before the continuation passed to OnCompleted was invoked.");

            continuation.Invoke();

            if (!SpinWait.SpinUntil(() => wasCalled, 1000))
            {
                Assert.Fail("GetResult was not called after the continuation passed to OnCompleted was invoked.");
            }

            SpinWait.SpinUntil(() => result != null);
            result.AssertPassed();
        }

        [Test]
        public void ExceptionThrownBeforeReturningAwaitableShouldBeHandled()
        {
            var getAwaiterWasCalled = false;
            var isCompletedWasCalled = false;
            var onCompletedWasCalled = false;
            var getResultWasCalled = false;

            var result = RunCurrentTestMethod(new AsyncWorkload(
                beforeReturningAwaitable: () => { throw new OddlyNamedException("Failure message"); },
                beforeReturningAwaiter: () => getAwaiterWasCalled = true,
                isCompleted: () => isCompletedWasCalled = true,
                onCompleted: continuation => onCompletedWasCalled = true,
                getResult: () => getResultWasCalled = true));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OddlyNamedException"));
            Assert.That(result.Message, Contains.Substring("Failure message"));

            Assert.That(getAwaiterWasCalled, Is.False);
            Assert.That(isCompletedWasCalled, Is.False);
            Assert.That(onCompletedWasCalled, Is.False);
            Assert.That(getResultWasCalled, Is.False);
        }

        [Test]
        public void ExceptionThrownInGetAwaiterShouldBeHandled()
        {
            var isCompletedWasCalled = false;
            var onCompletedWasCalled = false;
            var getResultWasCalled = false;

            var result = RunCurrentTestMethod(new AsyncWorkload(
                beforeReturningAwaitable: null,
                beforeReturningAwaiter: () => { throw new OddlyNamedException("Failure message"); },
                isCompleted: () => isCompletedWasCalled = true,
                onCompleted: continuation => onCompletedWasCalled = true,
                getResult: () => getResultWasCalled = true));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OddlyNamedException"));
            Assert.That(result.Message, Contains.Substring("Failure message"));

            Assert.That(isCompletedWasCalled, Is.False);
            Assert.That(onCompletedWasCalled, Is.False);
            Assert.That(getResultWasCalled, Is.False);
        }

        [Test]
        public void ExceptionThrownInIsCompletedShouldBeHandled()
        {
            var onCompletedWasCalled = false;
            var getResultWasCalled = false;

            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: () => { throw new OddlyNamedException("Failure message"); },
                onCompleted: continuation => onCompletedWasCalled = true,
                getResult: () => getResultWasCalled = true));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OddlyNamedException"));
            Assert.That(result.Message, Contains.Substring("Failure message"));

            Assert.That(onCompletedWasCalled, Is.False);
            Assert.That(getResultWasCalled, Is.False);
        }

        [Test]
        public void ExceptionThrownInOnCompletedShouldBeHandled()
        {
            var getResultWasCalled = false;

            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: false,
                onCompleted: continuation => { throw new OddlyNamedException("Failure message"); },
                getResult: () => getResultWasCalled = true));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OddlyNamedException"));
            Assert.That(result.Message, Contains.Substring("Failure message"));

            Assert.That(getResultWasCalled, Is.False);
        }

        [Test]
        public void ExceptionThrownInGetResultShouldBeHandled()
        {
            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: false,
                onCompleted: continuation => continuation.Invoke(),
                getResult: () => { throw new OddlyNamedException("Failure message"); }));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OddlyNamedException"));
            Assert.That(result.Message, Contains.Substring("Failure message"));
        }

        private sealed class OddlyNamedException : Exception
        {
            public OddlyNamedException(string message) : base(message)
            {
            }
        }

        [Test]
        public void OperationCanceledExceptionThrownInGetResultShouldBeReportedAsSuch()
        {
            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: false,
                onCompleted: continuation => continuation.Invoke(),
                getResult: () => { throw new OperationCanceledException(); }));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("OperationCanceledException"));
        }

#if TASK_PARALLEL_LIBRARY_API
        [Test]
        public void TaskCanceledExceptionThrownInGetResultShouldBeReportedAsSuch()
        {
            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: false,
                onCompleted: continuation => continuation.Invoke(),
                getResult: () => { throw new System.Threading.Tasks.TaskCanceledException(); }));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("TaskCanceledException"));
        }
#endif
    }
}
