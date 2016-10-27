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
        #region ClassNameFilter

        [Test]
        public void ClassNameFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><class>" + TestFilterTests.DUMMY_CLASS + "</class></filter>");

            Assert.That(filter, Is.TypeOf<ClassNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void ClassNameFilter_ToXml()
        {
            TestFilter filter = new ClassNameFilter("FULLNAME");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<class>FULLNAME</class>"));
        }

        [Test]
        public void ClassNameFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><class re='1'>Dummy</class></filter>");

            Assert.That(filter, Is.TypeOf<ClassNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void ClassNameFilter_ToXml_Regex()
        {
            TestFilter filter = new ClassNameFilter("FULLNAME") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<class re=\"1\">FULLNAME</class>"));
        }

        #endregion

        #region MethodNameFilter

        [Test]
        public void MethodNameFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><method>Test</method></filter>");

            Assert.That(filter, Is.TypeOf<MethodNameFilter>());
            Assert.That(filter.Match(_dummyFixture.Tests[0]));
            Assert.That(filter.Match(_anotherFixture.Tests[0]));
            Assert.False(filter.Match(_fixtureWithMultipleTests.Tests[0]));
        }

        [Test]
        public void MethodNameFilter_ToXml()
        {
            TestFilter filter = new MethodNameFilter("Test");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<method>Test</method>"));
        }

        [Test]
        public void BuildMethodNameFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><method re='1'>T.st</method></filter>");

            Assert.That(filter, Is.TypeOf<MethodNameFilter>());
            Assert.That(filter.Match(_dummyFixture.Tests[0]));
            Assert.That(filter.Match(_anotherFixture.Tests[0]));
            Assert.That(filter.Match(_fixtureWithMultipleTests.Tests[0]));
        }

        [Test]
        public void MethodNameFilter_ToXml_Regex()
        {
            TestFilter filter = new MethodNameFilter("Test") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<method re=\"1\">Test</method>"));
        }

        #endregion

        #region TestNameFilter

        [Test]
        public void TestNameFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><name>TestFilterTests+DummyFixture</name></filter>");

            Assert.That(filter, Is.TypeOf<TestNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void TestNameFilter_ToXml()
        {
            TestFilter filter = new TestNameFilter("TestFilterTests+DummyFixture");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<name>TestFilterTests+DummyFixture</name>"));
        }

        [Test]
        public void TestNameFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><name re='1'>Dummy</name></filter>");

            Assert.That(filter, Is.TypeOf<TestNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void TestNameFilter_ToXml_Regex()
        {
            TestFilter filter = new TestNameFilter("TestFilterTests+DummyFixture") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<name re=\"1\">TestFilterTests+DummyFixture</name>"));
        }

        #endregion

        #region FullNameFilter

        [Test]
        public void FullNameFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><test>" + TestFilterTests.DUMMY_CLASS + "</test></filter>");

            Assert.That(filter, Is.TypeOf<FullNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void FullNameFilter_ToXml()
        {
            TestFilter filter = new FullNameFilter("FULLNAME");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<test>FULLNAME</test>"));
        }

        [Test]
        public void FullNameFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><test re='1'>Dummy</test></filter>");

            Assert.That(filter, Is.TypeOf<FullNameFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void FullNameFilter_ToXml_Regex()
        {
            TestFilter filter = new FullNameFilter("FULLNAME") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<test re=\"1\">FULLNAME</test>"));
        }

        #endregion

        #region CategoryFilter

        [Test]
        public void CategoryFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>Dummy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void CategoryFilter_ToXml()
        {
            TestFilter filter = new CategoryFilter("CATEGORY");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat>CATEGORY</cat>"));
        }

        [Test]
        public void CategoryFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat re='1'>D.mmy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void CategoryFilter_ToXml_Regex()
        {
            TestFilter filter = new CategoryFilter("CATEGORY") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat re=\"1\">CATEGORY</cat>"));
        }

        #endregion

        #region PropertyFilter

        [Test]
        public void PropertyFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><prop name='Priority'>High</prop></filter>");

            Assert.That(filter, Is.TypeOf<PropertyFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void PropertyFilter_ToXml()
        {
            TestFilter filter = new PropertyFilter("Priority", "High");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<prop name=\"Priority\">High</prop>"));
        }

        [Test]
        public void PropertyFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><prop name='Author' re='1'>Charlie P</prop></filter>");

            Assert.That(filter, Is.TypeOf<PropertyFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void PropertyFilter_ToXml_Regex()
        {
            TestFilter filter = new PropertyFilter("Priority", "High") { IsRegex = true };
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<prop re=\"1\" name=\"Priority\">High</prop>"));
        }

        #endregion

        #region IdFilter

        [Test]
        public void IdFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                string.Format("<filter><id>{0}</id></filter>", _dummyFixture.Id));

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.False(filter.Match(_anotherFixture));
        }

        [Test]
        public void IdFilter_ToXml()
        {
            TestFilter filter = new IdFilter("ID-123");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<id>ID-123</id>"));
        }

        #endregion
    }
}
