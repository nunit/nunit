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
using System.IO;

namespace NUnit.Framework.CodeGeneration
{
    [TestFixture]
    public class IndentedTextWriterTests
    {
        private StringWriter stringWriter;
        private CodeWriter codeWriter;
        private static readonly string NL = Environment.NewLine;

        [SetUp]
        public void CreateWriters()
        {
            this.stringWriter = new StringWriter();
            this.codeWriter = new IndentedTextWriter(stringWriter);
        }

        [Test]
        public void WriteString()
        {
            codeWriter.Write("my string");
            Assert.That(Buffer, Is.EqualTo("my string"));
        }

        [Test]
        public void WriteInt()
        {
            codeWriter.Write(42);
            Assert.That(Buffer, Is.EqualTo("42"));
        }

        [Test]
        public void WriteLine()
        {
            codeWriter.WriteLine("here is the line");
            Assert.That(Buffer, Is.EqualTo("here is the line" + NL));
        }

        [Test]
        public void WriteLine_Empty()
        {
            codeWriter.WriteLine();
            Assert.That(Buffer, Is.EqualTo(NL));
        }

        [Test]
        public void PushIndent()
        {
            codeWriter.PushIndent(">>>   ");
            codeWriter.WriteLine("here is the line");
            Assert.That(Buffer, Is.EqualTo(">>>   here is the line" + NL));
        }

        [Test]
        public void MultipleIndents()
        {
            codeWriter.WriteLine("#if XXX");
            codeWriter.PushIndent("  ");
            codeWriter.WriteLine("public void MyMethod()");
            codeWriter.WriteLine("{");
            codeWriter.PushIndent("  ");
            codeWriter.WriteLine("// body");
            codeWriter.PopIndent();
            codeWriter.WriteLine("}");
            codeWriter.PopIndent();
            codeWriter.WriteLine("#endif"); // should be at margin

            string expected = 
                "#if XXX" + NL +
                "  public void MyMethod()" + NL +
                "  {" + NL +
                "    // body" + NL +
                "  }" + NL +
                "#endif" + NL;

            Assert.That(Buffer, Is.EqualTo(expected));
        }

        [Test]
        public void WriteLineNoTabs()
        {
            codeWriter.PushIndent(">>>");
            codeWriter.WriteLineNoTabs("at margin");
            Assert.That(Buffer, Is.EqualTo("at margin" + NL));
        }

        private string Buffer
        {
            get { return stringWriter.ToString(); }
        }
    }
}
