// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Text;

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
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void PassesAnything()
        {
            Assert.That(TestFilter.Empty.Match(_dummyFixture));
            Assert.That(TestFilter.Empty.Match(_anotherFixture));
            Assert.That(TestFilter.Empty.Match(_yetAnotherFixture));
        }

        [Test]
        public void MatchesNothingExplicitly()
        {
            Assert.False(TestFilter.Empty.IsExplicitMatch(_dummyFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_anotherFixture));
            Assert.False(TestFilter.Empty.IsExplicitMatch(_yetAnotherFixture));
        }
    }
}
