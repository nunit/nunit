using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnitLite.Runner.Tests
{
    public class TeamCityEventListenerTests
    {
        private TeamCityEventListener _teamCity;
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

            _teamCity = new TeamCityEventListener(outWriter, errWriter);
        }

        [Test]
        public void TestSuiteStarted()
        {
            _teamCity.TestStarted(new TestSuite("dummy"));

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteStarted name='dummy']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestSuiteFinished()
        {
            _teamCity.TestFinished(new TestSuite("dummy").MakeTestResult());

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteFinished name='dummy']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestStarted()
        {
            _teamCity.TestStarted(Fakes.GetTestMethod(this, "FakeTestMethod"));

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testStarted name='FakeTestMethod' captureStandardOutput='true']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFinished_Passed()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Success);
            result.Duration = TimeSpan.FromMilliseconds(1234);
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFinished name='FakeTestMethod' duration='1.234']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFinished_Inconclusive()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Inconclusive);
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Inconclusive']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFinished_Ignored()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Ignored, "Just because");
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Just because']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestFinished_Failed()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Failure, "Error message", "Stack trace");
            result.Duration = TimeSpan.FromMilliseconds(1234);
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFailed name='FakeTestMethod' message='Error message' details='Stack trace']" + NL +
                "##teamcity[testFinished name='FakeTestMethod' duration='1.234']" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestStandardOutput()
        {
            _teamCity.TestOutput(new TestOutput("Output to console", TestOutputType.Out));

            Assert.That(_output.ToString(), Is.EqualTo("Output to console" + NL));
            Assert.That(_error.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestErrorOutput()
        {
            _teamCity.TestOutput(new TestOutput("This is an error message", TestOutputType.Error));

            Assert.That(_error.ToString(), Is.EqualTo("This is an error message" + NL));
            Assert.That(_output.Length, Is.EqualTo(0));
        }

        private void FakeTestMethod() { }
    }
}
