// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssumeEqualsTests
    {
        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assume.Equals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("Assume.Equals should not be used."));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assume.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex?.Message, Does.StartWith("Assume.ReferenceEquals should not be used."));
        }
    }
}
