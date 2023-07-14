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
            Framework.Classic.Assert.IsTrue (true);
        }
    }

    public class RepeatFailOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnFirst()
        {
            Count++;
            Framework.Classic.Assert.IsFalse (true);
        }
    }

    public class RepeatFailOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            Count++;

            if (Count == 2)
                Framework.Classic.Assert.IsTrue(false);
        }
    }

    public class RepeatFailOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Repeat(3)]
        public void RepeatFailOnThird()
        {
            Count++;

            if (Count == 3)
                Framework.Classic.Assert.IsTrue(false);
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
            Framework.Classic.Assert.IsTrue(true);
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
}
