// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Text;
using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class ExtendedTextWrapperTests
    {
        private StringBuilder sb;
        private ExtendedTextWrapper writer;

        private const string LABEL = "My Label: ";
        private const string OPTION = "option value";
        private const string TESTING = "testing";
        private static readonly string NL = Environment.NewLine;

        [SetUp]
        public void SetUp()
        {
            sb = new StringBuilder();
            writer = new ExtendedTextWrapper(new StringWriter(sb));
        }

        [Test]
        public void Write()
        {
            writer.Write(TESTING);
            Assert.That(sb.ToString(), Is.EqualTo(TESTING));
        }

        [Test]
        public void Write_ColorStyle()
        {
            writer.Write(ColorStyle.Error, TESTING);
            Assert.That(sb.ToString(), Is.EqualTo(TESTING));
        }

        [Test]
        public void WriteLine()
        {
            writer.WriteLine(TESTING);
            Assert.That(sb.ToString(), Is.EqualTo(TESTING + NL));
        }

        [Test]
        public void WriteLine_ColorStyle()
        {
            writer.WriteLine(ColorStyle.SectionHeader, TESTING);
            Assert.That(sb.ToString(), Is.EqualTo(TESTING + NL));
        }

        [Test]
        public void WriteLabel()
        {
            writer.WriteLabel(LABEL, OPTION);
            Assert.That(sb.ToString(), Is.EqualTo(LABEL + OPTION));
        }

        [Test]
        public void WriteLabel_ColorStyle()
        {
            writer.WriteLabel(LABEL, OPTION, ColorStyle.Pass);
            Assert.That(sb.ToString(), Is.EqualTo(LABEL + OPTION));
        }

        [Test]
        public void WriteLabelLine()
        {
            writer.WriteLabelLine(LABEL, OPTION);
            Assert.That(sb.ToString(), Is.EqualTo(LABEL + OPTION + NL));
        }

        [Test]
        public void WriteLabelLine_ColorStyle()
        {
            writer.WriteLabelLine(LABEL, OPTION, ColorStyle.Pass);
            Assert.That(sb.ToString(), Is.EqualTo(LABEL + OPTION + NL));
        }
    }
}
