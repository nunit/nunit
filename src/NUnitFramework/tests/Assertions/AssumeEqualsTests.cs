using System;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssumeEqualsTests
    {
        [Test]
        public void EqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assume.Equals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("Assume.Equals should not be used for Assertions"));
        }

        [Test]
        public void ReferenceEqualsFailsWhenUsed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Assume.ReferenceEquals(string.Empty, string.Empty));
            Assert.That(ex.Message, Does.StartWith("Assume.ReferenceEquals should not be used for Assertions"));
        }
    }
}
