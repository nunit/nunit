// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;
#endif
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

#if !NETFRAMEWORK
        [DynamicDependency(nameof(ValueTask<object>.GetAwaiter), typeof(ValueTask<>))]
        [DynamicDependency(nameof(ValueTaskAwaiter<object>.IsCompleted), typeof(ValueTaskAwaiter<>))]
        [DynamicDependency(nameof(ValueTaskAwaiter<object>.GetResult), typeof(ValueTaskAwaiter<>))]
#endif
        public static AwaitAdapter? TryCreate(object task)
        {
            Type taskType = task.GetType();
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
#if !NETFRAMEWORK
                if (!RuntimeFeature.IsDynamicCodeSupported)
                {
                    return CSharpPatternBasedAwaitAdapter.TryCreate(task)
                        ?? throw new NotSupportedException($"Cannot await value task of type {taskType}.");
                }
#endif

                return (AwaitAdapter)typeof(GenericAdapter<>)
                    .MakeGenericType(taskType.GetGenericArguments()[0])
                    .GetConstructor(new[] { taskType })!
                    .Invoke(new object[] { task });
            }

            return null;
        }

        private sealed class NonGenericAdapter : DefaultBlockingAwaitAdapter
        {
            private readonly ValueTaskAwaiter _awaiter;

            public NonGenericAdapter(ValueTask task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            public override object? GetResult()
            {
                _awaiter.GetResult(); // Throws exceptions, if any
                return null;
            }
        }

        private sealed class GenericAdapter<T> : DefaultBlockingAwaitAdapter
        {
            private readonly ValueTaskAwaiter<T> _awaiter;

            public GenericAdapter(ValueTask<T> task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            public override object? GetResult()
            {
                return _awaiter.GetResult(); // Throws exceptions, if any
            }
        }
    }
}
