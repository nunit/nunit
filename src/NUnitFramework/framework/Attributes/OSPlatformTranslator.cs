// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific platform.
    /// </summary>
    internal static class OSPlatformTranslator
    {
        private static readonly Type? _osPlatformAttributeType = Type.GetType("System.Runtime.Versioning.OSPlatformAttribute, System.Runtime", false);
        private static readonly PropertyInfo? _platformNameProperty = _osPlatformAttributeType?.GetProperty("PlatformName", typeof(string));

        /// <summary>
        /// Converts one or more .NET 5+ OSPlatformAttributes into a single NUnit PlatformAttribute
        /// </summary>
        /// <param name="provider">The type we want the attributes from.</param>
        /// <returns>All attributes with OSPlatformAttributes translated into PlatformAttributes.</returns>
        public static IEnumerable<IApplyToTest> RetrieveAndTranslate(this ICustomAttributeProvider provider)
        {
            IApplyToTest[] applyToTestAttributes = provider.GetAttributes<IApplyToTest>(inherit: true);

            // OSPlatformAttribute is only available on NET5.O or greater
            var osPlatformAttributes = _osPlatformAttributeType is null ?
                Array.Empty<Attribute>() :
                (Attribute[])provider.GetCustomAttributes(_osPlatformAttributeType, inherit: true);

            return Translate(osPlatformAttributes, applyToTestAttributes);
        }

        internal static IEnumerable<IApplyToTest> Translate(Attribute[] osPlatformAttributes, IEnumerable<IApplyToTest> applyToTestAttributes)
        {
            var includes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var excludes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Filter out the Platform Attributes
            foreach (var attribute in applyToTestAttributes)
            {
                if (attribute is PlatformAttribute platformAttribute)
                {
                    Add(includes, platformAttribute.Include);
                    Add(excludes, platformAttribute.Exclude);

                    static void Add(HashSet<string> set, string? platforms)
                    {
                        if (platforms != null)
                        {
                            set.UnionWith(platforms.Split(','));
                        }
                    }
                }
                else
                {
                    // Return others
                    yield return attribute;
                }
            }

            // Translate OSPlatformAttribute
            foreach (var osPlatformAttribute in osPlatformAttributes)
            {
                string? platformName = (string?)_platformNameProperty?.GetValue(osPlatformAttribute);
                if (platformName is null)
                {
                    // Invalid property, ignore
                    continue;
                }

                string nunitPlatform = Translate(platformName);

                Type type = osPlatformAttribute.GetType();

                if (type.FullName == "System.Runtime.Versioning.SupportedOSPlatformAttribute")
                {
                    includes.Add(nunitPlatform);
                }
                else if (type.FullName == "System.Runtime.Versioning.UnsupportedOSPlatformAttribute")
                {
                    excludes.Add(nunitPlatform);
                }

                // Ignore others, e.g. SupportedOSPlatformGuard
            }

            // Compile all platform attributes into a single one
            if (includes.Count > 0 || excludes.Count > 0)
            {
                yield return new PlatformAttribute
                {
                    Include = includes.Count == 0 ? null : string.Join(",", includes),
                    Exclude = excludes.Count == 0 ? null : string.Join(",", excludes),
                };
            }
        }

        internal static string Translate(string platformName)
        {
            ParseOSAndVersion(platformName, out string os, out int majorVersion);
            string nunit = Translate(os, majorVersion);

            return nunit;
        }

        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static void ParseOSAndVersion(string plaformName,
            out string os, out int majorVersion)
        {
            int versionIndex = plaformName.IndexOfAny(Digits);
            if (versionIndex > 0)
            {
                os = plaformName.Substring(0, versionIndex);
                int dotIndex = plaformName.IndexOf('.', versionIndex);
                int nextCharacter = dotIndex > versionIndex ? dotIndex : plaformName.Length;
                int length = nextCharacter - versionIndex;
                majorVersion = int.Parse(plaformName.Substring(versionIndex, length));
            }
            else
            {
                os = plaformName;
                majorVersion = 0;
            }
        }

        private static string Translate(string osName, int majorVersion)
        {
            switch (osName.ToUpperInvariant())
            {
                case "WINDOWS":
                    return majorVersion < 7 ? "Win" : "Windows" + majorVersion;
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
