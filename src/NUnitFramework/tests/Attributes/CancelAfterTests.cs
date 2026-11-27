// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Api;
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
        private abstract class BaseTestsClass
        {
            protected const int TimeExceedingCancelAfter = 500;

            public const int CancelAfter = 50;

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

        private abstract class CancelableTestsBaseClass : BaseTestsClass
        {
            [CancelAfter(CancelAfter)]
            public abstract Task TestThatTimesOut(CancellationToken cancellationToken);
        }

        private sealed class InheritedCancelableTestsClass : CancelableTestsBaseClass
        {
            public override async Task TestThatTimesOut(CancellationToken cancellationToken)
            {
                await Task.Delay(TimeExceedingCancelAfter, cancellationToken);
                TestRanToCompletion = true;
            }
        }

        private sealed class OverriddenCancelableTestsClass : CancelableTestsBaseClass
        {
            [CancelAfter(2 * TimeExceedingCancelAfter)]
            public override async Task TestThatTimesOut(CancellationToken cancellationToken)
            {
                await Task.Delay(TimeExceedingCancelAfter, cancellationToken);
                TestRanToCompletion = true;
            }
        }

        private sealed class CallbackRaceTests : BaseTestsClass
        {
            private readonly BlockingDebugger _debugger;

            public CallbackRaceTests(BlockingDebugger debugger)
            {
                _debugger = debugger;
            }

            [CancelAfter(CancelAfter)]
            public void TestThatEndsWhileCancelAfterCallbackIsRunning()
            {
                // End the test only once the CancelAfter timer has fired and its callback is running.
                _debugger.CallbackStarted.Wait(TimeSpan.FromSeconds(10));
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

        [Test]
        public void TestThatEndsWhileCancelAfterCallbackIsRunningWaitsForTheCallbackToFinish()
        {
            // given
            var testThatEndsWhileCallbackIsRunning =
                TestBuilder.MakeTestCase(typeof(CallbackRaceTests), nameof(CallbackRaceTests.TestThatEndsWhileCancelAfterCallbackIsRunning));

            var debugger = new BlockingDebugger();
            var callbackRaceTests = new CallbackRaceTests(debugger);

            // when
            var run = Task.Run(() => TestBuilder.RunTest(testThatEndsWhileCallbackIsRunning, callbackRaceTests, debugger));

            // then
            try
            {
                Assert.That(debugger.CallbackStarted.Wait(TimeSpan.FromSeconds(10)), Is.True, "CancelAfter callback did not start");
                Assert.That(run.Wait(TimeSpan.FromMilliseconds(500)), Is.False, "Test completed while the CancelAfter callback was still running");
            }
            finally
            {
                debugger.ReleaseCallback();
            }

            Assert.That(run.Wait(TimeSpan.FromSeconds(10)), Is.True, "Test did not complete after the CancelAfter callback finished");
            Assert.That(callbackRaceTests.TestRanToCompletion, "Test did not run to completion");
            Assert.That(run.Result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void InheritedCancelAfterCausesOtherwisePassingTestToFail()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(InheritedCancelableTestsClass), nameof(InheritedCancelableTestsClass.TestThatTimesOut));

            var debugger = new StubDebugger { IsAttached = false };
            var tests = new InheritedCancelableTestsClass();

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, tests, debugger);

            Assert.Multiple(() =>
            {
                // then
                Assert.That(tests.TestRanToCompletion, Is.False, "Test ran to completion");

                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Is.EqualTo($"Test exceeded CancelAfter value of {InheritedCancelableTestsClass.CancelAfter}ms"));
            });
        }

        [Test]
        public void OverriddenCancelAfterCausesTestToPass()
        {
            // given
            var testThatTimesOutButOtherwisePasses =
                TestBuilder.MakeTestCase(typeof(OverriddenCancelableTestsClass), nameof(OverriddenCancelableTestsClass.TestThatTimesOut));

            var debugger = new StubDebugger { IsAttached = false };
            var tests = new OverriddenCancelableTestsClass();

            // when
            var result = TestBuilder.RunTest(testThatTimesOutButOtherwisePasses, tests, debugger);

            Assert.Multiple(() =>
            {
                // then
                Assert.That(tests.TestRanToCompletion, Is.True, "Test ran to completion");

                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            });
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
            var suite = TestBuilder.MakeFixture(fixture);
            var testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixture.InfiniteLoopWith50msCancelAfter), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            var result = TestBuilder.RunTest(testMethod, fixture);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Does.Contain("50ms"));
                Assert.That(fixture.TearDownWasRun, "TearDown was not run");
            });
        }

        [Test]
        public void SetUpTimesOutAndTearDownIsRun()
        {
            var fixture = new CancelAfterFixtureWithTimeoutInSetUp();
            var suite = TestBuilder.MakeFixture(fixture);
            var testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInSetUp.Test), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            var result = TestBuilder.RunTest(testMethod, fixture);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(result.Message, Does.Contain("50ms"));
                Assert.That(fixture.TearDownWasRun, "TearDown was not run");
            });
        }

        [Test]
        public void TearDownTimesOutAndNoFurtherTearDownIsRun()
        {
            var fixture = new CancelAfterFixtureWithTimeoutInTearDown();
            var suite = TestBuilder.MakeFixture(fixture);
            var testMethod = (TestMethod?)TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInTearDown.Test), suite, false);
            Assert.That(testMethod, Is.Not.Null);
            var result = TestBuilder.RunTest(testMethod, fixture);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.TearDown));
                Assert.That(result.Message, Does.Contain("50ms"));
                Assert.That(fixture.TearDownWasRun, "Base TearDown should not have been run but was");
            });
        }

        [Test]
        public void OneTimeSetUpTimesOutAndChildTestsAreNotRun()
        {
            var fixture = new CancelAfterFixtureWithTimeoutInOneTimeSetUp();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
                Assert.That(fixture.TestWasRun, Is.False, "Test was run");
                Assert.That(fixture.OneTimeTearDownWasRun, "OneTimeTearDown was not run");
                Assert.That(fixture.OneTimeTearDownTokenWasCancelled, Is.False, "OneTimeTearDown token was cancelled");
            });

            ITestResult? result = TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInOneTimeSetUp.Test), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Parent));
                Assert.That(result.Message, Does.Contain("50ms"));
            });
        }

        [Test]
        public void OneTimeTearDownTimesOut()
        {
            var fixture = new CancelAfterFixtureWithTimeoutInOneTimeTearDown();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.TearDown));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeTearDown exceeded CancelAfter value of 50ms"));
                Assert.That(fixture.TestWasRun, "Test was not run");
            });

            ITestResult? result = TestFinder.Find(nameof(CancelAfterFixtureWithTimeoutInOneTimeTearDown.Test), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void OneTimeSetUpAndTearDownWithinCancelAfterHaveTheirOwnCancellationToken()
        {
            var fixture = new CancelAfterFixtureWithOneTimeSetUpAndTearDownWithinCancelAfter();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(fixture.OneTimeSetUpToken.CanBeCanceled, "OneTimeSetUp token can't be cancelled");
                Assert.That(fixture.OneTimeSetUpToken.IsCancellationRequested, Is.False, "OneTimeSetUp token was cancelled");
                Assert.That(fixture.OneTimeTearDownToken.CanBeCanceled, "OneTimeTearDown token can't be cancelled");
                Assert.That(fixture.OneTimeTearDownToken.IsCancellationRequested, Is.False, "OneTimeTearDown token was cancelled");
                Assert.That(fixture.OneTimeSetUpToken, Is.Not.EqualTo(fixture.TestToken));
                Assert.That(fixture.OneTimeSetUpToken, Is.Not.EqualTo(fixture.OneTimeTearDownToken));
                Assert.That(fixture.OneTimeTearDownToken, Is.Not.EqualTo(fixture.TestToken));
            });
        }

        [Test]
        public void AllOneTimeSetUpMethodsInHierarchyShareOneCancellationToken()
        {
            var fixture = new CancelAfterDerivedFixtureWithOneTimeSetUp();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(fixture.BaseOneTimeSetUpToken.CanBeCanceled, "OneTimeSetUp token can't be cancelled");
                Assert.That(fixture.DerivedOneTimeSetUpToken, Is.EqualTo(fixture.BaseOneTimeSetUpToken));
            });
        }

        [Test]
        public void OneTimeSetUpIsNotCancelledWhenDebuggerIsAttached()
        {
            var fixture = new CancelAfterFixtureWithTimeoutInOneTimeSetUp();
            var attachedDebugger = new StubDebugger { IsAttached = true };

            ITestResult suiteResult = TestBuilder.RunTest(TestBuilder.MakeFixture(fixture), fixture, attachedDebugger);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(fixture.TestWasRun, "Test was not run");
            });
        }

        [Test]
        public void InheritedCancelAfterAppliesToOneTimeSetUp()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithInheritedCancelAfterAndTimeoutInOneTimeSetUp));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
            });
        }

        [Test]
        public void CancelAfterOnSetUpFixtureAppliesToOneTimeSetUp()
        {
            ITestResult runResult = RunNamespaceOf(typeof(NUnit.TestData.CancelAfterSetUpFixture.SetUpFixtureWithTimeoutInOneTimeSetUp));

            ITestResult? result = TestFinder.Find(nameof(NUnit.TestData.CancelAfterSetUpFixture.FixtureInSetUpFixtureNamespace.Test), runResult, true);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Parent));
                Assert.That(result.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
            });
        }

        [TestCase(typeof(NUnit.TestData.CancelAfterNestedSetUpFixture.FixtureWithOwnCancelAfter), 50)]
        [TestCase(typeof(NUnit.TestData.CancelAfterNestedSetUpFixture.FixtureWithEnclosingCancelAfter), 500)]
        public void OneTimeSetUpUsesTheClosestCancelAfter(Type fixtureType, int expectedTimeout)
        {
            ITestResult runResult = RunNamespaceOf(fixtureType);

            ITestResult? result = TestFinder.Find(fixtureType.Name, runResult, true);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(result.Message, Does.Contain($"OneTimeSetUp exceeded CancelAfter value of {expectedTimeout}ms"));
            });
        }

        [Test]
        public void NestedFixtureUsesItsOwnCancelAfterForOneTimeSetUp()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterOuterClassWithLongerCancelAfter.NestedFixtureWithOwnCancelAfter));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));

                // The outer class has CancelAfter(5000)
                Assert.That(suiteResult.Duration, Is.LessThan(2.5), "OneTimeSetUp was not cancelled after 50ms");
            });
        }

        [Test]
        public void NestedFixtureIgnoresCancelAfterOfOuterClass()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterOuterClassWithCancelAfter.NestedFixtureWithoutOwnCancelAfter));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success), suiteResult.Message);
        }

        [Test]
        public void FixtureConstructorTimesOut()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithTimeoutInConstructor));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
            });
        }

        [Test]
        public void AfterSuiteActionTimesOut()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithTimeoutInAfterSuiteAction));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeTearDown exceeded CancelAfter value of 50ms"));
            });
        }

        [Test]
        public void CancelAfterOnSetUpMethodCancelsSetUp()
        {
            var fixture = new CancelAfterOnSetUpMethodFixture();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            ITestResult? result = TestFinder.Find(nameof(CancelAfterOnSetUpMethodFixture.Test), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(result.Message, Does.Contain("SetUp exceeded CancelAfter value of 50ms").And.Not.Contain("OneTimeSetUp"));
                Assert.That(fixture.TestWasRun, Is.False, "Test was run");
                Assert.That(fixture.TearDownWasRun, "TearDown was not run");
            });
        }

        [Test]
        public void CancelAfterOnTearDownMethodCancelsTearDown()
        {
            var fixture = new CancelAfterOnTearDownMethodFixture();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            ITestResult? result = TestFinder.Find(nameof(CancelAfterOnTearDownMethodFixture.Test), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.TearDown));
                Assert.That(result.Message, Does.Contain("TearDown exceeded CancelAfter value of 50ms").And.Not.Contain("OneTimeTearDown"));
                Assert.That(fixture.TestWasRun, "Test was not run");
            });
        }

        [Test]
        public void CancelAfterOnOneTimeSetUpMethodCancelsOneTimeSetUp()
        {
            var fixture = new CancelAfterOnOneTimeSetUpMethodFixture();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
                Assert.That(fixture.TestWasRun, Is.False, "Test was run");
                Assert.That(fixture.OneTimeTearDownWasRun, "OneTimeTearDown was not run");
                Assert.That(fixture.OneTimeTearDownTokenWasCancelled, Is.False, "OneTimeTearDown token was cancelled");
            });
        }

        [Test]
        public void CancelAfterOnOneTimeTearDownMethodCancelsOneTimeTearDown()
        {
            var fixture = new CancelAfterOnOneTimeTearDownMethodFixture();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.TearDown));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeTearDown exceeded CancelAfter value of 50ms"));
                Assert.That(fixture.TestWasRun, "Test was not run");
            });
        }

        [Test]
        public void SetUpMethodsWithCancelAfterHaveTheirOwnCancellationToken()
        {
            var fixture = new CancelAfterOnSetUpMethodsWithinLimitFixture();
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(fixture.OneTimeSetUpParameterToken, Is.EqualTo(fixture.OneTimeSetUpContextToken));
                Assert.That(fixture.OneTimeSetUpContextToken.CanBeCanceled, "OneTimeSetUp token can't be cancelled");
                Assert.That(fixture.SetUpToken.CanBeCanceled, "SetUp token can't be cancelled");
                Assert.That(fixture.SetUpToken, Is.Not.EqualTo(fixture.OneTimeSetUpContextToken));
                Assert.That(fixture.TestToken, Is.Not.EqualTo(fixture.SetUpToken));
                Assert.That(fixture.TestToken, Is.Not.EqualTo(fixture.OneTimeSetUpContextToken));
            });
        }

        [TestCase(typeof(CancelAfterOnSetUpMethodDoesNotApplyToTestFixture))]
        [TestCase(typeof(CancelAfterOnBaseOneTimeSetUpMethodDoesNotApplyToDerivedFixture))]
        public void CancelAfterOnSetUpMethodOnlyAppliesToThatMethod(Type fixtureType)
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixtureType);
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success), suiteResult.Message);
        }

        [Test]
        public void CancelAfterOnFixtureAppliesAfterOneTimeSetUpMethodWithCancelAfter()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterOnFixtureStillAppliesAfterOneTimeSetUpMethodWithCancelAfter));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 100ms"));
            });
        }

        [TestCase(typeof(CancelAfterOnOneTimeSetUpMethodShorterThanOnFixture), 50)]
        [TestCase(typeof(CancelAfterOnOneTimeSetUpMethodLongerThanOnFixture), 50)]
        public void OneTimeSetUpMethodIsCancelledByTheShortestCancelAfter(Type fixtureType, int expectedTimeout)
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(fixtureType);
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.Message, Does.Contain($"OneTimeSetUp exceeded CancelAfter value of {expectedTimeout}ms"));
            });
        }

        private static ITestResult RunNamespaceOf(Type type)
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            var options = new Dictionary<string, object>
            {
                ["LOAD"] = new[] { type.Namespace! },
                ["NumberOfTestWorkers"] = 0,
            };
            Assert.That(runner.Load(AssemblyHelper.GetAssemblyPath(type.Assembly), options), Is.Not.Null);

            return runner.Run(TestListener.NULL, TestFilter.Empty);
        }

        [Test]
        public void CancelAfterCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithCancelAfterOnFixture));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            });
            ITestResult? result = TestFinder.Find(nameof(CancelAfterFixtureWithCancelAfterOnFixture.Test2ExceedsTimeout), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Does.Contain("50ms"));
            });
        }

        [Test]
        public void CancelAfterCanBeSetOnBaseTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(CancelAfterFixtureWithInheritedCancelAfterOnFixture));
            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
                Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            });
            ITestResult? result = TestFinder.Find(nameof(CancelAfterFixtureWithInheritedCancelAfterOnFixture.Test2ExceedsTimeout), suiteResult, false);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Does.Contain("50ms"));
            });
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

            Assert.Multiple(() =>
            {
                // then
                Assert.That(sampleTests.TestRanToCompletion, Is.False, "Test ran to completion");

                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.Test));
                Assert.That(result.Message, Is.EqualTo($"Test exceeded CancelAfter value of {SampleTests.CancelAfter}ms"));
            });
        }

        private static readonly CancellationToken CancelledToken = new(true);
        private static readonly CancellationToken PendingToken = new(false);

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

        private CancellationToken _cancellationTokenAtOneTimeSetUp;

        [OneTimeSetUp]
        public void SetUp()
        {
            _cancellationTokenAtOneTimeSetUp = TestExecutionContext.CurrentContext.CancellationToken;
        }

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterAttributeCurrentContextHasTestCancellationToken()
        {
            Assert.That(TestExecutionContext.CurrentContext.CancellationToken,
                        Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
        }

        [Test]
        public void TestWithoutCancelAfterAttributeCurrentContextHasGlobalCancellationToken()
        {
            Assert.That(TestExecutionContext.CurrentContext.CancellationToken, Is.EqualTo(_cancellationTokenAtOneTimeSetUp));
        }

        [Test, CancelAfter(500)]
        public void TestWithCancelAfterAttributeHasTestCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

#pragma warning disable NUnit1027 // The test method has parameters, but no arguments are supplied by attributes
        [Test]
#pragma warning restore NUnit1027 // The test method has parameters, but no arguments are supplied by attributes
        public void TestWithoutCancelAfterAttributeHasGlobalCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.EqualTo(_cancellationTokenAtOneTimeSetUp));
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [TestCase(1)]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndTestCaseHasTestCancellationToken(int value, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(value, Is.EqualTo(1));
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [TestCaseSource(nameof(Arguments))]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndTestCaseSourceHasTestCancellationToken(int value, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(value, Is.EqualTo(1));
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [Test]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndValueAttributeHasTestCancellationToken([Values] bool value, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(value, Is.True.Or.False);
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [Test]
        [CancelAfter(500)]
        [Sequential]
        public void TestWithCancelAfterAttributeAndMultipleValueAttributesSequentialHasTestCancellationToken([Values] bool a, [Values] bool b, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(a, Is.EqualTo(b));
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [Test]
        [CancelAfter(500)]
        [Pairwise]
        public void TestWithCancelAfterAttributeAndMultipleValueAttributesPairwiseHasTestCancellationToken([Values] bool a, [Values] bool b, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(a, Is.EqualTo(b).Or.Not.EqualTo(b));
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        private static readonly bool[] Parameters = { true, false };

        [Test]
        [CancelAfter(500)]
        public void TestWithCancelAfterAttributeAndValueSourceAttributeHasTestCancellationToken([ValueSource(nameof(Parameters))] bool value, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(value, Is.True.Or.False);
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        [Test]
        [CancelAfter(500)]
        [Pairwise]
        public void TestWithCancelAfterAttributeAndMultipleValueSourceAttributesPairwiseHasTestCancellationToken([ValueSource(nameof(Parameters))] bool a, [ValueSource(nameof(Parameters))] bool b, CancellationToken cancellationToken)
        {
            Assert.Multiple(() =>
            {
                Assert.That(a, Is.EqualTo(b).Or.Not.EqualTo(b));
                Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp).And.Not.EqualTo(CancellationToken.None));
            });
            Assert.That(cancellationToken, Is.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        private static readonly int[] Arguments = { 1 };

        [CancelAfter(500)]
#pragma warning disable NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method
        [TestCaseSource(nameof(CancellationTokens))]
#pragma warning restore NUnit1029 // The number of parameters provided by the TestCaseSource does not match the number of parameters in the Test method
        public void TestWithCancelAfterAttributeAndTestCaseSourceHasOwnCancellationToken(CancellationToken cancellationToken)
        {
            Assert.That(cancellationToken, Is.Not.EqualTo(_cancellationTokenAtOneTimeSetUp)
                                         .And.Not.EqualTo(CancellationToken.None)
                                         .And.Not.EqualTo(TestContext.CurrentContext.CancellationToken));
        }

        private static IEnumerable<CancellationToken> CancellationTokens
        {
            get
            {
                yield return new CancellationTokenSource(10).Token;
            }
        }

        private sealed class StubDebugger : IDebugger
        {
            public bool IsAttached { get; set; }
        }

        /// <summary>
        /// Holds the CancelAfter timer callback inside <see cref="IsAttached"/> until <see cref="ReleaseCallback"/> is called.
        /// </summary>
        private sealed class BlockingDebugger : IDebugger
        {
            private readonly TaskCompletionSource<bool> _callbackStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> _callbackReleased = new(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task CallbackStarted => _callbackStarted.Task;

            public bool IsAttached
            {
                get
                {
                    _callbackStarted.TrySetResult(true);
                    _callbackReleased.Task.Wait();

                    // Report a debugger as attached so the callback does not cancel the test:
                    // if the token sources were already disposed, cancelling here would crash the test run.
                    return true;
                }
            }

            public void ReleaseCallback() => _callbackReleased.TrySetResult(true);
        }
    }
}
