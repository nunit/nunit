// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.TestAttributes;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Threading
{
    internal class SynchronousHookInvocationTests
    {
        [Explicit($"This test should only be run as part of the {nameof(SynchronousHookInvocation_HookExecutesInSameThreadAsTest)} test")]
        private class SynchronousHookInvocationTestsTestUnderTest
        {
            [SetUp]
            public void Setup() => TestExecutionContext.CurrentContext.CurrentTest.Properties.Add("TestThreadId", Environment.CurrentManagedThreadId);

            [Test, ActivateSynchronousTestHook]
            public void TestPasses_WithAssertPass() => Assert.Pass("Test passed.");

            [Test, ActivateSynchronousTestHook]
            public void TestFails_WithAssertFail() => Assert.Fail("Test failed with Assert.Fail");

            [Test, ActivateSynchronousTestHook]
            public void TestFails_WithException() => throw new Exception("Test failed with Exception");
        }

        [Test]
        public void SynchronousHookInvocation_HookExecutesInSameThreadAsTest()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(SynchronousHookInvocationTestsTestUnderTest), TestFilter.Explicit);
            workItem.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(workItem.Result.PassCount, Is.EqualTo(1));
                Assert.That(workItem.Result.FailCount, Is.EqualTo(2));
            });

            foreach (var testCase in workItem.Test.Tests)
            {
                var testThreadId = ParseThreadIdFromProperty(testCase, "TestThreadId");
                var beforeTestHookThreadId = ParseThreadIdFromProperty(testCase, "BeforeTestHook_ThreadId");
                var afterTestHookThreadId = ParseThreadIdFromProperty(testCase, "AfterTestHook_ThreadId");

                Assert.Multiple(() =>
                {
                    Assert.That(testThreadId, Is.Not.EqualTo(-1), "Test thread ID could not be parsed from PropertyBag.");
                    Assert.That(beforeTestHookThreadId, Is.EqualTo(testThreadId));
                    Assert.That(afterTestHookThreadId, Is.EqualTo(testThreadId));
                });
            }
        }

        private static int ParseThreadIdFromProperty(ITest test, string propertyName)
        {
            return int.TryParse(test.Properties.Get(propertyName)?.ToString(), out var threadId) ? threadId : -1;
        }
    }
}
