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

using System.Threading;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Useful when wrapping awaiters whose <c>GetResult</c> method does not block until complete.
    /// Contains a default mechanism to implement <see cref="AwaitAdapter.BlockUntilCompleted"/>
    /// via <see cref="AwaitAdapter.IsCompleted"/> and <see cref="AwaitAdapter.OnCompleted"/>.
    /// </summary>
    internal abstract class DefaultBlockingAwaitAdapter : AwaitAdapter
    {
        private volatile ManualResetEventSlim _completedEvent;

        public sealed override async Task BlockUntilCompleted()
        {
            if (IsCompleted) return;

            var completedEvent = _completedEvent; // Volatile read (would be Volatile.Read if not for net40 support)
            if (completedEvent == null)
            {
                completedEvent = new ManualResetEventSlim();

#pragma warning disable 420 // Taking a ref to a volatile field is fine if the ref is only used by Interlocked or Volatile methods.
                var previous = Interlocked.CompareExchange(ref _completedEvent, completedEvent, null);
#pragma warning restore 420

                if (previous == null)
                {
                    // We are the first thread. (Though by this time, other threads may now be
                    // waiting on this ManualResetEvent.) Register to signal the event on completion.
                    // If completion has already happened by this time, OnCompleted is still obligated
                    // to execute the action we pass.
                    OnCompleted(completedEvent.Set);
                }
                else
                {
                    // We lost a race with another thread.
                    completedEvent.Dispose();
                    completedEvent = previous;
                }
            }

            completedEvent.Wait();
        }
    }
}
