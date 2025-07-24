// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    internal sealed class AfterHooks : Hooks
    {
        private readonly Stack<Action<HookData>> _stack;

        protected override IReadOnlyCollection<Action<HookData>> Handlers => _stack;

        internal override void AddHandler(Action<HookData> handler) => _stack.Push(handler);

        public AfterHooks()
        {
            _stack = new Stack<Action<HookData>>();
        }

        public AfterHooks(AfterHooks source)
        {
            _stack = new Stack<Action<HookData>>(source._stack);
        }
    }
}
