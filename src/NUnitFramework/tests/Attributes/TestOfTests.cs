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
    [TestOf(typeof(TestOfAttribute))]
    public class TestOfTests
    {
        static readonly Type FixtureType = typeof(TestOfFixture);

        [Test]
        public void ReflectionTest()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "Method");
            Assert.AreEqual(RunState.Runnable, testCase.RunState);
        }

        [Test]
        public void TestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "Method");
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void NoTestOf()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "NoTestOfMethod");
            Assert.IsNull(testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void FixtureTestOf()
        {
            var suite = new TestSuite("suite");
            suite.Add(TestBuilder.MakeFixture(FixtureType));

            var mockFixtureSuite = (TestSuite)suite.Tests[0];

            Assert.AreEqual("NUnit.Framework.TestOfAttribute", mockFixtureSuite.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void SeparateTestOfAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "SeparateTestOfTypeMethod");
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void SeparateTestOfStringMethod()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "SeparateTestOfStringMethod");
            Assert.AreEqual("NUnit.Framework.TestOfAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }

        [Test]
        public void TestOfOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestCaseWithTestOf");
            Assert.AreEqual("NUnit.Framework.TestAttribute", parameterizedMethodSuite.Properties.Get(PropertyNames.TestOf));
            var testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("NUnit.Framework.TestCaseAttribute", testCase.Properties.Get(PropertyNames.TestOf));
        }
    }
}