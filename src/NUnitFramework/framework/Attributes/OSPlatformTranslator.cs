// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific platform.
    /// </summary>
    public static class OSPlatformTranslator
    {
        /// <summary>
        /// Converts a .NET 5+ SupportedOSPlatformAttribute into an NUnit PlatformAttribute
        /// </summary>
        /// <param name="possibleSupportedOSPlatformAttribute"></param>
        /// <returns></returns>
        public static object Translate(object possibleSupportedOSPlatformAttribute)
        {
            Type type = possibleSupportedOSPlatformAttribute.GetType();

            bool include = type.FullName == "System.Runtime.Versioning.SupportedOSPlatformAttribute";
            bool exclude = type.FullName == "System.Runtime.Versioning.UnsupportedOSPlatformAttribute";

            if (!include && !exclude)
            {
                return possibleSupportedOSPlatformAttribute;
            }

            PropertyInfo? platformNameProperty = type.GetProperty("PlatformName", typeof(string));
            if (platformNameProperty is null)
            {
                return possibleSupportedOSPlatformAttribute;
            }

            string platformName = (string)platformNameProperty.GetValue(possibleSupportedOSPlatformAttribute);

            string nunitPlatform = Translate(platformName);

            if (include)
            {
                return new PlatformAttribute
                {
                    Include = nunitPlatform,
                };
            }
            else
            {
                return new PlatformAttribute
                {
                    Exclude = nunitPlatform,
                };
            }
        }

        private static string Translate(string platformName)
        {
            ParseOSAndVersion(platformName, out string os, out Version version);
            string nunit = Translate(os, version);

            return nunit;
        }
        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static void ParseOSAndVersion(string plaformName,
            out string os, out Version version)
        {
            version = new Version(0, 0);

            int versionIndex = plaformName.IndexOfAny(Digits);
            if (versionIndex > 0)
            {
                os = plaformName.Substring(0, versionIndex);
                if (Version.TryParse(plaformName.Substring(versionIndex), out Version result))
                {
                    version = result;
                }
            }
            else
            {
                os = plaformName;
            }
        }

        private static string Translate(string osName, Version version)
        {
            switch (osName.ToUpperInvariant())
            {
                case "WINDOWS":
                    if (version.Major < 7)
                    {
                        return "Win";
                    }
                    else
                    {
                        return "Windows" + version.Major;
                    }
                case "OSX":
                case "MACOS":
                    return "MacOsX";
                case "LINUX":
                    return "Linux";
                default:
                    return osName;  // It might or more likely is not support by NUnit.
            }
        }
    }
}
