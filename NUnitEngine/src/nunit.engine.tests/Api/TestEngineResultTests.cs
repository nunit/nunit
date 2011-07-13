// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using System.Xml;
using NUnit.Framework;

namespace NUnit.Engine.Tests
{
    public class TestEngineResultTests
    {
        private static readonly string message = "This is my message!";
        private static readonly string xmlText = string.Format("<error message=\"{0}\" />", message);

        [Test]
        public void CreateWithXmlString()
        {
            TestEngineResult result = new TestEngineResult(xmlText);

            Assert.AreEqual("error", result.ResultType);
            Assert.AreEqual(xmlText, result.Text);
            Assert.AreEqual(message, result.Xml.Attributes["message"].Value);
        }

        [Test]
        public void CreateWithXmlNode()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlText);
            TestEngineResult result = new TestEngineResult(doc.FirstChild);

            Assert.AreEqual("error", result.ResultType);
            // TODO: The following is not very robust. Should use an XML comparison.
            Assert.AreEqual(xmlText, result.Text);
            Assert.AreEqual(message, result.Xml.Attributes["message"].Value);
        }
    }
}
