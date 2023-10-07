// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    [TestFixture("NUnit.Framework.Tests.Internal.Filters", false, true)]
    [TestFixture("NUnit.Framework.*", true, true)]
    [TestFixture("NUnit.Framework", false, false)]
    public class NamespaceFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;
        private readonly bool _expected;

        public NamespaceFilterTests(string value, bool isRegex, bool expected)
        {
            _filter = new NamespaceFilter(value, isRegex);
            _expected = expected;
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(DummyFixtureSuite.Tests[0]), Is.EqualTo(_expected));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(NestingFixtureSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(NestedFixtureSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(EmptyNestedFixtureSuite), Is.EqualTo(_expected));

            Assert.That(_filter.Pass(TopLevelSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(DummyFixtureSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(DummyFixtureSuite.Tests[0]), Is.EqualTo(_expected));
        }
    }
}
