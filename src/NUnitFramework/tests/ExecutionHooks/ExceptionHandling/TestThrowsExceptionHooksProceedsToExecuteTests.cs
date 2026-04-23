// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.ExceptionHandling
{
    internal class TestThrowsExceptionHooksProceedsToExecuteTests
    {
        [Test]
        public void TestThrowsException_HooksProceedsToExecute()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestThrowsExceptionHooksProceedsToExecuteFixture));
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeSetUp),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.SetUp),
                nameof(ActivateBeforeTestHookAtMethodLevelAttribute),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.EmptyTest),
                nameof(ActivateAfterTestHookAtMethodLevelAttribute),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.TearDown),
                nameof(TestThrowsExceptionHooksProceedsToExecuteFixture.OneTimeTearDown)
            ]));
        }

        [Test]
        public void TestSetUpThrowsException_HooksReceiveException()
        {
            var obj = new TestLifeCycleThrowsExceptionPassesExceptionToAfterHook();
            var result = TestBuilder.RunTestFixture(obj);

            var testName = nameof(TestLifeCycleThrowsExceptionPassesExceptionToAfterHook.EmptyTest);
            var ex = obj.SetupErrors[testName];
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestTearDownThrowsException_HooksReceiveException()
        {
            var obj = new TestLifeCycleThrowsExceptionPassesExceptionToAfterHook();
            var result = TestBuilder.RunTestFixture(obj);

            var testName = nameof(TestLifeCycleThrowsExceptionPassesExceptionToAfterHook.EmptyTest);
            var ex = obj.TearDownErrors[testName];
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestThrowsException_HooksReceiveOriginalException()
        {
            var obj = new TestThrowsExceptionPassesExceptionToAfterHook();
            var result = TestBuilder.RunTestFixture(obj);

            var testName = nameof(TestThrowsExceptionPassesExceptionToAfterHook.WrappedExceptionExample);
            var ex = obj.TestErrors[testName];
            Assert.That(ex, Is.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestThrowsSuccessException_HooksReceiveNoException()
        {
            var obj = new TestThrowsExceptionPassesExceptionToAfterHook();
            var result = TestBuilder.RunTestFixture(obj);

            var testName = nameof(TestThrowsExceptionPassesExceptionToAfterHook.AssertPassedExample);
            Assert.That(obj.TestErrors, Does.Not.ContainKey(testName));
        }
    }
}
