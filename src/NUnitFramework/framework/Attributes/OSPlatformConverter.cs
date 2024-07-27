// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework
{
    /// <summary>
    /// Converts .NET6+ <see cref="OSPlatformAttribute"/> to <see cref="NetPlatformAttribute"/>
    /// which can be applied to tests.
    /// </summary>
    internal static class OSPlatformConverter
    {
        /// <summary>
        /// Converts one or more .NET 5+ <see cref="OSPlatformAttribute"/>s into a single NUnit <see cref="NetPlatformAttribute"/>.
        /// </summary>
        /// <remarks>
        /// Other <see cref="IApplyToTest"/> attributes are passed through unconverted.
        /// </remarks>
        /// <param name="provider">The type we want the attributes from.</param>
        /// <returns>All attributes with <see cref="OSPlatformAttribute"/>s translated into <see cref="NetPlatformAttribute"/>.</returns>
        public static IEnumerable<IApplyToTest> RetrieveAndConvert(this ICustomAttributeProvider provider)
        {
            IApplyToTest[] applyToTestAttributes = provider.GetAttributes<IApplyToTest>(inherit: true);

            OSPlatformAttribute[] osPlatformAttributes = provider.GetAttributes<OSPlatformAttribute>(inherit: true);

            return Convert(osPlatformAttributes, applyToTestAttributes);
        }

        internal static IEnumerable<IApplyToTest> Convert(OSPlatformAttribute[] osPlatformAttributes, IApplyToTest[] applyToTestAttributes)
        {
            var includes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var excludes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Filter out the NetPlatform Attributes
            foreach (var attribute in applyToTestAttributes)
            {
                if (attribute is NetPlatformAttribute netPlatformAttribute)
                {
                    Add(includes, netPlatformAttribute.Include);
                    Add(excludes, netPlatformAttribute.Exclude);

                    static void Add(HashSet<string> set, string? platforms)
                    {
                        if (platforms is not null)
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
                string platformName = osPlatformAttribute.PlatformName;

                if (osPlatformAttribute is SupportedOSPlatformAttribute)
                {
                    includes.Add(platformName);
                }
                else if (osPlatformAttribute is UnsupportedOSPlatformAttribute)
                {
                    excludes.Add(platformName);
                }

                // Ignore others, e.g. SupportedOSPlatformGuard
            }

            // Compile all platform attributes into a single one
            if (includes.Count > 0 || excludes.Count > 0)
            {
                yield return new NetPlatformAttribute
                {
                    Include = includes.Count == 0 ? null : string.Join(",", includes),
                    Exclude = excludes.Count == 0 ? null : string.Join(",", excludes),
                };
            }
        }
    }
}

#endif
