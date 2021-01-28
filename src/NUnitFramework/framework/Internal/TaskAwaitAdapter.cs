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

#if TASK_PARALLEL_LIBRARY_API && !NET40
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using NUnit.Compatibility;

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
#endif
