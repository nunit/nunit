using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    using Utilities;

    public class TeamCityEventHandlerTests
    {
        private TeamCityEventHandler _teamCity;
        private StringBuilder _output;

        private static readonly string NL = Environment.NewLine;

        [SetUp]
        public void CreateTeamCityEventHandler()
        {
            _output = new StringBuilder();
            var outWriter = new StringWriter(_output);

            _teamCity = new TeamCityEventHandler(outWriter);
        }

        [Test]
        [TestCase(null, "##teamcity[testStarted name='FULLNAME' captureStandardOutput='true' flowId='ID']")]
        [TestCase("", "##teamcity[testStarted name='FULLNAME' captureStandardOutput='true' flowId='ID']")]
        [TestCase("foo.dll", "##teamcity[testStarted name='foo.dll: FULLNAME' captureStandardOutput='true' flowId='ID']")]
        public void TestStarted(string assembly, string expectedMessage)
        {
            var startNode = CreateXmlNode("start-test", assembly);

            _teamCity.TestStarted(startNode);

            Assert.That(_output.ToString(), Is.EqualTo(expectedMessage + NL));
        }

        [Test]
        [TestCase(null, "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']")]
        [TestCase("", "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']")]
        [TestCase("foo.dll", "##teamcity[testFinished name='foo.dll: FULLNAME' duration='1.234' flowId='ID']")]
        public void TestFinished_Passed(string assembly, string expectedMessage)
        {
            var result = CreateXmlNode("test-case", assembly);
            result.AddAttribute("result", "Passed");
            result.AddAttribute("duration", "1.234");
            
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(expectedMessage + NL));
        }

        [Test]
        [TestCase(null, "##teamcity[testIgnored name='FULLNAME' message='Inconclusive' flowId='ID']")]
        [TestCase("", "##teamcity[testIgnored name='FULLNAME' message='Inconclusive' flowId='ID']")]
        [TestCase("foo.dll", "##teamcity[testIgnored name='foo.dll: FULLNAME' message='Inconclusive' flowId='ID']")]
        public void TestFinished_Inconclusive(string assembly, string expectedMessage)
        {
            var result = CreateXmlNode("test-case", assembly);
            result.AddAttribute("result", "Inconclusive");

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(expectedMessage + NL));
        }

        [Test]
        [TestCase(null, "##teamcity[testIgnored name='FULLNAME' message='Just because' flowId='ID']")]
        [TestCase("", "##teamcity[testIgnored name='FULLNAME' message='Just because' flowId='ID']")]
        [TestCase("foo.dll", "##teamcity[testIgnored name='foo.dll: FULLNAME' message='Just because' flowId='ID']")]
        public void TestFinished_Ignored(string assembly, string expectedMessage)
        {
            var result = CreateXmlNode("test-case", assembly);
            result.AddAttribute("result", "Skipped");
            result.AddAttribute("label", "Ignored");
            result.AddElement("reason").AddElement("message").InnerText = "Just because";

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(expectedMessage + NL));
        }

        [Test]
        [TestCase(null, new[] { "##teamcity[testFailed name='FULLNAME' message='Error message' details='Stack trace' flowId='ID']", "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']" })]
        [TestCase("", new[] { "##teamcity[testFailed name='FULLNAME' message='Error message' details='Stack trace' flowId='ID']", "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']" })]
        [TestCase("foo.dll", new[] { "##teamcity[testFailed name='foo.dll: FULLNAME' message='Error message' details='Stack trace' flowId='ID']", "##teamcity[testFinished name='foo.dll: FULLNAME' duration='1.234' flowId='ID']" })]
        public void TestFinished_Failed(string assembly, string[] expectedMessages)
        {
            var result = CreateXmlNode("test-case", assembly);
            result.AddAttribute("result", "Failed");
            result.AddAttribute("duration", "1.234");
            var failure = result.AddElement("failure");
            var message = failure.AddElement("message");
            var stacktrace = failure.AddElement("stack-trace");
            message.InnerText = "Error message";
            stacktrace.InnerText = "Stack trace";

            _teamCity.TestFinished(result);
            
            Assert.That(_output.ToString(), Is.EqualTo(string.Join(NL, expectedMessages) + NL));
        }

        private XmlNode CreateXmlNode(string elementName, string assembly)
        {
            var node = XmlHelper.CreateTopLevelElement(elementName);
            node.AddAttribute("name", "NAME");
            node.AddAttribute("fullname", "FULLNAME");
            node.AddAttribute("id", "ID");
            if (assembly != null)
            {
                node.AddAttribute("assembly", assembly);
            }

            return node;
        }

        private void FakeTestMethod() { }
    }
}
