// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !NETCOREAPP1_1
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Tests.Assemblies;

namespace NUnitLite.Tests
{
    public class NUnit3XmlOutputWriterTests
    {
        private XmlDocument _doc;
        private XmlNode _topNode;
        private XmlNode _envNode;
        private XmlNode _suiteNode;

        [OneTimeSetUp]
        public void RunMockAssemblyTests()
        {
            ITestResult result = NUnit.TestUtilities.TestBuilder.RunTestFixture(typeof(MockTestFixture));
            Assert.NotNull(result);

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            new NUnit3XmlOutputWriter().WriteResultFile(result, writer, null, null);
            writer.Close();

#if DEBUG
            StreamWriter sw = new StreamWriter("MockAssemblyResult.xml");
            sw.WriteLine(sb.ToString());
            sw.Close();
#endif

            _doc = new XmlDocument();
            _doc.LoadXml(sb.ToString());

            _topNode = _doc.SelectSingleNode("/test-run");
            if (_topNode is not null)
            {
                _suiteNode = _topNode.SelectSingleNode("test-suite");
                _envNode = _suiteNode.SelectSingleNode("environment");
            }
        }

        [Test]
        public void Document_HasTwoChildren()
        {
            Assert.That(_doc.ChildNodes.Count, Is.EqualTo(2));
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
            Assert.That(_doc.ChildNodes[1].Name, Is.EqualTo("test-run"));
        }

        [Test]
        public void Document_HasTestResults()
        {
            Assert.That(_topNode, Is.Not.Null);
            Assert.That(_topNode.Name, Is.EqualTo("test-run"));
        }

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(_topNode, "fullname"), Is.EqualTo("NUnit.Tests.Assemblies.MockTestFixture"));
            Assert.That(RequiredAttribute(_topNode, "name"), Is.EqualTo("MockTestFixture"));
        }

        [TestCase("testcasecount", MockTestFixture.Tests)]
        [TestCase("passed", MockTestFixture.Passed)]
        [TestCase("failed", MockTestFixture.Failed)]
        [TestCase("inconclusive", MockTestFixture.Inconclusive)]
        [TestCase("skipped", MockTestFixture.Skipped)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(_topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidStartTimeAttribute()
        {
            string startTimeString = RequiredAttribute(_topNode, "start-time");
            Assert.That(DateTime.TryParseExact(startTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Invalid start time attribute: {0}. Expecting DateTime in 'o' format.", startTimeString);
        }

        [Test]
        public void TestResults_HasValidEndTimeAttribute()
        {
            string endTimeString = RequiredAttribute(_topNode, "end-time");
            Assert.That(DateTime.TryParseExact(endTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Invalid end time attribute: {0}. Expecting DateTime in 'o' format.", endTimeString);
        }

        [Test]
        public void Environment_HasEnvironmentElement()
        {
            Assert.That(_envNode, Is.Not.Null, "Missing environment element");
        }

        [TestCase("framework-version")]
        [TestCase("clr-version")]
        [TestCase("os-version")]
        [TestCase("platform")]
        [TestCase("cwd")]
        [TestCase("machine-name")]
        [TestCase("user")]
        [TestCase("user-domain")]
        [TestCase("os-architecture")]
        public void Environment_HasRequiredAttribute(string name)
        {
            RequiredAttribute(_envNode, name);
        }

        [Test]
        public void CultureInfo_HasCultureInfoElement()
        {
            Assert.That(_envNode.Attributes["culture"], Is.Not.Null, "Missing culture-info attribute");
        }

        [TestCase("culture")]
        [TestCase("uiculture")]
        public void CultureInfo_HasRequiredAttribute(string name)
        {
            string cultureName = RequiredAttribute(_envNode, name);
            System.Globalization.CultureInfo culture = null;

            try
            {
                culture = System.Globalization.CultureInfo.CreateSpecificCulture(cultureName);
            }
            catch (ArgumentException)
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
        [TestCase("fullname", "NUnit.Tests.Assemblies.MockTestFixture")]
        [TestCase("classname", "NUnit.Tests.Assemblies.MockTestFixture")]
        [TestCase("runstate", "Runnable")]
        [TestCase("result", "Failed")]
        [TestCase("asserts", "0")]
        public void TestSuite_ExpectedAttribute(string name, string value)
        {
            Assert.That(RequiredAttribute(_suiteNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestSuite_HasValidStartTimeAttribute()
        {
            var startTimeString = RequiredAttribute(_suiteNode, "start-time");

            var success = DateTime.TryParse(startTimeString, out _);
            Assert.That(success, "{0} is an invalid value for start time", startTimeString);
        }

        [Test]
        public void TestSuite_HasValidEndTimeAttribute()
        {
            var endTimeString = RequiredAttribute(_suiteNode, "end-time");

            var success = DateTime.TryParse(endTimeString, out _);
            Assert.That(success, "{0} is an invalid value for end time", endTimeString);
        }

        [Test]
        public void IgnoredTestCases_HaveValidStartAndEndTimeAttributes()
        {
            DateTime.TryParse(RequiredAttribute(_topNode, "start-time"), out var testRunStartTime);
            DateTime.TryParse(RequiredAttribute(_topNode, "end-time"), out var testRunEndTime);

            var testCaseNodes = _suiteNode.SelectNodes("test-suite[@name='SkippedTest']/test-case");
            Assert.That(testCaseNodes, Is.Not.Null.And.Count.EqualTo(3));

            foreach (XmlNode testCase in testCaseNodes)
            {
                string startTimeStr = RequiredAttribute(testCase, "start-time");
                string endTimeStr = RequiredAttribute(testCase, "end-time");

                Assert.That(startTimeStr, Does.EndWith("Z"), "Ignored start-time is not UTC");
                Assert.That(endTimeStr, Does.EndWith("Z"), "Ignored end-time is not UTC");

                Assert.IsTrue(DateTime.TryParse(startTimeStr, out var startTime));
                Assert.IsTrue(DateTime.TryParse(endTimeStr, out var endTime));

                Assert.That(startTime, Is.InRange(testRunStartTime, testRunEndTime), "Ignored test cases should be set to approximately the start time of test suite");
                Assert.That(endTime, Is.InRange(testRunStartTime, testRunEndTime), "Ignored test cases should be set to approximately the end time of test suite");
            }
        }

        [Test]
        public void ExplicitTest_HasValidStartAndEndTimeAttributes()
        {
            DateTime.TryParse(RequiredAttribute(_topNode, "start-time"), out var testRunStartTime);
            DateTime.TryParse(RequiredAttribute(_topNode, "end-time"), out var testRunEndTime);

            var testCaseNodes = _suiteNode.SelectNodes("test-case[@name='ExplicitTest']");
            Assert.That(testCaseNodes, Is.Not.Null.And.Count.EqualTo(1));

            XmlNode testCase = testCaseNodes[0];

            string startTimeStr = RequiredAttribute(testCase, "start-time");
            string endTimeStr = RequiredAttribute(testCase, "end-time");

            Assert.That(startTimeStr, Does.EndWith("Z"), "Explicit start-time is not UTC");
            Assert.That(endTimeStr, Does.EndWith("Z"), "Explicit end-time is not UTC");

            Assert.IsTrue(DateTime.TryParse(startTimeStr, out var startTime));
            Assert.IsTrue(DateTime.TryParse(endTimeStr, out var endTime));

            Assert.That(startTime, Is.InRange(testRunStartTime, testRunEndTime), "Explicit test cases should be set to approximately the start time of test suite");
            Assert.That(endTime, Is.InRange(testRunStartTime, testRunEndTime), "Explicit test cases should be set to approximately the end time of test suite");
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
