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

using System;
using System.Xml;

namespace NUnit.Engine.Internal.Tests
{
    using Common;
    using Framework;

    public class ResultHelperTests
    {
        private const string resultText1 = "<test-assembly result=\"Passed\" total=\"23\" passed=\"23\" failed=\"0\" inconclusive=\"0\" skipped=\"0\" asserts=\"40\" />";
        private const string resultText2 = "<test-assembly result=\"Failed\" total=\"42\" passed=\"31\" failed=\"4\" inconclusive=\"5\" skipped=\"2\" asserts=\"53\" />";

        private TestEngineResult result1;
        private TestEngineResult result2;

        private TestEngineResult[] twoResults;

        private XmlNode[] twoNodes;

        [SetUp]
        public void SetUp()
        {
            result1 = new TestEngineResult(resultText1);
            result2 = new TestEngineResult(resultText2);
            twoResults = new TestEngineResult[] { result1, result2 };
            twoNodes = new XmlNode[] { result1.Xml, result2.Xml };
        }

        [Test]
        public void MergeTestResults()
        {
            TestEngineResult mergedResult = ResultHelper.Merge(twoResults);

            Assert.That(mergedResult.XmlNodes.Count, Is.EqualTo(2));
            Assert.That(mergedResult.XmlNodes[0].OuterXml, Is.EqualTo(resultText1));
            Assert.That(mergedResult.XmlNodes[1].OuterXml, Is.EqualTo(resultText2));
        }

        [Test]
        public void AggregateTestResult()
        {
            TestEngineResult combined = result2.Aggregate("test-run", "NAME", "FULLNAME");
            Assert.That(combined.IsSingle);

            XmlNode combinedNode = combined.Xml;

            Assert.That(combinedNode.Name, Is.EqualTo("test-run"));
            Assert.That(combinedNode.Attributes["result"].Value, Is.EqualTo("Failed"));
            Assert.That(combinedNode.Attributes["total"].Value, Is.EqualTo("42"));
            Assert.That(combinedNode.Attributes["passed"].Value, Is.EqualTo("31"));
            Assert.That(combinedNode.Attributes["failed"].Value, Is.EqualTo("4"));
            Assert.That(combinedNode.Attributes["inconclusive"].Value, Is.EqualTo("5"));
            Assert.That(combinedNode.Attributes["skipped"].Value, Is.EqualTo("2"));
            Assert.That(combinedNode.Attributes["asserts"].Value, Is.EqualTo("53"));
        }

        [Test]
        public void MergeAndAggregateTestResults()
        {
            TestEngineResult combined = ResultHelper.Merge(twoResults).Aggregate("test-suite", "Project", "NAME", "FULLNAME");
            Assert.That(combined.IsSingle);

            XmlNode combinedNode = combined.Xml;

            Assert.That(combinedNode.Name, Is.EqualTo("test-suite"));
            Assert.That(combinedNode.Attributes["type"].Value, Is.EqualTo("Project"));
            Assert.That(combinedNode.Attributes["name"].Value, Is.EqualTo("NAME"));
            Assert.That(combinedNode.Attributes["fullname"].Value, Is.EqualTo("FULLNAME"));
            Assert.That(combinedNode.Attributes["result"].Value, Is.EqualTo("Failed"));
            Assert.That(combinedNode.Attributes["total"].Value, Is.EqualTo("65"));
            Assert.That(combinedNode.Attributes["passed"].Value, Is.EqualTo("54"));
            Assert.That(combinedNode.Attributes["failed"].Value, Is.EqualTo("4"));
            Assert.That(combinedNode.Attributes["inconclusive"].Value, Is.EqualTo("5"));
            Assert.That(combinedNode.Attributes["skipped"].Value, Is.EqualTo("2"));
            Assert.That(combinedNode.Attributes["asserts"].Value, Is.EqualTo("93"));
        }

        [Test]
        public void AggregateXmlNodes()
        {
            DateTime startTime = new DateTime(2011, 07, 04, 12, 34, 56);

            XmlNode combined = ResultHelper.Aggregate("test-run", "NAME", "FULLNAME", twoNodes);

            Assert.That(combined.Name, Is.EqualTo("test-run"));
            Assert.That(combined.Attributes["name"].Value, Is.EqualTo("NAME"));
            Assert.That(combined.Attributes["fullname"].Value, Is.EqualTo("FULLNAME"));
            Assert.That(combined.Attributes["result"].Value, Is.EqualTo("Failed"));
            Assert.That(combined.Attributes["total"].Value, Is.EqualTo("65"));
            Assert.That(combined.Attributes["passed"].Value, Is.EqualTo("54"));
            Assert.That(combined.Attributes["failed"].Value, Is.EqualTo("4"));
            Assert.That(combined.Attributes["inconclusive"].Value, Is.EqualTo("5"));
            Assert.That(combined.Attributes["skipped"].Value, Is.EqualTo("2"));
            Assert.That(combined.Attributes["asserts"].Value, Is.EqualTo("93"));
        }

        [Test]
        public void InsertEnvironmentElement()
        {
            result1.Xml.InsertEnvironmentElement();

            var env = result1.Xml.SelectSingleNode("environment");
            Assert.NotNull(env);

            Assert.NotNull(env.GetAttribute("nunit-version"));
            Assert.NotNull(env.GetAttribute("clr-version"));
            Assert.NotNull(env.GetAttribute("os-version"));
            Assert.NotNull(env.GetAttribute("platform"));
            Assert.NotNull(env.GetAttribute("cwd"));
            Assert.NotNull(env.GetAttribute("machine-name"));
            Assert.NotNull(env.GetAttribute("user"));
            Assert.NotNull(env.GetAttribute("user-domain"));
            Assert.NotNull(env.GetAttribute("culture"));
            Assert.NotNull(env.GetAttribute("uiculture"));
        }
    }
}
