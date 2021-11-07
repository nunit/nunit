// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when an assertion failed. Here to preserve the inner
    /// exception and hence its stack trace.
    /// </summary>
    [Serializable]
    public class NUnitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitException"/> class.
        /// </summary>
        public NUnitException () : base()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        public NUnitException(string message) : base (message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the
        /// current exception</param>
        public NUnitException(string message, Exception inner) :
            base(message, inner)
        { }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected NUnitException(SerializationInfo info,
            StreamingContext context) : base(info,context){}
    }
}
