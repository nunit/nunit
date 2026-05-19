// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.TestData.ExecutionHooks
{
    public class CombinedHookTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateLongRunningBeforeTestHook]
        [ActivateAfterTestHookAtMethodLevel]
        [ActivateLongRunningAfterTestHook]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [ActivateBeforeTestHookAtClassLevel]
    [ActivateAfterTestHookAtClassLevel]
    public class CombinedHookAtClassAndMethodLevelTestsFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTestWithHooks() => TestLog.LogCurrentMethod();

        [Test]
        public void EmptyTestWithoutHooks() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    public class OneTestWithLoggingHooksAndOneWithoutFixture
    {
        [Test, ActivateTestHook, Order(1)]
        public void TestWithHookLogging() => TestLog.LogCurrentMethod();

        [Test, Order(2)]
        public void TestWithoutHookLogging() => TestLog.LogCurrentMethod();
    }

    public class TestThrowsExceptionHooksProceedsToExecuteFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [ExceptionLogging]
    public class TestLifeCycleThrowsExceptionPassesExceptionToAfterHook
    {
        public Dictionary<string, Exception> SetupErrors { get; } = [];
        public Dictionary<string, Exception> TearDownErrors { get; } = [];

        [SetUp]
        public void SetUp() => throw new InvalidOperationException(nameof(SetUp));

        [TearDown]
        public void TearDown() => throw new InvalidOperationException(nameof(TearDown));

        [Test]
        public void EmptyTest()
        {
        }
    }

    [ExceptionLogging]
    public class TestThrowsExceptionPassesExceptionToAfterHook
    {
        public Dictionary<string, Exception> TestErrors { get; } = [];

        [Test]
        public void WrappedExceptionExample() => throw new InvalidOperationException(nameof(WrappedExceptionExample));

        [Test]
        public void AssertPassedExample() => Assert.Pass();
    }

    [ExceptionLogging]
    public class TestActionThrowsExceptionPassesExceptionToAfterHook
    {
        public Dictionary<string, Exception> BeforeTestActionErrors { get; } = [];
        public Dictionary<string, Exception> AfterTestActionErrors { get; } = [];

        [Test]
        [BeforeTestActionThrowsException]
        [AfterTestActionThrowsException]
        public void TestActionTest()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BeforeTestActionThrowsExceptionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test) => throw new InvalidOperationException(nameof(BeforeTest));

        public void AfterTest(ITest test)
        {
        }

        public ActionTargets Targets => ActionTargets.Test;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AfterTestActionThrowsExceptionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
        }

        public void AfterTest(ITest test) => throw new InvalidOperationException(nameof(AfterTest));

        public ActionTargets Targets => ActionTargets.Test;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExceptionLoggingAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestThrowsExceptionPassesExceptionToAfterHook;
                fixture!.TestErrors[hookData.Context.Test.Name] = hookData.Exception;
            }
        }

        public override void AfterEverySetUpHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestLifeCycleThrowsExceptionPassesExceptionToAfterHook;
                fixture!.SetupErrors[hookData.Context.Test.Name] = hookData.Exception;
            }
        }

        public override void AfterEveryTearDownHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestLifeCycleThrowsExceptionPassesExceptionToAfterHook;
                fixture!.TearDownErrors[hookData.Context.Test.Name] = hookData.Exception;
            }
        }

        public override void AfterTestActionBeforeTestHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestActionThrowsExceptionPassesExceptionToAfterHook;
                fixture!.BeforeTestActionErrors[hookData.Context.Test.Name] = hookData.Exception;
            }
        }

        public override void AfterTestActionAfterTestHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestActionThrowsExceptionPassesExceptionToAfterHook;
                fixture!.AfterTestActionErrors[hookData.Context.Test.Name] = hookData.Exception;
            }
        }
    }

    public class TestFailsWithAssertHooksProceedsToExecuteFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest()
        {
            TestLog.LogCurrentMethod();
            Assert.Fail("Some failure in test");
        }

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }

    [SimpleTestAction]
    public class TestActionAndHookCombinationFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() => TestLog.LogCurrentMethod();

        [SetUp]
        public void SetUp() => TestLog.LogCurrentMethod();

        [Test]
        [ActivateBeforeTestHookAtMethodLevel]
        [ActivateAfterTestHookAtMethodLevel]
        public void EmptyTest() => TestLog.LogCurrentMethod();

        [TearDown]
        public void TearDown() => TestLog.LogCurrentMethod();

        [OneTimeTearDown]
        public void OneTimeTearDown() => TestLog.LogCurrentMethod();
    }
}
