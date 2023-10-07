// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Tests
{
    /// <summary>
    /// Adapts each NUnit API that executes async user code synchronously to a common interface
    /// to be tested for behavior.
    /// </summary>
    public abstract partial class AsyncExecutionApiAdapter
    {
        public static IEnumerable<AsyncExecutionApiAdapter> All { get; } = new AsyncExecutionApiAdapter[]
        {
            new TaskReturningTestMethodAdapter(),
            new TaskReturningSetUpAdapter(),
            new TaskReturningTearDownAdapter(),
            new TaskReturningOneTimeSetUpAdapter(),
            new TaskReturningOneTimeTearDownAdapter(),
            new FSharpTaskReturningTestMethodAdapter(),
            new FSharpTaskReturningSetUpAdapter(),
            new FSharpTaskReturningTearDownAdapter(),
            new FSharpTaskReturningOneTimeSetUpAdapter(),
            new FSharpTaskReturningOneTimeTearDownAdapter(),
            new AssertThrowsAsyncAdapter(),
            new AssertDoesNotThrowAsyncAdapter(),
            new AssertCatchAsyncAdapter(),
            new ThrowsNothingConstraintAdapter(),
            new ThrowsConstraintAdapter(),
            new ThrowsExceptionConstraintAdapter(),
            new NormalConstraintAdapter()
        };

        public abstract void Execute(AsyncTestDelegate asyncUserCode);
        public abstract override string ToString();
    }
}
