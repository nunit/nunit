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

#if NET40
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal static class Net40BclTaskAwaitAdapter
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

        private class NonGenericAdapter : AwaitAdapter
        {
            private readonly Task _task;

            public NonGenericAdapter(Task task)
            {
                _task = task;
            }

            public override bool IsCompleted => _task.IsCompleted;

            public override void OnCompleted(Action action)
            {
                if (action == null) return;

                // Normally we would call TaskAwaiter.UnsafeOnCompleted (https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/TaskAwaiter.cs)
                // We will have to polyfill on top of the TPL API.
                // Compare TaskAwaiter.OnCompletedInternal from Microsoft.Threading.Tasks.dll in Microsoft.Bcl.Async.nupkg.

                _task.ContinueWith(_ => action.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
            }

            public override void BlockUntilCompleted()
            {
                // Normally we would call TaskAwaiter.GetResult (https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/TaskAwaiter.cs)
                // We will have to polyfill on top of the TPL API.
                // Compare TaskAwaiter.ValidateEnd from Microsoft.Threading.Tasks.dll in Microsoft.Bcl.Async.nupkg.

                try
                {
                    _task.Wait(); // Wait even if the task is completed so that an exception is thrown for cancellation or failure.
                }
                catch (AggregateException ex) when (ex.InnerExceptions.Count == 1) // Task.Wait wraps every exception
                {
                    ExceptionHelper.Rethrow(ex.InnerException);
                }
            }

            public override object GetResult()
            {
                BlockUntilCompleted(); // Throw exceptions, if any
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

            public override void OnCompleted(Action action)
            {
                if (action == null) return;

                // Normally we would call TaskAwaiter.UnsafeOnCompleted (https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/TaskAwaiter.cs)
                // We will have to polyfill on top of the TPL API.
                // Compare TaskAwaiter.OnCompletedInternal from Microsoft.Threading.Tasks.dll in Microsoft.Bcl.Async.nupkg.

                _task.ContinueWith(_ => action.Invoke(), TaskScheduler.FromCurrentSynchronizationContext());
            }

            public override void BlockUntilCompleted()
            {
                // Normally we would call TaskAwaiter.GetResult (https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/TaskAwaiter.cs)
                // We will have to polyfill on top of the TPL API.
                // Compare TaskAwaiter.ValidateEnd from Microsoft.Threading.Tasks.dll in Microsoft.Bcl.Async.nupkg.

                try
                {
                    _task.Wait(); // Wait even if the task is completed so that an exception is thrown for cancellation or failure.
                }
                catch (AggregateException ex) when (ex.InnerExceptions.Count == 1) // Task.Wait wraps every exception
                {
                    ExceptionHelper.Rethrow(ex.InnerException);
                }
            }

            public override object GetResult()
            {
                // Normally we would call TaskAwaiter.GetResult (https://source.dot.net/#System.Private.CoreLib/src/System/Runtime/CompilerServices/TaskAwaiter.cs)
                // We will have to polyfill on top of the TPL API.
                // Compare TaskAwaiter.ValidateEnd from Microsoft.Threading.Tasks.dll in Microsoft.Bcl.Async.nupkg.

                try
                {
                    return _task.Result;
                }
                catch (AggregateException ex) when (ex.InnerExceptions.Count == 1) // Task.Wait wraps every exception
                {
                    ExceptionHelper.Rethrow(ex.InnerException);

                    // If this line is reached, ExceptionHelper.Rethrow is very broken.
                    throw new InvalidOperationException("ExceptionHelper.Rethrow failed to throw an exception.");
                }
            }
        }
    }
}
#endif
