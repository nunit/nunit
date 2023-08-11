// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    public class MultipleFilterTests : TestFilterTests
    {
        [Test]
        public void TestNestedAndFilters()
        {
            var filter = new AndFilter(
                new CategoryFilter("Dummy"),
                new PropertyFilter("Priority", "High"));

            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.IsExplicitMatch(DummyFixtureSuite));

            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(YetAnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(ExplicitFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(ExplicitFixtureSuite), Is.False);
        }

        [Test]
        public void TestNestedNotCategoryFilters()
        {
            var filter = new NotFilter(new CategoryFilter("NotDummy"));

            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.IsExplicitMatch(DummyFixtureSuite), Is.False);

            Assert.That(filter.Match(AnotherFixtureSuite));
            Assert.That(filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(YetAnotherFixtureSuite));
            Assert.That(filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(ExplicitFixtureSuite));
            Assert.That(filter.IsExplicitMatch(ExplicitFixtureSuite), Is.False);
        }

        [Test]
        public void TestNestedAndOrFilters()
        {
            var filter = new AndFilter(
                new NotFilter(new CategoryFilter("NotDummy")),
                new OrFilter(
                    new PropertyFilter("Priority", "High"),
                    new PropertyFilter("Priority", "Low")));

            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.IsExplicitMatch(DummyFixtureSuite), Is.False);

            Assert.That(filter.Match(AnotherFixtureSuite));
            Assert.That(filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(YetAnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(ExplicitFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(ExplicitFixtureSuite), Is.False);
        }

        [Test]
        public void TestNestedOrNotFilters()
        {
            var filter = new OrFilter(
                new CategoryFilter("Dummy"),
                new NotFilter(new CategoryFilter("Dummy")));

            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.IsExplicitMatch(DummyFixtureSuite));

            Assert.That(filter.Match(AnotherFixtureSuite));
            Assert.That(filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(YetAnotherFixtureSuite));
            Assert.That(filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(ExplicitFixtureSuite));
            Assert.That(filter.IsExplicitMatch(ExplicitFixtureSuite), Is.False);
        }

        [Test]
        public void TestLotsOfNestedNotFilters()
        {
            var filter = new NotFilter(
                new NotFilter(
                    new NotFilter(
                        new NotFilter(new CategoryFilter("Dummy")))));

            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.IsExplicitMatch(DummyFixtureSuite), Is.False);

            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(AnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(YetAnotherFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(YetAnotherFixtureSuite), Is.False);

            Assert.That(filter.Match(ExplicitFixtureSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(ExplicitFixtureSuite), Is.False);
        }

        [Test]
        public void NotOrNotFilterTests()
        {
            var filter = new NotFilter(
                new OrFilter(
                    new NotFilter(
                        new CategoryFilter("Dummy")),
                    new MethodNameFilter("Test1")));

            Assert.That(filter.Pass(TopLevelSuite));
            Assert.That(filter.Match(TopLevelSuite), Is.False);

            Assert.That(filter.Pass(FixtureWithMultipleTestsSuite));
            Assert.That(filter.Match(FixtureWithMultipleTestsSuite), Is.False);

            var test1 = FixtureWithMultipleTestsSuite.Tests[0];
            Assert.That(filter.Pass(test1), Is.False);
            Assert.That(filter.Match(test1), Is.False);

            var test2 = FixtureWithMultipleTestsSuite.Tests[1];
            Assert.That(filter.Pass(test2));
            Assert.That(filter.IsExplicitMatch(test2), Is.False);
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

            Assert.That(filter.Pass(TopLevelSuite));
            Assert.That(filter.Match(TopLevelSuite));

            Assert.That(filter.Pass(FixtureWithMultipleTestsSuite));
            Assert.That(filter.Match(FixtureWithMultipleTestsSuite));

            var test1 = FixtureWithMultipleTestsSuite.Tests[0];
            Assert.That(filter.Pass(test1), Is.False);
            Assert.That(filter.Match(test1), Is.False);

            var test2 = FixtureWithMultipleTestsSuite.Tests[1];
            Assert.That(filter.Pass(test2));
            Assert.That(filter.IsExplicitMatch(test2), Is.False);
            Assert.That(filter.Match(test2));
        }
    }
}
