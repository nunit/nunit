// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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
    using Interfaces;

    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
#if !NETSTANDARD1_6
    [Serializable]
#endif
    public class MultipleAssertException : ResultStateException
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MultipleAssertException(string message) : base(message) { }

        /// <summary>
        /// Construct based on the TestResult so far. This is the constructor
        /// used normally, when exiting the multiple assert block with failures.
        /// Not used internally but provided to facilitate debugging.
        /// </summary>
        /// <param name="testResult">
        /// The current result, up to this point. The result is not used 
        /// internally by NUnit but is provided to facilitate debugging.
        /// </param>
        public MultipleAssertException(ITestResult testResult)
            : base(testResult?.Message)
        {
            Guard.ArgumentNotNull(testResult, "testResult");
            TestResult = testResult;
        }

        /// <param name="message">The error message that explains 
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the 
        /// current exception</param>
        public MultipleAssertException(string message, Exception inner) :
            base(message, inner) 
        {}

#if !NETSTANDARD1_6
        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected MultipleAssertException(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context) : base(info,context)
        {}
#endif

        /// <summary>
        /// Gets the <see cref="ResultState"/> provided by this exception.
        /// </summary>
        public override ResultState ResultState
        {
            get { return ResultState.Failure; }
        }

        /// <summary>
        /// Gets the <see cref="ITestResult"/> of this test at the point the exception was thrown,
        /// </summary>
        public ITestResult TestResult { get; }
    }
}
