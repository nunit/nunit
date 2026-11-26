// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [NonParallelizable]
    public sealed class SetUpTearDownCancellationTokenTests
    {
        private const string InvalidParametersMessage = "SetUp and TearDown methods can only have a single CancellationToken parameter";

        [TestCase(typeof(SetUpTearDownWithCancellationTokenFixture))]
        [TestCase(typeof(SetUpTearDownWithCancellationTokenAndCancelAfterFixture))]
        public void CancellationTokenParameterReceivesTheContextCancellationToken(Type fixtureType)
        {
            var fixture = (SetUpTearDownWithCancellationTokenBaseFixture)Activator.CreateInstance(fixtureType)!;

            ITestResult suiteResult = TestBuilder.RunTestFixture(fixture);

            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(fixture.OneTimeSetUpParameter, Is.EqualTo(fixture.OneTimeSetUpContextToken));
                Assert.That(fixture.SetUpParameter, Is.EqualTo(fixture.SetUpContextToken));
                Assert.That(fixture.SetUpParameter, Is.EqualTo(fixture.TestContextToken));
                Assert.That(fixture.TearDownParameter, Is.EqualTo(fixture.TearDownContextToken));
                Assert.That(fixture.TearDownParameter, Is.EqualTo(fixture.TestContextToken));
                Assert.That(fixture.OneTimeTearDownParameter, Is.EqualTo(fixture.OneTimeTearDownContextToken));
            });
        }

        [Test]
        public void CancellationTokenParameterOfOneTimeSetUpIsCancelledByCancelAfter()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(OneTimeSetUpWithCancellationTokenExceedingCancelAfterFixture));

            Assert.Multiple(() =>
            {
                Assert.That(suiteResult.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(suiteResult.Message, Does.Contain("OneTimeSetUp exceeded CancelAfter value of 50ms"));
            });
        }

        [Test]
        public void CancellationTokenParameterOfSetUpIsCancelledByCancelAfter()
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(SetUpWithCancellationTokenExceedingCancelAfterFixture),
                                                         nameof(SetUpWithCancellationTokenExceedingCancelAfterFixture.Test));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
                Assert.That(result.Message, Does.Contain("Test exceeded CancelAfter value of 50ms"));
            });
        }

        [TestCase(typeof(SetUpWithNonCancellationTokenParameterFixture))]
        [TestCase(typeof(OneTimeTearDownWithTwoCancellationTokenParametersFixture))]
        public void OtherParametersAreNotAllowed(Type fixtureType)
        {
            TestSuite suite = TestBuilder.MakeFixture(fixtureType);

            Assert.Multiple(() =>
            {
                Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Does.StartWith(InvalidParametersMessage));
            });
        }
    }
}
