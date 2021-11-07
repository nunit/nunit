// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture("NUnit.Framework.Internal.Filters", false, true)]
    [TestFixture("NUnit.Framework.*", true, true)]
    [TestFixture("NUnit.Framework", false, false)]
    public class NamespaceFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;
        private readonly bool _expected;

        public NamespaceFilterTests(string value, bool isRegex, bool expected)
        {
            _filter = new NamespaceFilter(value) { IsRegex = isRegex };
            _expected = expected;
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(_filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(_dummyFixture.Tests[0]), Is.EqualTo(_expected));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(_nestingFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_nestedFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_emptyNestedFixture), Is.EqualTo(_expected));

            Assert.That(_filter.Pass(_topLevelSuite), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_dummyFixture), Is.EqualTo(_expected));
            Assert.That(_filter.Pass(_dummyFixture.Tests[0]), Is.EqualTo(_expected));
        }

    }
}
