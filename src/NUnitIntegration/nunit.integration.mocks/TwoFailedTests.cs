using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class TwoFailedTests
    {
        [Test, Category("TwoFailedTests")]
        public void Test1()
        {
            Utilities.DoSomething();
            Assert.Fail("Reason");
        }

        [Test, Category("TwoFailedTests")]
        public void Test2()
        {
            Utilities.DoSomething();
            Assert.Fail();
        }
    }
}
