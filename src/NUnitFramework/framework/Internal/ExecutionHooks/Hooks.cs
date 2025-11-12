// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    internal abstract class Hooks
    {
        protected abstract IReadOnlyCollection<Action<HookData>> Handlers { get; }

        internal int Count => Handlers.Count;

        internal abstract void AddHandler(Action<HookData> handler);

        internal IReadOnlyCollection<Action<HookData>> GetHandlers()
        {
            lock (Handlers)
            {
                return Handlers.ToArray();
            }
        }

        internal void InvokeHandlers(HookData hookInfo)
        {
            foreach (var handler in GetHandlers())
            {
                handler(hookInfo);
            }
        }
    }
}
