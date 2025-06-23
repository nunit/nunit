// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    internal sealed class ReverseTestHook : TestHook
    {
        private readonly Stack<Action<TestExecutionContext>> _stack;

        protected override IReadOnlyCollection<Action<TestExecutionContext>> Handlers => _stack;

        internal override void AddHandler(Action<TestExecutionContext> handler) => _stack.Push(handler);

        public ReverseTestHook()
        {
            _stack = new Stack<Action<TestExecutionContext>>();
        }

        public ReverseTestHook(ReverseTestHook source)
        {
            _stack = new Stack<Action<TestExecutionContext>>(source._stack);
        }
    }
}
