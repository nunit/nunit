﻿// ***********************************************************************
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
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Engine.Services;
using NUnit.Framework;
using NUnit.Tests.Assemblies;

namespace NUnit.Engine.Tests.Services.ResultWriters
{
    public class NUnit2XmlResultWriterTests : XmlOutputTest
    {
        private XmlDocument doc;
        private XmlNode topNode;
        private XmlNode envNode;
        private XmlNode cultureNode;
        private XmlNode fixtureNode;

        [OneTimeSetUp]
        public void ConvertEngineResultToXml()
        {
            var sb = new StringBuilder();
            var service = new ResultService();
            service.InitializeService();
            using (var writer = new StringWriter(sb))
            {
                service.GetResultWriter("nunit2", null).WriteResultFile(EngineResult.Xml, writer);
            }

            doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            topNode = doc.SelectSingleNode("/test-results");
            Assert.NotNull(topNode, "Test-results element not found");

            envNode = topNode.SelectSingleNode("environment");
            Assert.NotNull(envNode, "Environment element not found");

            cultureNode = topNode.SelectSingleNode("culture-info");
            Assert.NotNull(topNode, "CultureInfo element not found");

            fixtureNode = topNode.SelectSingleNode("descendant::test-suite[@name='MockTestFixture']");
            Assert.NotNull(fixtureNode, "MockTestFixture element not found");
        }

        #region Document Level Tests

        [Test]
        public void Document_HasThreeChildren()
        {
            Assert.That(doc.ChildNodes.Count, Is.EqualTo(3));
        }

        [Test]
        public void Document_FirstChildIsXmlDeclaration()
        {
            Assume.That(doc.FirstChild != null);
            Assert.That(doc.FirstChild.NodeType, Is.EqualTo(XmlNodeType.XmlDeclaration));
            Assert.That(doc.FirstChild.Name, Is.EqualTo("xml"));
        }

        [Test]
        public void Document_SecondChildIsComment()
        {
            Assume.That(doc.ChildNodes.Count >= 2);
            Assert.That(doc.ChildNodes[1].Name, Is.EqualTo("#comment"));
        }

        [Test]
        public void Document_ThirdChildIsTestResults()
        {
            Assume.That(doc.ChildNodes.Count >= 3);
            Assert.That(doc.ChildNodes[2].Name, Is.EqualTo("test-results"));
        }

        [Test]
        public void Document_HasTestResults()
        {
            Assert.That(topNode, Is.Not.Null);
            Assert.That(topNode.Name, Is.EqualTo("test-results"));
        }

        #endregion

        #region TestResults Element Tests

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(topNode, "name"), Is.EqualTo(AssemblyPath));
        }

        [TestCase("total", MockAssembly.Tests-MockAssembly.Explicit)]
        [TestCase("errors", MockAssembly.Errors)]
        [TestCase("failures", MockAssembly.Failures)]
        [TestCase("inconclusive", MockAssembly.Inconclusive)]
        [TestCase("not-run", MockAssembly.NotRun-MockAssembly.Explicit)]
        [TestCase("ignored", MockAssembly.Ignored)]
        [TestCase("skipped", 0)]
        [TestCase("invalid", MockAssembly.NotRunnable)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidDateAttribute()
        {
            string dateString = RequiredAttribute(topNode, "date");
#if !NETCF
            DateTime date;
            Assert.That(DateTime.TryParse(dateString, out date), "Invalid date attribute: {0}", dateString);
#endif
        }

        [Test]
        public void TestResults_HasValidTimeAttribute()
        {
            string timeString = RequiredAttribute(topNode, "time");
#if !NETCF
            DateTime time;
            Assert.That(DateTime.TryParse(timeString, out time), "Invalid time attribute: {0}", timeString);
#endif
        }

        #endregion

        #region Environment Element Tests

        [Test]
        public void Environment_HasEnvironmentElement()
        {
            Assert.That(envNode, Is.Not.Null, "Missing environment element");
        }

        [TestCase("nunit-version")]
        [TestCase("clr-version")]
        [TestCase("os-version")]
        [TestCase("platform")]
#if !NETCF
        [TestCase("cwd")]
        [TestCase("machine-name")]
        [TestCase("user")]
        [TestCase("user-domain")]
#endif
        public void Environment_HasRequiredAttribute(string name)
        {
            RequiredAttribute(envNode, name);
        }

        #endregion

        #region CultureInfo Element Tests

        [Test]
        public void CultureInfo_HasCultureInfoElement()
        {
            Assert.That(cultureNode, Is.Not.Null, "Missing culture-info element");
        }

        [TestCase("current-culture")]
        [TestCase("current-uiculture")]
        public void CultureInfo_HasRequiredAttribute(string name)
        {
            string cultureName = RequiredAttribute(cultureNode, name);
            System.Globalization.CultureInfo culture = null;

            try
            {
                culture = System.Globalization.CultureInfo.CreateSpecificCulture(cultureName);
            }
            catch(ArgumentException)
            {
                // Do nothing - culture will be null
            }

            Assert.That(culture, Is.Not.Null, "Invalid value for {0}: {1}", name, cultureName);
        }

        #endregion

        #region MockTestFixture Tests

        [TestCase("type", "TestFixture")]
        [TestCase("name", "MockTestFixture")]
        [TestCase("description", "Fake Test Fixture")]
        [TestCase("executed", "True")]
        [TestCase("result", "Failure")]
        [TestCase("success", "False")]
        [TestCase("asserts", "0")]
        public void TestFixture_ExpectedAttribute(string name, string value)
        {
            Assert.That(RequiredAttribute(fixtureNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestFixture_HasValidTimeAttribute()
        {
#if NETCF
            RequiredAttribute(suiteNode, "time");
#else
            double time;
            // NOTE: We use the TryParse overload with 4 args because it's supported in .NET 1.1
            Assert.That(double.TryParse(RequiredAttribute(fixtureNode, "time"),System.Globalization.NumberStyles.Float,null, out time), "Invalid value for time");
#endif
        }

        [Test]
        public void TestFixture_ResultIsFailure()
        {
        }

        #endregion

        #region Helper Methods

        private string RequiredAttribute(XmlNode node, string name)
        {
            XmlAttribute attr = node.Attributes[name];
            Assert.That(attr, Is.Not.Null, "Missing attribute {0} on element {1}", name, node.Name);

            return attr.Value;
        }

        #endregion
    }
}
