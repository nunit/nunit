// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.ExecutionHookTests
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class SomeTestActionAttribute : Attribute, ITestAction
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
    public sealed class ActivateClassLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage("ActivateBeforeClassHook");
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ActivateClassLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage("ActivateAfterClassHook");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage("ActivateBeforeTestMethodHook");
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ActivateMethodLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(TestExecutionContext context)
        {
            TestLog.LogMessage("ActivateAfterTestMethodHook");
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
