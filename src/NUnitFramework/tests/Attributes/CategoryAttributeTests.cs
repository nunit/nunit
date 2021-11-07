// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.CategoryAttributeData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Summary description for CategoryAttributeTests.
    /// </summary>
    [TestFixture]
    public class CategoryAttributeTests
    {
        TestSuite fixture;

        [SetUp]
        public void CreateFixture()
        {
            fixture = TestBuilder.MakeFixture( typeof( FixtureWithCategories ) );
        }

        [Test]
        public void CategoryOnFixture()
        {
            Assert.That(fixture.Properties["Category"], Contains.Item("DataBase"));
            //Assert.That( fixture.Properties.Contains("Category", "DataBase"));
        }

        [Test]
        public void CategoryOnTestMethod()
        {
            Test test1 = (Test)fixture.Tests[0];
            Assert.That(test1.Properties["Category"], Contains.Item("Long"));
            //Assert.That( test1.Properties.Contains("Category", "Long") );
        }

        [Test]
        public void CanDeriveFromCategoryAttribute()
        {
            Test test2 = (Test)fixture.Tests[1];
            Assert.That(test2.Properties["Category"], Contains.Item("Critical") );
        }
        
        [Test]
        public void DerivedCategoryMayBeInherited()
        {
            Assert.That(fixture.Properties["Category"], Contains.Item("MyCategory"));
            //Assert.That(fixture.Properties.Contains("Category", "MyCategory"));
        }

        [Test]
        public void CanSpecifyOnMethodAndTestCase()
        {
            TestSuite test3 = (TestSuite)fixture.Tests[2];
            Assert.That(test3.Name, Is.EqualTo("Test3"));
            Assert.That(test3.Properties["Category"], Contains.Item("Top"));
            Test testCase = (Test)test3.Tests[0];
            Assert.That(testCase.Name, Is.EqualTo("Test3(5)"));
            Assert.That(testCase.Properties["Category"], Contains.Item("Bottom"));
        }

        [Test]
        public void TestWithValidCategoryNameIsNotRunnable()
        {
            Test testValidSpecialChars = (Test)fixture.Tests[3];
            Assert.That(testValidSpecialChars.RunState, Is.EqualTo(RunState.Runnable));
        }
    }
}
