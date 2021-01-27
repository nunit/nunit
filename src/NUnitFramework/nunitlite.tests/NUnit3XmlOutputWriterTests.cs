// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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
        private XmlDocument doc;
        private XmlNode topNode;
        private XmlNode envNode;
        private XmlNode suiteNode;

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

            doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            topNode = doc.SelectSingleNode("/test-run");
            if (topNode != null)
            {
                suiteNode = topNode.SelectSingleNode("test-suite");
                envNode = suiteNode.SelectSingleNode("environment");
            }
        }

        [Test]
        public void Document_HasTwoChildren()
        {
            Assert.That(doc.ChildNodes.Count, Is.EqualTo(2));
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
            Assert.That(doc.ChildNodes[1].Name, Is.EqualTo("test-run"));
        }

        [Test]
        public void Document_HasTestResults()
        {
            Assert.That(topNode, Is.Not.Null);
            Assert.That(topNode.Name, Is.EqualTo("test-run"));
        }

        [Test]
        public void TestResults_AssemblyPathIsCorrect()
        {
            Assert.That(RequiredAttribute(topNode, "fullname"), Is.EqualTo("NUnit.Tests.Assemblies.MockTestFixture"));
            Assert.That(RequiredAttribute(topNode, "name"), Is.EqualTo("MockTestFixture"));
        }

        [TestCase("testcasecount", MockTestFixture.Tests)]
        [TestCase("passed", MockTestFixture.Passed)]
        [TestCase("failed", MockTestFixture.Failed)]
        [TestCase("inconclusive", MockTestFixture.Inconclusive)]
        [TestCase("skipped", MockTestFixture.Skipped)]
        public void TestResults_CounterIsCorrect(string name, int count)
        {
            Assert.That(RequiredAttribute(topNode, name), Is.EqualTo(count.ToString()));
        }

        [Test]
        public void TestResults_HasValidStartTimeAttribute()
        {
            string startTimeString = RequiredAttribute(topNode, "start-time");
            Assert.That(DateTime.TryParseExact(startTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Invalid start time attribute: {0}. Expecting DateTime in 'o' format.", startTimeString);
        }

        [Test]
        public void TestResults_HasValidEndTimeAttribute()
        {
            string endTimeString = RequiredAttribute(topNode, "end-time");
            Assert.That(DateTime.TryParseExact(endTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Invalid end time attribute: {0}. Expecting DateTime in 'o' format.", endTimeString);
        }

        [Test]
        public void Environment_HasEnvironmentElement()
        {
            Assert.That(envNode, Is.Not.Null, "Missing environment element");
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
            RequiredAttribute(envNode, name);
        }

        [Test]
        public void CultureInfo_HasCultureInfoElement()
        {
            Assert.That(envNode.Attributes["culture"], Is.Not.Null, "Missing culture-info attribute");
        }

        [TestCase("culture")]
        [TestCase("uiculture")]
        public void CultureInfo_HasRequiredAttribute(string name)
        {
            string cultureName = RequiredAttribute(envNode, name);
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
            Assert.That(suiteNode, Is.Not.Null, "Missing test-suite element");
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
            Assert.That(RequiredAttribute(suiteNode, name), Is.EqualTo(value));
        }

        [Test]
        public void TestSuite_HasValidStartTimeAttribute()
        {
            var startTimeString = RequiredAttribute(suiteNode, "start-time");

            var success = DateTime.TryParse(startTimeString, out _);
            Assert.That(success, "{0} is an invalid value for start time", startTimeString);
        }

        [Test]
        public void TestSuite_HasValidEndTimeAttribute()
        {
            var endTimeString = RequiredAttribute(suiteNode, "end-time");

            var success = DateTime.TryParse(endTimeString, out _);
            Assert.That(success, "{0} is an invalid value for end time", endTimeString);
        }

        [Test]
        public void IgnoredTestCases_HaveValidStartAndEndTimeAttributes()
        {
            DateTime.TryParse(RequiredAttribute(topNode, "start-time"), out var testRunStartTime);
            DateTime.TryParse(RequiredAttribute(topNode, "end-time"), out var testRunEndTime);

            var testCaseNodes = suiteNode.SelectNodes("test-suite[@name='SkippedTest']/test-case");
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
            DateTime.TryParse(RequiredAttribute(topNode, "start-time"), out var testRunStartTime);
            DateTime.TryParse(RequiredAttribute(topNode, "end-time"), out var testRunEndTime);

            var testCaseNodes = suiteNode.SelectNodes("test-case[@name='ExplicitTest']");
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
