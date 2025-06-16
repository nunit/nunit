// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    internal sealed class TestHook
    {
        private readonly List<EventHandler> _handlers;

        internal int Count
        {
            get
            {
                lock (_handlers)
                {
                    return _handlers.Count;
                }
            }
        }

        public TestHook()
        {
            _handlers = new List<EventHandler>();
        }

        public TestHook(TestHook source)
        {
            _handlers = new List<EventHandler>(source._handlers);
        }

        internal void AddHandler(EventHandler handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        internal void InvokeHandlers(object? sender, EventArgs e)
        {
            foreach (var handler in GetHandlers())
            {
                handler(sender, e);
            }
        }

        private IReadOnlyList<EventHandler> GetHandlers()
        {
            lock (_handlers)
                return _handlers.ToArray();
        }
    }
}
