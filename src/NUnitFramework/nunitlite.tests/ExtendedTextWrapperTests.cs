// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class ExtendedTextWrapperTests
    {
        private StringBuilder _sb;
        private ExtendedTextWrapper _writer;

        private const string LABEL = "My Label: ";
        private const string OPTION = "option value";
        private const string TESTING = "testing";
        private static readonly string NL = Environment.NewLine;

        [SetUp]
        public void SetUp()
        {
            _sb = new StringBuilder();
            _writer = new ExtendedTextWrapper(new StringWriter(_sb));
        }

        [Test]
        public void Write()
        {
            _writer.Write(TESTING);
            Assert.That(_sb.ToString(), Is.EqualTo(TESTING));
        }

        [Test]
        public void Write_ColorStyle()
        {
            _writer.Write(ColorStyle.Error, TESTING);
            Assert.That(_sb.ToString(), Is.EqualTo(TESTING));
        }

        [Test]
        public void WriteLine()
        {
            _writer.WriteLine(TESTING);
            Assert.That(_sb.ToString(), Is.EqualTo(TESTING + NL));
        }

        [Test]
        public void WriteLine_ColorStyle()
        {
            _writer.WriteLine(ColorStyle.SectionHeader, TESTING);
            Assert.That(_sb.ToString(), Is.EqualTo(TESTING + NL));
        }

        [Test]
        public void WriteLabel()
        {
            _writer.WriteLabel(LABEL, OPTION);
            Assert.That(_sb.ToString(), Is.EqualTo(LABEL + OPTION));
        }

        [Test]
        public void WriteLabel_ColorStyle()
        {
            _writer.WriteLabel(LABEL, OPTION, ColorStyle.Pass);
            Assert.That(_sb.ToString(), Is.EqualTo(LABEL + OPTION));
        }

        [Test]
        public void WriteLabelLine()
        {
            _writer.WriteLabelLine(LABEL, OPTION);
            Assert.That(_sb.ToString(), Is.EqualTo(LABEL + OPTION + NL));
        }

        [Test]
        public void WriteLabelLine_ColorStyle()
        {
            _writer.WriteLabelLine(LABEL, OPTION, ColorStyle.Pass);
            Assert.That(_sb.ToString(), Is.EqualTo(LABEL + OPTION + NL));
        }

        [Test]
        public void DisposeSkipsStdOut()
        {
            var originalStdOut = Console.Out;

            var textWriter = new DisposeCheckerWriter();

            Console.SetOut(textWriter);
            try
            {
                var colorWriter = new ColorConsoleWriter();
                colorWriter.Dispose();

                Assert.IsFalse(textWriter.IsDisposed);
            }
            finally
            {
                Console.SetOut(originalStdOut);
            }
        }

        private class DisposeCheckerWriter : StringWriter
        {
            public bool IsDisposed { get; private set; }
            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;

                base.Dispose(disposing);
            }
        }
    }
}
