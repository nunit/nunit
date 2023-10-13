// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Tests.TestUtilities;
#if THREAD_ABORT
using System.Linq;
using NUnit.TestData;
#endif

namespace NUnit.Framework.Tests.Attributes
{
    [NonParallelizable]
    public class TimeoutTests : ThreadingTests
    {
        private sealed class SampleTests
        {
            private const int TimeExceedingTimeout = 500;

            public const int Timeout = 50;
            private readonly Action _testAction;
            private readonly StubDebugger _debugger;

            public SampleTests(Action testAction, StubDebugger debugger)
            {
                _testAction = testAction;
                _debugger = debugger;
            }

            public bool TestRanToCompletion { get; private set; }

            [Timeout(Timeout)]
            public void TestThatTimesOut()
            {
                Thread.Sleep(TimeExceedingTimeout);
                TestRanToCompletion = true;
            }

            [Timeout(Timeout)]
            public void TestThatTimesOutAndInvokesAction()
            {
                Thread.Sleep(TimeExceedingTimeout);
                TestRanToCompletion = true;
                _testAction.Invoke();
            }

            [Timeout(Timeout)]
            public void TestThatInvokesActionImmediately()
            {
                _testAction.Invoke();
                TestRanToCompletion = true;
            }

            [Timeout(Timeout)]
            public void TestThatAttachesDebuggerAndTimesOut()
            {
                _debugger.IsAttached = true;
                Thread.Sleep(TimeExceedingTimeout);
                TestRanToCompletion = true;
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
            [ValueSource(typeof(TestAction), nameof(TestAction.PossibleTestOutcomes))] TestAction test,
            [Values] bool isDebuggerAttached)
        {
            // given
            var testThatCompletesWithoutTimeout =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatInvokesActionImmediately));

            var debugger = new StubDebugger { IsAttached = isDebuggerAttached };
            var sampleTests = new SampleTests(test.Action, debugger);

            // when
            var result = TestBuilder.RunTest(testThatCompletesWithoutTimeout, sampleTests, debugger);

            // then
            test.Assertion.Invoke(result);
        }

        [Test]
        [TestCaseSource(typeof(TestAction), nameof(TestAction.PossibleTestOutcomes))]
        public void TestThatTimesOutIsRanToCompletionAndItsResultIsPropagatedWhenDebuggerIsAttached(TestAction test)
        {
            // given
            var testThatTimesOut =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatTimesOutAndInvokesAction));

            var attachedDebugger = new StubDebugger { IsAttached = true };
            var sampleTests = new SampleTests(test.Action, attachedDebugger);

            // when
            var result = TestBuilder.RunTest(testThatTimesOut, sampleTests, attachedDebugger);

            // then
            Assert.That(sampleTests.TestRanToCompletion, "Test did not run to completion");

            test.Assertion.Invoke(result);
        }

        [Test]
        public void TestThatTimesOutIsRanToCompletionWhenDebuggerIsAttachedBeforeTimeOut()
        {
            // give
            var testThatAttachesDebuggerAndTimesOut =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatAttachesDebuggerAndTimesOut));

            var debugger = new StubDebugger { IsAttached = false };
            var sampleTests = new SampleTests(() => { }, debugger);

            // when
            var result = TestBuilder.RunTest(testThatAttachesDebuggerAndTimesOut, sampleTests, debugger);

            // then
            Assert.That(sampleTests.TestRanToCompletion, "Test did not run to completion");
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
            TestMethod? testMethod = (TestMethod?)TestFinder.Find("InfiniteLoopWith50msTimeout", suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test]
        public void SetUpTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find("Test1", suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test]
        public void TearDownTimesOutAndNoFurtherTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find("Test1", suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "Base TearDown should not have been run but was");
        }

        [Test]
        public void TimeoutCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutFixtureWithTimeoutOnFixture));
            Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            ITestResult? result = TestFinder.Find("Test2WithInfiniteLoop", suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TimeoutCausesOtherwisePassingTestToFailWithoutDebuggerAttached()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatTimesOut));

            var detachedDebugger = new StubDebugger { IsAttached = false };

            var sampleTests = new SampleTests(() => { }, detachedDebugger);

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, sampleTests, detachedDebugger);

            // then
            Assert.That(sampleTests.TestRanToCompletion, Is.False, "Test ran to completion");

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
                while (true)
                    ;
            }

            [Test, Timeout(50), RequiresThread]
            public void TestTimesOutUsingRequiresThread()
            {
                while (true)
                    ;
            }

            [Test, Timeout(50), Apartment(ApartmentState.STA)]
            public void TestTimesOutInSTA()
            {
                while (true)
                    ;
            }

            // TODO: The test in TimeoutTestCaseFixture work as expected when run
            // directly by NUnit. It's only when run via TestBuilder as a second
            // level test that the result is incorrect. We need to fix this.
            [Test]
            public void TestTimeOutTestCaseWithOutElapsed()
            {
                TimeoutTestCaseFixture fixture = new TimeoutTestCaseFixture();
                TestSuite suite = TestBuilder.MakeFixture(fixture);
                ParameterizedMethodSuite? methodSuite = (ParameterizedMethodSuite?)TestFinder.Find("TestTimeOutTestCase", suite, false);
                Assert.That(methodSuite, Is.Not.Null);
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

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
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
            var sampleTests = new SampleTests(() => { }, detachedDebugger);

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, sampleTests, detachedDebugger);

            // then
            Assert.That(sampleTests.TestRanToCompletion, Is.False, () => "Test ran to completion");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Is.EqualTo($"Test exceeded Timeout value of {SampleTests.Timeout}ms"));
        }
#endif

        private class StubDebugger : IDebugger
        {
            public bool IsAttached { get; set; }
        }
    }
}
