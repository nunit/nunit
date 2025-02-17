// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions
{
    internal class TestHookIMethodEventArgs : EventArgs
    {
        public TestHookIMethodEventArgs(TestExecutionContext context, IMethodInfo method)
        {
            Context = context;
            Method = method;
        }

        public TestExecutionContext Context { get; }
        public IMethodInfo Method { get; }
    }
}
