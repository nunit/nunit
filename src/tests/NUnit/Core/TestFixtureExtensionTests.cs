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
using NUnit.Framework;
using NUnit.Core.Builders;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureExtensionData;

namespace NUnit.Core.Tests
{
	/// <summary>
	/// Summary description for TestFixtureExtension.
	/// </summary>
	/// 
	[TestFixture]
	public class TestFixtureExtensionTests
	{
		private Test suite;

		private void RunTestOnFixture( object fixture )
		{
			Test suite = TestBuilder.MakeFixture( fixture );
            suite.Run(TestListener.NULL, TestFilter.Empty);
		}

		[SetUp] public void LoadFixture()
		{
			string testsDll = "test-assembly.dll";
			TestSuiteBuilder builder = new TestSuiteBuilder();
			TestPackage package = new TestPackage( testsDll );
			package.TestName = "NUnit.TestData.TestFixtureExtensionData.DerivedTestFixture";
			suite= builder.Build( package );
		}

		[Test] 
		public void CheckMultipleSetUp()
		{
			SetUpDerivedTestFixture fixture = new SetUpDerivedTestFixture();
			RunTestOnFixture( fixture );

			Assert.AreEqual(true, fixture.baseSetup);		}

		[Test]
		public void DerivedTest()
		{
			Assert.IsNotNull(suite);

            TestResult result = suite.Run(TestListener.NULL, TestFilter.Empty);
			Assert.IsTrue(result.IsSuccess);
		}

		[Test]
		public void InheritSetup()
		{
			DerivedTestFixture fixture = new DerivedTestFixture();
			RunTestOnFixture( fixture );

			Assert.AreEqual(true, fixture.baseSetup);
		}

		[Test]
		public void InheritTearDown()
		{
			DerivedTestFixture fixture = new DerivedTestFixture();
			RunTestOnFixture( fixture );

			Assert.AreEqual(true, fixture.baseTeardown);
		}
	}
}
