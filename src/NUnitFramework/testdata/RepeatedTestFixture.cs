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

    public class RepeatOutputTestCaseWithFailuresFixture : RepeatingTestsFixtureBase
    {
        [Repeat(5, StopOnFailure = false)]
        [Test]
        public void PrintTest()
        {
            Console.WriteLine(Count++);
            Assert.That(Count, Is.Not.EqualTo(2).And.Not.EqualTo(3));
        }
    }
}
