// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture(Author = "Rob Prouse <rob@prouse.org>"), Author("Charlie Poole", "Charlie@poole.org")]
    [Author("NUnit")]
    [TestOf(typeof(AuthorAttribute))]
    public class AuthorTests
    {
        private static readonly Type FixtureType = typeof(AuthorFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.Method));
            Assert.That(testCase.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void Author()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.Method));
            Assert.That(testCase.Properties.Get(PropertyNames.Author), Is.EqualTo("Rob Prouse"));
        }

        [Test]
        public void NoAuthor()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.NoAuthorMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.Author), Is.Null);
        }

        [Test]
        public void FixtureAuthor()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));

            var mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.That(mockFixtureSuite.Properties.Get(PropertyNames.Author), Is.EqualTo("Rob Prouse"));
        }

        [Test]
        public void SeparateAuthorAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.SeparateAuthorMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.Author), Is.EqualTo("Rob Prouse"));
        }

        [Test]
        public void SeparateAuthorWithEmailAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, nameof(AuthorFixture.SeparateAuthorWithEmailMethod));
            Assert.That(testCase.Properties.Get(PropertyNames.Author), Is.EqualTo("Rob Prouse <rob@prouse.org>"));
        }

        [Test]
        public void AuthorOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, nameof(AuthorFixture.TestCaseWithAuthor));
            Assert.That(parameterizedMethodSuite.Properties.Get(PropertyNames.Author), Is.EqualTo("Rob Prouse"));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.That(testCase.Properties.Get(PropertyNames.Author), Is.EqualTo("Charlie Poole"));
        }

        #region Multiple Authors
        [Test(Author = "Rob Prouse <rob@prouse.org>"), Author("Charlie Poole", "charlie@poole.org")]
        [Author("NUnit")]
        public void TestMethodMultipleAuthors()
        {
            Test test = TestBuilder.MakeTestFromMethod(FixtureType, nameof(AuthorFixture.TestMethodMultipleAuthors));
            Assert.That(test.Properties[PropertyNames.Author], Is.EquivalentTo(
                new[] { "Rob Prouse <rob@prouse.org>", "Charlie Poole <charlie@poole.org>", "NUnit" }));
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
