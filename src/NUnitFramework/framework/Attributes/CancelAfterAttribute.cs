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
    /// <para>
    /// The user has to monitor this cancellation token.
    /// </para>
    /// <para>
    /// When applied to a class, the timeout applies to each test in the fixture and also,
    /// separately, to all <see cref="OneTimeSetUpAttribute"/> and all <see cref="OneTimeTearDownAttribute"/>
    /// methods of the fixture. Each of these phases gets its own cancellation token.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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
