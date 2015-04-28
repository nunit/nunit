using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class TwoIgnoredTests
    {
        [Test, Category("TwoIgnoredTests")]
        public void Test1()
        {
            Assert.Ignore("Reason");
        }

        [Test, Category("TwoIgnoredTests")]
        public void Test2()
        {
            Assert.Ignore();
        }
    }
}
