// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework;
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

    public class TestThrowsExceptionPassesExceptionToAfterHook
    {
        public Dictionary<string, Exception> Errors { get; } = [];

        [Test]
        [ExceptionLogging]
        public void WrappedExceptionExample() => throw new InvalidOperationException();

        [Test]
        [ExceptionLogging]
        public void AssertPassedExample() => Assert.Pass();
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExceptionLoggingAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            if (hookData.Exception is not null)
            {
                var fixture = hookData.Context.Test.Parent!.Fixture as TestThrowsExceptionPassesExceptionToAfterHook;
                fixture!.Errors[hookData.Context.Test.Name] = hookData.Exception;
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
