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
            Assert.That(_filter.Match(_dummyFixture.Tests[0]));
            Assert.That(_filter.Match(_anotherFixture.Tests[0]));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(_topLevelSuite));
            Assert.That(_filter.Pass(_dummyFixture));
            Assert.That(_filter.Pass(_dummyFixture.Tests[0]));

            Assert.That(_filter.Pass(_anotherFixture));
            Assert.That(_filter.Pass(_anotherFixture.Tests[0]));
            Assert.That(_filter.Pass(_yetAnotherFixture), Is.False);
        }

        public void ExplicitMatch_SingleName()
        {
            Assert.That(_filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(_filter.IsExplicitMatch(_dummyFixture));
            Assert.That(_filter.IsExplicitMatch(_dummyFixture.Tests[0]), Is.False);

            Assert.That(_filter.IsExplicitMatch(_anotherFixture), Is.False);
        }
    }
}
