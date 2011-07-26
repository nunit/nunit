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
using System.Collections.Generic;
using System.Xml;
using NUnit.Engine.Internal;
using NUnit.Framework;

namespace NUnit.Engine.Tests
{
    [TestFixture]
    public class TestEngineResultTests
    {
        private static readonly string resultText1 = "<test-assembly result=\"Passed\" total=\"23\" passed=\"23\" failed=\"0\" inconclusive=\"0\" skipped=\"0\" asserts=\"40\" />";
        private static readonly string resultText2 = "<test-assembly result=\"Failed\" total=\"42\" passed=\"31\" failed=\"4\" inconclusive=\"5\" skipped=\"2\" asserts=\"53\" />";

        private static readonly TestEngineResult result1 = new TestEngineResult(resultText1);
        private static readonly TestEngineResult result2 = new TestEngineResult(resultText2);

        private static readonly TestEngineResult[] twoResults = new TestEngineResult[] { result1, result2 };

        [Test]
        public void CanCreateFromXmlString()
        {
            TestEngineResult result = new TestEngineResult(resultText1);
            Assert.True(result.IsSingle);
            Assert.That(result.Xml.Name, Is.EqualTo("test-assembly"));
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
            Assert.That(result.Xml.OuterXml, Is.EqualTo(resultText1));
        }

        [Test]
        public void TestResultsCanBeWrapped()
        {
            TestEngineResult wrapped = TestEngineResult.Wrap("test-wrapper", new TestEngineResult[] { result1, result2 });

            Assert.True(wrapped.IsSingle);
            Assert.That(wrapped.Xml.Name, Is.EqualTo("test-wrapper"));
            Assert.That(wrapped.Xml.ChildNodes.Count, Is.EqualTo(2));
            Assert.That(wrapped.Xml.Attributes.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestResultsCanBeMerged()
        {
            TestEngineResult mergedResult = TestEngineResult.Merge(twoResults);

            Assert.That(mergedResult.XmlNodes.Count, Is.EqualTo(2));
            Assert.That(mergedResult.XmlNodes[0].OuterXml, Is.EqualTo(resultText1));
            Assert.That(mergedResult.XmlNodes[1].OuterXml, Is.EqualTo(resultText2));
        }

        //[Test]
        public void CanMakeTestRunResult()
        {
            DateTime startTime = new DateTime(2011, 07, 04, 12, 34, 56);

            TestEngineResult combined = TestEngineResult.MakeTestRunResult(new TestPackage("dummy.dll"), startTime, result1);
            Assert.That(combined.IsSingle);

            XmlNode combinedNode = combined.Xml;

            Assert.That(combinedNode.Name, Is.EqualTo("test-run"));
            Assert.That(combinedNode.Attributes["result"].Value, Is.EqualTo("Passed"));
            Assert.That(combinedNode.Attributes["total"].Value, Is.EqualTo("65"));
            Assert.That(combinedNode.Attributes["passed"].Value, Is.EqualTo("54"));
            Assert.That(combinedNode.Attributes["failed"].Value, Is.EqualTo("4"));
            Assert.That(combinedNode.Attributes["inconclusive"].Value, Is.EqualTo("5"));
            Assert.That(combinedNode.Attributes["skipped"].Value, Is.EqualTo("2"));
            Assert.That(combinedNode.Attributes["asserts"].Value, Is.EqualTo("93"));

            Assert.That(combinedNode.Attributes["run-date"].Value, Is.EqualTo("2011-07-04"));
            Assert.That(combinedNode.Attributes["start-time"].Value, Is.EqualTo("12:34:56"));
        }

        [Test]
        public void CanMakeTestSuiteResult()
        {
            TestEngineResult combined = TestEngineResult.MakeProjectResult(new TestPackage("dummy.dll"), twoResults);
            Assert.That(combined.IsSingle);

            XmlNode combinedNode = combined.Xml;

            Assert.That(combinedNode.Name, Is.EqualTo("test-suite"));
            Assert.That(combinedNode.Attributes["type"].Value, Is.EqualTo("Project"));
            Assert.That(combinedNode.Attributes["result"].Value, Is.EqualTo("Failed"));
            Assert.That(combinedNode.Attributes["total"].Value, Is.EqualTo("65"));
            Assert.That(combinedNode.Attributes["passed"].Value, Is.EqualTo("54"));
            Assert.That(combinedNode.Attributes["failed"].Value, Is.EqualTo("4"));
            Assert.That(combinedNode.Attributes["inconclusive"].Value, Is.EqualTo("5"));
            Assert.That(combinedNode.Attributes["skipped"].Value, Is.EqualTo("2"));
            Assert.That(combinedNode.Attributes["asserts"].Value, Is.EqualTo("93"));
        }

        //[Test]
        //public void XmlNodesCanBeAggregated()
        //{
        //    DateTime startTime = new DateTime(2011, 07, 04, 12, 34, 56);

        //    XmlDocument doc1 = new XmlDocument();
        //    doc1.LoadXml(resultText1);

        //    XmlDocument doc2 = new XmlDocument();
        //    doc2.LoadXml(resultText2);

        //    List<XmlNode> nodes = new List<XmlNode>();
        //    nodes.Add(doc1.FirstChild);
        //    nodes.Add(doc2.FirstChild);
            
        //    XmlNode combined = TestEngineResult.Aggregate("test-run", null, new TestPackage("dummy.dll"), nodes);

        //    Assert.That(combined.Name, Is.EqualTo("test-run"));
        //    Assert.That(combined.Attributes["result"].Value, Is.EqualTo("Failed"));
        //    Assert.That(combined.Attributes["total"].Value, Is.EqualTo("65"));
        //    Assert.That(combined.Attributes["passed"].Value, Is.EqualTo("54"));
        //    Assert.That(combined.Attributes["failed"].Value, Is.EqualTo("4"));
        //    Assert.That(combined.Attributes["inconclusive"].Value, Is.EqualTo("5"));
        //    Assert.That(combined.Attributes["skipped"].Value, Is.EqualTo("2"));
        //    Assert.That(combined.Attributes["asserts"].Value, Is.EqualTo("93"));

        //    //Assert.That(combined.Attributes["run-date"].Value, Is.EqualTo("2011-07-04"));
        //    //Assert.That(combined.Attributes["start-time"].Value, Is.EqualTo("12:34:56"));
        //}
    }
}
