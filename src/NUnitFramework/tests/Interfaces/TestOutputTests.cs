// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
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
    }
}
