// **********************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2014 Charlie Poole
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// **********************************************************************************

#region Using Directives

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

#endregion

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [Author("Rob Prouse", "rob@prouse.org")]
    [TestOf(typeof(AuthorAttribute))]
    public class AuthorTests
    {
        static readonly Type FixtureType = typeof(AuthorFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "Method");
            Assert.AreEqual(RunState.Runnable, testCase.RunState);
        }

        [Test]
        public void Author()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "Method");
            Assert.AreEqual("Rob Prouse", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void NoAuthor()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "NoAuthorMethod");
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
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "SeparateAuthorMethod");
            Assert.AreEqual("Rob Prouse", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void SeparateAuthorWithEmailAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "SeparateAuthorWithEmailMethod");
            Assert.AreEqual("Rob Prouse <rob@prouse.org>", testCase.Properties.Get(PropertyNames.Author));
        }

        [Test]
        public void AuthorOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestCaseWithAuthor");
            Assert.AreEqual("Rob Prouse", parameterizedMethodSuite.Properties.Get(PropertyNames.Author));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("Charlie Poole", testCase.Properties.Get(PropertyNames.Author));
        }
    }
}