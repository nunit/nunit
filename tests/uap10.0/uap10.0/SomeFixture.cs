using NUnit.Framework;

namespace uap10_0
{
    [TestFixture] // https://github.com/nunit/nunit/issues/2896
    public static class SomeFixture
    {
        [Test]
        public static void SomeTest()
        {
            Assert.Pass();
        }
    }
}
