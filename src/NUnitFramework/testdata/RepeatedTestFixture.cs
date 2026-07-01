// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// TODO: Rework this
// RepeatAttribute should either
//  1) Apply at load time to create the exact number of tests, or
//  2) Apply at run time, generating tests or results dynamically
//
// #1 is feasible but doesn't provide much benefit
// #2 requires infrastructure for dynamic test cases first
using System;
using NUnit.Framework;

namespace NUnit.TestData.RepeatingTests
{
    public class RepeatSuccessFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatSuccess()
        {
            Count++;
            Assert.Pass();
        }
    }

    public class RepeatFailOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnFirst()
        {
            Count++;
            Assert.Fail();
        }
    }

    public class RepeatFailOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            Count++;

            if (Count == 2)
                Assert.Fail();
        }
    }

    public class RepeatFailOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            Count++;

            if (Count == 3)
                Assert.Fail();
        }
    }

    public class RepeatedTestWithIgnoreAttribute : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3), Ignore("Ignore this test")]
        public void RepeatShouldIgnore()
        {
            Assert.Fail("Ignored test executed");
        }
    }

    public class RepeatIgnoredOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;
            Assert.Ignore("Ignoring");
        }
    }

    public class RepeatIgnoredOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;

            if (Count == 2)
                Assert.Ignore("Ignoring");
        }
    }

    public class RepeatIgnoredOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;

            if (Count == 3)
                Assert.Ignore("Ignoring");
        }
    }

    public class RepeatErrorOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;
            throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatErrorOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;

            if (Count == 2)
                throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatErrorOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void Test()
        {
            Count++;

            if (Count == 3)
                throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatedTestWithCategory : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3), Category("SAMPLE")]
        public void TestWithCategory()
        {
            Count++;
            Assert.Pass();
        }
    }

    public class RepeatedTestVerifyAttempt : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void AlwaysPasses()
        {
            Count = TestContext.CurrentContext.CurrentRepeatCount;
        }

        [Test, Repeat(3)]
        public void PassesTwoTimes()
        {
            Assert.That(Count, Is.EqualTo(TestContext.CurrentContext.CurrentRepeatCount), "expected CurrentRepeatCount to be incremented only after first two attempts");
            if (Count > 1)
            {
                Assert.Fail("forced failure on 3rd repetition");
            }
            Count++;
        }
    }

    public class RepeatWithoutStopSucceedsOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void SucceedsEveryTime()
        {
            Count++;
            Assert.Pass();
        }
    }

    public class RepeatWithoutStopFailsEveryTimeFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void FailsEveryTime()
        {
            Count++;
            Assert.Fail();
        }
    }

    public class RepeatWithoutStopSucceedsOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void SucceedsOnSecondTry()
        {
            Count++;

            if (Count < 2)
                Assert.Fail();
        }
    }

    public class RepeatWithoutStopSucceedsOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void SucceedsOnThirdTry()
        {
            Count++;

            if (Count < 3)
                Assert.Fail();
        }
    }

    public class RepeatWithoutStopWithIgnoreAttributeFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false), Ignore("Ignore this test")]
        public void RepeatShouldIgnore()
        {
            Assert.Fail("Ignored test executed");
        }
    }

    public class RepeatWithoutStopIgnoredOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;
            Assert.Ignore("Ignoring");
        }
    }

    public class RepeatWithoutStopIgnoredOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;

            if (Count < 2)
                Assert.Fail("Failed");

            Assert.Ignore("Ignoring");
        }
    }

    public class RepeatWithoutStopIgnoredOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;

            if (Count < 3)
                Assert.Fail("Failed");

            Assert.Ignore("Ignoring");
        }
    }

    public class RepeatWithoutStopErrorOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;
            throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatWithoutStopErrorOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;

            if (Count < 2)
                Assert.Fail("Failed");

            throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatWithoutStopErrorOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, false)]
        public void Test()
        {
            Count++;

            if (Count < 3)
                Assert.Fail("Failed");

            throw new Exception("Deliberate Exception");
        }
    }

    public class RepeatWithoutStopTestCaseFixture : RepeatingTestsFixtureBase
    {
        [Repeat(3, false)]
        [TestCase(0)]
        public void FailsEveryTime(int unused)
        {
            Count++;
            Assert.Fail();
        }
    }

    public class RepeatStopOnFailurePropertyTrueTestCaseFixture : RepeatingTestsFixtureBase
    {
        [Repeat(3, StopOnFailure = true)]
        [TestCase]
        public void FailsEveryTime()
        {
            Count++;
            Assert.Fail();
        }
    }

    public class RepeatStopOnFailurePropertyFalseTestCaseFixture : RepeatingTestsFixtureBase
    {
        [Repeat(3, StopOnFailure = false)]
        [TestCase]
        public void FailsEveryTime()
        {
            Count++;
            Assert.Fail();
        }
    }

    public class RepeatOutputTestCaseFixture : RepeatingTestsFixtureBase
    {
        [Repeat(3, StopOnFailure = false)]
        [Test]
        public void PrintTest()
        {
            Console.WriteLine(Count++);
        }
    }

    public class RepeatOutputTestCaseWithMultipleFailuresFixture : RepeatingTestsFixtureBase
    {
        [Repeat(5, StopOnFailure = false)]
        [Test]
        public void PrintTest()
        {
            Console.WriteLine(Count++);
            Assert.That(Count, Is.Not.EqualTo(2).And.Not.EqualTo(3));
        }
    }

    public class RepeatOutputTestCaseWithFailuresFixture : RepeatingTestsFixtureBase
    {
        [Repeat(5, StopOnFailure = true)]
        [Test]
        public void PrintTest()
        {
            Console.WriteLine(Count++);
            Assert.That(Count, Is.Not.EqualTo(2).And.Not.EqualTo(3));
        }
    }

    public class RepeatWithThresholdAllPassFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, RequiredPassPercentage = 80)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    public class RepeatWithThresholdAboveThresholdFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = false, RequiredPassPercentage = 60)]
        public void FailsOnce()
        {
            Count++;
            if (Count == 1)
                Assert.Fail("Deliberate failure on first run");
        }
    }

    public class RepeatWithThresholdExactlyAtThresholdFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = false, RequiredPassPercentage = 80)]
        public void FailsOnce()
        {
            Count++;
            if (Count == 1)
                Assert.Fail("Deliberate failure on first run");
        }
    }

    public class RepeatWithThresholdBelowThresholdFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = false, RequiredPassPercentage = 80)]
        public void FailsMostRuns()
        {
            Count++;
            if (Count <= 3)
                Assert.Fail("Deliberate failure");
        }
    }

    public class RepeatWithThresholdStopOnFailureIgnoredFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = true, RequiredPassPercentage = 60)]
        public void FailsOnce()
        {
            Count++;
            if (Count == 1)
                Assert.Fail("Deliberate failure on first run");
        }
    }

    // Count=10, threshold=80%: stops early at run 8 (8 successes already guarantee 80% of 10)
    public class RepeatWithStopWhenDeterminedEarlySuccessFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(10, RequiredPassPercentage = 80, StopWhenOverallResultDetermined = true)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    // Count=10, threshold=80%: stops early at run 3 (max achievable drops to 7/10=70%, below 80%)
    public class RepeatWithStopWhenDeterminedEarlyFailureFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(10, RequiredPassPercentage = 80, StopWhenOverallResultDetermined = true)]
        public void AlwaysFails()
        {
            Count++;
            Assert.Fail("Deliberate failure");
        }
    }

    // -------------------------------------------------------------------------
    // Fixtures for StopOnFailure dual-property behaviour table (Issue #5220)
    // -------------------------------------------------------------------------

    // Row 1: [Repeat(5)] — StopOnFailure not set; defaults true via constructor chain
    public class StopOnFailureDualProp_Row1_DefaultFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5)]
        public void AlwaysFails()
        {
            Count++;
            Assert.Fail("deliberate");
        }
    }

    // Row 2: [Repeat(5, false)] — false supplied as constructor argument
    public class StopOnFailureDualProp_Row2_CtorFalseFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, false)]
        public void AlwaysFails()
        {
            Count++;
            Assert.Fail("deliberate");
        }
    }

    // Row 3: [Repeat(5, StopOnFailure=false)] — false set via property
    public class StopOnFailureDualProp_Row3_PropertyFalseFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = false)]
        public void AlwaysFails()
        {
            Count++;
            Assert.Fail("deliberate");
        }
    }

    // Row 4: [Repeat(5, StopOnFailure=true)] — true set via property
    public class StopOnFailureDualProp_Row4_PropertyTrueFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = true)]
        public void AlwaysFails()
        {
            Count++;
            Assert.Fail("deliberate");
        }
    }

    // Row 5: [Repeat(5, RequiredPassPercentage=80)] — threshold, StopOnFailure not explicitly set
    public class StopOnFailureDualProp_Row5_ThresholdDefaultFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, RequiredPassPercentage = 80)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    // Row 6: [Repeat(5, StopOnFailure=false, RequiredPassPercentage=80)] — threshold, explicit false
    public class StopOnFailureDualProp_Row6_ThresholdExplicitFalseFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = false, RequiredPassPercentage = 80)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    // Row 7: [Repeat(5, StopOnFailure=true, RequiredPassPercentage=80)] — should produce ResultState.Error
    public class StopOnFailureDualProp_Row7_ThresholdExplicitTrueFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(5, StopOnFailure = true, RequiredPassPercentage = 80)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    // RequiredPassPercentage = 0 — out of range, Wrap() throws ArgumentOutOfRangeException
    // which the framework catches and surfaces as ResultState.Error.
    public class RepeatWithInvalidPercentageBelowRangeFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, RequiredPassPercentage = 0)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }

    // RequiredPassPercentage = 101 — out of range, same Error path.
    public class RepeatWithInvalidPercentageAboveRangeFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3, RequiredPassPercentage = 101)]
        public void AlwaysPasses()
        {
            Count++;
        }
    }
}
