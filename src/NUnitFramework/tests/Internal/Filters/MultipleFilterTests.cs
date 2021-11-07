// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework.Internal.Filters
{
    public class MultipleFilterTests : TestFilterTests
    {
        [Test]
        public void TestNestedAndFilters()
        {
            var filter = new AndFilter(
                new CategoryFilter("Dummy"),
                new PropertyFilter("Priority", "High"));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));

            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));

            Assert.False(filter.Match(_explicitFixture));
            Assert.False(filter.IsExplicitMatch(_explicitFixture));
        }

        [Test]
        public void TestNestedNotCategoryFilters()
        {
            var filter = new NotFilter(new CategoryFilter("NotDummy"));

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture));

            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture));

            Assert.That(filter.Match(_yetAnotherFixture));
            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));

            Assert.That(filter.Match(_explicitFixture));
            Assert.False(filter.IsExplicitMatch(_explicitFixture));
        }

        [Test]
        public void TestNestedAndOrFilters()
        {
            var filter = new AndFilter(
                new NotFilter(new CategoryFilter("NotDummy")),
                new OrFilter(
                    new PropertyFilter("Priority", "High"),
                    new PropertyFilter("Priority", "Low")));

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture));

            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));

            Assert.False(filter.Match(_explicitFixture));
            Assert.False(filter.IsExplicitMatch(_explicitFixture));
        }

        [Test]
        public void TestNestedOrNotFilters()
        {
            var filter = new OrFilter(
                new CategoryFilter("Dummy"),
                new NotFilter(new CategoryFilter("Dummy")));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));

            Assert.That(filter.Match(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture));

            Assert.That(filter.Match(_yetAnotherFixture));
            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));

            Assert.That(filter.Match(_explicitFixture));
            Assert.False(filter.IsExplicitMatch(_explicitFixture));
        }

        [Test]
        public void TestLotsOfNestedNotFilters()
        {
            var filter = new NotFilter(
                new NotFilter(
                    new NotFilter(
                        new NotFilter(new CategoryFilter("Dummy")))));

            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture));

            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));

            Assert.False(filter.Match(_explicitFixture));
            Assert.False(filter.IsExplicitMatch(_explicitFixture));
        }

        [Test]
        public void NotOrNotFilterTests()
        {
            var filter = new NotFilter(
                new OrFilter(
                    new NotFilter(
                        new CategoryFilter("Dummy")),
                    new MethodNameFilter("Test1")));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.False(filter.Match(_topLevelSuite));

            Assert.That(filter.Pass(_fixtureWithMultipleTests));
            Assert.False(filter.Match(_fixtureWithMultipleTests));

            var test1 = _fixtureWithMultipleTests.Tests[0];
            Assert.False(filter.Pass(test1));
            Assert.False(filter.Match(test1));

            var test2 = _fixtureWithMultipleTests.Tests[1];
            Assert.That(filter.Pass(test2));
            Assert.False(filter.IsExplicitMatch(test2));
            Assert.That(filter.Match(test2));
        }

        [Test]
        public void NotAndNotFilterTests()
        {
            var filter = new NotFilter(
                new AndFilter(
                    new NotFilter(
                        new CategoryFilter("Dummy")),
                    new MethodNameFilter("Test1")));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Match(_topLevelSuite));

            Assert.That(filter.Pass(_fixtureWithMultipleTests));
            Assert.That(filter.Match(_fixtureWithMultipleTests));

            var test1 = _fixtureWithMultipleTests.Tests[0];
            Assert.False(filter.Pass(test1));
            Assert.False(filter.Match(test1));

            var test2 = _fixtureWithMultipleTests.Tests[1];
            Assert.That(filter.Pass(test2));
            Assert.False(filter.IsExplicitMatch(test2));
            Assert.That(filter.Match(test2));
        }
    }
}
