using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class IntervalTests
    {
        [Test]
        public void IsNonZeroInterval()
        {
            var interval = new Interval(1);
            Assert.IsTrue(interval.IsNotZero);

            interval =  new Interval(0);
            Assert.IsFalse(interval.IsNotZero);
        }
    }
}
