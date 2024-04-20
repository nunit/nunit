// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    using Interfaces;

    /// <summary>
    /// Abstract base for Exceptions that terminate a test and provide a ResultState.
    /// </summary>
    [Serializable]
    public abstract class ResultStateException : Exception
    {
        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        protected ResultStateException(string message) : base(message)
        {
        }

        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the
        /// current exception</param>
        public ResultStateException(string message, Exception? inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the ResultState provided by this exception
        /// </summary>
        public abstract ResultState ResultState { get; }
    }
}
