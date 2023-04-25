// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    public class NotFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.Match(_topLevelSuite), Is.True);
            Assert.That(filter.Match(_dummyFixture), Is.False);
            Assert.That(filter.Match(_dummyFixture.Tests[0]), Is.True);

            Assert.That(filter.Match(_anotherFixture), Is.True);

            Assert.That(filter.Match (_fixtureWithMultipleTests), Is.True);
            Assert.That(filter.Match (_fixtureWithMultipleTests.Tests[0]), Is.True);
            Assert.That(filter.Match (_fixtureWithMultipleTests.Tests[1]), Is.False);
        }

        [Test]
        public void PassTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.Pass(_topLevelSuite), Is.True);
            Assert.That(filter.Pass(_dummyFixture), Is.False);
            Assert.That(filter.Pass(_dummyFixture.Tests[0]), Is.False);

            Assert.That(filter.Pass(_anotherFixture), Is.True);
            Assert.That(filter.Pass(_anotherFixture.Tests[0]), Is.True);

            Assert.That(filter.Pass (_fixtureWithMultipleTests), Is.True);
            Assert.That(filter.Pass (_fixtureWithMultipleTests.Tests[0]), Is.True);
            Assert.That(filter.Pass (_fixtureWithMultipleTests.Tests[1]), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.IsExplicitMatch (_topLevelSuite), Is.False);
            Assert.That(filter.IsExplicitMatch (_dummyFixture), Is.False);
            Assert.That(filter.IsExplicitMatch (_dummyFixture.Tests[0]), Is.False);

            Assert.That(filter.IsExplicitMatch (_anotherFixture), Is.False);
            Assert.That(filter.IsExplicitMatch (_anotherFixture.Tests[0]), Is.False);
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><not><cat>Dummy</cat></not></filter>");

            Assert.That(filter, Is.TypeOf<NotFilter>());
            Assert.That(filter.Match(_dummyFixture), Is.False);
            Assert.That(filter.Match(_anotherFixture), Is.True);
        }
    }
}
