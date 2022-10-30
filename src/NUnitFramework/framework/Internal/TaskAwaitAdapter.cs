// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal static class TaskAwaitAdapter
    {
        public static AwaitAdapter Create(Task task)
        {
            var genericTaskType = task
                .GetType()
                .TypeAndBaseTypes()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>));

            if (genericTaskType != null)
            {
                var typeArgument = genericTaskType.GetGenericArguments()[0];
                return (AwaitAdapter)typeof(GenericAdapter<>)
                     .MakeGenericType(typeArgument)
                     .GetConstructor(new[] { typeof(Task<>).MakeGenericType(typeArgument) })
                     .Invoke(new object[] { task });
            }

            return new NonGenericAdapter(task);
        }

        private sealed class NonGenericAdapter : AwaitAdapter
        {
            private readonly TaskAwaiter _awaiter;

            public NonGenericAdapter(Task task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            [SecuritySafeCritical]
            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.Task.
            public override void BlockUntilCompleted() => _awaiter.GetResult();

            public override object GetResult()
            {
                _awaiter.GetResult(); // Throws exceptions, if any
                return null;
            }
        }

        private sealed class GenericAdapter<T> : AwaitAdapter
        {
            private readonly TaskAwaiter<T> _awaiter;

            public GenericAdapter(Task<T> task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            [SecuritySafeCritical]
            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.Task.
            public override void BlockUntilCompleted() => _awaiter.GetResult();

            public override object GetResult()
            {
                return _awaiter.GetResult(); // Throws exceptions, if any
            }
        }
    }
}
