// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnitLite.Runner.Tests
{
    public class CreateTestFilterTests
    {
        [Test]
        public void EmptyFilter()
        {
            var filter = GetFilter();
            Assert.That(filter.IsEmpty);
        }

        [Test]
        public void OneTestSelected()
        {
            var filter = GetFilter("--test:My.Test.Name");
            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(((SimpleNameFilter)filter).Names,
                Is.EqualTo(new string[] { "My.Test.Name" }));
        }

        [Test]
        public void ThreeTestsSelected()
        {
            var filter = GetFilter("--test:My.First.Test", "--test:My.Second.Test", "--test:My.Third.Test");
            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(((SimpleNameFilter)filter).Names, 
                Is.EqualTo(new string[] { "My.First.Test", "My.Second.Test", "My.Third.Test" } ));
        }

        [Test]
        public void OneCategoryIncluded()
        {
            var filter = GetFilter("--include:Dummy");
            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)filter).Categories,
                Is.EqualTo(new string[] { "Dummy" }));
        }

        [Test]
        public void ThreeCategoriesIncluded()
        {
            var filter = GetFilter("--include:Dummy,Another,StillAnother");
            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)filter).Categories,
                Is.EqualTo(new string[] { "Dummy", "Another", "StillAnother" }));
        }

        [Test]
        public void OneCategoryExcluded()
        {
            var filter = GetFilter("--exclude:Dummy");
            Assert.That(filter, Is.TypeOf<NotFilter>());
            var baseFilter = ((NotFilter)filter).BaseFilter;
            Assert.That(baseFilter, Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)baseFilter).Categories,
                Is.EqualTo(new string[] { "Dummy" }));
        }

        [Test]
        public void ThreeCategoriesExcluded()
        {
            var filter = GetFilter("--exclude:Dummy,Another,StillAnother");
            Assert.That(filter, Is.TypeOf<NotFilter>());
            var baseFilter = ((NotFilter)filter).BaseFilter;
            Assert.That(baseFilter, Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)baseFilter).Categories,
                Is.EqualTo(new string[] { "Dummy", "Another", "StillAnother" }));
        }

        [Test]
        public void OneTestAndOneCategory()
        {
            var filter = GetFilter("--test:My.Test.Name", "--include:Dummy");
            Assert.That(filter, Is.TypeOf<AndFilter>());
            var filters = ((AndFilter)filter).Filters;
            Assert.That(filters.Length, Is.EqualTo(2));

            Assert.That(filters[0], Is.TypeOf<SimpleNameFilter>());
            Assert.That(((SimpleNameFilter)filters[0]).Names,
                Is.EqualTo(new string[] { "My.Test.Name" }));

            Assert.That(filters[1], Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)filters[1]).Categories,
                Is.EqualTo(new string[] { "Dummy" }));
        }

        [Test]
        public void TwoCategoriesIncludedAndOneExcluded()
        {
            var filter = GetFilter("--include:Dummy,Another", "--exclude:Slow");
            Assert.That(filter, Is.TypeOf<AndFilter>());
            var filters = ((AndFilter)filter).Filters;
            Assert.That(filters.Length, Is.EqualTo(2));

            Assert.That(filters[0], Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)filters[0]).Categories,
                Is.EqualTo(new string[] { "Dummy", "Another" }));

            Assert.That(filters[1], Is.TypeOf<NotFilter>());
            var baseFilter = ((NotFilter)filters[1]).BaseFilter;
            Assert.That(baseFilter, Is.TypeOf<CategoryFilter>());
            Assert.That(((CategoryFilter)baseFilter).Categories,
                Is.EqualTo(new string[] { "Slow" }));
        }

        private TestFilter GetFilter(params string[] args)
        {
            return TextUI.CreateTestFilter(new CommandLineOptions(args));
        }
    }
}
