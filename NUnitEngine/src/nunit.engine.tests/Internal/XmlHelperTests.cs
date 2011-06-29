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

using System;
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Xml;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
    public class XmlHelperTests
    {
        [Test]
        public void SingleElement()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("myelement");

            Assert.That(node.Name, Is.EqualTo("myelement"));
            Assert.That(node.Attributes.Count, Is.EqualTo(0));
            Assert.That(node.ChildNodes.Count, Is.EqualTo(0));
        }

        [Test]
        public void SingleElementWithAttributes()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("person");
            XmlHelper.AddAttribute(node, "name", "Fred");
            XmlHelper.AddAttribute(node, "age", "42");
            XmlHelper.AddAttribute(node, "quotes", "'c' is a char but \"c\" is a string");

            Assert.That(node.Name, Is.EqualTo("person"));
            Assert.That(node.Attributes.Count, Is.EqualTo(3));
            Assert.That(node.ChildNodes.Count, Is.EqualTo(0));
            Assert.That(node.Attributes["name"].Value, Is.EqualTo("Fred"));
            Assert.That(node.Attributes["age"].Value, Is.EqualTo("42"));
            Assert.That(node.Attributes["quotes"].Value, Is.EqualTo("'c' is a char but \"c\" is a string"));
        }

        [Test]
        public void ElementContainsElementWithInnerText()
        {
            XmlNode top = XmlHelper.CreateTopLevelElement("top");
            XmlNode message = XmlHelper.AddElement(top, "message");
            message.InnerText = "This is my message";

            Assert.That(top.SelectSingleNode("message").InnerText, Is.EqualTo("This is my message"));
        }

        [Test]
        public void ElementContainsElementWithCData()
        {
            XmlNode top = XmlHelper.CreateTopLevelElement("top");
            XmlHelper.AddElementWithCDataSection(top, "message", "x > 5 && x < 7");

            Assert.That(top.SelectSingleNode("message").InnerText, Is.EqualTo("x > 5 && x < 7"));
        }

        [Test]
        public void SafeAttributeAccess()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("top");

            Assert.That(XmlHelper.GetAttribute(node, "junk"), Is.Null);
        }

        [Test]
        public void SafeAttributeAcessWithIntDefaultValue()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("top");
            Assert.That(XmlHelper.GetAttribute(node, "junk", 42), Is.EqualTo(42));
        }

        [Test]
        public void SafeAttributeAcessWithDoubleDefaultValue()
        {
            XmlNode node = XmlHelper.CreateTopLevelElement("top");
            Assert.That(XmlHelper.GetAttribute(node, "junk", 1.234), Is.EqualTo(1.234));
        }

        [Test] // TODO: This needs more thorough testing
        public void TestResultsCanBeCombined()
        {
            XmlNode node1 = XmlHelper.CreateTopLevelElement("test-case");
            XmlHelper.AddAttribute(node1, "result", "Passed");
            XmlHelper.AddAttribute(node1, "passed", "1");

            XmlNode node2 = XmlHelper.CreateTopLevelElement("test-case");
            XmlHelper.AddAttribute(node2, "result", "Passed");
            XmlHelper.AddAttribute(node2, "passed", "1");

#if CLR_2_0 || CLR_4_0
            List<XmlNode> results = new List<XmlNode>();
#else
            ArrayList results = new ArrayList();
#endif
            results.Add(node1);
            results.Add(node2);

            XmlNode combined = XmlHelper.CombineResultNodes(results);

            Assert.That(combined.Name, Is.EqualTo("test-suite"));
            Assert.That(combined.Attributes["type"].Value, Is.EqualTo("test-package"));
            Assert.That(combined.Attributes["result"].Value, Is.EqualTo("Passed"));
            Assert.That(combined.Attributes["passed"].Value, Is.EqualTo("2"));
        }
    }
}
