using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class SuccesfulIgnoredFailedTests
    {
        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test1()
        {
            Assert.Pass("Reason");
        }

        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test2()
        {
            Assert.Ignore("Reason");
        }

        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test3()
        {
            Assert.Fail("Reason");
        }
    }
}
