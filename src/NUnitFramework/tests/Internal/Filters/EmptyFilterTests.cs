using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class EmptyFilterTests : TestFilterTests
    {
        [Test]
        public void IsEmpty()
        {
            Assert.That(TestFilter.Empty.IsEmpty);
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml("<filter/>");

            Assert.That(filter.IsEmpty);
        }

        [Test]
        public void MatchesAnything()
        {
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void PassesAnything()
        {
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void MatchesNothihngExplicitly()
        {
            Assert.False(TestFilter.Empty.IsExplicitMatch(_dummyFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_anotherFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_yetAnotherFixture));
        }
    }
}
