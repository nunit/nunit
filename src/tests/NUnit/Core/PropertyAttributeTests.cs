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
using NUnit.TestUtilities;
using NUnit.TestData.PropertyAttributeTests;

namespace NUnit.Core.Tests
{
	[TestFixture]
	public class PropertyAttributeTests
	{
		TestSuite fixture;

		[SetUp]
		public void CreateFixture()
		{
			fixture = TestBuilder.MakeFixture( typeof( FixtureWithProperties ) );
		}

		[Test]
		public void PropertyWithStringValue()
		{
			Test test1 = (Test)fixture.Tests[0];
			Assert.AreEqual( "Charlie", test1.Properties["user"] );
		}

		[Test]
		public void PropertiesWithNumericValues()
		{
			Test test2 = (Test)fixture.Tests[1];
			Assert.AreEqual( 10.0, test2.Properties["X"] );
			Assert.AreEqual( 17.0, test2.Properties["Y"] );
		}

		[Test]
		public void PropertyWorksOnFixtures()
		{
			Assert.AreEqual( "SomeClass", fixture.Properties["ClassUnderTest"] );
		}

		[Test]
		public void CanDeriveFromPropertyAttribute()
		{
			Test test3 = (Test)fixture.Tests[2];
			Assert.AreEqual( 5, test3.Properties["Priority"] );
		}
	}
}
