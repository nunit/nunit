// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
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
    public sealed class CancelAfterTests : ThreadingTests
    {
        private sealed class SampleTests
        {
            private const int TimeExceedingCancelAfter = 500;

            public const int CancelAfter = 50;
            private readonly Action _testAction;
            private readonly StubDebugger _debugger;

            public SampleTests(Action testAction, StubDebugger debugger)
            {
                _testAction = testAction;
                _debugger = debugger;
            }

            public bool TestRanToCompletion { get; private set; }

            [CancelAfter(CancelAfter)]
            public async Task TestThatTimesOut(CancellationToken cancellationToken)
            {
                await Task.Delay(TimeExceedingCancelAfter, cancellationToken);
                TestRanToCompletion = true;
            }

            [CancelAfter(CancelAfter)]
            public async Task TestThatTimesOutAndInvokesAction(CancellationToken cancellationToken)
            {
                await Task.Delay(TimeExceedingCancelAfter, cancellationToken);
                TestRanToCompletion = true;
                _testAction.Invoke();
            }

            [CancelAfter(CancelAfter)]
            public void TestThatInvokesActionImmediately()
            {
                _testAction.Invoke();
                TestRanToCompletion = true;
            }

            [CancelAfter(CancelAfter)]
            public async Task TestThatAttachesDebuggerAndTimesOut(CancellationToken cancellationToken)
            {
                _debugger.IsAttached = true;
                await Task.Delay(TimeExceedingCancelAfter, cancellationToken);
                TestRanToCompletion = true;
            }
        }

        [Test, CancelAfter(500), SetCulture("fr-BE"), SetUICulture("es-BO")]
        public void TestWithCancelAfterRespectsCulture()
        {
            Assert.Multiple(() =>
            {
                Assert.That(CultureInfo.CurrentCulture.Name, Is.EqualTo("fr-BE"));
                Assert.That(CultureInfo.CurrentUICulture.Name, Is.EqualTo("es-BO"));
            });
        }

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterCurrentContextIsNotAnAdhocContext()
        {
            Assert.That(TestExecutionContext.CurrentContext, Is.Not.TypeOf<TestExecutionContext.AdhocContext>());
        }

        [Test]
        public void TestThatCompletesWithinCancelAfterPeriodHasItsOriginalResultPropagated(
            [ValueSource(typeof(TestAction), nameof(TestAction.PossibleTestOutcomes))] TestAction test,
            [Values] bool isDebuggerAttached)
        {
            // given
            var testThatCompletesWithoutCancelAfter =
                TestBuilder.MakeTestCase(typeof(SampleTests), nameof(SampleTests.TestThatInvokesActionImmediately));

            var debugger = new StubDebugger { IsAttached = isDebuggerAttached };
            var sampleTests = new SampleTests(test.Action, debugger);

            // when
            var result = TestBuilder.RunTest(testThatCompletesWithoutCancelAfter, sampleTests, debugger);

            // then
            test.Assertion.Invoke(result);
        }

        [Test]
        [TestCaseSource(typeof(TestAction), nameof(TestAction.PossibleTestOutcomes))]
        public void TestThatTimesOutRanToCompletionAndItsResultIsPropagatedWhenDebuggerIsAttached(TestAction test)
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
        public void TestThatTimesOutRanToCompletionWhenDebuggerIsAttachedBeforeTimeOut()
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

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterRunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        [Test]
        public void TestTimesOutAndTearDownIsRun()
        {
            var fixture = new CancelAfterFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixture.InfiniteLoopWith50msCancelAfter), suite, false);
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
            var fixture = new CancelAfterFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInSetUp.Test), suite, false);
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
            var fixture = new CancelAfterFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod? testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInTearDown.Test), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "Base TearDown should not have been run but was");
        }

        [Test]
        public void CancelAfterCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithCancelAfterOnFixture));
            Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            ITestResult? result = TestFinder.Find(nameof(CancelAfterFixtureWithCancelAfterOnFixture.Test2ExceedsTimeout), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void CancelAfterCausesOtherwisePassingTestToFailWithoutDebuggerAttached()
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
            Assert.That(result.Message, Is.EqualTo($"Test exceeded CancelAfter value of {SampleTests.CancelAfter}ms"));
        }

        private static readonly CancellationToken CancelledToken = new CancellationToken(true);
        private static readonly CancellationToken PendingToken = new CancellationToken(false);

        private static readonly object[] TokenTestCases = new object[]
        {
            new object[] { true, CancelledToken },
            new object[] { false, PendingToken },
        };

        [TestCaseSource(nameof(TokenTestCases))]
        public void WithExistingCancellationTokenArgument(bool cancelled, CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken.IsCancellationRequested, Is.EqualTo(cancelled));
        }

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterAttributeCurrentContextHasCancellationToken()
        {
            Assert.That(TestExecutionContext.CurrentContext.CancellationToken, Is.Not.EqualTo(CancellationToken.None));
        }

        [Test]
        public void TestWithoutCancelAfterAttributeCurrentContextHasNoneCancellationToken()
        {
            Assert.That(TestExecutionContext.CurrentContext.CancellationToken, Is.EqualTo(CancellationToken.None));
        }

#pragma warning disable NUnit1003 // The TestCaseAttribute provided too few arguments
#pragma warning disable NUnit1027 // The test method has parameters, but no arguments are supplied by attributes
#pragma warning disable NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterAttributeHasCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.Not.EqualTo(CancellationToken.None));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [Test]
        public void TestWithoutCancelAfterAttributeHasNoneCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.EqualTo(CancellationToken.None));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [TestCase(1)]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndTestCaseHasCancellationToken(int value, CancellationToken cancellationToken)
        {
            Assert.That(value, Is.EqualTo(1));
            Assert.That(cancellationToken, Is.Not.EqualTo(CancellationToken.None));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [TestCaseSource(nameof(Arguments))]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndTestCaseSourceHasCancellationToken(int value, CancellationToken cancellationToken)
        {
            Assert.That(value, Is.EqualTo(1));
            Assert.That(cancellationToken, Is.Not.EqualTo(CancellationToken.None));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        private static readonly int[] Arguments = { 1 };

#pragma warning restore NUnit1003 // The TestCaseAttribute provided too few arguments
#pragma warning restore NUnit1027 // The test method has parameters, but no arguments are supplied by attributes

        [TestCaseSource(nameof(CancellationTokens))]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndTestCaseSourceHasOwnCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.Not.EqualTo(CancellationToken.None).And.Not.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        private static IEnumerable<CancellationToken> CancellationTokens
        {
            get
            {
                yield return new CancellationTokenSource(10).Token;
            }
        }
#pragma warning restore NUnit1029
        private sealed class StubDebugger : IDebugger
        {
            public bool IsAttached { get; set; }
        }
    }
}
