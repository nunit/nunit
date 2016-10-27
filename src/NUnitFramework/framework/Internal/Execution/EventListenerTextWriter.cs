// ***********************************************************************
// Copyright (c) 2007-2016 Charlie Poole
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

#if !PORTABLE
using System;
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// EventListenerTextWriter sends text output to the currently active
    /// ITestEventListener in the form of a TestOutput object. If no event 
    /// listener is active in the contet, or if there is no context,
    /// the output is forwarded to the supplied default writer.
    /// </summary>
	public class EventListenerTextWriter : TextWriter
	{
        private TextWriter _defaultWriter;
		private string _streamName;

        /// <summary>
        /// Construct an EventListenerTextWriter
        /// </summary>
        /// <param name="streamName">The name of the stream to use for events</param>
        /// <param name="defaultWriter">The default writer to use if no listener is available</param>
		public EventListenerTextWriter( string streamName, TextWriter defaultWriter )
		{
			_streamName = streamName;
            _defaultWriter = defaultWriter;
		}

        /// <summary>
        /// Write a single char
        /// </summary>
        override public void Write(char aChar)
        {
            if (!TrySendToListener(aChar.ToString()))
                _defaultWriter.Write(aChar);
        }

        /// <summary>
        /// Write a string
        /// </summary>
        override public void Write(string aString)
        {
            if (!TrySendToListener(aString))
                _defaultWriter.Write(aString);
        }

        /// <summary>
        /// Write a string followed by a newline
        /// </summary>
        override public void WriteLine(string aString)
        {
            if (!TrySendToListener(aString + Environment.NewLine))
                _defaultWriter.WriteLine(aString);
        }

        /// <summary>
        /// Get the Encoding for this TextWriter
        /// </summary>
        override public System.Text.Encoding Encoding
		{
			get { return Encoding.Default; }
		}

        private bool TrySendToListener(string text)
        {
            var context = TestExecutionContext.GetTestExecutionContext();
            if (context == null || context.Listener == null)
                return false;

            string testName = context.CurrentTest != null
                ? context.CurrentTest.FullName
                : null;
            context.Listener.TestOutput(new TestOutput(text, _streamName, testName));
            return true;
        }
	}
}
#endif
