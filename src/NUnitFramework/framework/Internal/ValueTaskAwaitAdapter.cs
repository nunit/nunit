// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal static class ValueTaskAwaitAdapter
    {
        public static AwaitAdapter Create(ValueTask task)
        {
            return new NonGenericAdapter(task);
        }

        public static AwaitAdapter? TryCreate(object task)
        {
            Type taskType = task.GetType();
            if (taskType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                return (AwaitAdapter)typeof(GenericAdapter<>)
                    .MakeGenericType(taskType.GetGenericArguments()[0])
                    .GetConstructor(new[] { taskType })!
                    .Invoke(new object[] { task });
            }

            return null;
        }

        private sealed class NonGenericAdapter : AwaitAdapter
        {
            private readonly ValueTaskAwaiter _awaiter;

            public NonGenericAdapter(ValueTask task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.ValueTask.
            public override void BlockUntilCompleted() => _awaiter.GetResult();

            public override object? GetResult()
            {
                _awaiter.GetResult(); // Throws exceptions, if any
                return null;
            }
        }

        private sealed class GenericAdapter<T> : AwaitAdapter
        {
            private readonly ValueTaskAwaiter<T> _awaiter;

            public GenericAdapter(ValueTask<T> task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.ValueTask.
            public override void BlockUntilCompleted() => _awaiter.GetResult();

            public override object? GetResult()
            {
                return _awaiter.GetResult(); // Throws exceptions, if any
            }
        }
    }
}
