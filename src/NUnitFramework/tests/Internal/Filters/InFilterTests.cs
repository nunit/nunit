// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Attributes;

namespace NUnit.Framework.Internal.Filters
{
    public class InFilterTests : TestFilterTests
    {
        [Test]
        public void OptimizeSameKind()
        {
            var filter = new OrFilter(new IdFilter("Id-1"), new IdFilter("Id-2"));

            Assert.True(InFilter.TryOptimize(filter, out var optimized));

            Assert.NotNull(optimized);
            Assert.False(optimized.IsEmpty);

            Assert.True(optimized.Match(new TestDummy { Id = "Id-1" }));
            Assert.True(optimized.Match(new TestDummy { Id = "Id-2" }));

            Assert.False(optimized.Match(new TestDummy { Id = "id-1" }));
            Assert.False(optimized.Match(new TestDummy { Id = "Id-" }));
            Assert.False(optimized.Match(new TestDummy { Id = "" }));
        }

        [Test]
        public void OptimizeMixed()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new FullNameFilter(ANOTHER_CLASS));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));

            Assert.False(InFilter.TryOptimize(filter, out _));
        }

        [Test]
        public void OptimizeEmpty()
        {
            var filter = new OrFilter(new TestFilter[] {});

            Assert.False(InFilter.TryOptimize(filter, out _));
        }

        [Test]
        public void OptimizeAllRegex()
        {
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX) { IsRegex = true }, new FullNameFilter(ANOTHER_CLASS_REGEX) { IsRegex = true });

            Assert.False(InFilter.TryOptimize(filter, out _));
        }

        [Test]
        public void OptimizeSomeRegex()
        {
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX) { IsRegex = true }, new FullNameFilter("Dummy") { IsRegex = false });

            Assert.False(InFilter.TryOptimize(filter, out _));
        }
        
        [Test]
        public void BuildFromXmlFullName()
        {
            TestFilter filter = TestFilter.FromXml(
                $"<filter><or><test>{DUMMY_CLASS}</test><test>{ANOTHER_CLASS}</test></or></filter>");

            Assert.That(filter, Is.TypeOf<InFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
        }
    }
}
