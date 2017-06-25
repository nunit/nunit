// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

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
