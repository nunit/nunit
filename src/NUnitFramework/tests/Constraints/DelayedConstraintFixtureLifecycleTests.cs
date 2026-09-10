// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture, NonParallelizable]
    public class DelayedConstraintFixtureLifecycleTests
    {
        [Test]
        public void CanRunTheFixtureTwiceWithoutUnloadingItsAssembly()
        {
            // Exercise Delay on the test thread so a disposed event is reported as
            // a test failure rather than escaping from one of the fixture's raw threads.
            string testName = typeof(DelayedConstraintTests).FullName + "." +
                nameof(DelayedConstraintTests.ThatBlockingDelegateWhichSucceedsWithoutPolling_ReturnsAfterDelay);

            for (int run = 1; run <= 2; run++)
            {
                var work = TestBuilder.CreateWorkItem(typeof(DelayedConstraintTests), new FullNameFilter(testName));
                ITestResult result = TestBuilder.ExecuteWorkItem(work);

                Assert.Multiple(() =>
                {
                    Assert.That(result.TotalCount, Is.EqualTo(1), $"Fixture execution {run}");
                    Assert.That(result.ResultState, Is.EqualTo(ResultState.Success), result.ToXml(true).OuterXml);
                });
            }
        }
    }
}
