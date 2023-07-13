// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.CategoryAttributeData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    /// <summary>
    /// Summary description for CategoryAttributeTests.
    /// </summary>
    [TestFixture]
    public class CategoryAttributeTests
    {
        private TestSuite _fixture;

        [SetUp]
        public void CreateFixture()
        {
            _fixture = TestBuilder.MakeFixture( typeof( FixtureWithCategories ) );
        }

        [Test]
        public void CategoryOnFixture()
        {
            Assert.That(_fixture.Properties["Category"], Contains.Item("DataBase"));
            //Assert.That( fixture.Properties.Contains("Category", "DataBase"));
        }

        [Test]
        public void CategoryOnTestMethod()
        {
            Test test1 = (Test)_fixture.Tests[0];
            Assert.That(test1.Properties["Category"], Contains.Item("Long"));
            //Assert.That( test1.Properties.Contains("Category", "Long") );
        }

        [Test]
        public void CanDeriveFromCategoryAttribute()
        {
            Test test2 = (Test)_fixture.Tests[1];
            Assert.That(test2.Properties["Category"], Contains.Item("Critical") );
        }

        [Test]
        public void DerivedCategoryMayBeInherited()
        {
            Assert.That(_fixture.Properties["Category"], Contains.Item("MyCategory"));
            //Assert.That(fixture.Properties.Contains("Category", "MyCategory"));
        }

        [Test]
        public void CanSpecifyOnMethodAndTestCase()
        {
            TestSuite test3 = (TestSuite)_fixture.Tests[2];
            Assert.That(test3.Name, Is.EqualTo("Test3"));
            Assert.That(test3.Properties["Category"], Contains.Item("Top"));
            Test testCase = (Test)test3.Tests[0];
            Assert.That(testCase.Name, Is.EqualTo("Test3(5)"));
            Assert.That(testCase.Properties["Category"], Contains.Item("Bottom"));
        }

        [Test]
        public void TestWithValidCategoryNameIsNotRunnable()
        {
            Test testValidSpecialChars = (Test)_fixture.Tests[3];
            Assert.That(testValidSpecialChars.RunState, Is.EqualTo(RunState.Runnable));
        }
    }
}
