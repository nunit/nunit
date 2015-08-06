using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class OrFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void PassTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter.Pass(_anotherFixture));
            Assert.That(filter.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.That(filter.IsExplicitMatch(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><or><cat>Dummy</cat><cat>Another</cat></or></filter>");

            Assert.That(filter, Is.TypeOf<OrFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
        }
    }
}
