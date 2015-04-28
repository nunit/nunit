using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class TwoSuccesfulTests
    {
        [Test, Category("TwoSuccesfulTests")]
        public void Test1()
        {
        }

        [Test, Category("TwoSuccesfulTests")]
        public void Test2()
        {
        }
    }
}
