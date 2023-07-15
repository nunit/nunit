// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            Assert.That(interval.IsNotZero, Is.True);

            interval = new Interval(0);
            Assert.That(interval.IsNotZero, Is.False);
        }
    }
}
