// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Adapts various styles of asynchronous waiting to a common API.
    /// </summary>
    internal abstract class AwaitAdapter
    {
        public abstract bool IsCompleted { get; }
        public abstract void OnCompleted(Action action);
        public abstract void BlockUntilCompleted();
        public abstract object GetResult();

        public static bool IsAwaitable(Type awaitableType)
        {
            return
                CSharpPatternBasedAwaitAdapter.IsAwaitable(awaitableType)
                || FSharpAsyncAwaitAdapter.IsAwaitable(awaitableType);
        }

        public static Type GetResultType(Type awaitableType)
        {
            return
                CSharpPatternBasedAwaitAdapter.GetResultType(awaitableType)
                ?? FSharpAsyncAwaitAdapter.GetResultType(awaitableType);
        }

        public static AwaitAdapter FromAwaitable(object awaitable)
        {
            if (awaitable == null)
                throw new InvalidOperationException("A null reference cannot be awaited.");

            // TaskAwaitAdapter is more efficient because it can rely on Task’s
            // special quality of blocking until complete in GetResult.
            // As long as the pattern-based adapters are reflection-based, this
            // is much more efficient as well.
            if (awaitable is System.Threading.Tasks.Task task)
                return TaskAwaitAdapter.Create(task);

            // Await all the (C# and F#) things
            var adapter =
                CSharpPatternBasedAwaitAdapter.TryCreate(awaitable)
                ?? FSharpAsyncAwaitAdapter.TryCreate(awaitable);
            if (adapter != null)
                return adapter;

            throw new NotSupportedException("NUnit can only await objects which follow the C# specification for awaitable expressions.");
        }
    }
}
