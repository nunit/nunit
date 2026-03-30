// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// PlatformHelper class is used by the PlatformAttribute class to
    /// determine whether a platform is supported.
    /// </summary>
    public class PlatformHelper
    {
        private readonly OSPlatform _os;
        private readonly RuntimeFramework _rt;

        // Set whenever we fail to support a list of platforms

        private const string CommonOSPlatforms =
            "Win,Win32,Win32S,Win32NT,Win32Windows,Win95,Win98,WinMe,NT3,NT4,NT5,NT6," +
            "Win2008Server,Win2008ServerR2,Win2012Server,Win2012ServerR2," +
            "Win2K,WinXP,Win2003Server,Vista,Win7,Windows7,Win8,Windows8," +
            "Win8.1,Windows8.1,Win10,Windows10,Win11,Windows11,WindowsServer10,Unix,Linux";

        /// <summary>
        /// Comma-delimited list of all supported OS platform constants
        /// </summary>
        public const string OSPlatforms = CommonOSPlatforms + ",Xbox,MacOSX";

        /// <summary>
        /// Comma-delimited list of all supported Runtime platform constants
        /// </summary>
        public static readonly string RuntimePlatforms =
            "Net,NetCore,SSCLI,Rotor,Mono,MonoTouch";

        /// <summary>
        /// Default constructor uses the operating system and
        /// common language runtime of the system.
        /// </summary>
        public PlatformHelper()
        {
            _os = OSPlatform.CurrentPlatform;
            _rt = RuntimeFramework.CurrentFramework;
        }

        /// <summary>
        /// Construct a PlatformHelper for a particular operating
        /// system and common language runtime. Used in testing.
        /// </summary>
        /// <param name="rt">RuntimeFramework to be used</param>
        /// <param name="os">OperatingSystem to be used</param>
        public PlatformHelper(OSPlatform os, RuntimeFramework rt)
        {
            _os = os;
            _rt = rt;
        }

        /// <summary>
        /// Test to determine if one of a collection of platforms
        /// is being used currently.
        /// </summary>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool IsPlatformSupported(string[] platforms)
        {
            if (platforms.Length == 0)
                return false;
            return platforms.Any(IsPlatformSupported);
        }

        /// <summary>
        /// Tests to determine if the current platform is supported
        /// based on a platform attribute.
        /// </summary>
        /// <param name="platformAttribute">The attribute to examine</param>
        /// <returns></returns>
        public bool IsPlatformSupported(PlatformAttribute platformAttribute)
        {
            if (platformAttribute.Includes.Length > 0 && !IsPlatformSupported(platformAttribute.Includes))
            {
                Reason = $"Only supported on {platformAttribute.Include}";
                return false;
            }

            if (platformAttribute.Excludes.Length > 0 && IsPlatformSupported(platformAttribute.Excludes))
            {
                Reason = $"Not supported on {platformAttribute.Exclude}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests to determine if the current platform is supported
        /// based on a platform attribute.
        /// </summary>
        /// <param name="testCaseAttribute">The attribute to examine</param>
        /// <returns></returns>
        public bool IsPlatformSupported(TestCaseAttribute testCaseAttribute)
        {
            string? include = testCaseAttribute.IncludePlatform;
            string? exclude = testCaseAttribute.ExcludePlatform;

            return IsPlatformSupported(include, exclude);
        }

        private bool IsPlatformSupported(string? include, string? exclude)
        {
            if (include is not null && !IsPlatformSupported(include))
            {
                Reason = $"Only supported on {include}";
                return false;
            }

            if (exclude is not null && IsPlatformSupported(exclude))
            {
                Reason = $"Not supported on {exclude}";
                return false;
            }

            return true;
        }

        private static readonly Dictionary<string, Func<OSPlatform, bool>> PlatformChecks =
            new Dictionary<string, Func<OSPlatform, bool>>(StringComparer.OrdinalIgnoreCase)
            {
                { PlatformNames.Win, os => os.IsWindows },
                { PlatformNames.Win32, os => os.IsWindows },
                //
                { PlatformNames.Win32s, os => os.IsWin32S },
                //
                { PlatformNames.Win32Windows, os => os.IsWin32Windows },
                //
                { PlatformNames.Win32NT, os => os.IsWin32NT },
                //
                { PlatformNames.Win95, os => os.IsWin95 },
                //
                { PlatformNames.Win98, os => os.IsWin98 },
                //
                { PlatformNames.WinME, os => os.IsWinME },
                //
                { PlatformNames.NT3, os => os.IsNT3 },
                { PlatformNames.NT4, os => os.IsNT4 },
                { PlatformNames.NT5, os => os.IsNT5 },
                { PlatformNames.NT6, os => os.IsNT6 },
                //
                { PlatformNames.Win2K, os => os.IsWin2K },
                //
                { PlatformNames.WinXP, os => os.IsWinXP },
                //
                { PlatformNames.Vista, os => os.IsVista },
                //
                { PlatformNames.Win2003Server, os => os.IsWin2003Server },
                { PlatformNames.Win2008Server, os => os.IsWin2008Server },
                { PlatformNames.Win2008ServerR2, os => os.IsWin2008ServerR2 },
                { PlatformNames.Win2012Server, os => os.IsWin2012Server },
                { PlatformNames.Win2012ServerR2, os => os.IsWin2012ServerR2 },
                //
                { PlatformNames.Win7, os => os.IsWindows7 },
                { PlatformNames.Windows7, os => os.IsWindows7 },
                //
                { PlatformNames.Win8, os => os.IsWindows8 },
                { PlatformNames.Windows8, os => os.IsWindows8 },
                //
                { PlatformNames.Win81, os => os.IsWindows81 },
                { PlatformNames.Windows81, os => os.IsWindows81 },
                //
                { PlatformNames.Win10, os => os.IsWindows10 },
                { PlatformNames.Windows10, os => os.IsWindows10 },
                //
                { PlatformNames.Win11, os => os.IsWindows11 },
                { PlatformNames.Windows11, os => os.IsWindows11 },
                //
                { PlatformNames.WindowsServer10, os => os.IsWindowsServer10 },
                //
                { PlatformNames.UNIX, os => os.IsUnix },
                { PlatformNames.Linux, os => os.IsUnix },
                //
                { PlatformNames.XBox, os => os.IsXbox },
                //
                { PlatformNames.MacOSX, os => os.IsMacOSX },
                // These bitness tests relate to the process, not the OS
                { PlatformNames.X64Bit, _ => Environment.Is64BitProcess },
                { PlatformNames.X64BitProcess, _ => Environment.Is64BitProcess },
                { PlatformNames.X32Bit, _ => !Environment.Is64BitProcess },
                { PlatformNames.X32BitProcess, _ => !Environment.Is64BitProcess },
                //
                { PlatformNames.X64BitOS, _ => Environment.Is64BitOperatingSystem },
                { PlatformNames.X32BitOS, _ => !Environment.Is64BitOperatingSystem },
            };

        /// <summary>
        /// Test to determine if a particular platform or comma-delimited set of platforms is in use.
        /// </summary>
        /// <param name="platform">Name of the platform or comma-separated list of platform ids</param>
        /// <returns>True if the platform is in use on the system</returns>
        public bool IsPlatformSupported(string platform)
        {
            if (platform.IndexOf(',') >= 0)
                return IsPlatformSupported(platform.Split(','));

            string platformName = platform.Trim();
            bool isSupported;

            if (PlatformChecks.TryGetValue(platformName, out var check))
            {
                isSupported = check(_os);
            }
            else
            {
                isSupported = IsRuntimeSupported(platformName);
            }

            if (!isSupported)
                Reason = "Only supported on " + platform;

            return isSupported;
        }

        /// <summary>
        /// Return the last failure reason. Results are not
        /// defined if called before IsSupported( Attribute )
        /// is called.
        /// </summary>
        public string Reason { get; private set; } = string.Empty;

        private bool IsRuntimeSupported(string platformName)
        {
            string? versionSpecification = null;
            string[] parts = platformName.Split('-');
            if (parts.Length == 2)
            {
                platformName = parts[0];
                versionSpecification = parts[1];
            }

            switch (platformName.ToUpper())
            {
                case "NET":
                    return IsRuntimeSupported(RuntimeType.NetFramework, versionSpecification);

                case "NETCORE":
                    return IsNetCoreRuntimeSupported(RuntimeType.NetCore, versionSpecification);

                case "SSCLI":
                case "ROTOR":
                    return IsRuntimeSupported(RuntimeType.SSCLI, versionSpecification);

                case "MONO":
                    return IsRuntimeSupported(RuntimeType.Mono, versionSpecification);

                case "MONOTOUCH":
                    return IsRuntimeSupported(RuntimeType.MonoTouch, versionSpecification);

                default:
                    throw new InvalidPlatformException("Invalid platform name: " + platformName);
            }
        }

        private bool IsRuntimeSupported(RuntimeType runtime, string? versionSpecification)
        {
            Version version = versionSpecification is null
                ? RuntimeFramework.DefaultVersion
                : new Version(versionSpecification);

            var target = new RuntimeFramework(runtime, version);

            return _rt.Supports(target);
        }

        private bool IsNetCoreRuntimeSupported(RuntimeType runtime, string? versionSpecification)
        {
            if (versionSpecification is not null)
            {
                throw new PlatformNotSupportedException($"Detecting versions of .NET Core is not supported - {runtime}-{versionSpecification}");
            }

            var target = new RuntimeFramework(runtime, RuntimeFramework.DefaultVersion);

            return _rt.Supports(target);
        }
    }
}
