// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

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
    public sealed class ActivateClassLevelBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateBeforeClassHook");
            });
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActivateClassLevelAfterTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateAfterClassHook");
            });
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelBeforeTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateBeforeTestMethodHook");
            });
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ActivateMethodLevelAfterTestHooksAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage("ActivateAfterTestMethodHook");
            });
        }
    }

    public class ActivateBeforeTestHookAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
            });
        }
    }

    public class ActivateBeforeTestHookThrowingExceptionAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateBeforeTestHookThrowingExceptionAttribute));
                throw new Exception("Before test hook crashed!!");
            });
        }
    }
    public class ActivateLongRunningBeforeTestHookAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.BeforeTest.AddHandler((sender, eventArgs) =>
            {
                Thread.Sleep(500);
                TestLog.LogMessage(nameof(ActivateLongRunningBeforeTestHookAttribute));
            });
        }
    }

    public class ActivateAfterTestHookAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
            });
        }
    }

    public class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
                throw new Exception("After test hook crashed!!");
            });
        }
    }

    public class ActivateLongRunningAfterTestHookAttribute : ExecutionHookAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AfterTest.AddHandler((sender, eventArgs) =>
            {
                Thread.Sleep(500);
                TestLog.LogMessage(nameof(ActivateLongRunningAfterTestHookAttribute));
            });
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
