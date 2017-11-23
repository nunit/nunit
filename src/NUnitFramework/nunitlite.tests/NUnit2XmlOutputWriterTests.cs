// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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

#if !NETCOREAPP1_1
using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Tests.Assemblies;

namespace NUnitLite.Tests
{
    public class NUnit2XmlOutputWriterTests
    {
        private XmlDocument doc;
        private XmlNode topNode;
        private XmlNode envNode;
        private XmlNode cultureNode;
        private XmlNode suiteNode;

        [OneTimeSetUp]
        public void RunMockAssemblyTests()
        {
            ITestResult result = NUnit.TestUtilities.TestBuilder.RunTestFixture(typeof(MockTestFixture));
            Assert.NotNull(result);

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            new NUnit2XmlOutputWriter().WriteResultFile(result, writer, null, null);
            writer.Close();

#if DEBUG
            StreamWriter sw = new StreamWriter("MockAssemblyResult.xml");
            sw.WriteLine(sb.ToString());
            sw.Close();
#endif

            doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            topNode = doc.SelectSingleNode("/test-results");
            if (topNode != null)
            {
                envNode = topNode.SelectSingleNode("environment");
                cultureNode = topNode.SelectSingleNode("culture-info");
                suiteNode = topNode.SelectSingleNode("test-suite");
            }
        }

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

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(topNode, "name"), Is.EqualTo("NUnit.Tests.Assemblies.MockTestFixture"));
        }

        [TestCase("total", MockTestFixture.Tests)]
        [TestCase("errors", MockTestFixture.Failed_Error)]
        [TestCase("failures", MockTestFixture.Failed_Other)]
        [TestCase("inconclusive", MockTestFixture.Inconclusive)]
        [TestCase("not-run", MockTestFixture.Skipped+MockTestFixture.Failed_NotRunnable-MockTestFixture.Skipped_Explicit)]
        [TestCase("ignored", MockTestFixture.Skipped_Ignored)]
        [TestCase("skipped", MockTestFixture.Skipped-MockTestFixture.Skipped_Ignored-MockTestFixture.Skipped_Explicit)]
        [TestCase("invalid", MockTestFixture.Failed_NotRunnable)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidDateAttribute()
        {
            string dateString = RequiredAttribute(topNode, "date");
            DateTime date;
            Assert.That(DateTime.TryParse(dateString, out date), "Invalid date attribute: {0}", dateString);
        }

        [Test]
        public void TestResults_HasValidTimeAttribute()
        {
            string timeString = RequiredAttribute(topNode, "time");
            DateTime time;
            Assert.That(DateTime.TryParse(timeString, out time), "Invalid time attribute: {0}", timeString);
        }

        [Test]
        public void Environment_HasEnvironmentElement()
        {
            Assert.That(envNode, Is.Not.Null, "Missing environment element");
        }

        [TestCase("nunit-version")]
        [TestCase("clr-version")]
        [TestCase("os-version")]
        [TestCase("platform")]
        [TestCase("cwd")]
        [TestCase("machine-name")]
        [TestCase("user")]
        [TestCase("user-domain")]
        public void Environment_HasRequiredAttribute(string name)
        {
            RequiredAttribute(envNode, name);
        }

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

        [Test]
        public void TestSuite_HasTestSuiteElement()
        {
            Assert.That(suiteNode, Is.Not.Null, "Missing test-suite element");
        }

        [TestCase("type", "TestFixture")]
        [TestCase("name", "MockTestFixture")]
        [TestCase("description", "Fake Test Fixture")]
        [TestCase("executed", "True")]
        [TestCase("result", "Failure")]
        [TestCase("success", "False")]
        [TestCase("asserts", "0")]
        public void TestSuite_ExpectedAttribute(string name, string value)
        {
            Assert.That(RequiredAttribute(suiteNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestSuite_HasValidTimeAttribute()
        {
            double time;
            var timeString = RequiredAttribute(suiteNode, "time");
            // NOTE: We use the TryParse overload with 4 args because it's supported in .NET 1.1
            var success = double.TryParse(timeString,System.Globalization.NumberStyles.Float,System.Globalization.NumberFormatInfo.InvariantInfo, out time);
            Assert.That(success, "{0} is an invalid value for time", timeString);
        }

        [Test]
        public void TestSuite_ResultIsFailure()
        {
        }

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
#endif
