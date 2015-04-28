using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class OneFailedTest
    {
        [Test, Category("OneFailedTest")]
        public void Test1()
        {
            Utilities.DoSomething();
            Assert.Fail("Reason");
        }
    }
}
