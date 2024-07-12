// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific platform.
    /// </summary>
    /// <remarks>
    /// This class is a replacement for the <see cref="PlatformAttribute"/> class,
    /// the platform names are based on the values in the TargetFramrwork.
    /// See: https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1416
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class NetPlatformAttribute : IncludeExcludeAttribute, IApplyToTest
    {
        /// <summary>
        /// Constructor with no platforms specified, for use
        /// with named property syntax.
        /// </summary>
        public NetPlatformAttribute()
        {
        }

        /// <summary>
        /// Constructor taking one or more platforms
        /// </summary>
        /// <param name="platforms">Comma-delimited list of platforms</param>
        public NetPlatformAttribute(string? platforms)
            : base(platforms)
        {
        }

        #region IApplyToTest members

        /// <summary>
        /// Causes a test to be skipped if this NewPlatformAttribute is not satisfied.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable &&
                test.RunState != RunState.Ignored)
            {
                bool platformIsSupported = IsPlatformSupported(Include, Exclude, out string reason);

                if (!platformIsSupported)
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, reason);
                }
            }
        }

        private static bool IsPlatformSupported(string? include, string? exclude, out string reason)
        {
            if (include is not null && !NetPlatformHelper.IsPlatformSupported(include))
            {
                reason = $"Only supported on {include}";
                return false;
            }

            if (exclude is not null && NetPlatformHelper.IsPlatformSupported(exclude))
            {
                reason = $"Not supported on {exclude}";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        #endregion
    }
}
