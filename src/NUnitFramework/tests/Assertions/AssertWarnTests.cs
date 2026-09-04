// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

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
                nameof(WarningFixture.CallAssertWarnWithMessage));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Warning));
                Assert.That(result.Message, Contains.Substring("MESSAGE"));
            }
        }

        [Test]
        public void WarningsAreDisplayedWithFailure()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                nameof(WarningFixture.TwoWarningsAndFailure));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("This fails"));
            }
        }

        [Test]
        public void WarningsAreDisplayedWithIgnore()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                nameof(WarningFixture.TwoWarningsAndIgnore));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("Ignore this"));
            }
        }

        [Test]
        public void WarningsAreDisplayedWithInconclusive()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(WarningFixture),
                nameof(WarningFixture.TwoWarningsAndInconclusive));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3));
                Assert.That(result.Message, Contains.Substring("First warning"));
                Assert.That(result.Message, Contains.Substring("Second warning"));
                Assert.That(result.Message, Contains.Substring("This is inconclusive"));
            }
        }
    }
}
