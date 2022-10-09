// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertFailTests
    {
        [Test]
        public void ThrowsAssertionException()
        {
            Assert.That(
                () => Assert.Fail(),
                Throws.TypeOf<AssertionException>());
        }

        [Test]
        public void ThrowsAssertionException_MessageSpecified()
        {
            Assert.That(
                () => Assert.Fail(),
                Throws.TypeOf<AssertionException>(),
                "My Message");
        }

        [Test]
        public void ThrowsAssertionExceptionWithMessage()
        {
            Assert.That(
                () => Assert.Fail("MESSAGE"), 
                Throws.TypeOf<AssertionException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void ThrowsAssertionExceptionWithMessageAndArgs()
        {
            Assert.That(
                () => Assert.Fail("MESSAGE: {0}+{1}={2}", 2, 2, 4),
                Throws.TypeOf<AssertionException>().With.Message.EqualTo("MESSAGE: 2+2=4"));
        }

        [Test]
        public void AssertFailWorks()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                "CallAssertFail");

            Assert.AreEqual(ResultState.Failure, result.ResultState);

            Assert.AreEqual(1, result.AssertionResults.Count);
            var assertion = result.AssertionResults[0];
            Assert.That(assertion.Status, Is.EqualTo(AssertionStatus.Failed));
        }

        [Test]
        public void AssertFailWorksWithMessage()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                "CallAssertFailWithMessage");

            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.AreEqual("MESSAGE", result.Message);

            Assert.AreEqual(1, result.AssertionResults.Count);
            var assertion = result.AssertionResults[0];
            Assert.That(assertion.Status, Is.EqualTo(AssertionStatus.Failed));
            Assert.That(assertion.Message, Is.EqualTo("MESSAGE"));
        }

        [Test]
        public void AssertFailWorksWithMessageAndArgs()
        {
            ITestResult result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                "CallAssertFailWithMessageAndArgs");

            Assert.AreEqual(ResultState.Failure, result.ResultState);
            Assert.AreEqual("MESSAGE: 2+2=4", result.Message);

            Assert.AreEqual(1, result.AssertionResults.Count);
            var assertion = result.AssertionResults[0];
            Assert.That(assertion.Status, Is.EqualTo(AssertionStatus.Failed));
            Assert.That(assertion.Message, Is.EqualTo("MESSAGE: 2+2=4"));
        }

        [Test]
        public void TestStillFailsIfAssertionExceptionIsHandled()
        {
            var result = TestBuilder.RunTestCase(
                typeof(AssertFailFixture),
                nameof(AssertFailFixture.HandleAssertionException));

            Assert.That(result.AssertionResults.Count, Is.EqualTo(1));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Custom message"));
        }

        [Test]
        public void AssertCatchMakesTestPass()
        {
            Assert.Catch(() =>
            {
                Assert.Fail("This should not be seen");
            });

            // Ensure that no spurious info was recorded from the assertion
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.AssertionResults, Is.Empty);
        }

        [Test]
        public void AssertThrowsMakesTestPass()
        {
            Assert.Throws<AssertionException>(() =>
            {
                Assert.Fail("This should not be seen");
            });

            // Ensure that no spurious info was recorded from the assertion
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.AssertionResults.Count, Is.EqualTo(0));
        }
    }
}
