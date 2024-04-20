// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    using Interfaces;

    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
    [Serializable]
    public class MultipleAssertException : ResultStateException
    {
        /// <summary>
        /// Construct based on the TestResult so far. This is the constructor
        /// used normally, when exiting the multiple assert block with failures.
        /// Not used internally but provided to facilitate debugging.
        /// </summary>
        /// <param name="testResult">
        /// The current result, up to this point. The result is not used
        /// internally by NUnit but is provided to facilitate debugging.
        /// </param>
        public MultipleAssertException(ITestResult testResult)
            : base(testResult.Message)
        {
            Guard.ArgumentNotNull(testResult, "testResult");
            TestResult = testResult;
        }

        /// <summary>
        /// Gets the <see cref="ResultState"/> provided by this exception.
        /// </summary>
        public override ResultState ResultState => ResultState.Failure;

        /// <summary>
        /// Gets the <see cref="ITestResult"/> of this test at the point the exception was thrown,
        /// </summary>
        public ITestResult TestResult { get; }
    }
}
