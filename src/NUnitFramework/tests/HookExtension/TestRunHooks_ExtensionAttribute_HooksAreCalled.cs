// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension;

public class TestRunHooksAreCalledBeforeTestIsExecuted
{
    [TestSetupUnderTest]
    public class TestUnderTest
    {
        [OneTimeSetUp]
        public void RegisterLongRunningBeforeTestHooks()
        {
            TestExecutionContext.CurrentContext?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                // Delay to ensure that handlers run longer than the test case
                await System.Threading.Tasks.Task.Delay(1000);
                TestLog.LogCurrentMethod();
            });
            TestExecutionContext.CurrentContext?.HookExtension?.BeforeTest.AddHandler(async (sender, eventArgs) =>
            {
                // Delay to ensure that handlers run longer than the test case
                await System.Threading.Tasks.Task.Delay(1000);
                TestLog.LogCurrentMethod();
            });
        }

        [Test]
        public void SomeTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [Test]
    [NonParallelizable]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.That(testResult.Logs, Is.EqualTo(new string[]
        {
            nameof(TestUnderTest.RegisterLongRunningBeforeTestHooks),
            nameof(TestUnderTest.RegisterLongRunningBeforeTestHooks),
            nameof(TestUnderTest.SomeTest),
        }));
    }
}
