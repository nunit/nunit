// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class ExecutionProceedsOnlyAfterAllAfterTestHooksExecuteFixture
    {
        [Test]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHookThrowingException]
        public void TestPasses() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
