// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The TestOutput class holds a unit of output from 
    /// a test to a specific output stream
    /// </summary>
	public class TestOutput
	{
        /// <summary>
        /// Construct with text, ouput destination type and
        /// the name of the test that produced the output.
        /// </summary>
        /// <param name="text">Text to be output</param>
        /// <param name="stream">Name of the stream or channel to which the text should be written</param>
        /// <param name="testName">FullName of test that produced the output</param>
		public TestOutput(string text, string stream, string testName)
        {
            Text = text;
            Stream = stream;
            TestName = testName;
        }

        /// <summary>
        /// Return string representation of the object for debugging
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return Stream + ": " + Text;
		}

        /// <summary>
        /// Get the text 
        /// </summary>
		public string Text { get; private set; }

        /// <summary>
        /// Get the output type
        /// </summary>
		public string Stream { get; private set; }

        /// <summary>
        /// Get the name of the test that created the output
        /// </summary>
        public string TestName { get; private set; }

        /// <summary>
        /// Convert the TestOutput object to an XML string
        /// </summary>
        public string ToXml()
        {
            return TestName != null
                ? string.Format("<test-output stream='{0}' testname='{1}'>{2}</test-output>", Stream, TestName, Text)
                : string.Format("<test-output stream='{0}'>{1}</test-output>", Stream, Text);
        }
    }
}
