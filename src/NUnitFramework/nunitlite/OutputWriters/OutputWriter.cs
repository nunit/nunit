// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// OutputWriter is an abstract class used to write test
    /// results to a file in various formats. Specific 
    /// OutputWriters are derived from this class.
    /// </summary>
    public abstract class OutputWriter
    {
        /// <summary>
        /// Writes a test result to a file
        /// </summary>
        /// <param name="result">The result to be written</param>
        /// <param name="outputPath">Path to the file to which the result is written</param>
        /// <param name="runSettings">A dictionary of settings used for this test run</param>
        public void WriteResultFile(ITestResult result, string outputPath, IDictionary<string, object> runSettings, TestFilter filter)
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                WriteResultFile(result, writer, runSettings, filter);
            }
        }

        /// <summary>
        /// Writes test info to a file
        /// </summary>
        /// <param name="test">The test to be written</param>
        /// <param name="outputPath">Path to the file to which the test info is written</param>
        public void WriteTestFile(ITest test, string outputPath)
        {
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                WriteTestFile(test, writer);
            }
        }

        /// <summary>
        /// Abstract method that writes a test result to a TextWriter
        /// </summary>
        /// <param name="result">The result to be written</param>
        /// <param name="writer">A TextWriter to which the result is written</param>
        /// <param name="runSettings">A dictionary of settings used for this test run</param>
        /// <param name="filter"></param>
        public abstract void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter);

        /// <summary>
        /// Abstract method that writes test info to a TextWriter
        /// </summary>
        /// <param name="test">The test to be written</param>
        /// <param name="writer">A TextWriter to which the test info is written</param>
        public abstract void WriteTestFile(ITest test, TextWriter writer);
    }
}
