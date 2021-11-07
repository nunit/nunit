// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    partial class AsyncExecutionApiAdapter
    {
        private sealed class FSharpTaskReturningTestMethodAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.FSharp.AsyncExecutionApiAdapter.TaskReturningTestMethodFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[<Test>] member this.TestMethod() = async { … }";
        }

        private sealed class FSharpTaskReturningSetUpAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.FSharp.AsyncExecutionApiAdapter.TaskReturningSetUpFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[<SetUp>] member this.SetUp() = async { … }";
        }

        private sealed class FSharpTaskReturningTearDownAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.FSharp.AsyncExecutionApiAdapter.TaskReturningTearDownFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[<TearDown>] member this.TearDown() = async { … }";
        }

        private sealed class FSharpTaskReturningOneTimeSetUpAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.FSharp.AsyncExecutionApiAdapter.TaskReturningOneTimeSetUpFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[<OneTimeSetUp>] member this.OneTimeSetUp() = async { … }";
        }

        private sealed class FSharpTaskReturningOneTimeTearDownAdapter : AsyncExecutionApiAdapter
        {
            public override void Execute(AsyncTestDelegate asyncUserCode)
            {
                ExecuteFixture(
                    typeof(TestData.FSharp.AsyncExecutionApiAdapter.TaskReturningOneTimeTearDownFixture),
                    asyncUserCode);
            }

            public override string ToString() => "[<OneTimeTearDown>] member this.OneTimeTearDown() = async { … }";
        }
    }
}
