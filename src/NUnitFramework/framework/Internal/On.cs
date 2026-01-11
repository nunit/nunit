// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Enables the <see cref="On.Dispose"/> syntax.
    /// </summary>
    internal static class On
    {
        /// <summary>
        /// Wraps an action so that it is executed when the returned object is disposed.
        /// This disposal is thread-safe and the action will be executed at most once.
        /// </summary>
        public static IDisposable Dispose(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            return new DisposableAction(action);
        }

        private sealed class DisposableAction : IDisposable
        {
            private Action? _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                Interlocked.Exchange(ref _action, null)?.Invoke();
            }
        }
    }
}
