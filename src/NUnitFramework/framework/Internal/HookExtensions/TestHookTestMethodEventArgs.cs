// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.HookExtensions
{
    internal class TestHookTestMethodEventArgs : EventArgs
    {
        public TestHookTestMethodEventArgs(TestExecutionContext context, TestMethod testMethod)
        {
            Context = context;
            TestMethod = testMethod;
        }

        public TestExecutionContext Context { get; }
        public TestMethod TestMethod { get; }
    }
}
