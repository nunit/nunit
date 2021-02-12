// ***********************************************************************
// Copyright (c) 2012-2018 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [NonParallelizable]
    public class TimeoutTests : ThreadingTests
    {
        private static bool _testRanToCompletion;
        private static Action _testAction;
        private static StubDebugger _debugger;

        [TearDown]
        public void ResetTestCompletionFlag()
        {
            _testRanToCompletion = false;
        }

        private const string FailureMessage = "The test has failed";
        private const string IgnoreMessage = "The test was ignored";
        private const string InconclusiveMessage = "The test was inconclusive";

        public class TestAction
        {
            public string Name { get; }
            public Action Action { get; }
            public Action<ITestResult> Assertion { get; }

            public TestAction(Action action, Action<ITestResult> assertion, string name)
            {
                Action = action;
                Assertion = assertion;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private static IEnumerable<TestAction> PossibleTestOutcomes
        {
            get
            {
                yield return new TestAction(Assert.Pass, AssertPassedResult, "Pass");
                yield return new TestAction(() => throw new Exception(), AssertExceptionResult, "Exception");
                yield return new TestAction(() => Assert.Fail(FailureMessage), AssertFailResult, "Fail");
                yield return new TestAction(() => Assert.Ignore(IgnoreMessage), AssertIgnoreResult, "Ignore");
                yield return new TestAction(() => Assert.Inconclusive(InconclusiveMessage), AssertInconclusiveResult, "Inconclusive");
                yield return new TestAction(MultipleFail, AssertFailResult, "Multiple > Fail");
                yield return new TestAction(AsynchronousMultipleFail, AssertFailResult, "Multiple > Async Fail");
            }
        }

        private static void MultipleFail()
        {
            Assert.Multiple(() => Assert.Fail(FailureMessage));
        }

        private static void AsynchronousMultipleFail()
        {
            Assert.Multiple(async () =>
            {
                await System.Threading.Tasks.Task.Yield();
                Assert.Fail(FailureMessage);
            });
        }

        private static void AssertPassedResult(ITestResult result)
        {
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        private static void AssertExceptionResult(ITestResult result)
        {
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.ResultState.Label, Is.EqualTo(ResultState.Error.Label));
        }

        private static void AssertFailResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Failed, FailureMessage);
        }

        private static void AssertIgnoreResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Skipped, IgnoreMessage);
        }

        private static void AssertInconclusiveResult(ITestResult result)
        {
            AssertOutcome(result, TestStatus.Inconclusive, InconclusiveMessage);
        }

        private static void AssertOutcome(ITestResult result, TestStatus status, string message)
        {
            Assert.That(result.ResultState.Status, Is.EqualTo(status));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Is.EqualTo(message));
        }

        private class SampleTests
        {
            private const int TimeExceedingTimeout = 500;

            public const int Timeout = 50;

            [Timeout(Timeout)]
            public void TestThatTimesOut()
            {
                Thread.Sleep(TimeExceedingTimeout);
                _testRanToCompletion = true;
            }

            [Timeout(Timeout)]
            public void TestThatTimesOutAndInvokesAction()
            {
                Thread.Sleep(TimeExceedingTimeout);
                _testRanToCompletion = true;
                _testAction.Invoke();
            }

            [Timeout(Timeout)]
            public void TestThatInvokesActionImmediately()
            {
                _testAction.Invoke();
                _testRanToCompletion = true;
            }

            [Timeout(Timeout)]
            public void TestThatAttachesDebuggerAndTimesOut()
            {
                _debugger.IsAttached = true;
                Thread.Sleep(TimeExceedingTimeout);
                _testRanToCompletion = true;
            }
        }

        [Test, Timeout(500), SetCulture("fr-BE"), SetUICulture("es-BO")]
        public void TestWithTimeoutRespectsCulture()
        {
            Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-BE"));
            Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("es-BO"));
        }

        [Test, Timeout(500)]
        public void TestWithTimeoutCurrentContextIsNotAnAdhocContext()
        {
            Assert.That(TestExecutionContext.CurrentContext, Is.Not.TypeOf<TestExecutionContext.AdhocContext>());
        }

        [Test]
        public void TestThatCompletesWithinTimeoutPeriodHasItsOriginalResultPropagated(
            [ValueSource(nameof(PossibleTestOutcomes))] TestAction test,
            [Values] bool isDebuggerAttached)
        {
            // given
            _testAction = test.Action;

            var testThatCompletesWithoutTimeout =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatInvokesActionImmediately));

            var debugger = new StubDebugger { IsAttached = isDebuggerAttached };

            // when
            var result = TestBuilder.RunTest(testThatCompletesWithoutTimeout, new SampleTests(), debugger);

            // then
            test.Assertion.Invoke(result);
        }

        [Test]
        [TestCaseSource(nameof(PossibleTestOutcomes))]
        public void TestThatTimesOutIsRanToCompletionAndItsResultIsPropagatedWhenDebuggerIsAttached(TestAction test)
        {
            // given
            _testAction = test.Action;

            var testThatTimesOut =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatTimesOutAndInvokesAction));

            var attachedDebugger = new StubDebugger { IsAttached = true };

            // when
            var result = TestBuilder.RunTest(testThatTimesOut, new SampleTests(), attachedDebugger);

            // then
            Assert.That(_testRanToCompletion, () => "Test did not run to completion");

            test.Assertion.Invoke(result);
        }

        [Test]
        public void TestThatTimesOutIsRanToCompletionWhenDebuggerIsAttachedBeforeTimeOut()
        {
            // give
            var testThatAttachesDebuggerAndTimesOut =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatAttachesDebuggerAndTimesOut));

            _debugger = new StubDebugger { IsAttached = false };

            // when
            var result = TestBuilder.RunTest(testThatAttachesDebuggerAndTimesOut, new SampleTests(), _debugger);

            // then
            Assert.That(_testRanToCompletion, () => "Test did not run to completion");
        }

#if THREAD_ABORT
        [Test, Timeout(500)]
        public void TestWithTimeoutRunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test, Timeout(500)]
        public void TestWithTimeoutRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        [Test]
        public void TestTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("InfiniteLoopWith50msTimeout", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test]
        public void SetUpTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("Test1", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test]
        public void TearDownTimesOutAndNoFurtherTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("Test1", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "Base TearDown should not have been run but was");
        }

        [Test]
        public void TimeoutCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutFixtureWithTimeoutOnFixture));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
            Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            ITestResult result = TestFinder.Find("Test2WithInfiniteLoop", suiteResult, false);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TimeoutCausesOtherwisePassingTestToFailWithoutDebuggerAttached()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatTimesOut));

            var detachedDebugger = new StubDebugger { IsAttached = false };

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, new SampleTests(), detachedDebugger);

            // then
            Assert.That(_testRanToCompletion == false, () => "Test ran to completion");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Is.EqualTo($"Test exceeded Timeout value of {SampleTests.Timeout}ms"));
        }

        [Explicit("Tests that demonstrate Timeout failure")]
        public class ExplicitTests
        {
            [Test, Timeout(50)]
            public void TestTimesOut()
            {
                while (true) ;
            }

            [Test, Timeout(50), RequiresThread]
            public void TestTimesOutUsingRequiresThread()
            {
                while (true) ;
            }

            [Test, Timeout(50), Apartment(ApartmentState.STA)]
            public void TestTimesOutInSTA()
            {
                while (true) ;
            }

            // TODO: The test in TimeoutTestCaseFixture work as expected when run
            // directly by NUnit. It's only when run via TestBuilder as a second
            // level test that the result is incorrect. We need to fix this.
            [Test]
            public void TestTimeOutTestCaseWithOutElapsed()
            {
                TimeoutTestCaseFixture fixture = new TimeoutTestCaseFixture();
                TestSuite suite = TestBuilder.MakeFixture(fixture);
                ParameterizedMethodSuite methodSuite = (ParameterizedMethodSuite)TestFinder.Find("TestTimeOutTestCase", suite, false);
                ITestResult result = TestBuilder.RunTest(methodSuite, fixture);
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure), "Suite result");
                Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.Success), "First test");
                Assert.That(result.Children.ToArray()[1].ResultState, Is.EqualTo(ResultState.Failure), "Second test");
            }
        }

        [Test, Platform("Win")]
        public void TimeoutWithMessagePumpShouldAbort()
        {
            ITestResult result = TestBuilder.RunTest(
                TestBuilder.MakeTestFromMethod(typeof(TimeoutFixture), nameof(TimeoutFixture.TimeoutWithMessagePumpShouldAbort)),
                new TimeoutFixture());

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Test exceeded Timeout value of 500ms"));
        }
#endif

#if !THREAD_ABORT
        [Test]
        public void TimeoutCausesOtherwisePassingTestToFailWithoutDebuggerAttached()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatTimesOut));

            var detachedDebugger = new StubDebugger { IsAttached = false };

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, new SampleTests(), detachedDebugger);

            // then
            Assert.That(_testRanToCompletion == false, () => "Test ran to completion");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.ResultState.Label, Is.EqualTo($"Test exceeded Timeout value {SampleTests.Timeout}ms."));
        }
#endif

        private class StubDebugger : IDebugger
        {
            public bool IsAttached { get; set; }
        }
    }
}
