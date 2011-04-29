// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// ExpectedExceptionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExpectedExceptionAttribute : NUnitAttribute
    {
        private Type expectedException;
        private string expectedExceptionName;
        private string expectedMessage;
        private MessageMatch matchType;
        private string userMessage;
        private string handler;

        /// <summary>
        /// Constructor for a non-specific exception
        /// </summary>
        public ExpectedExceptionAttribute()
        {
        }

        /// <summary>
        /// Constructor for a given type of exception
        /// </summary>
        /// <param name="exceptionType">The type of the expected exception</param>
        public ExpectedExceptionAttribute(Type exceptionType)
        {
            this.expectedException = exceptionType;
            this.expectedExceptionName = exceptionType.FullName;
        }

        /// <summary>
        /// Constructor for a given exception name
        /// </summary>
        /// <param name="exceptionName">The full name of the expected exception</param>
        public ExpectedExceptionAttribute(string exceptionName)
        {
            this.expectedExceptionName = exceptionName;
        }

        /// <summary>
        /// Gets or sets the expected exception type
        /// </summary>
        public Type ExpectedException
        {
            get { return expectedException; }
            set
            {
                expectedException = value;
                expectedExceptionName = expectedException.FullName;
            }
        }

        /// <summary>
        /// Gets or sets the full Type name of the expected exception
        /// </summary>
        public string ExpectedExceptionName
        {
            get { return expectedExceptionName; }
            set { expectedExceptionName = value; }
        }

        /// <summary>
        /// Gets or sets the expected message text
        /// </summary>
        public string ExpectedMessage
        {
            get { return expectedMessage; }
            set { expectedMessage = value; }
        }

        /// <summary>
        /// Gets or sets the user message displayed in case of failure
        /// </summary>
        public string UserMessage
        {
            get { return userMessage; }
            set { userMessage = value; }
        }

        /// <summary>
        ///  Gets or sets the type of match to be performed on the expected message
        /// </summary>
        public MessageMatch MatchType
        {
            get { return matchType; }
            set { matchType = value; }
        }

        /// <summary>
        ///  Gets the name of a method to be used as an exception handler
        /// </summary>
        public string Handler
        {
            get { return handler; }
            set { handler = value; }
        }
    }
}
