using NUnit.TestUtilities;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public class ThrowsTests
    {
        [Test]
        public void ArgumentNullException_ConstraintMatchesThrownArgumentNullException()
        {
            Assert.That(TestDelegates.ThrowsArgumentNullException, Throws.ArgumentNullException);
        }
    }
}
