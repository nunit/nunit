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

using System.Globalization;
using System.Security;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Holds thread state which is captured and restored in order to sandbox user code.
    /// </summary>
    internal sealed class SandboxedThreadState
    {
        public CultureInfo Culture { get; }
        public CultureInfo UICulture { get; }

        /// <summary>
        /// Thread principal.
        /// This will be null on platforms that don't support <see cref="Thread.CurrentPrincipal"/>.
        /// </summary>
        public System.Security.Principal.IPrincipal Principal { get; }
        private readonly SynchronizationContext _synchronizationContext;

        private SandboxedThreadState(
            CultureInfo culture,
            CultureInfo uiCulture,
            System.Security.Principal.IPrincipal principal,
            SynchronizationContext synchronizationContext)
        {
            Culture = culture;
            UICulture = uiCulture;
            Principal = principal;
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Captures a snapshot of the tracked state of the current thread to be restored later.
        /// </summary>
        public static SandboxedThreadState Capture()
        {
            return new SandboxedThreadState(
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentUICulture,
                ThreadUtility.GetCurrentThreadPrincipal(),
                SynchronizationContext.Current);
        }

        /// <summary>
        /// Restores the tracked state of the current thread to the previously captured state.
        /// </summary>
        [SecurityCritical]
        public void Restore()
        {
            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = UICulture;
            ThreadUtility.SetCurrentThreadPrincipal(Principal);

            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }

        /// <summary>
        /// Returns a copy with the specified culture.
        /// </summary>
        public SandboxedThreadState WithCulture(CultureInfo culture)
        {
            return new SandboxedThreadState(
                culture,
                UICulture,
                Principal,
                _synchronizationContext);
        }

        /// <summary>
        /// Returns a copy with the specified UI culture.
        /// </summary>
        public SandboxedThreadState WithUICulture(CultureInfo uiCulture)
        {
            return new SandboxedThreadState(
                Culture,
                uiCulture,
                Principal,
                _synchronizationContext);
        }

        /// <summary>
        /// Returns a copy with the specified principal.
        /// </summary>
        public SandboxedThreadState WithPrincipal(System.Security.Principal.IPrincipal principal)
        {
            return new SandboxedThreadState(
                Culture,
                UICulture,
                principal,
                _synchronizationContext);
        }
    }
}
