// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.ExecutionHookTests
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SomeTestActionAttribute : Attribute, ITestAction
    {
        public static readonly string LogStringForBeforeTest = $"{nameof(SomeTestActionAttribute)}.{nameof(BeforeTest)}";
        public static readonly string LogStringForAfterTest = $"{nameof(SomeTestActionAttribute)}.{nameof(AfterTest)}";

        public void BeforeTest(ITest test)
        {
            TestLog.LogMessage(LogStringForBeforeTest);
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogMessage(LogStringForAfterTest);
        }

        public ActionTargets Targets => ActionTargets.Suite;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateClassLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateClassLevelBeforeTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateClassLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateClassLevelAfterTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelBeforeTestHooksAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelAfterTestHooksAttribute));
        }
    }

    public sealed class ActivateBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
        }
    }

    public sealed class ActivateBeforeTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
            throw new Exception("Before test hook crashed!!");
        }
    }

    public sealed class ActivateLongRunningBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
        }
    }

    public sealed class ActivateAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
        }
    }

    public sealed class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
            throw new Exception("After test hook crashed!!");
        }
    }

    public sealed class ActivateLongRunningAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            // Simulate a long-running after test hook
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class LogTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }

    public class TestActionLoggingHookExtensionAttribute : NUnitAttribute, IApplyToContext
    {
        public void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AddBeforeTestActionBeforeTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionBeforeTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionBeforeTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionBeforeTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddBeforeTestActionAfterTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionAfterTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionAfterTestHandler((context) =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionAfterTestHook({(context.CurrentTest.IsSuite ? "Suite" : "Test")})");
            });
        }
    }

    [TestFixture]
    public class FailingTestWithTestHookOnMethod
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
            Assert.Fail("Some failure in test");
        }
    }

    public class TestWithTestHooksOnMethodAndErrorOnAfterTestHook
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHookThrowingException]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    public class TestWithTestHooksOnMethodAndErrorOnBeforeTestHook
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHookThrowingException]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    public class TestWithTestHooksOnMethod
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    public class TestWithAfterTestHookOnMethod
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    public class TestWithBeforeTestHookOnMethod
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    public class TestWithNormalAndLongRunningTestHooks
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        [ActivateAfterTestHook]
        [ActivateLongRunningAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [ActivateClassLevelBeforeTestHooks]
    [ActivateClassLevelAfterTestHooks]
    public class TestClassWithTestHooksOneTestWithoutAndOneWithMethodTestHooks
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateMethodLevelBeforeTestHooks]
        [ActivateMethodLevelAfterTestHooks]
        public void EmptyTestWithHooks()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        public void EmptyTestWithoutHooks()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [SomeTestAction]
    public class TestWithTestHooksAndClassTestActionAttribute
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void SetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [Test]
        [ActivateBeforeTestHook]
        [ActivateAfterTestHook]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [TestFixture]
    [LogTestAction]
    [TestActionLoggingHookExtension]
    public class TestClassWithTestAction
    {
        [Test]
        public void TestUnderTest()
        {
            TestLog.LogCurrentMethod();
        }
    }
}
