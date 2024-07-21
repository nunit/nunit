// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertFailTests
    {
        [Test]
        public void ThrowsSingleAssertException()
        {
            Assert.That(Assert.Fail, Throws.TypeOf<SingleAssertException>());
        }

        [Test]
        public void ThrowsSingleAssertException_MessageSpecified()
        {
            Assert.That(Assert.Fail, Throws.TypeOf<SingleAssertException>(), "My Message");
        }

        [Test]
        public void ThrowsSingleAssertExceptionWithMessage()
        {
            Assert.That(() => Assert.Fail("MESSAGE"), Throws.TypeOf<SingleAssertException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void AssertFailWorks()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                "CallAssertFail");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.AssertionResults, Has.Count.EqualTo(1));
            var assertion = result.AssertionResults[0];
            Assert.That(assertion.Status, Is.EqualTo(AssertionStatus.Failed));
        }

        [Test]
        public void AssertFailWorksWithMessage()
        {
            ITestResult result = TestBuilder.RunTestCase(typeof(AssertFailFixture), "CallAssertFailWithMessage");
            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.Message, Is.EqualTo("MESSAGE"));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(1));
            });
            var assertion = result.AssertionResults[0];
            Assert.Multiple(() =>
            {
                Assert.That(assertion.Status, Is.EqualTo(AssertionStatus.Failed));
                Assert.That(assertion.Message, Is.EqualTo("MESSAGE"));
            });
        }

        [Test]
        public void TestStillFailsIfSingleAssertExceptionIsHandled()
        {
            var result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                nameof(AssertFailFixture.HandleAnyException));

            Assert.Multiple(() =>
            {
                Assert.That(result.AssertionResults, Has.Count.EqualTo(1));
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(result.Message, Is.EqualTo("Custom message"));
            });
        }

        [Test]
        public void AssertCatchMakesTestPass()
        {
            Assert.Catch(() => Assert.Fail("This should not be seen"));

            // Ensure that no spurious info was recorded from the assertion
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.AssertionResults, Is.Empty);
        }

        [Test]
        public void AssertThrowsMakesTestPass()
        {
            Assert.Throws<SingleAssertException>(() => Assert.Fail("This should not be seen"));

            // Ensure that no spurious info was recorded from the assertion
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.AssertionResults, Is.Empty);
        }
    }
}
