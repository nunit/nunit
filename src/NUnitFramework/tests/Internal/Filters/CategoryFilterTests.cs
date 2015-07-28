using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class CategoryFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new CategoryFilter("Dummy");

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void Match_SingleCategory()
        {
            var filter = new CategoryFilter("Dummy");

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void Pass_SingleCategory()
        {
            var filter = new CategoryFilter("Dummy");

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(filter.Pass(_anotherFixture));
        }

        [Test]
        public void ExplicitMatch_SingleCategory()
        {
            var filter = new CategoryFilter("Dummy");

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_anotherFixture));
        }

        [Test]
        public void Match_MultipleCategories()
        {
            var filter = new CategoryFilter(new string[] { "Dummy", "Another" });

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void Pass_MultipleCategories()
        {
            var filter = new CategoryFilter(new string[] { "Dummy", "Another" });

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter.Pass(_anotherFixture));
            Assert.That(filter.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatch_MultipleCategories()
        {
            var filter = new CategoryFilter(new string[] { "Dummy", "Another" });

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.That(filter.IsExplicitMatch(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));
        }

        [Test]
        public void AddCategories()
        {
            var filter = new CategoryFilter();
            filter.AddCategory("Dummy");
            filter.AddCategory("Another");

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildFromXml_SingleCategory()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>Dummy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFromXml_MultipleCategories()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>A,B,C,Dummy,Another,X,Y,Z</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }
    }
}
