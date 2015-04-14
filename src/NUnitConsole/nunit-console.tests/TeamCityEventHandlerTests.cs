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
        public void TestStarted()
        {
            var startNode = CreateXmlNode("start-test");

            _teamCity.TestStarted(startNode);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testStarted name='FULLNAME' captureStandardOutput='true' flowId='ID']" + NL));
        }

        [Test]
        public void TestFinished_Passed()
        {
            var result = CreateXmlNode("test-case");
            result.AddAttribute("result", "Passed");
            result.AddAttribute("duration", "1.234");

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']" + NL));
        }

        [Test]
        public void TestFinished_Inconclusive()
        {
            var result = CreateXmlNode("test-case");
            result.AddAttribute("result", "Inconclusive");

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FULLNAME' message='Inconclusive' flowId='ID']" + NL));
        }

        [Test]
        public void TestFinished_Ignored()
        {
            var result = CreateXmlNode("test-case");
            result.AddAttribute("result", "Skipped");
            result.AddAttribute("label", "Ignored");
            result.AddElement("reason").AddElement("message").InnerText = "Just because";

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FULLNAME' message='Just because' flowId='ID']" + NL));
        }

        [Test]
        public void TestFinished_Failed()
        {
            var result = CreateXmlNode("test-case");
            result.AddAttribute("result", "Failed");
            result.AddAttribute("duration", "1.234");
            var failure = result.AddElement("failure");
            var message = failure.AddElement("message");
            var stacktrace = failure.AddElement("stack-trace");
            message.InnerText = "Error message";
            stacktrace.InnerText = "Stack trace";

            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFailed name='FULLNAME' message='Error message' details='Stack trace' flowId='ID']" + NL +
                "##teamcity[testFinished name='FULLNAME' duration='1.234' flowId='ID']" + NL));
        }

        private XmlNode CreateXmlNode(string elementName)
        {
            var node = XmlHelper.CreateTopLevelElement(elementName);
            node.AddAttribute("name", "NAME");
            node.AddAttribute("fullname", "FULLNAME");
            node.AddAttribute("id", "ID");

            return node;
        }

        private void FakeTestMethod() { }
    }
}
