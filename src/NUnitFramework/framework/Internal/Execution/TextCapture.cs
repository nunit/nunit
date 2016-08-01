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

#if !SILVERLIGHT && !NETCF && !PORTABLE
using System;
using System.IO;
//using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The TextCapture class intercepts console output and writes it
    /// to the current execution context, if one is present on the thread.
    /// If no execution context is found, the output is written to a
    /// default destination, normally the original destination of the
    /// intercepted output.
    /// </summary>
    public class TextCapture : TextWriter
    {
        private TextWriter _defaultWriter;

        /// <summary>
        /// Construct a TextCapture object
        /// </summary>
        /// <param name="defaultWriter">The default destination for non-intercepted output</param>
        public TextCapture(TextWriter defaultWriter)
        {
            _defaultWriter = defaultWriter;
        }

        /// <summary>
        /// Gets the Encoding in use by this TextWriter
        /// </summary>
        public override System.Text.Encoding Encoding
        {
            get { return _defaultWriter.Encoding; }
        }

        /// <summary>
        /// Writes a single character
        /// </summary>
        /// <param name="value">The char to write</param>
        public override void Write(char value)
        {
            var context = TestExecutionContext.GetTestExecutionContext();

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.Write(value);
            else
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Writes a string
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void Write(string value)
        {
            var context = TestExecutionContext.GetTestExecutionContext();

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.Write(value);
            else
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Writes a string followed by a line terminator
        /// </summary>
        /// <param name="value">The string to write</param>
        public override void WriteLine(string value)
        {
            var context = TestExecutionContext.GetTestExecutionContext();

            if (context != null && context.CurrentResult != null)
                context.CurrentResult.OutWriter.WriteLine(value);
            else
                _defaultWriter.WriteLine(value);
        }
    }
}
#endif
