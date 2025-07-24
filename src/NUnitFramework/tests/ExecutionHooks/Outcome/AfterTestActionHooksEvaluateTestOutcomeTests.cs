// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTestActionHooksEvaluateTestOutcomeTests
{
    public class TestActionOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private TestResult? _beforeHookTestResult;
        private readonly BeforeOrAfterTest _beforeOrAfterTest;

        public TestActionOutcomeLogger(BeforeOrAfterTest beforeOrAfterTest)
        {
            _beforeOrAfterTest = beforeOrAfterTest;
        }

        public void ApplyToContext(TestExecutionContext context)
        {
            if (_beforeOrAfterTest == BeforeOrAfterTest.BeforeTest)
            {
                context.ExecutionHooks.AddBeforeTestActionBeforeTestHandler(BeforeTestActionHook);
                context.ExecutionHooks.AddAfterTestActionBeforeTestHandler(AfterTestActionHook);
            }
            else
            {
                context.ExecutionHooks.AddBeforeTestActionAfterTestHandler(BeforeTestActionHook);
                context.ExecutionHooks.AddAfterTestActionAfterTestHandler(AfterTestActionHook);
            }
        }

        public void BeforeTestActionHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.CurrentResult.Clone();
        }

        public void AfterTestActionHook(HookData hookData)
        {
            Assert.That(_beforeHookTestResult, Is.Not.Null, "BeforeTestAction was not called before AfterTestAction.");
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

    public enum FailingReason
    {
        Assertion4Failed,
        MultiAssertion4Failed,
        Exception4Failed,
        IgnoreAssertion4Ignored,
        IgnoreException4Ignored,
        Inconclusive4Inconclusive,
        Warning4Warning,
        None4Passed
    }

    /// <summary>
    /// Decides if we want to test the BeforeTest or AfterTest method of an <see cref="ITestAction"/>.
    /// </summary>
    public enum BeforeOrAfterTest
    {
        BeforeTest,
        AfterTest
    }

    internal class ActionAttributeWithInjectedFailuresAttribute : Attribute, ITestAction
    {
        private readonly FailingReason _failingReason;
        private readonly BeforeOrAfterTest _beforeOrAfterTest;

        public ActionAttributeWithInjectedFailuresAttribute(FailingReason failingReason, BeforeOrAfterTest beforeOrAfterTest)
        {
            _failingReason = failingReason;
            _beforeOrAfterTest = beforeOrAfterTest;
        }

        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            if (_beforeOrAfterTest == BeforeOrAfterTest.BeforeTest)
            {
                ExecuteFailingReason();
            }
        }

        public void AfterTest(ITest test)
        {
            if (_beforeOrAfterTest == BeforeOrAfterTest.AfterTest)
            {
                ExecuteFailingReason();
            }
        }

        private void ExecuteFailingReason()
        {
            switch (_failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("OneTimeSetUp fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st OneTimeSetUp fails by MultiAssertion_Failed.");
                        Assert.Fail("2nd OneTimeSetUp fails by MultiAssertion_Failed.");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("OneTimeSetUp throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("OneTimeSetUp ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("OneTimeSetUp ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("OneTimeSetUp is inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("OneTimeSetUp with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Explicit($"This test should only be run as part of the {nameof(AfterTestActionHooksEvaluateTestOutcomeTests)} test")]
    [TestActionOutcomeLogger(BeforeOrAfterTest.BeforeTest)]
    [TestFixture]
    public class TestsUnderTestsWithMixedOutcome_ForBeforeTest
    {
        [ActionAttributeWithInjectedFailures(FailingReason.None4Passed, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void PassedTest()
        {
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Assertion4Failed, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void FailedTestByAssertion()
        {
            Assert.Fail();
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Exception4Failed, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void FailedTestByException()
        {
            throw new System.Exception("some exception");
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Assertion4Failed, BeforeOrAfterTest.BeforeTest)]
        [TestCase(ExpectedResult = 1)]
        public int FailedTestByWrongExpectedResult() => 2;

        [ActionAttributeWithInjectedFailures(FailingReason.IgnoreAssertion4Ignored, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void TestIgnoredByAssertIgnore()
        {
            Assert.Ignore();
        }

        [ActionAttributeWithInjectedFailures(FailingReason.IgnoreException4Ignored, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void TestIgnoredByException()
        {
            throw new IgnoreException("Ignore this test");
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Warning4Warning, BeforeOrAfterTest.BeforeTest)]
        [Test]
        public void WarningTestWithWarnings()
        {
            Assert.Warn("Some warning.");
        }
    }

    [Explicit($"This test should only be run as part of the {nameof(AfterTestActionHooksEvaluateTestOutcomeTests)} test")]
    [TestActionOutcomeLogger(BeforeOrAfterTest.AfterTest)]
    [TestFixture]
    public class TestsUnderTestsWithMixedOutcome_ForAfterTest
    {
        [ActionAttributeWithInjectedFailures(FailingReason.None4Passed, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void PassedTest()
        {
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Assertion4Failed, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void FailedTestByAssertion()
        {
            Assert.Fail();
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Exception4Failed, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void FailedTestByException()
        {
            throw new System.Exception("some exception");
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Assertion4Failed, BeforeOrAfterTest.AfterTest)]
        [TestCase(ExpectedResult = 1)]
        public int FailedTestByWrongExpectedResult() => 2;

        [ActionAttributeWithInjectedFailures(FailingReason.IgnoreAssertion4Ignored, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void TestIgnoredByAssertIgnore()
        {
            Assert.Ignore();
        }

        [ActionAttributeWithInjectedFailures(FailingReason.IgnoreException4Ignored, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void TestIgnoredByException()
        {
            throw new IgnoreException("Ignore this test");
        }

        [ActionAttributeWithInjectedFailures(FailingReason.Warning4Warning, BeforeOrAfterTest.AfterTest)]
        [Test]
        public void WarningTestWithWarnings()
        {
            Assert.Warn("Some warning.");
        }
    }

    [Test]
    public void CheckThatAfterTestActionBeforeTestHooksEvaluateTestOutcome()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithMixedOutcome_ForBeforeTest), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (string logLine in currentTestLogs)
            {
                Assert.That(logLine, Does.StartWith(TestActionOutcomeLogger.OutcomeMatched));
            }
        });
    }

    [Test]
    public void CheckThatAfterTestActionAfterTestHooksEvaluateTestOutcome()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithMixedOutcome_ForAfterTest), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (string logLine in currentTestLogs)
            {
                Assert.That(logLine, Does.StartWith(TestActionOutcomeLogger.OutcomeMatched));
            }
        });
    }
}
