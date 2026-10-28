// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertInconclusiveTests
    {
        [Test]
        public void ThrowsInconclusiveException()
        {
            Assert.That(
                () => Assert.Inconclusive(),
                Throws.TypeOf<InconclusiveException>());
        }

        [Test]
        public void ThrowsInconclusiveExceptionWithMessage()
        {
            Assert.That(
                () => Assert.Inconclusive("MESSAGE"),
                Throws.TypeOf<InconclusiveException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void SetsCurrentResultBeforeThrowing()
        {
            try
            {
                Assert.Inconclusive("MESSAGE");
            }
            catch (InconclusiveException)
            {
            }

            Assert.That(TestContext.CurrentContext.Result.Outcome, Is.EqualTo(ResultState.Inconclusive));
            Assert.That(TestContext.CurrentContext.Result.Message, Is.EqualTo("MESSAGE"));
        }
    }
}
