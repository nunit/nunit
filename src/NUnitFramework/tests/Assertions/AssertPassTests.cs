// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
