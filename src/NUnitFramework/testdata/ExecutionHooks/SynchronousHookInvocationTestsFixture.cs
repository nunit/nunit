// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData.ExecutionHooks
{
    public class SynchronousHookInvocationTestsFixture
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
}
