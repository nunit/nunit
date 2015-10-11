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
using NUnit.Framework;

namespace NUnit.Framework.Internal.Filters
{
    public class TestFilterXmlTests : TestFilterTests
    {
        [Test]
        public void BuildClassNameFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><class>" + TestFilterTests.DUMMY_CLASS + "</class></filter>");

            Assert.That(filter, Is.TypeOf<ClassNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildClassNameFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><class re='1'>Dummy</class></filter>");

            Assert.That(filter, Is.TypeOf<ClassNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildMethodNameFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><method>Test</method></filter>");

            Assert.That(filter, Is.TypeOf<MethodNameFilter>());
            Assert.That(filter.Match(_dummyFixture.Tests[0]));
            Assert.That(filter.Match(_anotherFixture.Tests[0]));
            Assert.False(filter.Match(_fixtureWithMultipleTests.Tests[0]));
        }

        [Test]
        public void BuildMethodNameFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><method re='1'>T.st</method></filter>");

            Assert.That(filter, Is.TypeOf<MethodNameFilter>());
            Assert.That(filter.Match(_dummyFixture.Tests[0]));
            Assert.That(filter.Match(_anotherFixture.Tests[0]));
            Assert.That(filter.Match(_fixtureWithMultipleTests.Tests[0]));
        }

        [Test]
        public void BuildTestNameFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><name>TestFilterTests+DummyFixture</name></filter>");

            Assert.That(filter, Is.TypeOf<TestNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildTestNameFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><name re='1'>Dummy</name></filter>");

            Assert.That(filter, Is.TypeOf<TestNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFullNameFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><test>" + TestFilterTests.DUMMY_CLASS + "</test></filter>");

            Assert.That(filter, Is.TypeOf<FullNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildFullNameFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><test re='1'>Dummy</test></filter>");

            Assert.That(filter, Is.TypeOf<FullNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildCategoryFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>Dummy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildCategoryFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat re='1'>D.mmy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void BuildPropertyFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><prop name='Priority'>High</prop></filter>");

            Assert.That(filter, Is.TypeOf<PropertyFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildPropertyFilter_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><prop name='Author' re='1'>Charlie P</prop></filter>");

            Assert.That(filter, Is.TypeOf<PropertyFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void BuildIdFilter()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0}</id></filter>", _dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }
    }
}
