#if NETFRAMEWORK

// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [NonParallelizable]
    public class TimeoutTests : ThreadingTests
    {
        private static readonly string TestDataAssemblyPath = AssemblyHelper.GetAssemblyPath(typeof(TimeoutFixture).Assembly);

        private static ITestResult RunTestData(string fullNamePattern)
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            var options = new Dictionary<string, object>
            {
                ["LOAD"] = new[] { "NUnit.TestData" },
                ["NumberOfTestWorkers"] = 0
            };

            ITest test = runner.Load(TestDataAssemblyPath, options);
            Assert.That(test, Is.Not.Null, "TestData assembly not loaded");

            return runner.Run(TestListener.NULL, new FullNameFilter(fullNamePattern, isRegex: true));
        }

        private abstract class BaseTestsClass
        {
            protected const int TimeExceedingTimeout = 500;

            public const int Timeout = 50;

            public bool TestRanToCompletion { get; protected set; }
        }

        private sealed class SampleTests : BaseTestsClass
        {
            private readonly Action _testAction;
            private readonly StubDebugger _debugger;

            public SampleTests(Action testAction, StubDebugger debugger)
            {
                _testAction = testAction;
                _debugger = debugger;
            }

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

        private abstract class TimeoutTestsBaseClass : BaseTestsClass
        {
            [Timeout(Timeout)]
            public abstract void TestThatTimesOut();
        }

        private sealed class InheritedTimeoutTestsClass : TimeoutTestsBaseClass
        {
            public override void TestThatTimesOut()
            {
                Thread.Sleep(TimeExceedingTimeout);
                TestRanToCompletion = true;
            }
        }

        private sealed class OverriddenTimeoutTestsClass : TimeoutTestsBaseClass
        {
            [Timeout(2 * TimeExceedingTimeout)]
            public override void TestThatTimesOut()
            {
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

        [Test]
        public void InheritedTimeoutCausesOtherwisePassingTestToFail()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(InheritedTimeoutTestsClass), nameof(InheritedTimeoutTestsClass.TestThatTimesOut));

            var debugger = new StubDebugger { IsAttached = false };
            var tests = new InheritedTimeoutTestsClass();

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, tests, debugger);

            Assert.Multiple(() =>
            {
                // then
                Assert.That(tests.TestRanToCompletion, Is.False, "Test ran to completion");

                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Is.EqualTo($"Test exceeded Timeout value of {InheritedTimeoutTestsClass.Timeout}ms"));
            });
        }

        [Test]
        public void OverriddenTimeoutCausesTestToPass()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(OverriddenTimeoutTestsClass), nameof(OverriddenTimeoutTestsClass.TestThatTimesOut));

            var debugger = new StubDebugger { IsAttached = false };
            var tests = new OverriddenTimeoutTestsClass();

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, tests, debugger);

            Assert.Multiple(() =>
            {
                // then
                Assert.That(tests.TestRanToCompletion, Is.True, "Test ran to completion");

                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            });
        }

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
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            });
            ITestResult? result = TestFinder.Find(nameof(TimeoutFixtureWithTimeoutOnFixture.Test2WithLongDuration), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Does.Contain("50ms"));
            });
        }

        [Test]
        public void TimeoutCanBeSetOnBaseTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutFixtureWithInheritedTimeoutOnFixture));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            });
            ITestResult? result = TestFinder.Find(nameof(TimeoutFixtureWithInheritedTimeoutOnFixture.Test2WithLongDuration), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Does.Contain("50ms"));
            });
        }

        [Test]
        public void TestTimesOutViaAssemblyRunner()
        {
            ITestResult result = RunTestData(@"NUnit\.TestData\.TimeoutHangFixture\.TestTimesOut$");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TestTimesOutUsingRequiresThreadViaAssemblyRunner()
        {
            ITestResult result = RunTestData(@"NUnit\.TestData\.TimeoutHangFixture\.TestTimesOutUsingRequiresThread$");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TestTimesOutInSTAViaAssemblyRunner()
        {
            ITestResult result = RunTestData(@"NUnit\.TestData\.TimeoutHangFixture\.TestTimesOutInSTA$");

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TestTimeOutTestCaseViaAssemblyRunner()
        {
            ITestResult passingResult = RunTestData(@"NUnit\.TestData\.TimeoutTestCaseFixture\.TestTimeOutTestCase\(10\)");
            ITestResult failingResult = RunTestData(@"NUnit\.TestData\.TimeoutTestCaseFixture\.TestTimeOutTestCase\(500\)");

            Assert.That(passingResult.ResultState, Is.EqualTo(ResultState.Success), "First test");
            Assert.That(failingResult.ResultState, Is.EqualTo(ResultState.Failure), "Second test");
            Assert.That(failingResult.Message, Does.Contain("100ms"));
        }

        [Test, Platform(PlatformNames.Win)]
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
            [Timeout(2_000)] // Ok status will be Passed
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

#endif
