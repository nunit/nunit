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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureBuilderData;

namespace NUnit.Framework.Tests
{
	// TODO: Figure out what this is really testing and eliminate if not needed
	[TestFixture]
	public class TestFixtureBuilderTests
	{
		[Test]
		public void GoodSignature()
		{
			string methodName = "TestVoid";
			TestSuite fixture = TestBuilder.MakeFixture( typeof( SignatureTestFixture ) );
			Test foundTest = TestFinder.Find( methodName, fixture, true );
			Assert.IsNotNull( foundTest );
			Assert.AreEqual( RunState.Runnable, foundTest.RunState );
		}

		[Test]
		public void LoadCategories() 
		{
			Test fixture = TestBuilder.MakeFixture( typeof( HasCategories ) );
			Assert.IsNotNull(fixture);
			Assert.AreEqual(2, fixture.Categories.Count);
		}
	}
}
