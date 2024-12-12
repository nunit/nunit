// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [Test, ActivateHookLogging, Order(1)]
            public void TestWithHookLogging()
            {
                TestLog.LogCurrentMethod();
            }

            [Test, Order(2)]
            public void TestWithoutHookLogging()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        [NonParallelizable]
        public void CheckLoggingTest()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.Logs, Is.EqualTo(new[]
            {
                $"- BeforeTestCase({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithHookLogging),
                $"- AfterTestCase({nameof(TestUnderTest.TestWithHookLogging)})",
                nameof(TestUnderTest.TestWithoutHookLogging)
            }));
        }
    }
}
