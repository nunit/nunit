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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Core;
using NUnit.TestData.FixtureSetUpTearDownData;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureData;
using IgnoredFixture = NUnit.TestData.TestFixtureData.IgnoredFixture;

namespace NUnit.Framework.Tests
{
	/// <summary>
	/// Tests of the NUnitTestFixture class
	/// </summary>
	[TestFixture]
	public class TestFixtureTests
	{
		[Test]
		public void ConstructFromType()
		{
			TestSuite fixture = TestBuilder.MakeFixture( typeof( FixtureWithTestFixtureAttribute ) );
			Assert.AreEqual( "FixtureWithTestFixtureAttribute", fixture.Name );
            Assert.AreEqual("NUnit.TestData.TestFixtureData.FixtureWithTestFixtureAttribute", fixture.FullName);
		}

		[Test]
		public void ConstructFromNestedType()
		{
			TestSuite fixture = TestBuilder.MakeFixture( typeof( OuterClass.NestedTestFixture ) );
			Assert.AreEqual( "OuterClass+NestedTestFixture", fixture.Name );
            Assert.AreEqual("NUnit.TestData.TestFixtureData.OuterClass+NestedTestFixture", fixture.FullName);
		}

		[Test]
		public void ConstructFromDoublyNestedType()
		{
			TestSuite fixture = TestBuilder.MakeFixture( typeof( OuterClass.NestedTestFixture.DoublyNestedTestFixture ) );
			Assert.AreEqual( "OuterClass+NestedTestFixture+DoublyNestedTestFixture", fixture.Name );
            Assert.AreEqual("NUnit.TestData.TestFixtureData.OuterClass+NestedTestFixture+DoublyNestedTestFixture", fixture.FullName);
		}

        [Test]
        public void ConstructFromTypeWithoutTestFixtureAttribute()
        {
            TestSuite fixture = TestBuilder.MakeFixture(typeof(FixtureWithoutTestFixtureAttribute));
            Assert.AreEqual("FixtureWithoutTestFixtureAttribute", fixture.Name);
            Assert.AreEqual("NUnit.TestData.TestFixtureData.FixtureWithoutTestFixtureAttribute", fixture.FullName);
        }

#if CLR_2_0
        [Test]
        public void ConstructFromStaticTypeWithoutTestFixtureAttribute()
        {
            TestSuite fixture = TestBuilder.MakeFixture(typeof(StaticFixtureWithoutTestFixtureAttribute));
            Assert.AreEqual("StaticFixtureWithoutTestFixtureAttribute", fixture.Name);
            Assert.AreEqual("NUnit.TestData.TestFixtureData.StaticFixtureWithoutTestFixtureAttribute", fixture.FullName);
        }
#endif

        [Test]
        public void CannotRunConstructorWithArgsNotSupplied()
        {
            TestAssert.IsNotRunnable(typeof(NoDefaultCtorFixture));
        }

        [Test]
        public void CanRunConstructorWithArgsSupplied()
        {
            TestAssert.IsRunnable(typeof(FixtureWithArgsSupplied));
        }

        [Test]
		public void CannotRunBadConstructor()
		{
            TestAssert.IsNotRunnable(typeof(BadCtorFixture));
		}

		[Test] 
		public void CanRunMultipleSetUp()
		{
            TestAssert.IsRunnable(typeof(MultipleSetUpAttributes));
		}

		[Test] 
		public void CanRunMultipleTearDown()
		{
            TestAssert.IsRunnable(typeof(MultipleTearDownAttributes));
		}

		[Test]
		public void CannotRunIgnoredFixture()
		{
			TestSuite suite = TestBuilder.MakeFixture( typeof( IgnoredFixture ) );
			Assert.AreEqual( RunState.Ignored, suite.RunState );
			Assert.AreEqual( "testing ignore a fixture", suite.IgnoreReason );
		}

		[Test]
		public void CannotRunAbstractFixture()
		{
            TestAssert.IsNotRunnable(typeof(AbstractTestFixture));
		}

#if CLR_2_0
        [Test]
        public void CanRunStaticFixture()
        {
            TestAssert.IsRunnable(typeof(StaticFixtureWithoutTestFixtureAttribute));
        }
#endif

		[Test]
		public void CannotRunAbstractDerivedFixture()
		{
            TestAssert.IsNotRunnable(typeof(AbstractDerivedTestFixture));
		}

		[Test] 
		public void CanRunMultipleTestFixtureSetUp()
		{
            TestAssert.IsRunnable(typeof(MultipleFixtureSetUpAttributes));
		}

		[Test] 
		public void CanRunMultipleTestFixtureTearDown()
		{
            TestAssert.IsRunnable(typeof(MultipleFixtureTearDownAttributes));
		}

        [Test]
        public void CanRunTestFixtureWithNoTests()
        {
            TestAssert.IsRunnable(typeof(FixtureWithNoTests));
        }

		#region SetUp Signature
		[Test] 
		public void CannotRunPrivateSetUp()
		{
            TestAssert.IsNotRunnable(typeof(PrivateSetUp));
		}

		[Test] 
		public void CanRunProtectedSetUp()
		{
            TestAssert.IsRunnable(typeof(ProtectedSetUp));
		}

        /// <summary>
        /// Determines whether this instance [can run static set up].
        /// </summary>
		[Test] 
		public void CanRunStaticSetUp()
		{
            TestAssert.IsRunnable(typeof(StaticSetUp));
		}

		[Test]
		public void CannotRunSetupWithReturnValue()
		{
            TestAssert.IsNotRunnable(typeof(SetUpWithReturnValue));
		}

		[Test]
		public void CannotRunSetupWithParameters()
		{
            TestAssert.IsNotRunnable(typeof(SetUpWithParameters));
		}
		#endregion

		#region TearDown Signature
		[Test] 
		public void CannotRunPrivateTearDown()
		{
            TestAssert.IsNotRunnable(typeof(PrivateTearDown));
		}

		[Test] 
		public void CanRunProtectedTearDown()
		{
            TestAssert.IsRunnable(typeof(ProtectedTearDown));
		}

		[Test] 
		public void CanRunStaticTearDown()
		{
            TestAssert.IsRunnable(typeof(StaticTearDown));
		}

		[Test]
		public void CannotRunTearDownWithReturnValue()
		{
            TestAssert.IsNotRunnable(typeof(TearDownWithReturnValue));
		}

		[Test]
		public void CannotRunTearDownWithParameters()
		{
            TestAssert.IsNotRunnable(typeof(TearDownWithParameters));
		}
		#endregion

		#region TestFixtureSetUp Signature
		[Test] 
		public void CannotRunPrivateFixtureSetUp()
		{
            TestAssert.IsNotRunnable(typeof(PrivateFixtureSetUp));
		}

		[Test] 
		public void CanRunProtectedFixtureSetUp()
		{
            TestAssert.IsRunnable(typeof(ProtectedFixtureSetUp));
		}

		[Test] 
		public void CanRunStaticFixtureSetUp()
		{
            TestAssert.IsRunnable(typeof(StaticFixtureSetUp));
		}

		[Test]
		public void CannotRunFixtureSetupWithReturnValue()
		{
            TestAssert.IsNotRunnable(typeof(FixtureSetUpWithReturnValue));
		}

		[Test]
		public void CannotRunFixtureSetupWithParameters()
		{
            TestAssert.IsNotRunnable(typeof(FixtureSetUpWithParameters));
		}
		#endregion

		#region TestFixtureTearDown Signature
		[Test] 
		public void CannotRunPrivateFixtureTearDown()
		{
            TestAssert.IsNotRunnable(typeof(PrivateFixtureTearDown));
		}

		[Test] 
		public void CanRunProtectedFixtureTearDown()
		{
            TestAssert.IsRunnable(typeof(ProtectedFixtureTearDown));
		}

		[Test] 
		public void CanRunStaticFixtureTearDown()
		{
            TestAssert.IsRunnable(typeof(StaticFixtureTearDown));
		}

//		[TestFixture]
//			[Category("fixture category")]
//			[Category("second")]
//			private class HasCategories 
//		{
//			[Test] public void OneTest()
//			{}
//		}
//
//		[Test]
//		public void LoadCategories() 
//		{
//			TestSuite fixture = LoadFixture("NUnit.Core.Tests.TestFixtureBuilderTests+HasCategories");
//			Assert.IsNotNull(fixture);
//			Assert.AreEqual(2, fixture.Categories.Count);
//		}

		[Test]
		public void CannotRunFixtureTearDownWithReturnValue()
		{
            TestAssert.IsNotRunnable(typeof(FixtureTearDownWithReturnValue));
		}

		[Test]
		public void CannotRunFixtureTearDownWithParameters()
		{
            TestAssert.IsNotRunnable(typeof(FixtureTearDownWithParameters));
		}
		#endregion
	}
}
