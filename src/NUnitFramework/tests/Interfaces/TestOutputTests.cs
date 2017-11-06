// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
