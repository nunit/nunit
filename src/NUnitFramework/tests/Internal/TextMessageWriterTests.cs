// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class TextMessageWriterTests
    {
        private static readonly string NL = Environment.NewLine;

        private TextMessageWriter? _writer;

        [SetUp]
        public void SetUp()
        {
            _writer = new TextMessageWriter();
        }

        [TearDown]
        public void TearDown()
        {
            _writer.Dispose();
        }

        [Test]
        public void DisplayStringDifferences()
        {
            string s72 = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string exp = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXY...";

            _writer.DisplayStringDifferences(s72, "abcde", 5, 5, false, false, true);
            string message = _writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + Q(exp) + NL +
                TextMessageWriter.Pfx_Actual + Q("abcde") + NL +
                "  ----------------^" + NL));
        }

        [Test]
        public void DisplayStringDifferences_NoClipping()
        {
            string s72 = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            _writer.DisplayStringDifferences(s72, "abcde", 5, 5, false, false, false);
            string message = _writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + Q(s72) + NL +
                TextMessageWriter.Pfx_Actual + Q("abcde") + NL +
                "  ----------------^" + NL));
        }

        [Test]
        public void DisplayStringDifferences_IgnoreWhiteSpace()
        {
            string expected = "abc def";
            string actual = "a b c d e g";

            _writer.DisplayStringDifferences(expected, actual, 6, 10, false, true, false);
            string message = _writer.ToString();
            string expectedMessage =
                TextMessageWriter.Pfx_Expected + Q(expected) + ", ignoring white-space" + NL +
                "  -----------------^" + NL +
                TextMessageWriter.Pfx_Actual + Q(actual) + NL +
                "  ---------------------^" + NL;
            Assert.That(message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void WriteMessageLine_EmbeddedZeroes()
        {
            string expected, message;

            expected = message = "here's an embedded zero \0, in me";
            expected = "  " + expected.Replace("\0", "\\0") + NL;

            _writer.WriteMessageLine(0, message, null);
            message = _writer.ToString();

            Assert.That(message, Is.EqualTo(expected));
        }

        [Test]
        public void WriteMessageLine_EmbeddedZeroesAsArgs()
        {
            string expected, message, arg0;

            message = "here's an embedded zero {0}, in my args";
            arg0 = "\0";
            expected = "  " + string.Format(message, arg0).Replace("\0", "\\0") + NL;

            _writer.WriteMessageLine(0, message, arg0);
            message = _writer.ToString();

            Assert.That(message, Is.EqualTo(expected));
        }

        [Test]
        public void WriteMessageLine_EmbeddedNonNullControlChars()
        {
            string expected, message;

            expected = message = "Here we have embedded control characters \b\f in the string!";

            _writer.WriteMessageLine(0, message, null);
            message = _writer.ToString();
            expected = "  " + expected.Replace("\0", "\\0") + NL;

            Assert.That(message, Is.EqualTo(expected));
        }

        [Test]
        public void WriteMessageLine_DisplayDifference_WhenActual_IsNull()
        {
            string message;
            var mockTolerance = new Tolerance("00:00:10");

            _writer.DisplayDifferences("2023-01-01 13:00:00", null, mockTolerance);
            message = _writer.ToString();

            Assert.That(message, Does.Not.Contain("Off by"));
        }

        [Test]
        public void WriteMessageLine_DisplayDifference_WhenExpected_IsNull()
        {
            string message;
            var mockTolerance = new Tolerance("00:00:10");

            _writer.DisplayDifferences(null, "2023-01-01 13:00:00", mockTolerance);
            message = _writer.ToString();

            Assert.That(message, Does.Not.Contain("Off by"));
        }

        [Test]
        public void WriteMessageLine_DisplayDifference_WhenActual_IsNotNull()
        {
            string message;
            var mockTolerance = new Tolerance("00:00:10");

            _writer.DisplayDifferences("2023-01-01 13:00:00", "2023-01-01 13:00:12", mockTolerance);
            message = _writer.ToString();

            Assert.That(message, Does.Not.Contain("Off by"));
        }

        private string Q(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
