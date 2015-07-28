using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class SimpleNameFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new SimpleNameFilter(_dummyFixture.FullName);

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void Match_SingleName()
        {
            var filter = new SimpleNameFilter(_dummyFixture.FullName);

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void Pass_SingleName()
        {
            var filter = new SimpleNameFilter(_dummyFixture.FullName);

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(filter.Pass(_anotherFixture));
        }

        public void ExplicitMatch_SingleName()
        {
            var filter = new SimpleNameFilter(_dummyFixture.FullName);

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void Match_MultipleNames()
        {
            var filter = new SimpleNameFilter(new string[] { _dummyFixture.FullName, _anotherFixture.FullName });

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void Pass_MultipleNames()
        {
            var filter = new SimpleNameFilter(new string[] { _dummyFixture.FullName, _anotherFixture.FullName });

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter.Pass(_anotherFixture));
            Assert.That(filter.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatch_MultipleNames()
        {
            var filter = new SimpleNameFilter(new string[] { _dummyFixture.FullName, _anotherFixture.FullName });

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.That(filter.IsExplicitMatch(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));
        }

        [Test]
        public void AddNames()
        {
            var filter = new SimpleNameFilter();
            filter.Add(_dummyFixture.FullName);
            filter.Add(_anotherFixture.FullName);

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildFromXml_SingleName()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><tests><test>" + _dummyFixture.FullName + "</test></tests></filter>");

            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFromXml_MultipleNames()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><tests><test>" + _dummyFixture.FullName + "</test><test>" + _anotherFixture.FullName + "</test></tests></filter>");

            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }
    }
}
