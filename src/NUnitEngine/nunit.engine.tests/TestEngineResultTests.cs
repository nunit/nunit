// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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

namespace NUnit.Engine.Tests
{
    using Common;
    using Framework;
    using Internal;

    [TestFixture]
    public class TestEngineResultTests
    {
        private static readonly string xmlText = "<test-assembly result=\"Passed\" total=\"23\" passed=\"23\" failed=\"0\" inconclusive=\"0\" skipped=\"0\" asserts=\"40\" />";

        [Test]
        public void CanCreateFromXmlString()
        {
            TestEngineResult result = new TestEngineResult(xmlText);
            Assert.True(result.IsSingle);
            Assert.That(result.Xml.Name, Is.EqualTo("test-assembly"));
            Assert.That(result.Xml.Attributes["result"].Value, Is.EqualTo("Passed"));
            Assert.That(result.Xml.Attributes["total"].Value, Is.EqualTo("23"));
            Assert.That(result.Xml.Attributes["passed"].Value, Is.EqualTo("23"));
            Assert.That(result.Xml.Attributes["failed"].Value, Is.EqualTo("0"));
            Assert.That(result.Xml.Attributes["inconclusive"].Value, Is.EqualTo("0"));
            Assert.That(result.Xml.Attributes["skipped"].Value, Is.EqualTo("0"));
            Assert.That(result.Xml.Attributes["asserts"].Value, Is.EqualTo("40"));
        }

        [Test]
        public void CanCreateFromXmlNode()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("test-assembly");
            XmlHelper.AddAttribute(node, "result", "Passed");
            XmlHelper.AddAttribute(node, "total", "23");
            XmlHelper.AddAttribute(node, "passed", "23");
            XmlHelper.AddAttribute(node, "failed", "0");
            XmlHelper.AddAttribute(node, "inconclusive", "0");
            XmlHelper.AddAttribute(node, "skipped", "0");
            XmlHelper.AddAttribute(node, "asserts", "40");

            TestEngineResult result = new TestEngineResult(node);
            Assert.True(result.IsSingle);
            Assert.That(result.Xml.OuterXml, Is.EqualTo(xmlText));
        }
    }
}
