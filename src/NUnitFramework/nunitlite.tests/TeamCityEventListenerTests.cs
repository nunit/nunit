// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnitLite.Tests
{
    public class TeamCityEventListenerTests
    {
        private TeamCityEventListener _teamCity;
        private StringBuilder _output;

        private static readonly string NL = Environment.NewLine;

        [SetUp]
        public void CreateListener()
        {
            _output = new StringBuilder();
            var outWriter = new StringWriter(_output);

            _teamCity = new TeamCityEventListener(outWriter);
        }

        [Test]
        public void TestSuiteStarted()
        {
            _teamCity.TestStarted(new TestSuite("dummy"));

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteStarted name='dummy']" + NL));
        }

        [Test]
        public void TestSuiteFinished()
        {
            _teamCity.TestFinished(new TestSuite("dummy").MakeTestResult());

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testSuiteFinished name='dummy']" + NL));
        }

        [Test]
        public void TestStarted()
        {
            _teamCity.TestStarted(Fakes.GetTestMethod(this, "FakeTestMethod"));

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testStarted name='FakeTestMethod' captureStandardOutput='true']" + NL));
        }

        [Test]
        public void TestFinished_Passed()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Success);
            result.Duration = 1.234;
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFinished name='FakeTestMethod' duration='1234']" + NL));
        }

        [Test]
        public void TestFinished_Inconclusive()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Inconclusive);
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Inconclusive']" + NL));
        }

        [Test]
        public void TestFinished_Ignored()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Ignored, "Just because");
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testIgnored name='FakeTestMethod' message='Just because']" + NL));
        }

        [Test]
        public void TestFinished_Failed()
        {
            var result = Fakes.GetTestMethod(this, "FakeTestMethod").MakeTestResult();
            result.SetResult(ResultState.Failure, "Error message", "Stack trace");
            result.Duration = 1.234;
            _teamCity.TestFinished(result);

            Assert.That(_output.ToString(), Is.EqualTo(
                "##teamcity[testFailed name='FakeTestMethod' message='Error message' details='Stack trace']" + NL +
                "##teamcity[testFinished name='FakeTestMethod' duration='1234']" + NL));
        }

        private void FakeTestMethod() { }
    }
}
