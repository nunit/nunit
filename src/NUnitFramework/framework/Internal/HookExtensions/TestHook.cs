// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    public class TestHook
    {
        private readonly List<Delegate> _handlers = new();

        /// <summary>
        /// Adds a test hook handler.
        /// </summary>
        /// <param name="handler">The event handler to be attached to the handler</param>
        public void AddHandler(EventHandler handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }
    }
}
