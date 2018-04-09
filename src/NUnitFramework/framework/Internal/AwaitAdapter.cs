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

#if ASYNC
using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal abstract class AwaitAdapter
    {
        public abstract bool IsCompleted { get; }
        public abstract void OnCompleted(Action action);
        public abstract void BlockUntilCompleted();

        public static AwaitAdapter FromAwaitable(object awaitable)
        {
            if (awaitable == null) throw new ArgumentNullException(nameof(awaitable));

            var task = awaitable as Task;
            if (task == null)
                throw new NotImplementedException("Proper awaitable implementation to follow.");

#if NET40
            // TODO: use the general reflection-based awaiter if net40 build is running against a newer BCL
            return new Net40BclTaskAwaitAdapter(task);
#else
            return new TaskAwaitAdapter(task);
#endif
        }

#if NET40
        private sealed class Net40BclTaskAwaitAdapter : AwaitAdapter
        {
            private readonly Task _task;

            public Net40BclTaskAwaitAdapter(Task task)
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
        }
#else
        private sealed class TaskAwaitAdapter : AwaitAdapter
        {
            private readonly TaskAwaiter _awaiter;

            public TaskAwaitAdapter(Task task)
            {
                _awaiter = task.GetAwaiter();
            }

            public override bool IsCompleted => _awaiter.IsCompleted;

            [SecuritySafeCritical]
            public override void OnCompleted(Action action) => _awaiter.UnsafeOnCompleted(action);

            // Assumption that GetResult blocks until complete is only valid for System.Threading.Tasks.Task.
            public override void BlockUntilCompleted() => _awaiter.GetResult();
        }
#endif
    }
}
#endif
