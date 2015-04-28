using NUnit.Framework;

namespace NUnit.Integration.Mocks
{
    public class OneIgnoredTest
    {
        [Test, Category("OneIgnoredTest")]
        public void Test1()
        {
            Assert.Ignore("Reason");
        }
    }
}
