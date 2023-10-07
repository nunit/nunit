// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System;
using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// This class ensures the <see cref="System.Runtime.Remoting.Messaging.CallContext"/> is correctly cleared
    /// or restored around framework actions. This is important if running multiple assemblies within the same
    /// process, to ensure no leakage from one assembly to the next. See https://github.com/nunit/nunit-console/issues/325
    /// </summary>
    internal class NUnitCallContext : IDisposable
    {
        private readonly object _oldContext;
        public const string TestExecutionContextKey = "NUnit.Framework.TestExecutionContext";

        public NUnitCallContext()
        {
            _oldContext = CallContext.GetData(TestExecutionContextKey);
        }

        public void Dispose()
        {
            if (_oldContext is null)
                CallContext.FreeNamedDataSlot(TestExecutionContextKey);
            else
                CallContext.SetData(TestExecutionContextKey, _oldContext);
        }
    }
}
#endif
