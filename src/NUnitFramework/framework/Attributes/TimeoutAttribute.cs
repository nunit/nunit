// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Applies a timeout in milliseconds to a test.
    /// When applied to a method, the test is cancelled if the timeout is exceeded.
    /// When applied to a class or assembly, the default timeout is set for all contained test methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
#if !NETFRAMEWORK
    [Obsolete(".NET No longer supports aborting threads as it is not a safe thing to do. Update your tests to use CancelAfterAttribute instead")]
#endif
    public class TimeoutAttribute : PropertyAttribute, IApplyToContext
    {
        private readonly int _timeout;

        /// <summary>
        /// Construct a TimeoutAttribute given a time in milliseconds
        /// </summary>
        /// <param name="timeout">The timeout value in milliseconds</param>
        public TimeoutAttribute(int timeout)
            : base(timeout)
        {
            _timeout = timeout;
        }

        #region IApplyToContext

        void IApplyToContext.ApplyToContext(TestExecutionContext context)
        {
            context.TestCaseTimeout = _timeout;
        }

        #endregion
    }
}
