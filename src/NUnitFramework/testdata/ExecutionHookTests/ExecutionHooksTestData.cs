// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.TestData.ExecutionHookTests
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class SomeTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethod("BeforeTest_Action");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethod("AfterTest_Action");
        }

        public ActionTargets Targets { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActivateClassLevelBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateBeforeClassHook");
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActivateClassLevelAfterTestHooksAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateAfterClassHook");
            });
        }
        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActivateMethodLevelBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateBeforeTestMethodHook");
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActivateMethodLevelAfterTestHooksAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateAfterTestMethodHook");
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    public class ActivateBeforeTestHookAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    public class ActivateBeforeTestHookThrowingExceptionAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
                throw new Exception("Before test hook crashed!!");
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }
    public class ActivateLongRunningBeforeTestHookAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                Thread.Sleep(500);
                TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    public class ActivateAfterTestHookAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    public class ActivateAfterTestHookThrowingExceptionAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
                throw new Exception("After test hook crashed!!");
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    public class ActivateLongRunningAfterTestHookAttribute : NUnitAttribute, IApplyToContext, IWrapTestMethod
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                Thread.Sleep(500);
                TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
            });
        }

        public TestCommand Wrap(TestCommand command)
        {
            return command is HookDelegatingTestCommand ? command : new HookDelegatingTestCommand(command);
        }
    }

    [TestFixture]
    public class EmptyTestFor_TestFailsWithAssert_HooksProceedsToExecute
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

    [TestFixture]
    public class EmptyTestFor_AfterTestHookThrowsException_ExecutionProceeds
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

    [TestFixture]
    public class EmptyTestFor_BeforeTestHookThrowsException_TestStops_AfterTestHookExecutes
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

    [TestFixture]
    public class EmptyTestFor_TestThrowsException_HooksProceedsToExecute
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
    public class EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes
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

    [TestFixture]
    public class EmptyTestFor_ExecutionProceedsAfterBeforeTestHookCompletes
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

    [TestFixture]
    public class EmptyTestFor_ExecutionProceedsAfterBothTestHooksComplete
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

    [TestFixture]
    [ActivateClassLevelBeforeTestHooks]
    [ActivateClassLevelAfterTestHooks]
    public class EmptyTestFor_ExecutionProceedsAfterBothTestHooksCompleteAtClassAndMethodLevels
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
        public void EmptyTestWithoutHooks1()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [TestFixture]
    [SomeTestAction]
    public class EmptyTestFor_ExecutionProceedsAfterTheAfterTestHookCompletes2
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
}
