// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// TestCaseTimeoutException is thrown when a test running directly
    /// on a TestWorker thread is cancelled due to timeout.
    /// </summary>
    [Serializable]
    public class TestCaseTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseTimeoutException"/> class.
        /// </summary>
        public TestCaseTimeoutException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestCaseTimeoutException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public TestCaseTimeoutException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected TestCaseTimeoutException(SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
