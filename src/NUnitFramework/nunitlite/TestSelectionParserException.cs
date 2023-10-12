// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Runtime.Serialization;

namespace NUnit.Common
{
    /// <summary>
    /// TestSelectionParserException is thrown when an error
    /// is found while parsing the selection expression.
    /// </summary>
    [Serializable]
    public class TestSelectionParserException : Exception
    {
        /// <summary>
        /// Construct with a message
        /// </summary>
        public TestSelectionParserException(string message) : base(message)
        {
        }

        /// <summary>
        /// Construct with a message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public TestSelectionParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        public TestSelectionParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
