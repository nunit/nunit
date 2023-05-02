// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture("TestFilterTests+DummyFixture", false)]
    [TestFixture("Dummy", true)]
    public class DoubleNegativeFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public DoubleNegativeFilterTests(string value, bool isRegex)
        {
            _filter = new NotFilter(new NotFilter(new TestNameFilter(value, isRegex)));
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
                Assert.That(_filter.Match(_dummyFixture));
                Assert.That(_filter.Match(_anotherFixture), Is.False);
            });
        }

        [Test]
        public void PassTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.Pass(_topLevelSuite));
                Assert.That(_filter.Pass(_dummyFixture));
                Assert.That(_filter.Pass(_dummyFixture.Tests[0]));

                Assert.That(_filter.Pass(_anotherFixture), Is.False);
            });
        }

        [Test]
        [Ignore("Previously this didn't have a Test attribute and hence was never tested")]
        public void ExplicitMatchTest()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_filter.IsExplicitMatch(_topLevelSuite));
                Assert.That(_filter.IsExplicitMatch(_dummyFixture));
                Assert.That(_filter.IsExplicitMatch(_dummyFixture.Tests[0]), Is.False);

                Assert.That(_filter.IsExplicitMatch(_anotherFixture), Is.False);
            });
        }
    }
}
