// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.AssertIgnoreData;

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// Tests of IgnoreException and Assert.Ignore
    /// </summary>
    [TestFixture]
    public class AssertIgnoreTests
    {
        [Test]
        public void ThrowsIgnoreException()
        {
            Assert.That(Assert.Ignore, Throws.TypeOf<IgnoreException>());
        }

        [Test]
        public void ThrowsIgnoreExceptionWithMessage()
        {
            Assert.That(() => Assert.Ignore("MESSAGE"), Throws.TypeOf<IgnoreException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void IgnoreWorksForTestCase()
        {
            Type fixtureType = typeof(IgnoredTestCaseFixture);
            ITestResult result = TestBuilder.RunTestCase(fixtureType, "CallsIgnore");
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
            Assert.That(result.Message, Is.EqualTo("Ignore me"));
        }

        [Test]
        public void IgnoreWorksForTestSuite()
        {
            var suite = new TestSuite("IgnoredTestFixture");
            suite.Add(TestBuilder.MakeFixture(typeof(IgnoredTestSuiteFixture)));
            ITestResult fixtureResult = TestBuilder.RunTest(suite).Children.ToArray()[0];

            Assert.That(fixtureResult.ResultState, Is.EqualTo(ResultState.Ignored.WithSite(FailureSite.SetUp)));

            foreach (ITestResult testResult in fixtureResult.Children)
            {
                Assert.That(testResult.ResultState, Is.EqualTo(ResultState.Ignored.WithSite(FailureSite.Parent)));
            }
        }

        [Test]
        public void IgnoreWorksFromSetUp()
        {
            ITestResult fixtureResult = TestBuilder.RunTestFixture(typeof(IgnoreInSetUpFixture));

            // TODO: Decide whether to pass Ignored state to containing fixture
            //Assert.AreEqual(ResultState.Ignored, fixtureResult.ResultState);

            foreach (var testResult in fixtureResult.Children)
                Assert.That(testResult.ResultState, Is.EqualTo(ResultState.Ignored));
        }

        [Test]
        public void IgnoreWithUserMessage()
        {
            try
            {
                Assert.Ignore("my message");
            }
            catch (IgnoreException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("my message"));
            }
        }
    }
}
