// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Applies a timeout in milliseconds to a test.
    /// When applied to a method, the test's cancellation token is cancelled if the timeout is exceeded.
    /// </summary>
    /// <remarks>
    /// The user has to monitor this cancellation token.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CancelAfterAttribute : PropertyAttribute, IApplyToContext
    {
        private readonly int _timeout;

        /// <summary>
        /// Construct a CancelAfterAttribute given a time in milliseconds
        /// </summary>
        /// <param name="timeout">The timeout value in milliseconds</param>
        public CancelAfterAttribute(int timeout)
            : base(PropertyNames.Timeout, timeout)
        {
            _timeout = timeout;
            Properties.Add(PropertyNames.UseCancellation, true);
        }

        #region IApplyToContext

        void IApplyToContext.ApplyToContext(TestExecutionContext context)
        {
            context.TestCaseTimeout = _timeout;
            context.UseCancellation = true;
        }

        #endregion
    }
}
