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
        private readonly List<Action<TestExecutionContext>> _handlers;

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
            _handlers = new List<Action<TestExecutionContext>>();
        }

        public TestHook(TestHook source)
        {
            _handlers = new List<Action<TestExecutionContext>>(source._handlers);
        }

        internal void AddHandler(Action<TestExecutionContext> handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        internal void InvokeHandlers(TestExecutionContext context)
        {
            foreach (var handler in GetHandlers())
            {
                handler(context);
            }
        }

        private IReadOnlyList<Action<TestExecutionContext>> GetHandlers()
        {
            lock (_handlers)
                return _handlers.ToArray();
        }
    }
}
