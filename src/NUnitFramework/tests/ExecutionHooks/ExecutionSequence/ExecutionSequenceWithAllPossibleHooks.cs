// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionSequenceWithAllPossibleHooks
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(ExecutionSequenceWithAllPossibleHooksFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.OneTimeSetUpBase),
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.OneTimeSetUp),

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.SetupBase),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.Setup),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeTestHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.TestPasses),
                HookIdentifiers.AfterTestHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.TearDown),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.TearDownBase),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.SetupBase),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeEverySetUpHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.Setup),
                HookIdentifiers.AfterEverySetUpHook,

                HookIdentifiers.BeforeTestHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.TestFails),
                HookIdentifiers.AfterTestHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixture.TearDown),
                HookIdentifiers.AfterEveryTearDownHook,

                HookIdentifiers.BeforeEveryTearDownHook,
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.TearDownBase),
                HookIdentifiers.AfterEveryTearDownHook,

                nameof(ExecutionSequenceWithAllPossibleHooksFixture.OneTimeTearDown),
                nameof(ExecutionSequenceWithAllPossibleHooksFixtureBase.OneTimeTearDownBase),
            ]));
        }
    }
}
