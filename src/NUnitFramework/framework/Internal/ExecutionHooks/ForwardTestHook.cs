// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    internal sealed class ForwardTestHook : TestHook
    {
        private readonly List<Action<TestExecutionContext>> _list;

        protected override IReadOnlyCollection<Action<TestExecutionContext>> Handlers => _list;

        internal override void AddHandler(Action<TestExecutionContext> handler) => _list.Add(handler);

        public ForwardTestHook()
        {
            _list = new List<Action<TestExecutionContext>>();
        }

        public ForwardTestHook(ForwardTestHook source)
        {
            _list = new List<Action<TestExecutionContext>>(source._list);
        }
    }
}
