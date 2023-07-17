// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
    }
}
