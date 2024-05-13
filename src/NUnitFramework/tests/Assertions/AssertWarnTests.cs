// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertWarnTests
    {
        [Test]
        public void AssertWarnWorksWithMessage()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "CallAssertWarnWithMessage");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
            Assert.That(result.Message, Contains.Substring("MESSAGE"));
        }

        [Test]
        public void WarningsAreDisplayedWithFailure()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndFailure");
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("This fails"));
            });
        }

        [Test, Ignore("Currently Fails: Ignored message is displayed without the warnings")]
        public void WarningsAreDisplayedWithIgnore()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndIgnore");
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("Ignore this"));
            });
        }

        [Test, Ignore("Currently Fails: Inconclusive message is displayed without the warnings")]
        public void WarningsAreDisplayedWithInconclusive()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndInconclusive");
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("This is inconclusive"));
            });
        }
    }
}
