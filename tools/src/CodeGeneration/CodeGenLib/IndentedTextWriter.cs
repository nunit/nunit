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
using System.Collections.Generic;
using System.IO;

namespace NUnit.Framework.CodeGeneration
{
    public class IndentedTextWriter : CodeWriter
    {
        private TextWriter writer;
        private string prefix;
        private Stack<string> prefixes = new Stack<string>();

        public IndentedTextWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public override void PushIndent(string indent)
        {
            prefixes.Push(prefix);
            prefix = prefix + indent;
        }

        public override void PopIndent()
        {
            prefix = prefixes.Pop();
        }

        public override System.Text.Encoding Encoding
        {
            get { return writer.Encoding; }
        }

        public override void Write(string value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Write a line using current indent.
        /// </summary>
        /// <param name="value">Text to write</param>
        public override void WriteLine(string value)
        {
            writer.Write(prefix);
            writer.Write(value);
            writer.Write("\r\n");
        }

        /// <summary>
        /// Write standard CRLF line ending. We do
        /// this rather than using the underlying
        /// WriteLine so that the text is generated
        /// the same way no matter what system we 
        /// are running on. Use of CRLF is for 
        /// historical reasons only: NUnit was
        /// first developed on Windows. Most IDEs
        /// should cope with this.
        /// </summary>
        public override void WriteLine()
        {
            writer.Write("\r\n");
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override void Close()
        {
            writer.Close();
        }

        public override void WriteLineNoTabs(string value)
        {
            writer.WriteLine(value);
        }
    }
}
