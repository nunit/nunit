using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class TwoSuccesfulTests
    {
        [Test, Category("TwoSuccesfulTests")]
        public void Test1()
        {
            Utilities.DoSomething();
            Assert.Pass("Reason");
        }

        [Test, Category("TwoSuccesfulTests")]
        public void Test2()
        {
            Utilities.DoSomething();
        }
    }
}
