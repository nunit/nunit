// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using NUnit.Framework.Internal.Filters;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    public class TestFilterTests
    {
        private static readonly TestSuite dummyFixture = TestBuilder.MakeFixture(typeof(DummyFixture));
        private static readonly TestSuite anotherFixture = TestBuilder.MakeFixture(typeof(AnotherFixture));
        private static readonly TestSuite yetAnotherFixture = TestBuilder.MakeFixture(typeof(YetAnotherFixture));

        private static readonly string dummyName = dummyFixture.FullName;
        private static readonly string anotherName = anotherFixture.FullName;

        #region EmptyFilter
        [Test]
        public void EmptyFilterIsEmpty()
        {
            Assert.That(TestFilter.Empty.IsEmpty);
        }
        #endregion

        #region SimpleNameFilter
        [Test]
        public void SimpleNameFilter_SingleNameConstructor()
        {
            var filter = new SimpleNameFilter(dummyName);

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void SimpleNameFilter_MultipleNameConstructor()
        {
            var filter = new SimpleNameFilter(new string[] { dummyName, anotherName });

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }

        [Test]
        public void SimpleNameFilter_AddNames()
        {
            var filter = new SimpleNameFilter();
            filter.Add(dummyName);
            filter.Add(anotherName);

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }

#if !NUNITLITE
        [Test]
        public void SimpleNameFilter_BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><tests><test>NUnit.Framework.Internal.TestFilterTests+DummyFixture</test></tests></filter>");

            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(filter.Match(dummyFixture));
        }
#endif
        #endregion

        #region CategoryFilter
        [Test]
        public void CategoryFilter_SingleCategoryConstructor()
        {
            var filter = new CategoryFilter("Dummy");

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void SimpleCategoryFilter_MultipleCategoryConstructor()
        {
            var filter = new CategoryFilter(new string[] { "Dummy", "Another" });

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }

        [Test]
        public void CategoryFilter_AddCategories()
        {
            var filter = new CategoryFilter();
            filter.AddCategory("Dummy");
            filter.AddCategory("Another");

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }
        #endregion

        [Category("Dummy")]
        private class DummyFixture
        {
        }

        [Category("Another")]
        private class AnotherFixture
        {
        }

        private class YetAnotherFixture
        {
        }
    }
}
