// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [NonParallelizable]
    public class TimeoutTests : ThreadingTests
    {
#if !NETFRAMEWORK
#pragma warning disable CS0618 // Type or member is obsolete
#endif

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
#endif

        [Test]
        public void TestTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(TimeoutFixture.VeryLongTestWith50msTimeout), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));

            Thread.Sleep(1000);
            Assert.That(result.Message, Does.Contain("50ms"), "After 1s");

            Thread.Sleep(2000);
            Assert.That(result.Message, Does.Contain("50ms"), "After another 2s");

            // Only if we can abort the Test, we can ensure the Teardown is run immediately,
            // Otherwise it will be run eventually ... or not at all if the test is really hanging
#if THREAD_ABORT
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
#endif
        }

        [Test]
        public void OutputIsCapturedOnTimedoutTest()
        {
            var suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutWithSetupAndOutputFixture));
            var testMethod = suiteResult.Children.First();

            Assert.That(testMethod.Output, Does.Contain("setup"));
            Assert.That(testMethod.Output, Does.Contain("method output"));
        }

        [Test]
        public void OutputIsCapturedOnNonTimedoutTest()
        {
            var suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutWithSetupTestAndTeardownOutputFixture));
            var testMethod = suiteResult.Children.First();

            Assert.That(testMethod.Output, Does.Contain("setup"));
            Assert.That(testMethod.Output, Does.Contain("method output"));
            Assert.That(testMethod.Output, Does.Contain("teardown"));
        }

        [Test]
        public void OutputIsCapturedOnTimedoutTestAfterTimeout()
        {
            var suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutWithSetupAndOutputAfterTimeoutFixture));
            var testMethod = suiteResult.Children.First();

            Assert.That(testMethod.Output, Does.Contain("setup"));
            Assert.That(testMethod.Output, Does.Contain("method output before pause"));
            Assert.That(testMethod.Output, Does.Not.Contain("method output after pause"));
        }

        [Test]
        public void SetUpTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(TimeoutFixtureWithTimeoutInSetUp.Test1), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));

            // Only if we can abort the Test, we can ensure the Teardown is run immediately,
            // Otherwise it will be run eventually ... or not at all if the test is really hanging
#if THREAD_ABORT
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
#endif
        }

        [Test]
        public void TearDownTimesOutAndNoFurtherTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(TimeoutFixtureWithTimeoutInTearDown.Test1), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));

            // Only if we can abort the Test, we can ensure the Teardown is run immediately,
            // Otherwise it will be run eventually ... or not at all if the test is really hanging
#if THREAD_ABORT
            Assert.That(fixture.TearDownWasRun, "Base TearDown should have been run but was not");
#endif
        }

        [Test]
        public void TimeoutCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutFixtureWithTimeoutOnFixture));
            Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            ITestResult? result = TestFinder.Find(nameof(TimeoutFixtureWithTimeoutOnFixture.Test2WithLongDuration), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
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
                ParameterizedMethodSuite? methodSuite = (ParameterizedMethodSuite?)TestFinder.Find(nameof(TimeoutTestCaseFixture.TestTimeOutTestCase), suite, false);
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

        private class StubDebugger : IDebugger
        {
            public bool IsAttached { get; set; }
        }

        [TestFixture]
        internal sealed class Issue4723
        {
            [Test]
#if NETFRAMEWORK
            [Timeout(2_000)] // Ok status will be Passed
#else
#pragma warning disable CS0618
            [Timeout(2_000)] // Ok status will be Passed
#pragma warning restore CS0618
#endif
            public async Task Test_Timeout()
            {
                await Task.Delay(1_000);
                Assert.Pass();
            }

            [Test]
            [CancelAfter(2_000)] // Ok status will be Passed
            public async Task Test_CancelAfter(CancellationToken ct)
            {
                await Task.Delay(1_000, ct);
                Assert.Pass();
            }

            [Test] // Ok status will be Passed
            public async Task Test()
            {
                await Task.Delay(1_000);
                Assert.Pass();
            }

            [TearDown]
            public void Cleanup()
            {
                Assert.That(TestContext.CurrentContext.Result.Outcome.Status, Is.EqualTo(TestStatus.Passed));
            }
        }
    }
}
