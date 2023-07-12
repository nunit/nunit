// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    [TestFixture(TestFilterTests.DUMMY_CLASS, false)]
    [TestFixture("Dummy", true)]
    public class FullNameFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public FullNameFilterTests(string value, bool isRegex)
        {
            _filter = new FullNameFilter(value, isRegex);
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
                Assert.That(_filter.Match(DummyFixtureSuite));
                Assert.That(_filter.Match(AnotherFixtureSuite), Is.False);
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

                Assert.That(_filter.Pass(AnotherFixtureSuite), Is.False);
            });
        }

        [Test]
        [Ignore("Previously this didn't have a Test attribute and hence was never tested")]
        public void ExplicitMatchTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.IsExplicitMatch(TopLevelSuite));
                Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite));
                Assert.That(_filter.IsExplicitMatch(DummyFixtureSuite.Tests[0]), Is.False);

                Assert.That(_filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);
            });
        }
    }
}
