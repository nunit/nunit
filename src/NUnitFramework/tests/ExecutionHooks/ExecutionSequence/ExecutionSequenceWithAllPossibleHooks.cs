// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionSequenceWithAllPossibleHooks
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestData.ExecutionHooks.TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.OneTimeSetUpBase),
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.OneTimeSetUp),

                TestData.ExecutionHooks.HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.SetupBase),
                TestData.ExecutionHooks.HookIdentifiers.AfterEverySetUpHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.Setup),
                TestData.ExecutionHooks.HookIdentifiers.AfterEverySetUpHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeTestHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.TestPasses),
                TestData.ExecutionHooks.HookIdentifiers.AfterTestHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.TearDown),
                TestData.ExecutionHooks.HookIdentifiers.AfterEveryTearDownHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.TearDownBase),
                TestData.ExecutionHooks.HookIdentifiers.AfterEveryTearDownHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.SetupBase),
                TestData.ExecutionHooks.HookIdentifiers.AfterEverySetUpHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEverySetUpHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.Setup),
                TestData.ExecutionHooks.HookIdentifiers.AfterEverySetUpHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeTestHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.TestFails),
                TestData.ExecutionHooks.HookIdentifiers.AfterTestHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.TearDown),
                TestData.ExecutionHooks.HookIdentifiers.AfterEveryTearDownHook,

                TestData.ExecutionHooks.HookIdentifiers.BeforeEveryTearDownHook,
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.TearDownBase),
                TestData.ExecutionHooks.HookIdentifiers.AfterEveryTearDownHook,

                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixture.OneTimeTearDown),
                nameof(TestData.ExecutionHooks.ExecutionSequenceWithAllPossibleHooksFixtureBase.OneTimeTearDownBase),
            ]));
        }
    }
}
