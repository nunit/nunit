// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if NET35 || NET40 || NET45
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
            if (_oldContext == null)
                CallContext.FreeNamedDataSlot(TestExecutionContextKey);
            else
                CallContext.SetData(TestExecutionContextKey, _oldContext);
        }
    }
}
#endif
