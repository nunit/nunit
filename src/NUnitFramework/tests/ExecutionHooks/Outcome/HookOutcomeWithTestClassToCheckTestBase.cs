// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public abstract class HookOutcomeWithTestClassToCheckTestBase(Type testClassToCheckHookOutcome) : HookOutcomeTestsBase()
{
    private static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

    protected void CheckHookOutcomes()
    {
        var numberOfFailingReasons = Enum.GetNames(typeof(FailingReason)).Length;
        var workItem = TestBuilder.CreateWorkItem(testClassToCheckHookOutcome, TestFilter.Explicit);
        var numberOfFixtures = workItem.Test.Tests.Count;
        Assert.That(numberOfFixtures, Is.EqualTo(numberOfFailingReasons));
        var numberOfTestsPerFixture = workItem.Test.Tests[0].Tests.Count;

        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(workItem.Result.TotalCount, Is.EqualTo(numberOfFixtures * numberOfTestsPerFixture));
        Assert.Multiple(() =>
        {
            var i = 0;
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(OutcomeMismatch));
                i++;
            }
            Assert.That(i, Is.EqualTo(numberOfFailingReasons));
        });
    }
}
