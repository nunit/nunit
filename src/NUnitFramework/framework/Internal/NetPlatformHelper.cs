// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Class is used by the <see cref="NetPlatformAttribute"/> class to
    /// determine whether a platform is supported.
    /// </summary>
    /// <remarks>
    /// This replaces the <see cref="PlatformHelper"/>.
    /// </remarks>
    public static class NetPlatformHelper
    {
        /// <summary>
        /// Test to determine if one of a collection of platforms
        /// is being used currently.
        /// </summary>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public static bool IsPlatformSupported(string[] platforms)
        {
            return platforms.Any(IsPlatformSupported);
        }

        /// <summary>
        /// Test to determine if a particular platform or comma-delimited set of platforms is in use.
        /// </summary>
        /// <param name="platform">Name of the platform or comma-separated list of platform ids</param>
        /// <returns>True if the platform is in use on the system</returns>
        public static bool IsPlatformSupported(string platform)
        {
            if (platform.Contains(','))
                return IsPlatformSupported(platform.Split(','));

            string platformName = platform.Trim();
            ParseOSAndVersion(platformName, out string os, out Version? version);

            if (version is not null && os.Equals("Windows", StringComparison.OrdinalIgnoreCase))
            {
                // Actual versions don't match public known names
                // https://github.com/dotnet/sdk/issues/37220
                if (version.Major == 7)
                {
                    version = new Version(6, 1);
                }
                else if (version.Major == 8)
                {
                    version = new Version(6, version.Minor == 1 ? 3 : 2);
                }
            }

            return version is null ?
                OperatingSystem.IsOSPlatform(os) :
                OperatingSystem.IsOSPlatformVersionAtLeast(os, version.Major, version.Minor, version.Build, version.MajorRevision);
        }

        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static void ParseOSAndVersion(string plaformName,
            out string os, out Version? version)
        {
            int versionIndex = plaformName.IndexOfAny(Digits);
            if (versionIndex > 0)
            {
                os = plaformName.Substring(0, versionIndex);
                Version.TryParse(plaformName.Substring(versionIndex), out version);
            }
            else
            {
                os = plaformName;
                version = null;
            }
        }

#if !NET6_0_OR_GREATER
        private static class OperatingSystem
        {
            public static bool IsOSPlatform(string platform)
            {
                // https://github.com/dotnet/sdk/blob/main/src/Tasks/Microsoft.NET.Build.Tasks/targets/Microsoft.NET.SupportedPlatforms.props
                if (platform.Equals("Windows", StringComparison.OrdinalIgnoreCase))
                    return Environment.OSVersion.Platform == PlatformID.Win32NT;
                else if (platform.Equals("Linux", StringComparison.OrdinalIgnoreCase))
                    return Environment.OSVersion.Platform == PlatformID.Unix;
                else if (platform.Equals("macOS", StringComparison.OrdinalIgnoreCase))
                    return Environment.OSVersion.Platform == PlatformID.MacOSX;
                else
                    return false;
            }

            public static bool IsOSPlatformVersionAtLeast(string os, int major, int minor, int build, int revision)
            {
                return IsOSPlatform(os) && IsOSVersionAtLeast(major, minor, build, revision);
            }

            private static bool IsOSVersionAtLeast(int major, int minor, int build, int revision)
            {
                Version current = Environment.OSVersion.Version;

                if (current.Major != major)
                {
                    return current.Major > major;
                }
                if (current.Minor != minor)
                {
                    return current.Minor > minor;
                }
                if (current.Build != build)
                {
                    return current.Build > build;
                }

                return current.Revision >= revision
                    || (current.Revision == -1 && revision == 0); // it is unavailable on OSX and Environment.OSVersion.Version.Revision returns -1
            }
        }
#endif
    }
}
