// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData.RepeatingTests
{
    public class RetrySucceedsOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void SucceedsEveryTime()
        {
            Count++;
            Assert.IsTrue(true);
        }
    }

    public class RetryFailsEveryTimeFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void FailsEveryTime()
        {
            Count++;
            Assert.IsFalse(true);
        }
    }

    public class RetrySucceedsOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void SucceedsOnSecondTry()
        {
            Count++;

            if (Count < 2)
                Assert.IsTrue(false);
        }
    }

    public class RetrySucceedsOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void SucceedsOnThirdTry()
        {
            Count++;

            if (Count < 3)
                Assert.IsTrue(false);
        }
    }

    public class RetryWithIgnoreAttributeFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3), Ignore("Ignore this test")]
        public void RepeatShouldIgnore()
        {
            Assert.Fail("Ignored test executed");
        }
    }

    public class RetryIgnoredOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;
            Assert.Ignore("Ignoring");
        }
    }

    public class RetryIgnoredOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;

            if (Count < 2)
                Assert.Fail("Failed");

            Assert.Ignore("Ignoring");
        }
    }

    public class RetryIgnoredOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;

            if (Count < 3)
                Assert.Fail("Failed");

            Assert.Ignore("Ignoring");
        }
    }

    public class RetryErrorOnFirstTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;
            throw new Exception("Deliberate Exception");
        }
    }

    public class RetryErrorOnSecondTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;

            if (Count < 2)
                Assert.Fail("Failed");

            throw new Exception("Deliberate Exception");
        }
    }

    public class RetryErrorOnThirdTryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void Test()
        {
            Count++;

            if (Count < 3)
                Assert.Fail("Failed");

            throw new Exception("Deliberate Exception");
        }
    }

    public class RetryTestWithCategoryFixture : RepeatingTestsFixtureBase
    {
        [Test, Retry(3), Category("SAMPLE")]
        public void TestWithCategory()
        {
            Count++;
            Assert.IsTrue(true);
        }
    }

    public class RetryTestCaseFixture : RepeatingTestsFixtureBase
    {
        [Retry(3)]
        [TestCase(0)]
        public void FailsEveryTime(int unused)
        {
            Count++;
            Assert.IsTrue(false);
        }
    }

    public sealed class RetryWithoutSetUpOrTearDownFixture
    {
        public int Count { get; private set; }

        [Test, Retry(3)]
        public void SucceedsOnThirdTry()
        {
            Count++;

            if (Count < 3)
                Assert.Fail();
        }

        [Test, Retry(3)]
        public void FailsEveryTime()
        {
            Count++;
            Assert.Fail();
        }

        [Test, Retry(3)]
        public void ErrorsOnFirstTry()
        {
            Count++;
            throw new Exception("Deliberate exception");
        }
    }

    public class RetryTestVerifyAttempt : RepeatingTestsFixtureBase
    {
        [Test, Retry(3)]
        public void NeverPasses()
        {
            Count = TestContext.CurrentContext.CurrentRepeatCount;
            Assert.Fail("forcing a failure so we retry maximum times");
        }

        [Test, Retry(3)]
        public void PassesOnLastRetry()
        {
            Assert.That(Count, Is.EqualTo(TestContext.CurrentContext.CurrentRepeatCount), "expected CurrentRepeatCount to be incremented only after first attempt");
            if (Count < 2) // second Repeat is 3rd Retry (i.e. end of attempts)
            {
                Count++;
                Assert.Fail("forced failure so we will use maximum number of Retries for PassesOnLastRetry");
            }
        }
    }
}
