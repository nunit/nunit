// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.ExecutionHookTests;

namespace NUnit.Framework.Tests.ExecutionHooks.ThreadingTests
{
    internal class SynchronousHookInvocationTests
    {
        [Test]
        public void SynchronousHookInvocation_HookExecutesInSameThreadAsTest()
        {
            TestLog.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(SynchronousHookInvocationTests_TestUnderTest));
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
