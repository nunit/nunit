// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
    /// <summary>
    /// <para>
    /// Canonical platform names for use with <see cref="PlatformAttribute"/>. Note that
    /// the <c>platforms</c> argument to <see cref="PlatformAttribute"/> supports
    /// syntax beyond just naming platforms, such as comma-delimited lists, but using
    /// these comments ensures that the precise name of a platform cannot be mistyped
    /// or misspelled.
    /// </para>
    /// <para>
    /// You can use these constants in conjunction with extended syntax by building
    /// the desired expression using string interpolation:
    /// </para>
    /// <code>
    /// [Platform(PlatformNames.Windows)]
    /// [Platform(PlatformNames.Linux)]
    ///
    /// // or
    ///
    /// [Platform($"{PlatformNames.Windows},{PlatformNames.Linux}")]
    /// </code>
    /// </summary>
    public static class PlatformNames
    {
        /// <summary>
        /// Platform: "Win"
        /// </summary>
        public const string Win = "Win";
        /// <summary>
        /// Platform: "Win32"
        /// </summary>
        public const string Win32 = "Win32";
        /// <summary>
        /// Platform: "Win32s"
        /// </summary>
        public const string Win32s = "Win32s";
        /// <summary>
        /// Platform: "Win32Windows"
        /// </summary>
        public const string Win32Windows = "Win32Windows";
        /// <summary>
        /// Platform: "Win32NT"
        /// </summary>
        public const string Win32NT = "Win32NT";
        /// <summary>
        /// Platform: "Win95"
        /// </summary>
        public const string Win95 = "Win95";
        /// <summary>
        /// Platform: "Win98"
        /// </summary>
        public const string Win98 = "Win98";
        /// <summary>
        /// Platform: "WinME"
        /// </summary>
        public const string WinME = "WinME";
        /// <summary>
        /// Platform: "NT3"
        /// </summary>
        public const string NT3 = "NT3";
        /// <summary>
        /// Platform: "NT4"
        /// </summary>
        public const string NT4 = "NT4";
        /// <summary>
        /// Platform: "NT5"
        /// </summary>
        public const string NT5 = "NT5";
        /// <summary>
        /// Platform: "Win2K"
        /// </summary>
        public const string Win2K = "Win2K";
        /// <summary>
        /// Platform: "WinXP"
        /// </summary>
        public const string WinXP = "WinXP";
        /// <summary>
        /// Platform: "Win2003Server"
        /// </summary>
        public const string Win2003Server = "Win2003Server";
        /// <summary>
        /// Platform: "NT6"
        /// </summary>
        public const string NT6 = "NT6";
        /// <summary>
        /// Platform: "Vista"
        /// </summary>
        public const string Vista = "Vista";
        /// <summary>
        /// Platform: "Win2008Server"
        /// </summary>
        public const string Win2008Server = "Win2008Server";
        /// <summary>
        /// Platform: "Win2012Server"
        /// </summary>
        public const string Win2012Server = "Win2012Server";
        /// <summary>
        /// Platform: "Win2012ServerR2"
        /// </summary>
        public const string Win2012ServerR2 = "Win2012ServerR2";
        /// <summary>
        /// Platform: "Win7"
        /// </summary>
        public const string Win7 = "Win7";
        /// <summary>
        /// Platform: "Windows7"
        /// </summary>
        public const string Windows7 = "Windows7";
        /// <summary>
        /// Platform: "Win8"
        /// </summary>
        public const string Win8 = "Win8";
        /// <summary>
        /// Platform: "Windows8"
        /// </summary>
        public const string Windows8 = "Windows8";
        /// <summary>
        /// Platform: "Win8.1"
        /// </summary>
        public const string Win81 = "Win8.1";
        /// <summary>
        /// Platform: "Windows8.1"
        /// </summary>
        public const string Windows81 = "Windows8.1";
        /// <summary>
        /// Platform: "Win10"
        /// </summary>
        public const string Win10 = "Win10";
        /// <summary>
        /// Platform: "Windows10"
        /// </summary>
        public const string Windows10 = "Windows10";
        /// <summary>
        /// Platform: "Win11"
        /// </summary>
        public const string Win11 = "Win11";
        /// <summary>
        /// Platform: "Windows11"
        /// </summary>
        public const string Windows11 = "Windows11";
        /// <summary>
        /// Platform: "WindowsServer10"
        /// </summary>
        public const string WindowsServer10 = "WindowsServer10";
        /// <summary>
        /// Platform: "UNIX"
        /// </summary>
        public const string UNIX = "UNIX";
        /// <summary>
        /// Platform: "Linux"
        /// </summary>
        public const string Linux = "Linux";
        /// <summary>
        /// Platform: "XBox"
        /// </summary>
        public const string XBox = "XBox";
        /// <summary>
        /// Platform: "MacOSX"
        /// </summary>
        public const string MacOSX = "MacOSX";

        /// <summary>
        /// Platform: "64-bit"
        /// </summary>
        public const string _64Bit = "64-bit";
        /// <summary>
        /// Platform: "64-bit-process"
        /// </summary>
        public const string _64BitProcess = "64-bit-process";
        /// <summary>
        /// Platform: "32-bit"
        /// </summary>
        public const string _32Bit = "32-bit";
        /// <summary>
        /// Platform: "32-bit-process"
        /// </summary>
        public const string _32BitProcess = "32-bit-process";

        /// <summary>
        /// Platform: "64-bit-os"
        /// </summary>
        public const string _64BitOS = "64-bit-os";
        /// <summary>
        /// Platform: "32-bit-os"
        /// </summary>
        public const string _32BitOS = "32-bit-os";

        /// <summary>
        /// Platform: "NET"
        /// </summary>
        public const string DotNET = "NET";
        /// <summary>
        /// Platform: "NETCore"
        /// </summary>
        public const string DotNETCore = "NETCore";
        /// <summary>
        /// Platform: "SSCLI"
        /// </summary>
        public const string SSCLI = "SSCLI";
        /// <summary>
        /// Platform: "Rotor"
        /// </summary>
        public const string Rotor = "Rotor";
        /// <summary>
        /// Platform: "Mono"
        /// </summary>
        public const string Mono = "Mono";
        /// <summary>
        /// Platform: "MonoTouch"
        /// </summary>
        public const string MonoTouch = "MonoTouch";
    }
}
