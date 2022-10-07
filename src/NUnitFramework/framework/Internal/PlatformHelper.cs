// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
        private string _reason = string.Empty;

        const string CommonOSPlatforms =
            "Win,Win32,Win32S,Win32NT,Win32Windows,Win95,Win98,WinMe,NT3,NT4,NT5,NT6," +
            "Win2008Server,Win2008ServerR2,Win2012Server,Win2012ServerR2," +
            "Win2K,WinXP,Win2003Server,Vista,Win7,Windows7,Win8,Windows8,"+
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
        public PlatformHelper( OSPlatform os, RuntimeFramework rt )
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
        public bool IsPlatformSupported( string[] platforms )
        {
            return platforms.Any( IsPlatformSupported );
        }

        /// <summary>
        /// Tests to determine if the current platform is supported
        /// based on a platform attribute.
        /// </summary>
        /// <param name="platformAttribute">The attribute to examine</param>
        /// <returns></returns>
        public bool IsPlatformSupported( PlatformAttribute platformAttribute )
        {
            string include = platformAttribute.Include;
            string exclude = platformAttribute.Exclude;

            return IsPlatformSupported( include, exclude );
        }

        /// <summary>
        /// Tests to determine if the current platform is supported
        /// based on a platform attribute.
        /// </summary>
        /// <param name="testCaseAttribute">The attribute to examine</param>
        /// <returns></returns>
        public bool IsPlatformSupported(TestCaseAttribute testCaseAttribute)
        {
            string include = testCaseAttribute.IncludePlatform;
            string exclude = testCaseAttribute.ExcludePlatform;

            return IsPlatformSupported(include, exclude);
        }

        private bool IsPlatformSupported(string include, string exclude)
        {
            if (include != null && !IsPlatformSupported(include))
            {
                _reason = $"Only supported on {include}";
                return false;
            }

            if (exclude != null && IsPlatformSupported(exclude))
            {
                _reason = $"Not supported on {exclude}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Test to determine if a particular platform or comma-delimited set of platforms is in use.
        /// </summary>
        /// <param name="platform">Name of the platform or comma-separated list of platform ids</param>
        /// <returns>True if the platform is in use on the system</returns>
        public bool IsPlatformSupported( string platform )
        {
            if ( platform.IndexOf( ',' ) >= 0 )
                return IsPlatformSupported( platform.Split(',') );

            string platformName = platform.Trim();
            bool isSupported;

            switch( platformName.ToUpper() )
            {
                case "WIN":
                case "WIN32":
                    isSupported = _os.IsWindows;
                    break;
                case "WIN32S":
                    isSupported = _os.IsWin32S;
                    break;
                case "WIN32WINDOWS":
                    isSupported = _os.IsWin32Windows;
                    break;
                case "WIN32NT":
                    isSupported = _os.IsWin32NT;
                    break;
                case "WIN95":
                    isSupported = _os.IsWin95;
                    break;
                case "WIN98":
                    isSupported = _os.IsWin98;
                    break;
                case "WINME":
                    isSupported = _os.IsWinME;
                    break;
                case "NT3":
                    isSupported = _os.IsNT3;
                    break;
                case "NT4":
                    isSupported = _os.IsNT4;
                    break;
                case "NT5":
                    isSupported = _os.IsNT5;
                    break;
                case "WIN2K":
                    isSupported = _os.IsWin2K;
                    break;
                case "WINXP":
                    isSupported = _os.IsWinXP;
                    break;
                case "WIN2003SERVER":
                    isSupported = _os.IsWin2003Server;
                    break;
                case "NT6":
                    isSupported = _os.IsNT6;
                    break;
                case "VISTA":
                    isSupported = _os.IsVista;
                    break;
                case "WIN2008SERVER":
                    isSupported = _os.IsWin2008Server;
                    break;
                case "WIN2008SERVERR2":
                    isSupported = _os.IsWin2008ServerR2;
                    break;
                case "WIN2012SERVER":
                    isSupported = _os.IsWin2012ServerR1 || _os.IsWin2012ServerR2;
                    break;
                case "WIN2012SERVERR2":
                    isSupported = _os.IsWin2012ServerR2;
                    break;
                case "WIN7":
                case "WINDOWS7":
                    isSupported = _os.IsWindows7;
                    break;
                case "WINDOWS8":
                case "WIN8":
                    isSupported = _os.IsWindows8;
                    break;
                case "WINDOWS8.1":
                case "WIN8.1":
                    isSupported = _os.IsWindows81;
                    break;
                case "WINDOWS10":
                case "WIN10":
                    isSupported = _os.IsWindows10;
                    break;
                case "WINDOWS11":
                case "WIN11":
                    isSupported = _os.IsWindows11;
                    break;
                case "WINDOWSSERVER10":
                    isSupported = _os.IsWindowsServer10;
                    break;
                case "UNIX":
                case "LINUX":
                    isSupported = _os.IsUnix;
                    break;
                case "XBOX":
                    isSupported = _os.IsXbox;
                    break;
                case "MACOSX":
                    isSupported = _os.IsMacOSX;
                    break;

                // These bitness tests relate to the process, not the OS.
                case "64-BIT":
                case "64-BIT-PROCESS":
                    isSupported = Environment.Is64BitProcess;
                    break;
                case "32-BIT":
                case "32-BIT-PROCESS":
                    isSupported = !Environment.Is64BitProcess;
                    break;

                case "64-BIT-OS":
                    isSupported = Environment.Is64BitOperatingSystem;
                    break;
                case "32-BIT-OS":
                    isSupported = !Environment.Is64BitOperatingSystem;
                    break;

                default:
                    isSupported = IsRuntimeSupported(platformName);
                    break;
            }

            if (!isSupported)
                _reason = "Only supported on " + platform;

            return isSupported;
        }

        /// <summary>
        /// Return the last failure reason. Results are not
        /// defined if called before IsSupported( Attribute )
        /// is called.
        /// </summary>
        public string Reason
        {
            get { return _reason; }
        }

        private bool IsRuntimeSupported(string platformName)
        {
            string versionSpecification = null;
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

        private bool IsRuntimeSupported(RuntimeType runtime, string versionSpecification)
        {
            Version version = versionSpecification == null
                ? RuntimeFramework.DefaultVersion
                : new Version(versionSpecification);

            RuntimeFramework target = new RuntimeFramework(runtime, version);

            return _rt.Supports(target);
        }

        private bool IsNetCoreRuntimeSupported(RuntimeType runtime, string versionSpecification)
        {
            if (versionSpecification != null)
            {
                throw new PlatformNotSupportedException($"Detecting versions of .NET Core is not supported - {runtime.ToString()}-{versionSpecification}");
            }

            RuntimeFramework target = new RuntimeFramework(runtime, RuntimeFramework.DefaultVersion);

            return _rt.Supports(target);
        }
    }
}
