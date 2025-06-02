// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Framework.Internal.HookExtensions
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
        /// <param name="handler">The event handler to be attached to the handler</param>
        public void AddHandler(EventHandler handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        internal IReadOnlyList<EventHandler> GetHandlers()
        {
            lock (_handlers)
                return _handlers;
        }

        internal void InvokeHandlers(object? sender, EventArgs e)
        {
            if (!_handlers.Any())
            {
                return;
            }
            EventHandler[] syncHandlers;

            lock (_handlers)
                syncHandlers = _handlers.ToArray();

            foreach (var handler in syncHandlers)
            {
                handler(sender, e);
            }
        }
    }
}
