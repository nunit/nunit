// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
