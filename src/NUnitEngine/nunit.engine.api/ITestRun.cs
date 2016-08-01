// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// The ITestRun class represents an ongoing test run.
    /// </summary>
    public interface ITestRun
    {
        /// <summary>
        /// Get the result of the test.
        /// </summary>
        /// <returns>An XmlNode representing the test run result</returns>
        XmlNode Result { get; }

        /// <summary>
        /// Blocks the current thread until the current test run completes
        /// or the timeout is reached
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.Int32"/> that represents the number of milliseconds to wait or -1 milliseconds to wait indefinitely. </param>
        /// <returns>True if the run completed</returns>
        bool Wait(int timeout);
    }
}
