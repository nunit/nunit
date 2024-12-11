// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InvalidPlatformException is thrown when the platform name supplied
    /// to a test is not recognized.
    /// </summary>
    [Serializable]
    internal class InvalidPlatformException : ArgumentException
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class.
        /// </summary>
        public InvalidPlatformException() : base()
        {
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidPlatformException(string message) : base(message)
        {
        }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidPlatformException(string message, Exception inner) : base(message, inner)
        {
        }

#if !NET8_0_OR_GREATER
        /// <summary>
        /// Serialization constructor for the <see cref="InvalidPlatformException"/> class
        /// </summary>
        protected InvalidPlatformException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
