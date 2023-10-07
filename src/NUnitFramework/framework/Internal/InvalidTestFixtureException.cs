// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// InvalidTestFixtureException is thrown when an appropriate test
    /// fixture constructor using the provided arguments cannot be found.
    /// </summary>
    [Serializable]
    public class InvalidTestFixtureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTestFixtureException"/> class.
        /// </summary>
        public InvalidTestFixtureException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTestFixtureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidTestFixtureException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTestFixtureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidTestFixtureException(string message, Exception inner) : base(message, inner)
        { }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected InvalidTestFixtureException(SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}
