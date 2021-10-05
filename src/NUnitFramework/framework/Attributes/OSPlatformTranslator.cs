// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific platform.
    /// </summary>
    internal static class OSPlatformTranslator
    {
        /// <summary>
        /// Converts one or more .NET 5+ OSPlatformAttributes into a single NUnit PlatformAttribute
        /// </summary>
        /// <param name="allAttributes">Enumeration of all attributes.</param>
        /// <returns>All attributes with OSPlatformAttributes translated into PlatformAttributes.</returns>
        public static IEnumerable<object> Translate(IEnumerable<object> allAttributes)
        {
            var includes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var excludes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var attribute in allAttributes)
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

                    continue;
                }

                Type type = attribute.GetType();

                bool include = type.FullName == "System.Runtime.Versioning.SupportedOSPlatformAttribute";
                bool exclude = type.FullName == "System.Runtime.Versioning.UnsupportedOSPlatformAttribute";

                if (!include && !exclude)
                {
                    // Not a translatable attribute, keep it.
                    yield return attribute;
                    continue;
                }

                PropertyInfo? platformNameProperty = type.GetProperty("PlatformName", typeof(string));
                string? platformName = (string?)platformNameProperty?.GetValue(attribute);
                if (platformNameProperty is null || platformName is null)
                {
                    yield return attribute;
                    continue;
                }

                string nunitPlatform = Translate(platformName);

                if (include)
                {
                    includes.Add(nunitPlatform);
                }
                else
                {
                    excludes.Add(nunitPlatform);
                }
            }

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
