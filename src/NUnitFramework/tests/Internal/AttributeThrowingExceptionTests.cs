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

        [Test]
        public void FailureInTestAttributeIsDetected()
        {
            ITestResult result = RunDataTestCase(nameof(AttributeOnTestMethodThrowingExceptionFixture.SpecialTestHandling));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
            Assert.That(result.Message, Does.Contain("Failure building Test"));
            Assert.That(result.FullName, Does.Contain(nameof(AttributeOnTestMethodThrowingExceptionFixture.SpecialTestHandling)));
            Assert.That(result.StackTrace, Does.Contain(nameof(ExceptionThrowingApplyToAttribute) + "." + nameof(IApplyToTest.ApplyToTest)));
        }

        [TestCase(typeof(AttributeOnTestMethodThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnFixtureThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnOneTimeSetUpMethodsThrowingExceptionFixture))]
        [TestCase(typeof(AttributeOnSetUpMethodsThrowingExceptionFixture))]
        public void TestSuiteContainsAllTests(Type fixtureType)
        {
            TestSuite suite = TestBuilder.MakeFixture(fixtureType);

            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests, Has.Count.EqualTo(3));
        }
    }
}
