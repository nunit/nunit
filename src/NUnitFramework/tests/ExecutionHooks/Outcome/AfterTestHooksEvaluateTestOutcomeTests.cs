// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTestHooksEvaluateTestOutcomeTests
{
    public class AfterTestOutcomeLogger : ExecutionHookAttribute
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private TestResult? _beforeHookTestResult;

        public override void BeforeTestHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.CurrentResult.Clone();
        }

        public override void AfterTestHook(HookData hookData)
        {
            Assert.That(_beforeHookTestResult, Is.Not.Null, "BeforeTestHook was not called before AfterTestHook.");
            Assert.That(hookData.Context.CurrentTest.MethodName, Is.Not.Null, "Hook was not called on a method.");

            TestResult testResult
                    = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.ExceptionContext);

            string outcomeMatchStatement = testResult.ResultState switch
            {
                { Status: TestStatus.Failed } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("FailedTest") => OutcomeMatched,
                { Status: TestStatus.Passed } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("PassedTest") => OutcomeMatched,
                { Status: TestStatus.Skipped } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("TestIgnored") => OutcomeMatched,
                { Status: TestStatus.Warning } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("WarningTest") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.CurrentTest.MethodName} -> {hookData.Context.CurrentResult.ResultState}");
        }
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckThatAfterTestHooksEvaluateTestOutcome)} test")]
    [AfterTestOutcomeLogger]
    [TestFixture]
    public class TestsUnderTestsWithMixedOutcome
    {
        [Test]
        public void PassedTest()
        {
        }

        [Test]
        public void FailedTestByAssertion()
        {
            Assert.Fail();
        }

        [Test]
        public void FailedTestByException()
        {
            throw new System.Exception("some exception");
        }

        [TestCase(ExpectedResult = 1)]
        public int FailedTestByWrongExpectedResult() => 2;

        [Test]
        public void TestIgnoredByAssertIgnore()
        {
            Assert.Ignore();
        }

        [Test]
        public void TestIgnoredByException()
        {
            throw new IgnoreException("Ignore this test");
        }

        [Test]
        public void WarningTestWithWarnings()
        {
            Assert.Warn("Some warning.");
        }
    }

    [Test]
    public void CheckThatAfterTestHooksEvaluateTestOutcome()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithMixedOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var logLine in currentTestLogs)
            {
                Assert.That(logLine, Does.StartWith(AfterTestOutcomeLogger.OutcomeMatched));
            }
        });
    }
}
