// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

namespace NUnit.Framework.Internal
{
    internal partial class SingleThreadedTestSynchronizationContext
    {
        private struct ScheduledWork
        {
            private readonly SendOrPostCallback _callback;
            private readonly object? _state;
            private readonly ManualResetEventSlim? _finished;

            public ScheduledWork(SendOrPostCallback callback, object? state, ManualResetEventSlim? finished)
            {
                _callback = callback;
                _state = state;
                _finished = finished;
            }

            public void Execute()
            {
                _callback.Invoke(_state);
                _finished?.Set();
            }
        }
    }
}
