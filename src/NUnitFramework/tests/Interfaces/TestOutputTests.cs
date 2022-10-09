// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text;

namespace NUnit.Framework.Tests.Interfaces
{
    [TestFixture]
    class TestOutputTests
    {
        [TestCase("text", "stream", "testId", "testName")]
        [TestCase("text", "stream", null, "testName")]
        [TestCase("text", "stream", "testId", null)]
        public void ToXml_IncludeAttributesInProperFormatting(string text, string stream, string testId, string testName)
        {
            var testOutput = new TestOutput(text, stream, testId, testName);
            var expected = new StringBuilder();
            expected.AppendFormat("<test-output stream=\"{0}\"", stream);

            if (testId != null)
            {
                expected.AppendFormat(" testid=\"{0}\"", testId);
            }

            if (testName != null)
            {
                expected.AppendFormat(" testname=\"{0}\"", testName);
            }

            expected.AppendFormat("><![CDATA[{0}]]></test-output>", text);
            Assert.That(testOutput.ToXml(), Is.EqualTo(expected.ToString()));
        }

        [Test]
        public void SerializeXmlWithInvalidCharacter()
        {
            var testOutput = new TestOutput("\u001bHappyFace", string.Empty, string.Empty, string.Empty);
            // This throws if the output value is not properly escaped
            Assert.That(testOutput.ToXml(), Contains.Substring("><![CDATA[\\u001bHappyFace]]></test-output>"));
        }

        [Test]
        public void SerializeXmlWithInvalidCharacter_NonFirstPosition()
        {
            var testOutput = new TestOutput("Happy\u001bFace", string.Empty, string.Empty, string.Empty);
            // This throws if the output value is not properly escaped
            Assert.That(testOutput.ToXml(), Contains.Substring("><![CDATA[Happy\\u001bFace]]></test-output>"));
        }
    }
}
