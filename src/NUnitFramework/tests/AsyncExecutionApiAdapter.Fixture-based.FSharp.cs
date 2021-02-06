// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if TASK_PARALLEL_LIBRARY_API
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
#endif
