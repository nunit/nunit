// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Filters
{
    [TestFixture(TestFilterTests.DUMMY_CLASS, false)]
    [TestFixture("Dummy", true)]
    public class FullNameFilterTests : TestFilterTests
    {
        private readonly TestFilter _filter;

        public FullNameFilterTests(string value, bool isRegex)
        {
            _filter = new FullNameFilter(value, isRegex);
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.False(_filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            Assert.That(_filter.Match(_dummyFixture));
            Assert.False(_filter.Match(_anotherFixture));
        }

        [Test]
        public void PassTest()
        {
            Assert.That(_filter.Pass(_topLevelSuite));
            Assert.That(_filter.Pass(_dummyFixture));
            Assert.That(_filter.Pass(_dummyFixture.Tests[0]));

            Assert.False(_filter.Pass(_anotherFixture));
        }

        public void ExplicitMatchTest()
        {
            Assert.That(_filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(_filter.IsExplicitMatch(_dummyFixture));
            Assert.False(_filter.IsExplicitMatch(_dummyFixture.Tests[0]));

            Assert.False(_filter.IsExplicitMatch(_anotherFixture));
        }
    }
}
