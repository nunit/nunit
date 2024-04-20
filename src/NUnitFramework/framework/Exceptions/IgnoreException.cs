// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    using Interfaces;

    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
    [Serializable]
    public class IgnoreException : ResultStateException
    {
        /// <param name="message"></param>
        public IgnoreException(string message) : base(message)
        {
        }

        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the
        /// current exception</param>
        public IgnoreException(string message, Exception? inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the ResultState provided by this exception
        /// </summary>
        public override ResultState ResultState => ResultState.Ignored;
    }
}
