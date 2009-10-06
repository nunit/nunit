// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using NUnit.Framework;
using NUnit.TestData.AttributeInheritanceData;
using NUnit.TestUtilities;

namespace NUnit.Core.Tests
{
	[TestFixture]
	public class AttributeInheritanceTests
	{
		[Test]
		public void InheritedFixtureAttributeIsRecognized()
		{
			Assert.That( TestBuilder.MakeFixture( typeof (When_collecting_test_fixtures) ) != null );
		}

		[Test]
		public void InheritedTestAttributeIsRecognized()
		{
			Test fixture = TestBuilder.MakeFixture( typeof( When_collecting_test_fixtures ) );
			Assert.AreEqual( 1, fixture.TestCount );
		}
    }
}
