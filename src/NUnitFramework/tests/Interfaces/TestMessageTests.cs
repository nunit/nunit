// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Interfaces;
using System;
using System.Text;

namespace NUnit.Framework.Tests.Interfaces
{
    [TestFixture]
    class TestMessageTests
    {
        [TestCase("destination", "text", "testId")]
        [TestCase("destination", "text", null)]
        public void ToXml_IncludeAttributesInProperFormatting(string destination, string message, string testId)
        {
            var testMessage = new TestMessage(destination, message, testId);
            var expected = new StringBuilder("<test-message");

            expected.AppendFormat(" destination=\"{0}\"", destination);

            if (testId != null)
            {
                expected.AppendFormat(" testid=\"{0}\"", testId);
            }

            expected.AppendFormat("><![CDATA[{0}]]></test-message>", message);
            Assert.That(testMessage.ToXml(), Is.EqualTo(expected.ToString()));
        }

        [Test]
        public void TextParameterCannotBeNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { new TestMessage("destination", null, "testId"); });
        }

        [Test]
        public void DestinationParameterCannotBeNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { new TestMessage(null, "text", "testId"); });
        }
    }
}
