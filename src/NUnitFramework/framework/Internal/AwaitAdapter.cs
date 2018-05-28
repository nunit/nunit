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

using System;

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
            return CSharpPatternBasedAwaitAdapter.IsAwaitable(awaitableType);
        }

        public static Type GetResultType(Type awaitableType)
        {
            return CSharpPatternBasedAwaitAdapter.GetResultType(awaitableType);
        }

        public static AwaitAdapter FromAwaitable(object awaitable)
        {
            if (awaitable == null)
                throw new InvalidOperationException("A null reference cannot be awaited.");

#if !(NET35 || NET40)
            // TaskAwaitAdapter is more efficient because it can rely on Task’s
            // special quality of blocking until complete in GetResult.
            // As long as the pattern-based adapters are reflection-based, this
            // is much more efficient as well.
            var task = awaitable as System.Threading.Tasks.Task;
            if (task != null) return TaskAwaitAdapter.Create(task);
#endif

            // Await all the (C# and F#) things
            var adapter =
                CSharpPatternBasedAwaitAdapter.TryCreate(awaitable)
                ?? FSharpAsyncAwaitAdapter.TryCreate(awaitable);
            if (adapter != null) return adapter;

#if NET40
            // If System.Threading.Tasks.Task does not have a GetAwaiter instance method
            // (we don’t heuristically search for AsyncBridge-style extension methods),
            // we still need to be able to await it to preserve NUnit behavior on machines
            // which have a max .NET Framework version of 4.0 installed, such as the default
            // for versions of Windows earlier than 8.
            var task = awaitable as System.Threading.Tasks.Task;
            if (task != null) return Net40BclTaskAwaitAdapter.Create(task);
#endif

            throw new NotSupportedException("NUnit can only await objects which follow the C# specification for awaitable expressions.");
        }
    }
}
