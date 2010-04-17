// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Reflection;
using System.Collections;
using NUnit.Framework.Api;
using NUnit.Core.Builders;
using NUnit.TestData.DescriptionFixture;
using NUnit.TestUtilities;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
	// TODO: Review to see if we need these tests

	[TestFixture]
	public class DescriptionTests
	{
		static readonly Type FixtureType = typeof( DescriptionFixture );

		[Test]
		public void ReflectionTest()
		{
			Test testCase = TestBuilder.MakeTestCase( FixtureType, "Method" );
			Assert.AreEqual( RunState.Runnable, testCase.RunState );
		}

        [Test]
        public void Description()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "Method");
            Assert.AreEqual("Test Description", testCase.Description);
        }

        [Test]
		public void NoDescription()
		{
			Test testCase = TestBuilder.MakeTestCase( FixtureType, "NoDescriptionMethod" );
			Assert.IsNull(testCase.Description);
		}

		[Test]
		public void FixtureDescription()
		{
			TestSuite suite = new TestSuite("suite");
			suite.Add( TestBuilder.MakeFixture( typeof( DescriptionFixture ) ) );

			IList tests = suite.Tests;
			TestSuite mockFixtureSuite = (TestSuite)tests[0];

			Assert.AreEqual("Fixture Description", mockFixtureSuite.Description);
		}

        [Test]
        public void SeparateDescriptionAttribute()
        {
            Test testCase = TestBuilder.MakeTestCase(FixtureType, "SeparateDescriptionMethod");
            Assert.AreEqual("Separate Description", testCase.Description);
        }

        [Test]
        public void DescriptionOnTestCase()
        {
            TestSuite parameterizedMethodSuite = TestBuilder.MakeParameterizedMethodSuite(FixtureType, "TestCaseWithDescription");
            Assert.AreEqual("method description", parameterizedMethodSuite.Description);
            Test testCase = (Test)parameterizedMethodSuite.Tests[0];
            Assert.AreEqual("case description", testCase.Description);
        }
    }
}
