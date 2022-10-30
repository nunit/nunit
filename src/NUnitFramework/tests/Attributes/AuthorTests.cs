// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#region Using Directives

using NUnit.TestData;

#endregion

namespace NUnit.Framework.Attributes
{
    [TestFixture(Author = "Rob Prouse <rob@prouse.org>"), Author("Charlie Poole", "Charlie@poole.org")]
    [Author("NUnit")]
    [TestOf(typeof(AuthorAttribute))]
    public class AuthorTests
    {
        static readonly Type FixtureType = typeof(AuthorFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.Method));
            Assert.AreEqual(RunState.Runnable, testCase.RunState);
        }

        [Test]
        public void Author()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.Method));
            Assert.AreEqual("Rob Prouse", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void NoAuthor()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.NoAuthorMethod));
            Assert.IsNull(testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void FixtureAuthor()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));

            var mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.AreEqual("Rob Prouse", mockFixtureSuite.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void SeparateAuthorAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.SeparateAuthorMethod));
            Assert.AreEqual("Rob Prouse", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void SeparateAuthorWithEmailAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.SeparateAuthorWithEmailMethod));
            Assert.AreEqual("Rob Prouse <rob@prouse.org>", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void AuthorOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(AuthorFixture.TestCaseWithAuthor));
            Assert.AreEqual("Rob Prouse", parameterizedMethodSuite.Properties.Get(PropertyNames.Author));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("Charlie Poole", testCase.Properties.Get(PropertyNames.Author));
        }

        #region Multiple Authors
        [Test(Author = "Rob Prouse <rob@prouse.org>"),Author("Charlie Poole", "charlie@poole.org")]
        [Author("NUnit")]
        public void TestMethodMultipleAuthors()
        {
            Test test = TestBuilder.MakeTestFromMethod(FixtureType, nameof(AuthorFixture.TestMethodMultipleAuthors));
            Assert.That(test.Properties[PropertyNames.Author], Is.EquivalentTo(
                new[] { "Rob Prouse <rob@prouse.org>","Charlie Poole <charlie@poole.org>", "NUnit"}));
        }

        [Test]
        public void TestFixtureMultipleAuthors()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));
            var mockFixtureSuite = (TestSuite)suite.Tests[0];
            Assert.That(mockFixtureSuite.Properties[PropertyNames.Author], Is.EquivalentTo(
                new[] { "Rob Prouse", "Charlie Poole <charlie@poole.org>", "NUnit" }));
        }

        #endregion
    }
}
