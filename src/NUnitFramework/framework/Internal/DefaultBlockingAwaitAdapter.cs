// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Useful when wrapping awaiters whose <c>GetResult</c> method does not block until complete.
    /// Contains a default mechanism to implement <see cref="AwaitAdapter.BlockUntilCompleted"/>
    /// via <see cref="AwaitAdapter.IsCompleted"/> and <see cref="AwaitAdapter.OnCompleted"/>.
    /// </summary>
    internal abstract class DefaultBlockingAwaitAdapter : AwaitAdapter
    {
        private volatile ManualResetEventSlim? _completedEvent;

        public sealed override void BlockUntilCompleted()
        {
            if (IsCompleted) return;

            var completedEvent = _completedEvent; // Volatile read (would be Volatile.Read if not for net40 support)
            if (completedEvent is null)
            {
                completedEvent = new ManualResetEventSlim();

#pragma warning disable 420 // Taking a ref to a volatile field is fine if the ref is only used by Interlocked or Volatile methods.
                var previous = Interlocked.CompareExchange(ref _completedEvent, completedEvent, null);
#pragma warning restore 420

                if (previous is null)
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
