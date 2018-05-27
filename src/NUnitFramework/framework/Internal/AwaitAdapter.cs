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
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal abstract class AwaitAdapter
    {
        public abstract bool IsCompleted { get; }
        public abstract void OnCompleted(Action action);
        public abstract void BlockUntilCompleted();
        public abstract object GetResult();

        public static AwaitAdapter FromAwaitable(object awaitable)
        {
            if (awaitable == null) throw new ArgumentNullException(nameof(awaitable));

            var task = awaitable as Task;
            if (task == null)
                throw new NotImplementedException("Proper awaitable implementation to follow.");

#if NET40
            // TODO: use the general reflection-based awaiter if net40 build is running against a newer BCL
            return Net40BclTaskAwaitAdapter.Create(task);
#else
            return TaskAwaitAdapter.Create(task);
#endif
        }
    }
}
#endif
