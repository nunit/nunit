// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnit.TestData.TestContextData
{
    [TestFixture]
    public class TestStateRecordingFixture
    {
        public string StateList;

        public bool TestFailure;
        public bool TestInconclusive;
        public bool SetUpFailure;
        public bool SetUpIgnore;

        [SetUp]
        public void SetUp()
        {
            StateList = TestContext.CurrentContext.Result.Outcome + "=>";

            if (SetUpFailure)
                Assert.Fail("Failure in SetUp");
            if (SetUpIgnore)
                Assert.Ignore("Ignored in SetUp");
        }

        [Test]
        public void TheTest()
        {
            StateList += TestContext.CurrentContext.Result.Outcome;

            if (TestFailure)
                Assert.Fail("Deliberate failure");
            if (TestInconclusive)
                Assert.Inconclusive("Inconclusive test");
        }

        [TearDown]
        public void TearDown()
        {
            StateList += "=>" + TestContext.CurrentContext.Result.Outcome;
        }
    }

    public class AssertionResultFixture
    {
        public IEnumerable<AssertionResult> Assertions;

        public void ThreeAsserts_TwoFailed()
        {
            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(5));
                Assert.That(2 + 2, Is.EqualTo(4));
                Assert.That(2 + 2, Is.EqualTo(5));

                Assertions = TestContext.CurrentContext.Result.Assertions;
            });
        }

        public void WarningPlusFailedAssert()
        {
            Warn.Unless(2 + 2, Is.EqualTo(5));

            Assert.Multiple(() =>
            {
                Assert.That(2 + 2, Is.EqualTo(5));

                Assertions = TestContext.CurrentContext.Result.Assertions;
            });
        }
    }

    [TestFixture]
    public class TestTestContextInTearDown
    {
        public int FailCount { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        [Test]
        public void FailingTest()
        {
            Assert.Fail("Deliberate failure");
        }

        [TearDown]
        public void TearDown()
        {
            FailCount = TestContext.CurrentContext.Result.FailCount;
            Message = TestContext.CurrentContext.Result.Message;
            StackTrace = TestContext.CurrentContext.Result.StackTrace;
        }
    }

    [TestFixture]
    public class TestTestContextInOneTimeTearDown
    {
        public int PassCount { get; private set; }
        public int FailCount { get; private set; }
        public int WarningCount { get; private set; }
        public int SkipCount { get; private set; }
        public int InconclusiveCount { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }

        [Test]
        public void FailingTest()
        {
            Assert.Fail("Deliberate failure");
        }

        [Test]
        public void PassingTest()
        {
            Assert.Pass();
        }

        [Test]
        public void AnotherPassingTest()
        {
            Assert.Pass();
        }

        [Test]
        public void IgnoredTest()
        {
            Assert.Ignore("I don't want to run this test");
        }

        [Test]
        public void IgnoredTestTwo()
        {
            Assert.Ignore("I don't want to run this test either");
        }

        [Test]
        public void IgnoredTestThree()
        {
            Assert.Ignore("Nor do I want to run this test");
        }

        [Test]
        public void AssumeSomething()
        {
            Assume.That(false);
        }

        [Test]
        public void AssumeSomethingElse()
        {
            Assume.That(false);
        }

        [Test]
        public void NeverAssume()
        {
            Assume.That(false);
        }

        [Test]
        public void AssumeTheWorldIsFlat()
        {
            Assume.That(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PassCount = TestContext.CurrentContext.Result.PassCount;
            FailCount = TestContext.CurrentContext.Result.FailCount;
            WarningCount = TestContext.CurrentContext.Result.WarningCount;
            SkipCount = TestContext.CurrentContext.Result.SkipCount;
            InconclusiveCount = TestContext.CurrentContext.Result.InconclusiveCount;
            Message = TestContext.CurrentContext.Result.Message;
        }
    }
}
