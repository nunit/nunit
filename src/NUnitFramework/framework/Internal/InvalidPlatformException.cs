// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InvalidPlatformException is thrown when the platform name supplied
    /// to a test is not recognized.
    /// </summary>
    [Serializable]
    class InvalidPlatformException : ArgumentException
    {
        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class.
        /// </summary>
        public InvalidPlatformException() : base() { }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidPlatformException(string message) : base(message) { }

        /// <summary>
        /// Instantiates a new instance of the <see cref="InvalidPlatformException"/> class
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidPlatformException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Serialization constructor for the <see cref="InvalidPlatformException"/> class
        /// </summary>
        protected InvalidPlatformException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
