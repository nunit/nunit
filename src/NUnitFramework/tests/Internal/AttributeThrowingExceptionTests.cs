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
        public void FailRecordsExceptionFromAttribute()
        {
            ITestResult result = RunDataTestCase(nameof(AttributeOnTestMethodThrowingExceptionFixture.TestWithFailingAttribute));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
            Assert.That(result.Message, Does.Contain("Failure building Test"));
            Assert.That(result.FullName, Does.Contain(nameof(AttributeOnTestMethodThrowingExceptionFixture.TestWithFailingAttribute)));
            Assert.That(result.StackTrace, Does.Contain(nameof(ExceptionThrowingAttribute) + "..ctor"));
        }

        [Test]
        public void OtherTestCasesRunNormally()
        {
            ITestResult result = RunDataTestCase(nameof(AttributeOnTestMethodThrowingExceptionFixture.NormalTest));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [TestCase(typeof(AttributeOnTestMethodThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnFixtureThrowingExceptionFixture))]
        public void TestSuiteContainsAllTests(Type fixtureType)
        {
            TestSuite suite = TestBuilder.MakeFixture(fixtureType);

            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests, Has.Count.EqualTo(2));
        }

        [TestCase(typeof(AttributeOnOneTimeSetUpMethodsThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnSetUpMethodsThrowingExceptionFixture))]
        public void TestSuiteWithErrorsContainsNoTests(Type fixtureType)
        {
            TestSuite suite = TestBuilder.MakeFixture(fixtureType);

            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(suite.Tests, Has.Count.EqualTo(0));

            ITestResult result = TestBuilder.RunTest(suite);
            Assert.That(result.ResultState.Status, Is.EqualTo(ResultState.NotRunnable.Status));
            Assert.That(result.Message, Does.Contain("Failure building TestFixture"));
            Assert.That(result.FullName, Is.EqualTo(fixtureType.FullName));
            Assert.That(result.StackTrace, Does.Contain(nameof(ExceptionThrowingAttribute) + "..ctor"));
        }
    }
}
