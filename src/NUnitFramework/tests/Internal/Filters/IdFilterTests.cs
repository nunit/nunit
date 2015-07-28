using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class IdFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new IdFilter(_dummyFixture.Id);

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void Match_SingleId()
        {
            var filter = new IdFilter(_dummyFixture.Id);

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void Pass_SingleId()
        {
            var filter = new IdFilter(_dummyFixture.Id);

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(filter.Pass(_anotherFixture));
        }

        [Test]
        public void ExplicitMatch_SingleId()
        {
            var filter = new IdFilter(_dummyFixture.Id);

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void Match_MultipleIds()
        {
            var filter = new IdFilter(new string[] { _dummyFixture.Id, _anotherFixture.Id });

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void Pass_MultipleIds()
        {
            var filter = new IdFilter(new string[] { _dummyFixture.Id, _anotherFixture.Id });

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter.Pass(_anotherFixture));
            Assert.That(filter.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatch_MultipleIds()
        {
            var filter = new IdFilter(new string[] { _dummyFixture.Id, _anotherFixture.Id });

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.True(filter.IsExplicitMatch(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));
        }

        [Test]
        public void AddIds()
        {
            var filter = new IdFilter();
            filter.Add(_dummyFixture.Id);
            filter.Add(_anotherFixture.Id);

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildFromXml_SingleId()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0}</id></filter>", _dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFromXml_MultipleIds()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0},{1}</id></filter>", _dummyFixture.Id, _anotherFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }
    }
}
