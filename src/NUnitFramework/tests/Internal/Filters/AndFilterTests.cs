using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class AndFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void PassTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(filter.Pass(_anotherFixture));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><and><cat>Dummy</cat><id>{0}</id></and></filter>", _dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFromXml_TopLevelDefaultsToAnd()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><cat>Dummy</cat><id>{0}</id></filter>", _dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }
    }
}
