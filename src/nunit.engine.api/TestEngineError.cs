// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

namespace NUnit.Engine
{
    /// <summary>
    /// TestEngineError represents an error that occured in 
    /// executing an API method.
    /// </summary>
    public class TestEngineError
    {
        private string message;
        private string stackTrace;

        /// <summary>
        /// Construct a TestEngineError from a message
        /// </summary>
        /// <param name="message">The error message</param>
        public TestEngineError(string message)
            : this(message, null) { }

        /// <summary>
        /// Construct a TestEngineError from a message and stack trace.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="stackTrace">A stack trace, or null</param>
        public TestEngineError(string message, string stackTrace)
        {
            this.message = message;
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// Gets the error message from an error
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the stack trace for an error, if present
        /// </summary>
        public string StackTrace 
        {
            get { return stackTrace; }
        }
    }
}
