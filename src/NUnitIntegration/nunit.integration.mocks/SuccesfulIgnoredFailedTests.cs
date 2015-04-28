using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class SuccesfulIgnoredFailedTests
    {
        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test1()
        {
            Utilities.DoSomething();
            Assert.Pass("Reason");
        }

        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test2()
        {
            Utilities.DoSomething();
            Assert.Ignore("Reason");
        }

        [Test, Category("SuccesfulIgnoredFailedTests")]
        public void Test3()
        {
            Utilities.DoSomething();
            Assert.Fail("Reason");
        }
    }
}
