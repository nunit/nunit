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

using System;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ErrorReport is an AsyncResult returned when an
    /// error has arisen.
    /// </summary>
    [Serializable]
    public class ErrorReport : FinalResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReport"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">A message giving the reason for the failure</param>
        public ErrorReport(string message)
        {
            this.xmlResult = string.Format("<error message=\"{0}\"/>", message);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReport"/> class
        /// with a specified message and stack trace.
        /// </summary>
        /// <param name="message">A message giving the reason for the failure</param>
        /// <param name="stackTrace">The stack trace at the point of failure</param>
        public ErrorReport(string message, string stackTrace)
        {
            this.xmlResult = string.Format("<error message=\"{0}\" stackTrace=\"{1}\"/>", message, stackTrace);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReport"/> class
        /// based on an Exception that was thrown.
        /// </summary>
        /// <param name="ex">An Exception</param>
        public ErrorReport(Exception ex)
        {
            if (ex is System.Reflection.TargetInvocationException)
                ex = ex.InnerException;

            if (ex is System.IO.FileNotFoundException || ex is System.BadImageFormatException)
                this.xmlResult = string.Format("<error message=\"{0}\"/>", ex.Message);
            else
                this.xmlResult = string.Format("<error message=\"{0}\" stackTrace=\"{1}\"/>", ex.Message, ex.StackTrace);
        }
    }
}