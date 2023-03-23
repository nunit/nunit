// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

        class DisposeCheckerWriter : StringWriter
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
