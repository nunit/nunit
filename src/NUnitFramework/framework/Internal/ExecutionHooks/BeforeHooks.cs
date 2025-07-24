// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    internal sealed class BeforeHooks : Hooks
    {
        private readonly List<Action<HookData>> _list;

        protected override IReadOnlyCollection<Action<HookData>> Handlers => _list;

        internal override void AddHandler(Action<HookData> handler) => _list.Add(handler);

        public BeforeHooks()
        {
            _list = new List<Action<HookData>>();
        }

        public BeforeHooks(BeforeHooks source)
        {
            _list = new List<Action<HookData>>(source._list);
        }
    }
}
