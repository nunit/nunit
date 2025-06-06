// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    public sealed class TestHook
    {
        private readonly List<EventHandler> _handlers = new();

        /// <summary>
        /// Adds a test hook handler.
        /// </summary>
        /// <param name="handler">The event handler to be attached to the hook.</param>
        public void AddHandler(EventHandler handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        internal IReadOnlyList<EventHandler> GetHandlers()
        {
            lock (_handlers)
                return _handlers.ToArray();
        }

        internal void InvokeHandlers(object? sender, EventArgs e)
        {
            foreach (var handler in GetHandlers())
            {
                handler(sender, e);
            }
        }
    }
}
