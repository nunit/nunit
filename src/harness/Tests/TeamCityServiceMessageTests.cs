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
        private StringBuilder _error;

        private static readonly string NL = NUnit.Env.NewLine;

        [SetUp]
        public void CreateListener()
        {
            _output = new StringBuilder();
            var outWriter = new StringWriter(_output);

            _error = new StringBuilder();
            var errWriter = new StringWriter(_error);

            _teamCity = new TeamCityServiceMessages(outWriter, errWriter);
        }

        [Test]
        public void TestSuiteStarted()
        {
            _teamCity.TestSuiteStarted("dummy");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteStarted name='dummy']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestSuiteFinished()
        {
            _teamCity.TestSuiteFinished("dummy");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteFinished name='dummy']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestStarted()
        {
            _teamCity.TestStarted("FakeTestMethod");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testStarted name='FakeTestMethod' captureStandardOutput='true']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFinished_Passed()
        {
            _teamCity.TestFinished("FakeTestMethod", 1.234);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFinished name='FakeTestMethod' duration='1.234']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestIgnored()
        {
            _teamCity.TestIgnored("FakeTestMethod", "Just because");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Just because']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFailed()
        {
            _teamCity.TestFailed("FakeTestMethod", "Error message", "Stack trace");

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFailed name='FakeTestMethod' message='Error message' details='Stack trace']" + NL +
                "##teamcity[testFinished name='FakeTestMethod' duration='1.234']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestStandardOutput()
        {
            _teamCity.TestOutput("Output to console");

            Assert.That(_output.ToString(), Is.EqualTo("Output to console" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestErrorOutput()
        {
            _teamCity.TestOutput("This is an error message");

            Assert.That(_error.ToString(), Is.EqualTo("This is an error message" + NL));
            Assert.That(_output.Length, Is.EqualTo(0));
        }

        private void FakeTestMethod() { }
    }
}
