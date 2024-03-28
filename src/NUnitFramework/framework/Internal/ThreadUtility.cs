// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ThreadUtility provides a set of static methods convenient
    /// for working with threads.
    /// </summary>
    public static partial class ThreadUtility
    {
        internal static void BlockingDelay(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>Gets <see cref="Thread.CurrentPrincipal"/> or <see langword="null" /> if the current platform does not support it.</summary>
        public static System.Security.Principal.IPrincipal? GetCurrentThreadPrincipal()
        {
            try
            {
                return Thread.CurrentPrincipal;
            }
            catch (PlatformNotSupportedException) //E.g. Mono.WASM
            {
                return null;
            }
        }

        /// <summary>Sets <see cref="Thread.CurrentPrincipal"/> if current platform supports it.</summary>
        /// <param name="principal">Value to set. If the current platform does not support <see cref="Thread.CurrentPrincipal"/> then the only allowed value is <see langword="null"/>.</param>
        public static void SetCurrentThreadPrincipal(System.Security.Principal.IPrincipal? principal)
        {
            try
            {
                Thread.CurrentPrincipal = principal;
            }
            catch (PlatformNotSupportedException) when (principal is null) //E.g. Mono.WASM
            {
            }
        }
    }
}
