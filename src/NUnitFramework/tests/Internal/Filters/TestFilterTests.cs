// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal.Filters
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
    //    <test>xxxxxxx.xxx</test>
    //
    // Name filter
    //    <name>xxxxx</name>
    //
    // Namespace filter
    //    <namespace>xxxxx</namespace>
    //
    // Category filter
    //    <cat>cat1</cat>
    //    <cat>cat1,cat2,cat3</cat>
    //
    // Property filter
    //    <prop name="xxxx">value</prop>
    //
    // And Filter
    //    <and><filter>...</filter><filter>...</filter></and>
    //    <filter><filter>...</filter><filter>...</filter></filter>
    //
    // Or Filter
    //    <or><filter>...</filter><filter>...</filter></or>

    public abstract class TestFilterTests
    {
        public const string DUMMY_CLASS = "NUnit.Framework.Tests.Internal.Filters.TestFilterTests+DummyFixture";
        public const string ANOTHER_CLASS = "NUnit.Framework.Tests.Internal.Filters.TestFilterTests+AnotherFixture";
        public const string DUMMY_CLASS_REGEX = "NUnit.*\\+DummyFixture";
        public const string ANOTHER_CLASS_REGEX = "NUnit.*\\+AnotherFixture";

        protected readonly TestSuite DummyFixtureSuite = TestBuilder.MakeFixture(typeof(DummyFixture));
        protected readonly TestSuite AnotherFixtureSuite = TestBuilder.MakeFixture(typeof(AnotherFixture));
        protected readonly TestSuite YetAnotherFixtureSuite = TestBuilder.MakeFixture(typeof(YetAnotherFixture));
        protected readonly TestSuite FixtureWithMultipleTestsSuite = TestBuilder.MakeFixture(typeof(FixtureWithMultipleTests));
        protected readonly TestSuite NestingFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture));
        protected readonly TestSuite NestedFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture.NestedFixture));
        protected readonly TestSuite EmptyNestedFixtureSuite = TestBuilder.MakeFixture(typeof(NestingFixture.EmptyNestedFixture));
        protected readonly TestSuite TopLevelSuite = new TestSuite("MySuite");
        protected readonly TestSuite ExplicitFixtureSuite = TestBuilder.MakeFixture(typeof(ExplicitFixture));
        protected readonly TestSuite SpecialFixtureSuite = TestBuilder.MakeFixture(typeof(SpecialCharactersFixture));

        [OneTimeSetUp]
        public void SetUpSuite()
        {
            TopLevelSuite.Add(DummyFixtureSuite);
            TopLevelSuite.Add(AnotherFixtureSuite);
            TopLevelSuite.Add(YetAnotherFixtureSuite);
            TopLevelSuite.Add(FixtureWithMultipleTestsSuite);
            TopLevelSuite.Add(NestingFixtureSuite);

            NestingFixtureSuite.Add(NestedFixtureSuite);
            NestingFixtureSuite.Add(EmptyNestedFixtureSuite);
        }

        #region Fixtures Used by Tests

        [Category("Dummy"), Property("Priority", "High"), Author("Charlie Poole")]
        private class DummyFixture
        {
            [Test]
            public void Test() { }
        }

        [Category("Special,Character-Fixture+!")]
        private class SpecialCharactersFixture
        {
            [Test]
            public void Test() { }
        }

        [Category("Another"), Property("Priority", "Low"), Author("Fred Smith")]
        private class AnotherFixture
        {
            [Test]
            public void Test() { }
        }

        private class YetAnotherFixture
        { }

        private class FixtureWithMultipleTests
        {
            [Test]
            public void Test1() { }

            [Test, Category("Dummy")]
            public void Test2() { }
        }

        private class NestingFixture
        {
            public class NestedFixture
            {
                [Test]
                public void Test() { }
            }

            internal class EmptyNestedFixture { }
        }

        [Explicit]
        private class ExplicitFixture
        { }
        #endregion
    }
}
