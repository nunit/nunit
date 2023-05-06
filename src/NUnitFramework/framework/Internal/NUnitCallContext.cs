// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System;
using System.Runtime.Remoting.Messaging;
using System.Security;

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

        // This method invokes security critical members on the 'System.Runtime.Remoting.Messaging.CallContext' class.
        // Callers of this method have no influence on how these methods are used so we define a 'SecuritySafeCriticalAttribute'
        // rather than a 'SecurityCriticalAttribute' to enable use by security transparent callers.
        [SecuritySafeCritical]
        public NUnitCallContext()
        {
            _oldContext = CallContext.GetData(TestExecutionContextKey);
        }

        [SecuritySafeCritical]
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
