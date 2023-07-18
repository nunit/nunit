// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    /// <summary>
    /// Tests for MaxTime decoration.
    /// </summary>
    [TestFixture, NonParallelizable]
    public class MaxTimeTests
    {
        [Test, MaxTime(1000)]
        public void MaxTimeNotExceeded()
        {
        }

        // TODO: We need a way to simulate the clock reliably
        [Test]
        public void MaxTimeExceeded()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixture));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.Message, Does.Contain("exceeds maximum of 1ms"));
        }

        [Test]
        public void MaxTimeExceededOnTestCase()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithTestCase));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
            ITestResult result = suiteResult.Children.ToArray()[0].Children.ToArray()[0];
            Assert.That(result.Message, Does.Contain("exceeds maximum of 1ms"));
        }

        [Test, MaxTime(1000)]
        public void FailureReport()
        {
            Assert.That(
                () => Assert.Fail("Intentional Failure"),
                Throws.TypeOf<AssertionException>().With.Message.EqualTo("Intentional Failure"));
        }

        [Test]
        public void FailureReportHasPriorityOverMaxTime()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithFailure));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
            result = result.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Intentional Failure"));
        }

        [Test]
        public void ErrorReportHasPriorityOverMaxTime()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithError));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
            result = result.Children.ToArray()[0];
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Error));
            Assert.That(result.Message, Does.Contain("Exception message"));
        }
    }
}
