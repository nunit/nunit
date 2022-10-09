// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Globalization;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class TextMessageWriterTests
    {
        private static readonly string NL = Environment.NewLine;

        private TextMessageWriter writer;

        [SetUp]
        public void SetUp()
        {
            writer = new TextMessageWriter();
        }

        [Test]
        public void DisplayStringDifferences()
        {
            string s72 = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string exp = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXY...";

            writer.DisplayStringDifferences(s72, "abcde", 5, false, true);
            string message = writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + Q(exp) + NL +
                TextMessageWriter.Pfx_Actual + Q("abcde") + NL +
                "  ----------------^" + NL));
        }

        [Test]
        public void DisplayStringDifferences_NoClipping()
        {
            string s72 = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            writer.DisplayStringDifferences(s72, "abcde", 5, false, false);
            string message = writer.ToString();
            Assert.That(message, Is.EqualTo(
                TextMessageWriter.Pfx_Expected + Q(s72) + NL +
                TextMessageWriter.Pfx_Actual + Q("abcde") + NL +
                "  ----------------^" + NL));
        }

        [Test]
        public void WriteMessageLine_EmbeddedZeroes()
        {
            string expected, message;

            expected = message = "here's an embedded zero \0, in me";
            expected = "  " + expected.Replace("\0", "\\0") + NL;

            writer.WriteMessageLine(0, message, null);
            message = writer.ToString();

            Assert.That(message, Is.EqualTo(expected));
        }

        [Test]
        public void WriteMessageLine_EmbeddedZeroesAsArgs()
        {
            string expected, message, arg0;

            message = "here's an embedded zero {0}, in my args";
            arg0 = "\0";
            expected = "  " + string.Format(message, arg0).Replace("\0", "\\0") + NL;

            writer.WriteMessageLine(0, message, arg0);
            message = writer.ToString();

            Assert.That(message, Is.EqualTo(expected));
        }

        [Test]
        public void WriteMessageLine_EmbeddedNonNullControlChars()
        {
            string expected, message;

            expected = message = "Here we have embedded control characters \b\f in the string!";

            writer.WriteMessageLine(0, message, null);
            message = writer.ToString();
            expected = "  " + expected.Replace("\0", "\\0") + NL;

            Assert.That(message, Is.EqualTo(expected));
        }

        private string Q(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
