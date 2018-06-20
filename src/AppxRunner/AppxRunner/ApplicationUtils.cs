using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AppxRunner
{
    internal static class ApplicationUtils
    {
        /// <summary>
        /// Launches the specified application and returns the launched process ID.
        /// </summary>
        public static int Launch(string familyName, string appId, string args)
        {
            return new ApplicationActivationManager().ActivateApplication(familyName + '!' + appId, args, AO.NONE);
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedMember.Global

        [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
        private sealed class ApplicationActivationManager : IApplicationActivationManager
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern int ActivateApplication(string appUserModelId, string activationContext, AO options);
        }

        [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IApplicationActivationManager
        {
            int ActivateApplication(string appUserModelId, string activationContext, AO options);
        }

        [Flags]
        private enum AO : uint
        {
            NONE = 0,
            DESIGNMODE = 1 << 0,
            NOERRORUI = 1 << 1,
            NOSPLASHSCREEN = 1 << 2,
            PRELAUNCH = 1 << 25
        }
    }
}
