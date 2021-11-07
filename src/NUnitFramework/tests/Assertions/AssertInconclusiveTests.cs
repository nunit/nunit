// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Assertions
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
        public void ThrowsInconclusiveExceptionWithMessageAndArgs()
        {
            Assert.That(
                () => Assert.Inconclusive("MESSAGE: {0}+{1}={2}", 2, 2, 4),
                Throws.TypeOf<InconclusiveException>().With.Message.EqualTo("MESSAGE: 2+2=4"));
        }
    }
}
