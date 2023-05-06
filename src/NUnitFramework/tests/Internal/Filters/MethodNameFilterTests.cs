// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture("Test", false)]
    [TestFixture("T.st", true)]
    public class MethodNameFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public MethodNameFilterTests(string value, bool isRegex)
        {
            _filter = new MethodNameFilter(value, isRegex);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.Match(DummyFixtureSuite.Tests[0]));
                Assert.That(_filter.Match(AnotherFixtureSuite.Tests[0]));
            });
        }

        [Test]
        public void PassTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.Pass(TopLevelSuite));
                Assert.That(_filter.Pass(DummyFixtureSuite));
                Assert.That(_filter.Pass(DummyFixtureSuite.Tests[0]));

                Assert.That(_filter.Pass(AnotherFixtureSuite));
                Assert.That(_filter.Pass(AnotherFixtureSuite.Tests[0]));
                Assert.That(_filter.Pass(YetAnotherFixtureSuite), Is.False);
            });
        }

        [Test]
        public void ExplicitMatch_SingleName()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.IsExplicitMatch(TopLevelSuite));
                Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite));
                Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite.Tests[0]));
                Assert.That(_filter.IsExplicitMatch(AnotherFixtureSuite));
            });
        }
    }
}
