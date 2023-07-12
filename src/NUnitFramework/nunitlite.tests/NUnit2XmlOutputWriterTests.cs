// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private XmlDocument _doc;
        private XmlNode _topNode;
        private XmlNode _envNode;
        private XmlNode _cultureNode;
        private XmlNode _suiteNode;

        [OneTimeSetUp]
        public void RunMockAssemblyTests()
        {
            ITestResult result = NUnit.TestUtilities.TestBuilder.RunTestFixture(typeof(MockTestFixture));
            Assert.That(result,Is.Not.Null);

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            new NUnit2XmlOutputWriter().WriteResultFile(result, writer, null, null);
            writer.Close();

#if DEBUG
            StreamWriter sw = new StreamWriter("MockAssemblyResult.xml");
            sw.WriteLine(sb.ToString());
            sw.Close();
#endif

            _doc = new XmlDocument();
            _doc.LoadXml(sb.ToString());

            _topNode = _doc.SelectSingleNode("/test-results");
            if (_topNode is not null)
            {
                _envNode = _topNode.SelectSingleNode("environment");
                _cultureNode = _topNode.SelectSingleNode("culture-info");
                _suiteNode = _topNode.SelectSingleNode("test-suite");
            }
        }

        [Test]
        public void Document_HasThreeChildren()
        {
            Assert.That(_doc.ChildNodes.Count, Is.EqualTo(3));
        }

        [Test]
        public void Document_FirstChildIsXmlDeclaration()
        {
            Assume.That(_doc.FirstChild is not null);
            Assert.That(_doc.FirstChild.NodeType, Is.EqualTo(XmlNodeType.XmlDeclaration));
            Assert.That(_doc.FirstChild.Name, Is.EqualTo("xml"));
        }

        [Test]
        public void Document_SecondChildIsComment()
        {
            Assume.That(_doc.ChildNodes.Count >= 2);
            Assert.That(_doc.ChildNodes[1].Name, Is.EqualTo("#comment"));
        }

        [Test]
        public void Document_ThirdChildIsTestResults()
        {
            Assume.That(_doc.ChildNodes.Count >= 3);
            Assert.That(_doc.ChildNodes[2].Name, Is.EqualTo("test-results"));
        }

        [Test]
        public void Document_HasTestResults()
        {
            Assert.That(_topNode, Is.Not.Null);
            Assert.That(_topNode.Name, Is.EqualTo("test-results"));
        }

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(_topNode, "name"), Is.EqualTo("NUnit.Tests.Assemblies.MockTestFixture"));
        }

        [TestCase("total", MockTestFixture.Tests)]
        [TestCase("errors", MockTestFixture.Failed_Error)]
        [TestCase("failures", MockTestFixture.Failed_Other)]
        [TestCase("inconclusive", MockTestFixture.Inconclusive)]
        [TestCase("not-run", MockTestFixture.Skipped+MockTestFixture.Failed_NotRunnable)]
        [TestCase("ignored", MockTestFixture.Skipped_Ignored)]
        [TestCase("skipped", MockTestFixture.Skipped-MockTestFixture.Skipped_Ignored-MockTestFixture.Skipped_Explicit)]
        [TestCase("invalid", MockTestFixture.Failed_NotRunnable)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(_topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidDateAttribute()
        {
            string dateString = RequiredAttribute(_topNode, "date");
            Assert.That(DateTime.TryParse(dateString, out _), "Invalid date attribute: {0}", dateString);
        }

        [Test]
        public void TestResults_HasValidTimeAttribute()
        {
            string timeString = RequiredAttribute(_topNode, "time");
            Assert.That(DateTime.TryParse(timeString, out _), "Invalid time attribute: {0}", timeString);
        }

        [Test]
        public void Environment_HasEnvironmentElement()
        {
            Assert.That(_envNode, Is.Not.Null, "Missing environment element");
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
            RequiredAttribute(_envNode, name);
        }

        [Test]
        public void CultureInfo_HasCultureInfoElement()
        {
            Assert.That(_cultureNode, Is.Not.Null, "Missing culture-info element");
        }

        [TestCase("current-culture")]
        [TestCase("current-uiculture")]
        public void CultureInfo_HasRequiredAttribute(string name)
        {
            string cultureName = RequiredAttribute(_cultureNode, name);
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
            Assert.That(_suiteNode, Is.Not.Null, "Missing test-suite element");
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
            Assert.That(RequiredAttribute(_suiteNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestSuite_HasValidTimeAttribute()
        {
            var timeString = RequiredAttribute(_suiteNode, "time");
            // NOTE: We use the TryParse overload with 4 args because it's supported in .NET 1.1
            var success = double.TryParse(timeString, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out _);
            Assert.That(success, "{0} is an invalid value for time", timeString);
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
