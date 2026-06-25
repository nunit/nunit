// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// TODO: Rework this
// RepeatAttribute should either
//  1) Apply at load time to create the exact number of tests, or
//  2) Apply at run time, generating tests or results dynamically
//
// #1 is feasible but doesn't provide much benefit
// #2 requires infrastructure for dynamic test cases first

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;
using NUnit.TestData.RepeatingTests;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public partial class RepeatAttributeTests
    {
        [TestCase(typeof(RepeatFailOnFirstTryFixture), "Failed(Child)", 1)]
        [TestCase(typeof(RepeatFailOnSecondTryFixture), "Failed(Child)", 2)]
        [TestCase(typeof(RepeatFailOnThirdTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatSuccessFixture), "Passed", 3)]
        [TestCase(typeof(RepeatedTestWithIgnoreAttribute), "Skipped:Ignored(Child)", 0)]
        [TestCase(typeof(RepeatIgnoredOnFirstTryFixture), "Skipped:Ignored(Child)", 1)]
        [TestCase(typeof(RepeatIgnoredOnSecondTryFixture), "Skipped:Ignored(Child)", 2)]
        [TestCase(typeof(RepeatIgnoredOnThirdTryFixture), "Skipped:Ignored(Child)", 3)]
        [TestCase(typeof(RepeatErrorOnFirstTryFixture), "Failed(Child)", 1)]
        [TestCase(typeof(RepeatErrorOnSecondTryFixture), "Failed(Child)", 2)]
        [TestCase(typeof(RepeatErrorOnThirdTryFixture), "Failed(Child)", 3)]
        public void RepeatWorksAsExpected(Type fixtureType, string outcome, int nTries)
        {
            var fixture = (RepeatingTestsFixtureBase)Reflect.Construct(fixtureType);
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.ToString(), Is.EqualTo(outcome));
                Assert.That(fixture.FixtureSetupCount, Is.EqualTo(1));
                Assert.That(fixture.FixtureTeardownCount, Is.EqualTo(1));
                Assert.That(fixture.SetupCount, Is.EqualTo(nTries));
                Assert.That(fixture.TeardownCount, Is.EqualTo(nTries));
                Assert.That(fixture.Count, Is.EqualTo(nTries));
            });
        }

        [Test]
        public void RepeatUpdatesCurrentRepeatCountPropertyOnEachAttempt()
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(typeof(RepeatedTestVerifyAttempt));
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(RepeatedTestVerifyAttempt.PassesTwoTimes));

            Assert.That(fixture.TearDownResults, Has.Count.EqualTo(fixture.Count + 1), "expected the CurrentRepeatCount property to be one less than the number of executions");
            Assert.That(result.FailCount, Is.EqualTo(1), "expected that the test failed the last repetition");
        }

        [Test]
        public void RepeatUpdatesCurrentRepeatCountPropertyOnGreenTest()
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(typeof(RepeatedTestVerifyAttempt));
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(RepeatedTestVerifyAttempt.AlwaysPasses));

            Assert.That(fixture.TearDownResults, Has.Count.EqualTo(fixture.Count + 1), "expected the CurrentRepeatCount property to be one less than the number of executions");
            Assert.That(result.FailCount, Is.EqualTo(0), "expected that the test passes all repetitions without a failure");
        }

        [Test]
        public void CategoryWorksWithRepeatedTest()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(RepeatedTestWithCategory));
            var test = suite.Tests[0] as Test;
            Assert.That(test, Is.Not.Null);
            System.Collections.IList categories = test.Properties["Category"];
            Assert.That(categories, Is.Not.Null);
            Assert.That(categories, Has.Count.EqualTo(1));
            Assert.That(categories[0], Is.EqualTo("SAMPLE"));
        }

        [Test]
        public void NotRunnableWhenIMethodInfoAbstractionReturnsMultipleIRepeatTestAttributes()
        {
            var fixtureSuite = new DefaultSuiteBuilder().BuildFrom(new CustomTypeWrapper(
                new TypeWrapper(typeof(FixtureWithMultipleRepeatAttributesOnSameMethod)),
                extraMethodAttributes: new Attribute[]
                {
                    new CustomRepeatAttribute(),
                    new RepeatAttribute(2)
                }));

            var method = fixtureSuite.Tests.Single();

            Assert.That(method.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(method.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Multiple attributes that repeat a test may cause issues."));
        }

        [Test]
        public void IRepeatTestAttributeIsEffectiveWhenAddedThroughIMethodInfoAbstraction()
        {
            var fixtureSuite = new DefaultSuiteBuilder().BuildFrom(new CustomTypeWrapper(
                new TypeWrapper(typeof(FixtureWithMultipleRepeatAttributesOnSameMethod)),
                extraMethodAttributes: new Attribute[]
                {
                    new RepeatAttribute(2)
                }));

            var fixtureInstance = new FixtureWithMultipleRepeatAttributesOnSameMethod();
            fixtureSuite.Fixture = fixtureInstance;
            TestBuilder.RunTest(fixtureSuite, fixtureInstance);

            Assert.That(fixtureInstance.MethodRepeatCount, Is.EqualTo(2));
        }

        private sealed class FixtureWithMultipleRepeatAttributesOnSameMethod
        {
            public int MethodRepeatCount { get; private set; }

            // The IRepeatTest attributes are dynamically applied via CustomTypeWrapper.
            [Test]
            public void MethodWithMultipleRepeatAttributes()
            {
                MethodRepeatCount++;
            }
        }

        private sealed class CustomRepeatAttribute : Attribute, IRepeatTest
        {
            public TestCommand Wrap(TestCommand command)
            {
                throw new NotImplementedException();
            }
        }

        [TestCase(typeof(RepeatWithoutStopSucceedsOnFirstTryFixture), "Passed", 3)]
        [TestCase(typeof(RepeatWithoutStopSucceedsOnSecondTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopSucceedsOnThirdTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopFailsEveryTimeFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopWithIgnoreAttributeFixture), "Skipped:Ignored(Child)", 0)]
        [TestCase(typeof(RepeatWithoutStopIgnoredOnFirstTryFixture), "Skipped:Ignored(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopIgnoredOnSecondTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopIgnoredOnThirdTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopErrorOnFirstTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopErrorOnSecondTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopErrorOnThirdTryFixture), "Failed(Child)", 3)]
        [TestCase(typeof(RepeatWithoutStopTestCaseFixture), "Failed(Child)", 3)]
        public void RepeatWithoutStoppingWorksAsExpectedOnFixturesWithSetupAndTeardown(Type fixtureType, string outcome, int nTries)
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(fixtureType);
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.ToString(), Is.EqualTo(outcome));
                Assert.That(fixture.FixtureSetupCount, Is.EqualTo(1));
                Assert.That(fixture.FixtureTeardownCount, Is.EqualTo(1));
                Assert.That(fixture.SetupCount, Is.EqualTo(nTries));
                Assert.That(fixture.TeardownCount, Is.EqualTo(nTries));
                Assert.That(fixture.Count, Is.EqualTo(nTries));
            });
        }

        [TestCase(typeof(RepeatStopOnFailurePropertyTrueTestCaseFixture), "Failed(Child)", 1)]
        [TestCase(typeof(RepeatStopOnFailurePropertyFalseTestCaseFixture), "Failed(Child)", 3)]
        public void RepeatStopOnFailurePropertyTest(Type fixtureType, string outcome, int nTries)
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(fixtureType);
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.ToString(), Is.EqualTo(outcome));
                Assert.That(fixture.FixtureSetupCount, Is.EqualTo(1));
                Assert.That(fixture.FixtureTeardownCount, Is.EqualTo(1));
                Assert.That(fixture.SetupCount, Is.EqualTo(nTries));
                Assert.That(fixture.TeardownCount, Is.EqualTo(nTries));
                Assert.That(fixture.Count, Is.EqualTo(nTries));
            });
        }

        [Test]
        public void RepeatFullOutputTest()
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(RepeatOutputTestCaseFixture), nameof(RepeatOutputTestCaseFixture.PrintTest));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            Assert.That(result.Output, Is.EqualTo("0" + Environment.NewLine +
                                                  "1" + Environment.NewLine +
                                                  "2" + Environment.NewLine));
        }

        [Test]
        public void RepeatFullOutputTestWithFailures()
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(RepeatOutputTestCaseWithFailuresFixture), nameof(RepeatOutputTestCaseWithFailuresFixture.PrintTest));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));

            Assert.That(result.Output, Is.EqualTo("0" + Environment.NewLine +
                                                  "1" + Environment.NewLine));

            Assert.That(result.AssertCount, Is.EqualTo(2), "Expected 1 assert per run");

            Assert.That(result.AssertionResults, Has.Count.EqualTo(1), "Expected one failing assertions in second run");
            Assert.That(result.AssertionResults[0].Status, Is.EqualTo(AssertionStatus.Failed));

            Assert.That(result.Message, Does.Not.StartWith("Multiple failures or warnings in test").And
                                            .Contain("Expected: not equal to 2 and not equal to 3").And
                                            .Contain("But was:  2"));
        }

        [Test]
        public void RepeatFullOutputTestWithMultipleFailures()
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(RepeatOutputTestCaseWithMultipleFailuresFixture), nameof(RepeatOutputTestCaseWithMultipleFailuresFixture.PrintTest));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));

            Assert.That(result.Output, Is.EqualTo("0" + Environment.NewLine +
                                                  "1" + Environment.NewLine +
                                                  "2" + Environment.NewLine +
                                                  "3" + Environment.NewLine +
                                                  "4" + Environment.NewLine));

            Assert.That(result.AssertCount, Is.EqualTo(5), "Expected 1 assert per run");

            Assert.That(result.AssertionResults, Has.Count.EqualTo(2), "Expected two failing assertions in five runs");
            Assert.That(result.AssertionResults[0].Status, Is.EqualTo(AssertionStatus.Failed));
            Assert.That(result.AssertionResults[1].Status, Is.EqualTo(AssertionStatus.Failed));

            Assert.That(result.Message, Does.StartWith("Multiple failures or warnings in test").And
                                            .Contain("Expected: not equal to 2 and not equal to 3").And
                                            .Contain("But was:  2").And
                                            .Contain("But was:  3"));
        }

        [Test]
        public void RepeatWithThresholdAllPassReportsSuccess_Issue5220()
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(typeof(RepeatWithThresholdAllPassFixture));
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5));
                Assert.That(result.Output, Does.Not.Contain("pass threshold"));
            });
        }

        [Test]
        public void RepeatWithThresholdAboveThresholdReportsPassed_Issue5220()
        {
            var fixture = new RepeatWithThresholdAboveThresholdFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(RepeatWithThresholdAboveThresholdFixture.FailsOnce));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5), "All 5 runs should execute when threshold < 100");
                Assert.That(result.Output, Does.Contain("4 of 5 runs passed").And.Contain("meeting the required 60% pass threshold"));
            });
        }

        [Test]
        public void RepeatWithThresholdExactlyAtThresholdReportsPassed_Issue5220()
        {
            var fixture = new RepeatWithThresholdExactlyAtThresholdFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(RepeatWithThresholdExactlyAtThresholdFixture.FailsOnce));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5));
                Assert.That(result.Output, Does.Contain("4 of 5 runs passed (80%), meeting the required 80% pass threshold"));
            });
        }

        [Test]
        public void RepeatWithThresholdBelowThresholdReportsFailed_Issue5220()
        {
            var fixture = new RepeatWithThresholdBelowThresholdFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(RepeatWithThresholdBelowThresholdFixture.FailsMostRuns));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(fixture.Count, Is.EqualTo(5), "All 5 runs should execute even when failing");

                // The threshold message must be the primary message, not the generic "Multiple failures" text
                // that RecordTestCompletion would produce if it were the final result-setter.
                Assert.That(result.Message, Does.Contain("below the required 80% pass threshold")
                                               .And.Not.Contain("Multiple failures or warnings in test"));

                // The individual assertion results from each failing run must be preserved in the result
                // so that IDE test runners can show per-run failure details.
                Assert.That(result.AssertionResults, Has.Count.EqualTo(3),
                    "Expected one AssertionResult per failing run (3 of 5 runs fail)");
                Assert.That(result.AssertionResults, Has.All.Property(nameof(AssertionResult.Status))
                                                              .EqualTo(AssertionStatus.Failed));
            });
        }

        [Test]
        public void RepeatWithThresholdStopOnFailureIsIgnoredWhenThresholdSet_Issue5220()
        {
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(typeof(RepeatWithThresholdStopOnFailureIgnoredFixture));
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5), "StopOnFailure should be ignored when RequiredPassPercentage < 100");
            });
        }

        [TestCase(0)]
        [TestCase(101)]
        public void RepeatWithThresholdInvalidPercentageThrows_Issue5220(int invalidPercentage)
        {
            var attr = new RepeatAttribute(3) { RequiredPassPercentage = invalidPercentage };
            var testMethod = TestBuilder.MakeTestCase(
                typeof(FixtureWithMultipleRepeatAttributesOnSameMethod),
                nameof(FixtureWithMultipleRepeatAttributesOnSameMethod.MethodWithMultipleRepeatAttributes));
            var command = new TestMethodCommand(testMethod);

            Assert.That(() => attr.Wrap(command), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void RepeatWithZeroOrNegativeCountThrows(int invalidCount)
        {
            var attr = new RepeatAttribute(invalidCount);
            var testMethod = TestBuilder.MakeTestCase(
                typeof(FixtureWithMultipleRepeatAttributesOnSameMethod),
                nameof(FixtureWithMultipleRepeatAttributesOnSameMethod.MethodWithMultipleRepeatAttributes));
            var command = new TestMethodCommand(testMethod);

            Assert.That(() => attr.Wrap(command), Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
