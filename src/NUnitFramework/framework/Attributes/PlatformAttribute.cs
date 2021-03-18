// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks an assembly, test fixture or test method as applying to a specific platform.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true, Inherited=false)]
    public class PlatformAttribute : IncludeExcludeAttribute, IApplyToTest
    {
        private readonly PlatformHelper platformHelper = new PlatformHelper();

        /// <summary>
        /// Constructor with no platforms specified, for use
        /// with named property syntax.
        /// </summary>
        public PlatformAttribute() { }

        /// <summary>
        /// Constructor taking one or more platforms
        /// </summary>
        /// <param name="platforms">Comma-delimited list of platforms</param>
        public PlatformAttribute(string? platforms) : base(platforms) { }

        #region IApplyToTest members

        /// <summary>
        /// Causes a test to be skipped if this PlatformAttribute is not satisfied.
        /// </summary>
        /// <param name="test">The test to modify</param>
        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable &&
                test.RunState != RunState.Ignored)
            {
                bool platformIsSupported = false;
                try
                {
                    platformIsSupported = platformHelper.IsPlatformSupported(this);
                }
                catch (InvalidPlatformException ex)
                {
                    test.RunState = RunState.NotRunnable;
                    test.Properties.Add(PropertyNames.SkipReason, ex.Message);
                    return;
                }

                if (!platformIsSupported)
                {
                    test.RunState = RunState.Skipped;
                    test.Properties.Add(PropertyNames.SkipReason, platformHelper.Reason);
                }
            }
        }

        #endregion
    }
}
