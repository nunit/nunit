// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.TestData.AttributeThrowingExceptionFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class AttributeThrowingExceptionTests
    {
        private static ITestResult RunDataTestCase(string methodName)
        {
            return TestBuilder.RunTestCase(typeof(AttributeOnTestMethodThrowingExceptionFixture), methodName);
        }

        [Test]
        public void DoesNotTriggerUserAttribute()
        {
            ITestResult result = RunDataTestCase(nameof(AttributeOnTestMethodThrowingExceptionFixture.TestWithFailingAttribute));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void OtherTestCasesRunNormally()
        {
            ITestResult result = RunDataTestCase(nameof(AttributeOnTestMethodThrowingExceptionFixture.NormalTest));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [TestCase(typeof(AttributeOnTestMethodThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnFixtureThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnOneTimeSetUpMethodsThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnSetUpMethodsThrowingExceptionFixture))]
        public void TestSuiteContainsAllTests(Type fixtureType)
        {
            TestSuite suite = TestBuilder.MakeFixture(fixtureType);

            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests, Has.Count.EqualTo(2));
        }
    }
}
