// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class AfterTestHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterTheAfterTestHookCompletes)} test")]
        public class TestWithAfterTestHookOnMethod
        {
            [OneTimeSetUp]
            public void OneTimeSetUp() => TestLog.LogCurrentMethod();

            [SetUp]
            public void SetUp() => TestLog.LogCurrentMethod();

            [Test]
            [ActivateAfterTestHook]
            public void EmptyTest() => TestLog.LogCurrentMethod();

            [TearDown]
            public void TearDown() => TestLog.LogCurrentMethod();

            [OneTimeTearDown]
            public void OneTimeTearDown() => TestLog.LogCurrentMethod();
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithAfterTestHookOnMethod), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestWithAfterTestHookOnMethod.OneTimeSetUp),
                nameof(TestWithAfterTestHookOnMethod.SetUp),
                nameof(TestWithAfterTestHookOnMethod.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithAfterTestHookOnMethod.TearDown),
                nameof(TestWithAfterTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
