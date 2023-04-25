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

            Assert.That(InFilter.TryOptimize(filter, out var optimized), Is.True);

            Assert.That(optimized, Is.Not.Null);
            Assert.That(optimized.IsEmpty, Is.False);

            Assert.That(optimized.Match(new TestDummy { Id = "Id-1" }), Is.True);
            Assert.That(optimized.Match(new TestDummy { Id = "Id-2" }), Is.True);

            Assert.That(optimized.Match(new TestDummy { Id = "id-1" }), Is.False);
            Assert.That(optimized.Match(new TestDummy { Id = "Id-" }), Is.False);
            Assert.That(optimized.Match(new TestDummy { Id = "" }), Is.False);
        }

        [Test]
        public void OptimizeMixed()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new FullNameFilter(ANOTHER_CLASS));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));

            Assert.That(InFilter.TryOptimize(filter, out _), Is.False);
        }

        [Test]
        public void OptimizeEmpty()
        {
            var filter = new OrFilter(new TestFilter[] {});

            Assert.That(InFilter.TryOptimize(filter, out _), Is.False);
        }

        [Test]
        public void OptimizeAllRegex()
        {
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX, true), new FullNameFilter(ANOTHER_CLASS_REGEX, true));

            Assert.That(InFilter.TryOptimize(filter, out _), Is.False);
        }

        [Test]
        public void OptimizeSomeRegex()
        {
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX, true), new FullNameFilter("Dummy", false));

            Assert.That(InFilter.TryOptimize(filter, out _), Is.False);
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
        
        [Test]
        public void WriteToXml()
        {
            var orFilters = new[]
            {
                new OrFilter(new ClassNameFilter("Id-1"), new ClassNameFilter("Id-2")),
                new OrFilter(new FullNameFilter("Id-1"), new FullNameFilter("Id-2")),
                new OrFilter(new IdFilter("Id-1"), new IdFilter("Id-2")),
                new OrFilter(new MethodNameFilter("Id-1"), new MethodNameFilter("Id-2")),
                new OrFilter(new NamespaceFilter("Id-1"), new NamespaceFilter("Id-2")),
                new OrFilter(new TestNameFilter("Id-1"), new TestNameFilter("Id-2")),
            };

            foreach (var orFilter in orFilters)
            {
                InFilter.TryOptimize(orFilter, out var inFilter);
                Assert.That(inFilter, Is.Not.Null);

                var orFilterXml = orFilter.ToXml(true).OuterXml;
                var inFilterXml = inFilter.ToXml(true).OuterXml;

                Assert.That(inFilterXml, Is.EqualTo(orFilterXml));
            }
        }
    }
}
