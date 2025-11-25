// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class OutcomeLoggerBaseAttribute : ExecutionHookAttribute
    {
        private TestContext.ResultAdapter? _beforeHookTestResult;
        private static readonly string OutcomeMatched = "Outcome Matched";
        private static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private static readonly string OutcomePropertyKey = "ExpectedOutcome";

        protected void BeforeHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.Result.Clone();
        }

        protected void AfterHook(HookData hookData)
        {
            if (_beforeHookTestResult is null)
            {
                return;
            }

            string outcomeMatchStatement;
            var hookedMethodTestResult =
                hookData.Context.Result.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.Exception);
            var expectedOutcome = hookData.Context.Test.Properties.Get(OutcomePropertyKey);
            if (expectedOutcome is not null && (ResultState)expectedOutcome == hookedMethodTestResult.Outcome)
            {
                outcomeMatchStatement = OutcomeMatched;
            }
            else
            {
                outcomeMatchStatement = OutcomeMismatch;
            }

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.Test.FullName} -> {hookData.Context.Result.Outcome}, but expected was {expectedOutcome}");
        }
    }

    public enum FailingReason
    {
        Assertion,
        MultiAssertion,
        Exception,
        IgnoreAssertion,
        IgnoreException,
        Inconclusive,
        Warning,
        NoFailing
    }

    public static class FailingReasonExecutor
    {
        private static readonly string OutcomePropertyKey = "ExpectedOutcome";

        public static IEnumerable<TestFixtureData> GetReasonsToFail()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
        }

        public static void ExecuteFailingReason(FailingReason failingReason)
        {
            switch (failingReason)
            {
                case FailingReason.Assertion:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Failure);
                    Assert.Fail("Hooked method: Assertion.Fail");
                    break;
                case FailingReason.MultiAssertion:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Failure);
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("Hooked method: 1st Assert.Fail of Assertion.Multiple");
                        Assert.Fail("Hooked method: 2nd Assert.Fail of Assertion.Multiple");
                    });
                    break;
                case FailingReason.Exception:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Error);
                    throw new Exception("Hooked method: throws exception");
                case FailingReason.NoFailing:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Success);
                    break;
                case FailingReason.IgnoreAssertion:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Ignored);
                    Assert.Ignore("Hooked method: Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Ignored);
                    throw new IgnoreException("Hooked method: throws IgnoreException");
                case FailingReason.Inconclusive:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Inconclusive);
                    Assert.Inconclusive("Hooked method: Assert.Inconclusive.");
                    break;
                case FailingReason.Warning:
                    TestExecutionContext.CurrentContext.CurrentTest.Properties.Set(OutcomePropertyKey, ResultState.Warning);
                    Assert.Warn("Hooked method: Assert.Warn");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
