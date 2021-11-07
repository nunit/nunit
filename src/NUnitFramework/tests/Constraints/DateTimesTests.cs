// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DateTimesTests
    {
        [Test]
        public void CanCalculateTimeSpanDifference()
        {
            Assert.That(DateTimes.Difference(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(100)), Is.EqualTo(TimeSpan.FromSeconds(90)));
        }

        [Test]
        public void CanCalculateDateTimeDifference()
        {
            var date = DateTime.Now;
            Assert.That(DateTimes.Difference(date, date.AddSeconds(90)), Is.EqualTo(TimeSpan.FromSeconds(90)));
        }

        [Test]
        public void CanCalculateDateTimeOffsetDifference()
        {
            var dateOffset = DateTimeOffset.Now;
            Assert.That(DateTimes.Difference(dateOffset, dateOffset.AddSeconds(90)), Is.EqualTo(TimeSpan.FromSeconds(90)));
        }

        [Test]
        public void FailsOnNonDateTimeTypes()
        {
            Assert.Throws<ArgumentException>(() => DateTimes.Difference(10, 100));
        }
    }
}
