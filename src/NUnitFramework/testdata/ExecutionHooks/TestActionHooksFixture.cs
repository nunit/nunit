// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    [LogTestAction]
    [TestActionLoggingExecutionHooks]
    public class TestActionHooksFixture
    {
        [Test]
        public void TestUnderTest() => TestLog.LogCurrentMethod();
    }
}
