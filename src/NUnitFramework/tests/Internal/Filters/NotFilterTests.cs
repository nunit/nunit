using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class NotFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.True(filter.Match(_topLevelSuite));
            Assert.False(filter.Match(_dummyFixture));
            Assert.True(filter.Match(_dummyFixture.Tests[0]));

            Assert.True(filter.Match(_anotherFixture));
        }

        [Test]
        public void PassTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False(filter.Pass(_topLevelSuite));
            Assert.False(filter.Pass(_dummyFixture));
            Assert.False(filter.Pass(_dummyFixture.Tests[0]));

            Assert.True(filter.Pass(_anotherFixture));
            Assert.True(filter.Pass(_anotherFixture.Tests[0]));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False(filter.IsExplicitMatch(_topLevelSuite));
            Assert.False(filter.IsExplicitMatch(_dummyFixture));
            Assert.True(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.True(filter.IsExplicitMatch(_anotherFixture));
            Assert.True(filter.IsExplicitMatch(_anotherFixture.Tests[0]));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><not><cat>Dummy</cat></not></filter>");

            Assert.That(filter, Is.TypeOf<NotFilter>());
            Assert.That(filter, Has.Property("TopLevel").EqualTo(true));
            Assert.False(filter.Match(_dummyFixture));
            Assert.True(filter.Match(_anotherFixture));
        }
    }
}
