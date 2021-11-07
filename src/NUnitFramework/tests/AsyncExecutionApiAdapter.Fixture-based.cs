// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.TestUtilities;

namespace NUnit.Framework
{
    partial class AsyncExecutionApiAdapter
    {
        private static void ExecuteFixture(Type fixtureType, AsyncTestDelegate asyncUserCode)
        {
            TestBuilder.RunTest(
                new NUnitTestFixtureBuilder().BuildFrom(new TypeWrapper(fixtureType), PreFilter.Empty, new TestFixtureData(asyncUserCode))
            ).AssertPassed();
        }

        private sealed class TaskReturningTestMethodAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.AsyncExecutionApiAdapter.TaskReturningTestMethodFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[Test] Task TestMethod() { … }";
        }

        private sealed class TaskReturningSetUpAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.AsyncExecutionApiAdapter.TaskReturningSetUpFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[SetUp] Task SetUp() { … }";
        }

        private sealed class TaskReturningTearDownAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.AsyncExecutionApiAdapter.TaskReturningTearDownFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[TearDown] Task TearDown() { … }";
        }

        private sealed class TaskReturningOneTimeSetUpAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.AsyncExecutionApiAdapter.TaskReturningOneTimeSetUpFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[OneTimeSetUp] Task OneTimeSetUp() { … }";
        }

        private sealed class TaskReturningOneTimeTearDownAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.AsyncExecutionApiAdapter.TaskReturningOneTimeTearDownFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[OneTimeTearDown] Task OneTimeTearDown() { … }";
        }
    }
}
