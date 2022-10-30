// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test fixture as requiring all child tests to be run on the 
    /// same thread as the OneTimeSetUp and OneTimeTearDown. A flag in the
    /// <see cref="TestExecutionContext"/> is set forcing all child tests 
    /// to be run sequentially on the current thread. 
    /// Any <see cref="ParallelScope"/> setting is ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class SingleThreadedAttribute : NUnitAttribute, IApplyToContext
    {
        #region IApplyToContext Members

        /// <summary>
        /// Apply changes to the TestExecutionContext
        /// </summary>
        /// <param name="context">The TestExecutionContext</param>
        public void ApplyToContext(TestExecutionContext context)
        {
            context.IsSingleThreaded = true;
        }

        #endregion
    }
}
