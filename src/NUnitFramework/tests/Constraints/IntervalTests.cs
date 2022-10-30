// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
