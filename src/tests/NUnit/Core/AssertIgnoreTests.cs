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
using NUnit.Framework;
using NUnit.Core.Builders;
using NUnit.TestUtilities;
using NUnit.TestData.AssertIgnoreData;

namespace NUnit.Core.Tests
{
	/// <summary>
	/// Tests of IgnoreException and Assert.Ignore
	/// </summary>
	[TestFixture]
	public class AssertIgnoreTests
	{
		[Test]
		public void IgnoreThrowsIgnoreException()
		{
			//Note that we can't use ExpectedException here because
			//Assert.Ignore takes precedence and the test is ignored.
			try
			{
				Assert.Ignore("I threw this!");
			}
			catch(IgnoreException ex)
			{
				Assert.AreEqual("I threw this!", ex.Message);
			}
		}

		[Test]
		public void IgnoreWorksForTestCase()
		{
            Type fixtureType = typeof(IgnoredTestCaseFixture);
            Test test = TestBuilder.MakeTestCase(fixtureType, "CallsIgnore");
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.IsFalse(result.Executed, "Test should not run");
            Assert.AreEqual("Ignore me", result.Message);
        }

        [Test]
        public void IgnoreTakesPrecedenceOverExpectedException()
        {
            Type fixtureType = typeof(IgnoredTestCaseFixture);
            Test test = TestBuilder.MakeTestCase(fixtureType, "CallsIgnoreWithExpectedException");
            TestResult result = test.Run(TestListener.NULL, TestFilter.Empty);
            Assert.IsFalse(result.Executed, "Test should not run");
            Assert.AreEqual("Ignore me", result.Message);
        }

		[Test]
		public void IgnoreWorksForTestSuite()
		{
			//IgnoredTestSuiteFixture testFixture = new IgnoredTestSuiteFixture();
			TestSuite suite = new TestSuite("IgnoredTestFixture");
			suite.Add( TestBuilder.MakeFixture( typeof( IgnoredTestSuiteFixture ) ) );
            TestResult result = suite.Run(TestListener.NULL, TestFilter.Empty);

			TestResult fixtureResult = (TestResult)result.Results[0];
			Assert.IsFalse( fixtureResult.Executed, "Fixture should not have been executed" );
			
			foreach( TestResult testResult in fixtureResult.Results )
				Assert.IsFalse( testResult.Executed, "Test case should not have been executed" );
		}

		[Test]
		public void IgnoreWorksFromSetUp()
		{
			TestSuite testFixture = TestBuilder.MakeFixture( typeof( IgnoreInSetUpFixture ) );
            TestResult fixtureResult = testFixture.Run(TestListener.NULL, TestFilter.Empty);

			Assert.IsTrue( fixtureResult.Executed, "Fixture should have been executed" );
			
			foreach( TestResult testResult in fixtureResult.Results )
				Assert.IsFalse( testResult.Executed, "Test case should not have been executed" );
		}

		[Test]
		public void IgnoreWithUserMessage()
		{
			try
			{
				Assert.Ignore( "my message" );
			}
			catch( IgnoreException ex )
			{
				Assert.AreEqual( "my message", ex.Message );
			}
		}

		[Test]
		public void IgnoreWithUserMessage_OneArg()
		{
			try
			{
				Assert.Ignore( "The number is {0}", 5 );
			}
			catch( IgnoreException ex )
			{
				Assert.AreEqual( "The number is 5", ex.Message );
			}
		}

		[Test]
		public void IgnoreWithUserMessage_ThreeArgs()
		{
			try
			{
				Assert.Ignore( "The numbers are {0}, {1} and {2}", 1, 2, 3 );
			}
			catch( IgnoreException ex )
			{
				Assert.AreEqual( "The numbers are 1, 2 and 3", ex.Message );
			}
		}

		[Test]
		public void IgnoreWithUserMessage_ArrayOfArgs()
		{
			try
			{
			Assert.Ignore( "The numbers are {0}, {1} and {2}", new object[] { 1, 2, 3 } );
			}
			catch( IgnoreException ex )
			{
				Assert.AreEqual( "The numbers are 1, 2 and 3", ex.Message );
			}
		}
	}
}
