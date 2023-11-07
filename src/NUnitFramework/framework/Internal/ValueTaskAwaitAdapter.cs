// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal static class ValueTaskAwaitAdapter
    {
        public static AwaitAdapter Create(ValueTask task)
        {
            var genericValueTaskType = task
                .GetType()
                .TypeAndBaseTypes()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ValueTask<>));

            if (genericValueTaskType is not null)
            {
                var typeArgument = genericValueTaskType.GetGenericArguments()[0];
                return (AwaitAdapter)typeof(GenericAdapter<>)
                     .MakeGenericType(typeArgument)
                     .GetConstructor(new[] { typeof(ValueTask<>).MakeGenericType(typeArgument) })!
                     .Invoke(new object[] { task });
            }

            return new NonGenericAdapter(task);
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
