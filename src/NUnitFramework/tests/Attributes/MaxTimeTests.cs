// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Tests for MaxTime decoration.
    /// </summary>
    [TestFixture, NonParallelizable]
    public class MaxTimeTests
    {
        [Test,MaxTime(1000)]
        public void MaxTimeNotExceeded()
        {
        }

        // TODO: We need a way to simulate the clock reliably
        [Test]
        public void MaxTimeExceeded()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixture));
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
            ITestResult result = suiteResult.Children.ToArray()[0];
            Assert.That(result.Message, Does.Contain("exceeds maximum of 1ms"));
        }

        [Test]
        public void MaxTimeExceededOnTestCase()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithTestCase));
            Assert.AreEqual(ResultState.ChildFailure, suiteResult.ResultState);
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
            Assert.AreEqual(ResultState.ChildFailure, result.ResultState);
            result = result.Children.ToArray()[0];
            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.That(result.Message, Is.EqualTo("Intentional Failure"));
        }

        [Test]
        public void ErrorReportHasPriorityOverMaxTime()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(MaxTimeFixtureWithError));
            Assert.AreEqual(ResultState.ChildFailure, result.ResultState);
            result = result.Children.ToArray()[0];
            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.That(result.Message, Does.Contain("Exception message"));
        }
    }
}
