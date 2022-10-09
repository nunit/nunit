// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
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

            Assert.AreEqual(ResultState.Warning, result.ResultState);
            Assert.AreEqual("MESSAGE", result.Message);
        }

        [Test]
        public void AssertWarnWorksWithMessageAndArgs()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "CallAssertWarnWithMessageAndArgs");

            Assert.AreEqual(ResultState.Warning, result.ResultState);
            Assert.AreEqual("MESSAGE: 2+2=4", result.Message);
        }

        [Test]
        public void WarningsAreDisplayedWithFailure()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndFailure");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("This fails"));
        }

        [Test, Ignore("Currently Fails: Ignored message is displayed without the warnings")]
        public void WarningsAreDisplayedWithIgnore()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndIgnore");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("Ignore this"));
        }

        [Test, Ignore("Currently Fails: Inconclusive message is displayed without the warnings")]
        public void WarningsAreDisplayedWithInconclusive()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                "TwoWarningsAndInconclusive");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
            Assert.That(result.AssertionResults.Count, Is.EqualTo(3));
            Assert.That(result.Message, Contains.Substring("First warning"));
            Assert.That(result.Message, Contains.Substring("Second warning"));
            Assert.That(result.Message, Contains.Substring("This is inconclusive"));
        }
    }
}
