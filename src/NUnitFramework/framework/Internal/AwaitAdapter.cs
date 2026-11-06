// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

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
        public abstract object? GetResult();

        public static bool IsAwaitable(Type awaitableType)
        {
            return
                TryGetTaskResultType(awaitableType, out _)
                || CSharpPatternBasedAwaitAdapter.IsAwaitable(awaitableType)
                || FSharpAsyncAwaitAdapter.IsAwaitable(awaitableType);
        }

        public static Type GetResultType(Type awaitableType)
        {
            if (TryGetTaskResultType(awaitableType, out var taskResultType))
                return taskResultType;

            return
                CSharpPatternBasedAwaitAdapter.GetResultType(awaitableType)
                ?? FSharpAsyncAwaitAdapter.GetResultType(awaitableType)
                ?? throw new InvalidOperationException("Cannot determine result type for: " + awaitableType);
        }

        /// <summary>
        /// Recognizes <see cref="Task"/>, <see cref="Task{TResult}"/>, <see cref="ValueTask"/> and
        /// <see cref="ValueTask{TResult}"/> without looking up their awaiter members by reflection.
        /// </summary>
        /// <remarks>
        /// The pattern-based lookup finds <c>GetAwaiter</c>, <c>IsCompleted</c>, <c>GetResult</c> and
        /// <c>INotifyCompletion.OnCompleted</c> by name. Trimming tools (ILLink, NativeAOT) cannot see
        /// that usage and may remove those members, which would make every async test non-runnable.
        /// The result types of these well-known types are known statically.
        /// </remarks>
        private static bool TryGetTaskResultType(Type awaitableType, [NotNullWhen(true)] out Type? resultType)
        {
            if (awaitableType == typeof(Task) || awaitableType == typeof(ValueTask))
            {
                resultType = typeof(void);
                return true;
            }

            if (awaitableType.IsGenericType)
            {
                var genericTypeDefinition = awaitableType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(ValueTask<>))
                {
                    resultType = awaitableType.GetGenericArguments()[0];
                    return true;
                }
            }

            resultType = null;
            return false;
        }

        public static AwaitAdapter FromAwaitable(object? awaitable)
        {
            if (awaitable is null)
                throw new InvalidOperationException("A null reference cannot be awaited.");

            // (Value)TaskAwaitAdapter is more efficient because it can rely on Task’s
            // special quality of blocking until complete in GetResult.
            // As long as the pattern-based adapters are reflection-based, this
            // is much more efficient as well.
            if (awaitable is System.Threading.Tasks.Task task)
                return TaskAwaitAdapter.Create(task);
            if (awaitable is System.Threading.Tasks.ValueTask valueTask)
                return ValueTaskAwaitAdapter.Create(valueTask);

            var adapter = ValueTaskAwaitAdapter.TryCreate(awaitable)
                // Await all the (C# and F#) things
                ?? CSharpPatternBasedAwaitAdapter.TryCreate(awaitable)
                ?? FSharpAsyncAwaitAdapter.TryCreate(awaitable);
            if (adapter is not null)
                return adapter;

            throw new NotSupportedException("NUnit can only await objects which follow the C# specification for awaitable expressions.");
        }
    }
}
