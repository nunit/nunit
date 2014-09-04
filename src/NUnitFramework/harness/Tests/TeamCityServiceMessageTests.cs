using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.TestHarness.Tests
{
    public class TeamCityEventListenerTests
    {
        private TeamCityServiceMessages _teamCity;
        private StringBuilder _output;

        private static readonly string NL = NUnit.Env.NewLine;

        [SetUp]
        public void CreateListener()
        {
            _output = new StringBuilder();
            var outWriter = new StringWriter(_output);

            _teamCity = new TeamCityServiceMessages(outWriter);
        }

        [Test]
        public void TestSuiteStarted()
        {
            _teamCity.TestSuiteStarted("dummy");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteStarted name='dummy']" + NL));
        }

        [Test]
        public void TestSuiteFinished()
        {
            _teamCity.TestSuiteFinished("dummy");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteFinished name='dummy']" + NL));
        }

        [Test]
        public void TestStarted()
        {
            _teamCity.TestStarted("FakeTestMethod");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testStarted name='FakeTestMethod' captureStandardOutput='true']" + NL));
        }

        [Test]
        public void TestFinished_Passed()
        {
            _teamCity.TestFinished("FakeTestMethod", 1.234);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFinished name='FakeTestMethod' duration='1.234']" + NL));
        }

        [Test]
        public void TestIgnored()
        {
            _teamCity.TestIgnored("FakeTestMethod", "Just because");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Just because']" + NL));
        }

        [Test]
        public void TestFailed()
        {
            _teamCity.TestFailed("FakeTestMethod", "Error message", "Stack trace");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFailed name='FakeTestMethod' message='Error message' details='Stack trace']" + NL ));
        }

        private void FakeTestMethod() { }
    }
}
