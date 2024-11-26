// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    using System;
#if !NET8_0_OR_GREATER
    using System.Runtime.Serialization;
#endif

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
        public InvalidTestFixtureException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTestFixtureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidTestFixtureException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTestFixtureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidTestFixtureException(string message, Exception inner) : base(message, inner)
        {
        }

#if !NET8_0_OR_GREATER
        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected InvalidTestFixtureException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}
