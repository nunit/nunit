// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public abstract class HookOutcomeTestsBase(Type testClassToCheckHookOutcome)
{
    private static readonly string OutcomeMatched = "Outcome Matched";
    private static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

    protected static System.Collections.Generic.IEnumerable<TestFixtureData> GetReasonsToFail()
    {
        return TestData.ExecutionHooks.FailingReasonExecutor.GetReasonsToFail();
    }

    [Test]
    public void CheckHookOutcomes()
    {
        int numberOfFailingReasons = Enum.GetNames(typeof(TestData.ExecutionHooks.FailingReason)).Length;
        var workItem = TestBuilder.CreateWorkItem(testClassToCheckHookOutcome, TestFilter.Explicit);
        int numberOfFixtures = workItem.Test.Tests.Count;
        Assert.That(numberOfFixtures, Is.EqualTo(numberOfFailingReasons));
        int numberOfTestsPerFixture = workItem.Test.Tests[0].Tests.Count;

        workItem.Execute();
        var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

        Assert.That(workItem.Result.TotalCount, Is.EqualTo(numberOfFixtures * numberOfTestsPerFixture));
        Assert.Multiple(() =>
        {
            int i = 0;
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(OutcomeMismatch));
                i++;
            }
            Assert.That(i, Is.EqualTo(numberOfFailingReasons));
        });
    }
}
