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

            Assert.That(filter.Match(TopLevelSuite), Is.True);
            Assert.That(filter.Match(DummyFixtureSuite), Is.False);
            Assert.That(filter.Match(DummyFixtureSuite.Tests[0]), Is.True);

            Assert.That(filter.Match(AnotherFixtureSuite), Is.True);

            Assert.That(filter.Match (FixtureWithMultipleTestsSuite), Is.True);
            Assert.That(filter.Match (FixtureWithMultipleTestsSuite.Tests[0]), Is.True);
            Assert.That(filter.Match (FixtureWithMultipleTestsSuite.Tests[1]), Is.False);
        }

        [Test]
        public void PassTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.Pass(TopLevelSuite), Is.True);
            Assert.That(filter.Pass(DummyFixtureSuite), Is.False);
            Assert.That(filter.Pass(DummyFixtureSuite.Tests[0]), Is.False);

            Assert.That(filter.Pass(AnotherFixtureSuite), Is.True);
            Assert.That(filter.Pass(AnotherFixtureSuite.Tests[0]), Is.True);

            Assert.That(filter.Pass (FixtureWithMultipleTestsSuite), Is.True);
            Assert.That(filter.Pass (FixtureWithMultipleTestsSuite.Tests[0]), Is.True);
            Assert.That(filter.Pass (FixtureWithMultipleTestsSuite.Tests[1]), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.That(filter.IsExplicitMatch (TopLevelSuite), Is.False);
            Assert.That(filter.IsExplicitMatch (DummyFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch (DummyFixtureSuite.Tests[0]), Is.False);

            Assert.That(filter.IsExplicitMatch (AnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch (AnotherFixtureSuite.Tests[0]), Is.False);
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><not><cat>Dummy</cat></not></filter>");

            Assert.That(filter, Is.TypeOf<NotFilter>());
            Assert.That(filter.Match(DummyFixtureSuite), Is.False);
            Assert.That(filter.Match(AnotherFixtureSuite), Is.True);
        }
    }
}
