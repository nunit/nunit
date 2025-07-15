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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateBeforeTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
            throw new Exception("Before test hook crashed!!");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            // Delay to ensure that handlers run longer than the test case
            Thread.Sleep(1000);
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ActivateAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
            throw new Exception("After test hook crashed!!");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateLongRunningAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            // Simulate a long-running after test hook
            Thread.Sleep(500);
            TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook + $"({context.CurrentTest.MethodName})");
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook + $"({context.CurrentTest.MethodName})");
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TestActionLoggingExecutionHooksAttribute : NUnitAttribute, IApplyToContext
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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateAllSynchronousTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeAnySetUpsHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeAnySetUpsHook);
        }

        public override void AfterAnySetUpsHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterAnySetUpsHook);
        }

        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);
        }

        public override void BeforeAnyTearDownsHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeAnyTearDownsHook);
        }

        public override void AfterAnyTearDownsHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterAnyTearDownsHook);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateSynchronousTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);

            TestExecutionContext.CurrentContext
                                    .CurrentTest.Properties
                                    .Add("BeforeTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }

        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);

            TestExecutionContext.CurrentContext
                                .CurrentTest.Properties
                                .Add("AfterTestHook_ThreadId", Environment.CurrentManagedThreadId);
        }
    }

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
    [TestActionLoggingExecutionHooks]
    public class TestClassWithTestAction
    {
        [Test]
        public void TestUnderTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    /// <remarks>
    /// This TestUnderTest class is failing by intention. Therefore is currently cannot reside in the same
    /// class as the calling test inside nunit.framework.tests since it would fail the whole test suite.
    /// Once https://github.com/nunit/nunit/issues/5002 is solved it can be moved back and marked as explicit.
    /// Then it can be executed with the TestBuilder while still not execute during the test suite execution.
    /// </remarks>
    public class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest
    {
        [Test]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHook]
        [ActivateAfterTestHookThrowingException]
        public void TestPasses()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }
    }

    /// <remarks>
    /// This TestUnderTest class is failing by intention. Therefore is currently cannot reside in the same
    /// class as the calling test inside nunit.framework.tests since it would fail the whole test suite.
    /// Once https://github.com/nunit/nunit/issues/5002 is solved it can be moved back and marked as explicit.
    /// Then it can be executed with the TestBuilder while still not execute during the test suite execution.
    /// </remarks>
    public class SynchronousHookInvocationTests_TestUnderTest
    {
        [SetUp]
        public void Setup()
        {
            TestExecutionContext.CurrentContext
                                .CurrentTest.Properties
                                .Add("TestThreadId", Environment.CurrentManagedThreadId);
        }

        [Test, ActivateSynchronousTestHook]
        public void TestPasses_WithAssertPass()
        {
            Assert.Pass("Test passed.");
        }

        [Test, ActivateSynchronousTestHook]
        public void TestFails_WithAssertFail()
        {
            Assert.Fail("Test failed with Assert.Fail");
        }

        [Test, ActivateSynchronousTestHook]
        public void TestFails_WithException()
        {
            throw new Exception("Test failed with Exception");
        }
    }

    public static class HookIdentifiers
    {
        public static readonly string Hook = "_Hook";

        public static readonly string AfterTestHook = $"AfterTestHook{Hook}";
        public static readonly string BeforeAnySetUpsHook = $"BeforeAnySetUpsHook{Hook}";
        public static readonly string AfterAnySetUpsHook = $"AfterAnySetUpsHook{Hook}";
        public static readonly string BeforeTestHook = $"BeforeTestHook{Hook}";
        public static readonly string BeforeAnyTearDownsHook = $"BeforeAnyTearDownsHook{Hook}";
        public static readonly string AfterAnyTearDownsHook = $"AfterAnyTearDownsHook{Hook}";
    }
}
