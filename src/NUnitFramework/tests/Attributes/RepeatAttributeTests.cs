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
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
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
            RepeatingTestsFixtureBase fixture = (RepeatingTestsFixtureBase)Reflect.Construct(fixtureType);
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.That(result.ResultState.ToString(), Is.EqualTo(outcome));
            Assert.That(fixture.FixtureSetupCount, Is.EqualTo(1));
            Assert.That(fixture.FixtureTeardownCount, Is.EqualTo(1));
            Assert.That(fixture.SetupCount, Is.EqualTo(nTries));
            Assert.That(fixture.TeardownCount, Is.EqualTo(nTries));
            Assert.That(fixture.Count, Is.EqualTo(nTries));
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
            Test? test = suite.Tests[0] as Test;
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
    }
}
