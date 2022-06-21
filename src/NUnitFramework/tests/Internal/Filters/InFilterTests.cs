// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX, true), new FullNameFilter(ANOTHER_CLASS_REGEX, true));

            Assert.False(InFilter.TryOptimize(filter, out _));
        }

        [Test]
        public void OptimizeSomeRegex()
        {
            var filter = new OrFilter(new FullNameFilter(DUMMY_CLASS_REGEX, true), new FullNameFilter("Dummy", false));

            Assert.False(InFilter.TryOptimize(filter, out _));
        }
    }
}
