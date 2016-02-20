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
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using NUnit.Tests.Assemblies;

namespace NUnit.Engine.Services.ResultWriters.Tests
{
    public class NUnit2XmlResultWriterTests : XmlOutputTest
    {
        private XmlDocument _doc;
        private XmlNode _topNode;
        private XmlNode _envNode;
        private XmlNode _cultureNode;
        private XmlNode _fixtureNode;
        private XmlNode _testCaseNode;
        private XmlNode _invalidTestCaseNode;

        [OneTimeSetUp]
        public void ConvertEngineResultToXml()
        {
            ServiceContext services = new ServiceContext();
            services.Add(new ExtensionService());
            ResultService resultService = new ResultService();
            services.Add(resultService);
            services.ServiceManager.StartServices();

            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                var nunit2Writer = resultService.GetResultWriter("nunit2", null);
                Assert.NotNull(nunit2Writer, "Unable to get nunit2 result writer");
                nunit2Writer.WriteResultFile(EngineResult.Xml, writer);
            }

            _doc = new XmlDocument();
            _doc.LoadXml(sb.ToString());

            _topNode = _doc.SelectSingleNode("/test-results");
            Assert.NotNull(_topNode, "Test-results element not found");

            _envNode = _topNode.SelectSingleNode("environment");
            Assert.NotNull(_envNode, "Environment element not found");

            _cultureNode = _topNode.SelectSingleNode("culture-info");
            Assert.NotNull(_topNode, "CultureInfo element not found");

            _fixtureNode = _topNode.SelectSingleNode("descendant::test-suite[@name='MockTestFixture']");
            Assert.NotNull(_fixtureNode, "MockTestFixture element not found");

            _testCaseNode = _fixtureNode.SelectSingleNode("descendant::test-case[@name='NUnit.Tests.Assemblies.MockTestFixture.TestWithDescription']");
            Assert.NotNull(_testCaseNode, "TestWithDescription element not found");

            _invalidTestCaseNode = _fixtureNode.SelectSingleNode("descendant::test-case[@name='NUnit.Tests.Assemblies.MockTestFixture.NonPublicTest']");
            Assert.NotNull(_invalidTestCaseNode, "NonPublicTest element not found");
        }

        #region Document Level Tests

        [Test]
        public void Document_HasThreeChildren()
        {
            Assert.That(_doc.ChildNodes.Count, Is.EqualTo(3));
        }

        [Test]
        public void Document_FirstChildIsXmlDeclaration()
        {
            Assume.That(_doc.FirstChild != null);
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

        #endregion

        #region TestResults Element Tests

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(_topNode, "name"), Is.EqualTo(AssemblyPath));
        }

        [TestCase("total", MockAssembly.Tests)]
        [TestCase("errors", MockAssembly.Errors)]
        [TestCase("failures", MockAssembly.Failures)]
        [TestCase("inconclusive", MockAssembly.Inconclusive)]
        [TestCase("not-run", MockAssembly.NotRun)]
        [TestCase("ignored", MockAssembly.Ignored)]
        [TestCase("skipped", MockAssembly.Explicit)]
        [TestCase("invalid", MockAssembly.NotRunnable)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(_topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidDateAttribute()
        {
            string dateString = RequiredAttribute(_topNode, "date");
#if !NETCF
            DateTime date;
            Assert.That(DateTime.TryParse(dateString, out date), "Invalid date attribute: {0}", dateString);
#endif
        }

        [Test]
        public void TestResults_HasValidTimeAttribute()
        {
            string timeString = RequiredAttribute(_topNode, "time");
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
            Assert.That(_envNode, Is.Not.Null, "Missing environment element");
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
            RequiredAttribute(_envNode, name);
        }

        #endregion

        #region CultureInfo Element Tests

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
            Assert.That(RequiredAttribute(_fixtureNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestFixture_HasValidTimeAttribute()
        {
#if NETCF
            RequiredAttribute(suiteNode, "time");
#else
            double time;
            // NOTE: We use the TryParse overload with 4 args because it's supported in .NET 1.1
            Assert.That(double.TryParse(RequiredAttribute(_fixtureNode, "time"),System.Globalization.NumberStyles.Float,null, out time), "Invalid value for time");
#endif
        }

        [Test]
        public void TestFixture_ResultIsFailure()
        {
            Assert.That(RequiredAttribute(_fixtureNode, "result"), Is.EqualTo("Failure"));
        }

        [Test]
        public void TestFixture_SuccessIsFalse()
        {
            Assert.That(RequiredAttribute(_fixtureNode, "success"), Is.EqualTo("False"));
        }

        [Test]
        public void TestFixture_HasCategoryElement()
        {
            var categoryNode = _fixtureNode.SelectSingleNode("descendant::category[@name='FixtureCategory']");
            Assert.That(categoryNode, Is.Not.Null);
        }

        [Test]
        public void TestFixture_HasPropertyElement()
        {
            var propertyNode = _fixtureNode.SelectSingleNode("descendant::property[@name='Description']");
            Assert.That(propertyNode, Is.Not.Null);
            Assert.That(RequiredAttribute(propertyNode, "value"), Is.EqualTo("Fake Test Fixture"));
        }

        #endregion

        #region test-case tests

        [Test]
        public void TestCase_ResultIsSuccess()
        {
            Assert.That(RequiredAttribute(_testCaseNode, "result"), Is.EqualTo("Success"));
        }

        [Test]
        public void TestCase_ExecutedIsTrue()
        {
            Assert.That(RequiredAttribute(_testCaseNode, "executed"), Is.EqualTo("True"));
        }

        [Test]
        public void TestCase_HasCategoryElement()
        {
            var categoryNode = _testCaseNode.SelectSingleNode("descendant::category[@name='MockCategory']");
            Assert.That(categoryNode, Is.Not.Null);
        }

        [Test]
        public void TestCase_HasPropertyElement()
        {
            var propertyNode = _testCaseNode.SelectSingleNode("descendant::property[@name='Severity']");
            Assert.That(propertyNode, Is.Not.Null);
            Assert.That(RequiredAttribute(propertyNode, "value"), Is.EqualTo("Critical"));
        }

        [Test]
        public void TestCase_CategoryElementsDoNotContainProperties()
        {
            var categoryNodes = _testCaseNode.SelectNodes("descendant::category");
            Assert.That(categoryNodes, Is.Not.Null);
            Assert.That(categoryNodes.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestCase_PropertyElementsDoNotContainCategories()
        {
            var propertyNodes = _testCaseNode.SelectNodes("descendant::property");
            Assert.That(propertyNodes, Is.Not.Null);
            Assert.That(propertyNodes.Count, Is.EqualTo(2));
        }

        [Test]
        public void InvalidTestCase_HasFailedResult()
        {
            Assert.That(RequiredAttribute(_invalidTestCaseNode, "result"), Is.EqualTo("Failure"));
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
