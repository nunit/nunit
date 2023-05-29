// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    public class EmptyFilterTests : TestFilterTests
    {
        [Test]
        public void IsEmpty()
        {
            Assert.That(TestFilter.Empty.IsEmpty);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("<filter/>")]
        public void BuildFromXml(string xml)
        {
            TestFilter filter = TestFilter.FromXml(xml);

            Assert.That(filter.IsEmpty);
        }

        [Test]
        public void MatchesAnything()
        {
            Assert.That(TestFilter.Empty.Match(DummyFixtureSuite));
            Assert.That(TestFilter.Empty.Match(AnotherFixtureSuite));
            Assert.That(TestFilter.Empty.Match(YetAnotherFixtureSuite));
        }

        [Test]
        public void PassesAnything()
        {
            Assert.That(TestFilter.Empty.Match(DummyFixtureSuite));
            Assert.That(TestFilter.Empty.Match(AnotherFixtureSuite));
            Assert.That(TestFilter.Empty.Match(YetAnotherFixtureSuite));
        }

        [Test]
        public void MatchesNothingExplicitly()
        {
            Assert.That(TestFilter.Empty.IsExplicitMatch(DummyFixtureSuite), Is.False);
            Assert.That(TestFilter.Empty.IsExplicitMatch(AnotherFixtureSuite), Is.False);
            Assert.That(TestFilter.Empty.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);
        }
    }
}
