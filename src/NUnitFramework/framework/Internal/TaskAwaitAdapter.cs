// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
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
                .FirstOrDefault(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>));

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
            private readonly Task _task;

            public NonGenericAdapter(Task task)
            {
                _task = task;
            }

            public override bool IsCompleted => _task.IsCompleted;

            [SecuritySafeCritical]
            public override void OnCompleted(Action action) => _task.GetAwaiter().UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.Task.
            public override Task BlockUntilCompleted() => _task;

            public override object GetResult()
            {
                _task.GetAwaiter().GetResult(); // Throws exceptions, if any
                return null;
            }
        }

        private sealed class GenericAdapter<T> : AwaitAdapter
        {
            private readonly Task<T> _task;

            public GenericAdapter(Task<T> task)
            {
                _task = task;
            }

            public override bool IsCompleted => _task.IsCompleted;

            [SecuritySafeCritical]
            public override void OnCompleted(Action action) => _task.GetAwaiter().UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.Task.
            public override Task BlockUntilCompleted() => _task;

            public override object GetResult()
            {
                return _task.GetAwaiter().GetResult(); // Throws exceptions, if any
            }
        }
    }
}
