// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    using Interfaces;

    /// <summary>
    /// Thrown when a test executes inconclusively.
    /// </summary>
    [Serializable]
    public class InconclusiveException : ResultStateException
    {
        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        public InconclusiveException(string message)
            : base(message)
        {
        }

        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the
        /// current exception</param>
        public InconclusiveException(string message, Exception? inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected InconclusiveException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the ResultState provided by this exception
        /// </summary>
        public override ResultState ResultState => ResultState.Inconclusive;
    }
}
