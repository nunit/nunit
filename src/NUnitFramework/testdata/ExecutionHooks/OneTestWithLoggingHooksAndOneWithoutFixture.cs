// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData.ExecutionHooks
{
    public class OneTestWithLoggingHooksAndOneWithoutFixture
    {
        [Test, ActivateTestHook, Order(1)]
        public void TestWithHookLogging() => TestLog.LogCurrentMethod();

        [Test, Order(2)]
        public void TestWithoutHookLogging() => TestLog.LogCurrentMethod();
    }
}
