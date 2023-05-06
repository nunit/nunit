// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture("Priority", "High", false)]
    [TestFixture("Author", "Charlie", true)]
    public class PropertyFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public PropertyFilterTests(string name, string value, bool isRegex)
        {
            _filter = new PropertyFilter(name, value, isRegex);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(DummyFixtureSuite));
            Assert.That(_filter.Match(AnotherFixtureSuite), Is.False);
            Assert.That(_filter.Match(YetAnotherFixtureSuite), Is.False);
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(TopLevelSuite));
            Assert.That(_filter.Pass(DummyFixtureSuite));
            Assert.That(_filter.Pass(DummyFixtureSuite.Tests[0]));

            Assert.That(_filter.Pass(AnotherFixtureSuite), Is.False);
            Assert.That(_filter.Pass(YetAnotherFixtureSuite), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            Assert.That(_filter.IsExplicitMatch(TopLevelSuite));
            Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite));
            Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite.Tests[0]), Is.False);

            Assert.That(_filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);
            Assert.That(_filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);
        }
    }
}
