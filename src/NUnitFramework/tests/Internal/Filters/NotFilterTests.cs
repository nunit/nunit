// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class NotFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.True(filter.Match(_topLevelSuite));
            Assert.False(filter.Match(_dummyFixture));
            Assert.True(filter.Match(_dummyFixture.Tests[0]));

            Assert.True(filter.Match(_anotherFixture));

            Assert.True (filter.Match (_fixtureWithMultipleTests));
            Assert.True (filter.Match (_fixtureWithMultipleTests.Tests[0]));
            Assert.False (filter.Match (_fixtureWithMultipleTests.Tests[1]));
        }

        [Test]
        public void PassTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.True(filter.Pass(_topLevelSuite));
            Assert.False(filter.Pass(_dummyFixture));
            Assert.False(filter.Pass(_dummyFixture.Tests[0]));

            Assert.True(filter.Pass(_anotherFixture));
            Assert.True(filter.Pass(_anotherFixture.Tests[0]));

            Assert.True (filter.Pass (_fixtureWithMultipleTests));
            Assert.True (filter.Pass (_fixtureWithMultipleTests.Tests[0]));
            Assert.False (filter.Pass (_fixtureWithMultipleTests.Tests[1]));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False (filter.IsExplicitMatch (_topLevelSuite));
            Assert.False (filter.IsExplicitMatch (_dummyFixture));
            Assert.False (filter.IsExplicitMatch (_dummyFixture.Tests[0]));

            Assert.False (filter.IsExplicitMatch (_anotherFixture));
            Assert.False (filter.IsExplicitMatch (_anotherFixture.Tests[0]));
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><not><cat>Dummy</cat></not></filter>");

            Assert.That(filter, Is.TypeOf<NotFilter>());
            Assert.That(filter, Has.Property("TopLevel").EqualTo(true));
            Assert.False(filter.Match(_dummyFixture));
            Assert.True(filter.Match(_anotherFixture));
        }
    }
}
