// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void ClassNameFilter_ToXml_Regex()
        {
            TestFilter filter = new ClassNameFilter("FULLNAME", isRegex: true);
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
            Assert.That(filter.Match(DummyFixtureSuite.Tests[0]));
            Assert.That(filter.Match(AnotherFixtureSuite.Tests[0]));
            Assert.That(filter.Match(FixtureWithMultipleTestsSuite.Tests[0]), Is.False);
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
            Assert.That(filter.Match(DummyFixtureSuite.Tests[0]));
            Assert.That(filter.Match(AnotherFixtureSuite.Tests[0]));
            Assert.That(filter.Match(FixtureWithMultipleTestsSuite.Tests[0]));
        }

        [Test]
        public void MethodNameFilter_ToXml_Regex()
        {
            TestFilter filter = new MethodNameFilter("Test", isRegex: true);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void TestNameFilter_ToXml_Regex()
        {
            TestFilter filter = new TestNameFilter("TestFilterTests+DummyFixture", isRegex: true);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void FullNameFilter_ToXml_Regex()
        {
            TestFilter filter = new FullNameFilter("FULLNAME", isRegex: true);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void CategoryFilterWithSpecialCharacters_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat>Special,Character-Fixture+!</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(SpecialFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void CategoryFilter_ToXml()
        {
            TestFilter filter = new CategoryFilter("CATEGORY");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat>CATEGORY</cat>"));
        }

        [Test]
        public void CategoryFilterWithSpecialCharacters_ToXml()
        {
            TestFilter filter = new CategoryFilter("Special,Character-Fixture+!");
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat>Special,Character-Fixture+!</cat>"));
        }

        [Test]
        public void CategoryFilter_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><cat re='1'>D.mmy</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void CategoryFilterWithSpecialCharacters_FromXml_Regex()
        {
            TestFilter filter = TestFilter.FromXml(
                @"<filter><cat re='1'>Special,Character-Fixture\+!</cat></filter>");

            Assert.That(filter, Is.TypeOf<CategoryFilter>());
            Assert.That(filter.Match(SpecialFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
        }

        [Test]
        public void CategoryFilter_ToXml_Regex()
        {
            TestFilter filter = new CategoryFilter("CATEGORY", isRegex: true);
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat re=\"1\">CATEGORY</cat>"));
        }

        [Test]
        public void CategoryFilterWithSpecialCharacters_ToXml_Regex()
        {
            TestFilter filter = new CategoryFilter("Special,Character-Fixture+!", isRegex: true);
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<cat re=\"1\">Special,Character-Fixture+!</cat>"));
        }

        #endregion

        #region PropertyFilter

        [Test]
        public void PropertyFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><prop name='Priority'>High</prop></filter>");

            Assert.That(filter, Is.TypeOf<PropertyFilter>());
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
            Assert.That(filter.Match(YetAnotherFixtureSuite), Is.False);
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
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
            Assert.That(filter.Match(YetAnotherFixtureSuite), Is.False);
        }

        [Test]
        public void PropertyFilter_ToXml_Regex()
        {
            TestFilter filter = new PropertyFilter("Priority", "High", isRegex: true);
            Assert.That(filter.ToXml(false).OuterXml, Is.EqualTo("<prop re=\"1\" name=\"Priority\">High</prop>"));
        }

        #endregion

        #region IdFilter

        [Test]
        public void IdFilter_FromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                $"<filter><id>{DummyFixtureSuite.Id}</id></filter>");

            Assert.That(filter, Is.TypeOf<IdFilter>());
            Assert.That(filter.Match(DummyFixtureSuite));
            Assert.That(filter.Match(AnotherFixtureSuite), Is.False);
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
