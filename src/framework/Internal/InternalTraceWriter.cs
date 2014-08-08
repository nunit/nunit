// ***********************************************************************
// Copyright (c) 2008-2013 Charlie Poole
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
using System.Diagnostics;
using System.IO;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InternalTraceWriter can write to a separate file per domain
    /// and process using it or feed output to another TextWriter;
    /// </summary>
    public class InternalTraceWriter : TextWriter
    {
        TextWriter writer;
        object myLock = new object();

        /// <summary>
        /// Construct an InternalTraceWriter that writes to a file.
        /// </summary>
        /// <param name="logPath">Path to the file to use</param>
        public InternalTraceWriter(string logPath)
        {
            var streamWriter = new StreamWriter(new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Write));
            streamWriter.AutoFlush = true;
            this.writer = streamWriter;
        }

        /// <summary>
        /// Construct an InternalTraceWriter that writes to a 
        /// TextWriter provided by the caller.
        /// </summary>
        /// <param name="writer"></param>
        public InternalTraceWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Returns the character encoding in which the output is written.
        /// </summary>
        /// <returns>The character encoding in which the output is written.</returns>
        public override System.Text.Encoding Encoding
        {
            get { return writer.Encoding; }
        }

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            lock (myLock)
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(string value)
        {
            lock (myLock)
            {
                writer.WriteLine(value);
            }
        }
    }
}
