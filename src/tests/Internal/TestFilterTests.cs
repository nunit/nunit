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
    // Filter XML formats
    //
    // Empty Filter:
    //    <filter/>
    //
    // Id Filter:
    //    <id>1</id>
    //    <id>1,2,3</id>
    // 
    // TestName filter
    //    <tests><test>xxxxxxx.xxx</test><test>yyyyyyy.yyy</test></tests>
    //
    // Name filter
    //    <name>xxxxx</name>
    //
    // Category filter 
    //    <cat>cat1</cat>
    //    <cat>cat1,cat2,cat3</cat>
    //
    // Property filter
    //    <prop>name=value</prop>
    //
    // And Filter
    //    <and><filter>...</filter><filter>...</filter></and>
    //    <filter><filter>...</filter><filter>...</filter></filter>
    //
    // Or Filter
    //    <or><filter>...</filter><filter>...</filter></or>

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

#if !NUNITLITE
        [Test]
        public void EmptyFilter_BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml("<filter/>");

            Assert.That(filter.IsEmpty);
        }
#endif
        #endregion

        #region IdFilter

        [Test]
        public void IdFilter_ConstructWithSingleId()
        {
            var filter = new IdFilter(dummyFixture.Id);

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void IdFilter_ConstructWithMultipleIds()
        {
            var filter = new IdFilter(new int[] { dummyFixture.Id, anotherFixture.Id });

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }

        [Test]
        public void IdFilter_AddIds()
        {
            var filter = new IdFilter();
            filter.Add(dummyFixture.Id);
            filter.Add(anotherFixture.Id);

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }

#if !NUNITLITE
        [Test]
        public void IdFilter_BuildFromXml_SingleId()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0}</id></filter>", dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void IdFilter_BuildFromXml_MultipleIds()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0},{1}</id></filter>", dummyFixture.Id, anotherFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }
#endif

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
        public void SimpleNameFilter_BuildFromXml_SingleTest()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><tests><test>NUnit.Framework.Internal.TestFilterTests+DummyFixture</test></tests></filter>");

            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void SimpleNameFilter_BuildFromXml_MultipleTests()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><tests><test>NUnit.Framework.Internal.TestFilterTests+DummyFixture</test><test>NUnit.Framework.Internal.TestFilterTests+AnotherFixture</test></tests></filter>");

            Assert.That(filter, Is.TypeOf<SimpleNameFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
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

#if !NUNITLITE
        [Test]
        public void CategoryFilter_BuildFromXml_SingleCategory()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>Dummy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void CategoryFilter_BuildFromXml_MultipleCategories()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>A,B,C,Dummy,Another,X,Y,Z</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
            Assert.False(filter.Match(yetAnotherFixture));
        }
#endif

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

        #endregion

        #region NotFilter

        [Test]
        public void NotFilter_Constructor()
        {
            var filter = new NotFilter(new CategoryFilter("Dummy"));

            Assert.False(filter.IsEmpty);
            Assert.False(filter.Match(dummyFixture));
            Assert.True(filter.Match(anotherFixture));
        }

#if !NUNITLITE
        [Test]
        public void NotFilter_BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><not><cat>Dummy</cat></not></filter>");

            Assert.That(filter, Is.TypeOf<NotFilter>());
            Assert.False(filter.Match(dummyFixture));
            Assert.True(filter.Match(anotherFixture));
        }
#endif
        
        #endregion

        #region OrFilter

        [Test]
        public void OrFilter_Constructor()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
        }

#if !NUNITLITE
        [Test]
        public void OrFilter_BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><or><cat>Dummy</cat><cat>Another</cat></or></filter>");

            Assert.That(filter, Is.TypeOf<OrFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.That(filter.Match(anotherFixture));
        }
#endif

        #endregion

        #region AndFilter

        [Test]
        public void AndFilter_Constructor()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(dummyFixture.Id));

            Assert.False(filter.IsEmpty);
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

#if !NUNITLITE
        [Test]
        public void AndFilter_BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><and><cat>Dummy</cat><id>{0}</id></and></filter>", dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }

        [Test]
        public void AndFilter_BuildFromXml_TopLevelDefaultsToAnd()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><cat>Dummy</cat><id>{0}</id></filter>", dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(dummyFixture));
            Assert.False(filter.Match(anotherFixture));
        }
#endif

        #endregion
    }
}
