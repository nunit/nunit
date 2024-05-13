// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Interfaces
{
    [TestFixture]
    internal class TestMessageTests
    {
        [TestCase("destination", "text", "testId")]
        [TestCase("destination", "text", null)]
        public void ToXml_IncludeAttributesInProperFormatting(string destination, string message, string? testId)
        {
            var testMessage = new TestMessage(destination, message, testId);
            var expected = new StringBuilder("<test-message");

            expected.AppendFormat(" destination=\"{0}\"", destination);

            if (testId is not null)
            {
                expected.AppendFormat(" testid=\"{0}\"", testId);
            }

            expected.AppendFormat("><![CDATA[{0}]]></test-message>", message);
            Assert.That(testMessage.ToXml(), Is.EqualTo(expected.ToString()));
        }

        [Test]
        public void TextParameterCannotBeNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new TestMessage("destination", null!, "testId"));
        }

        [Test]
        public void DestinationParameterCannotBeNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new TestMessage(null!, "text", "testId"));
        }
    }
}
