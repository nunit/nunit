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

        private TextMessageWriter _writer;

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

        [Test]
        public void DisplayDifferences_Unsigned_LinearTolerance_NoUnderflow()
        {
            _writer.DisplayDifferences(1UL, 3UL, new Tolerance(1UL));
            string message = _writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + "1 +/- 1" + NL +
                TextMessageWriter.Pfx_Actual + "3" + NL +
                TextMessageWriter.Pfx_Difference + "2" + NL));
        }

        [Test]
        public void DisplayDifferences_Unsigned_PercentTolerance_NoUnderflow()
        {
            _writer.DisplayDifferences(100UL, 120UL, new Tolerance(10.0).Percent);
            string message = _writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + "100 +/- 10.0d Percent" + NL +
                TextMessageWriter.Pfx_Actual + "120" + NL +
                TextMessageWriter.Pfx_Difference + "20.0d Percent" + NL));
        }

        [Test]
        public void DisplayDifferences_Signed_LinearTolerance_NoOverflow()
        {
            _writer.DisplayDifferences(int.MaxValue, int.MinValue, new Tolerance(1));
            string message = _writer.ToString();
            Assert.That(message, Does.EndWith(TextMessageWriter.Pfx_Difference + "4294967295" + NL));
        }

        private string Q(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
