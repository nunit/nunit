// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertPassTests
    {
        [Test]
        public void ThrowsSuccessException()
        {
            Assert.That(
                () => Assert.Pass(),
                Throws.TypeOf<SuccessException>());
        }

        [Test]
        public void ThrowsSuccessExceptionWithMessage()
        {
            Assert.That(
                () => Assert.Pass("MESSAGE"),
                Throws.TypeOf<SuccessException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void SetsCurrentResultBeforeThrowing()
        {
            try
            {
                Assert.Pass("MESSAGE");
            }
            catch (SuccessException)
            {
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(TestContext.CurrentContext.Result.Outcome, Is.EqualTo(ResultState.Success));
                Assert.That(TestContext.CurrentContext.Result.Message, Is.EqualTo("MESSAGE"));
            }
        }

        [Test]
        public void AssertPassReturnsSuccess()
        {
            Assert.Pass("This test is OK!");
        }

        [Test]
        public void SubsequentFailureIsIrrelevant()
        {
            Assert.Pass("This test is OK!");
            Assert.Fail("No it's NOT!");
        }
    }
}
