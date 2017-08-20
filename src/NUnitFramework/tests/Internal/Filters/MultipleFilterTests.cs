// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
        public void TestLotsOfNestedOrFilters()
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
    }
}
