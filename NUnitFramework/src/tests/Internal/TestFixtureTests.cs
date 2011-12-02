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
using NUnit.TestData.FixtureSetUpTearDownData;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureData;
using IgnoredFixture = NUnit.TestData.TestFixtureData.IgnoredFixture;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Tests of the NUnitTestFixture class
	/// </summary>
	[TestFixture]
	public class TestFixtureTests
	{
#if NUNITLITE
        private static string dataAssembly = "nunitlite.testdata";
#else
        private static string dataAssembly = "nunit.testdata";
#endif

        private static void CanConstructFrom(Type fixtureType)
        {
            CanConstructFrom(fixtureType, fixtureType.Name);
        }

        private static void CanConstructFrom(Type fixtureType, string expectedName)
        {
            TestSuite fixture = TestBuilder.MakeFixture(fixtureType);
            Assert.AreEqual(expectedName, fixture.Name);
            Assert.AreEqual(fixtureType.FullName, fixture.FullName);
        }

        private static Type GetTestDataType(string typeName)
        {
            string qualifiedName = string.Format("{0},{1}", typeName, dataAssembly);
            Type type = Type.GetType(qualifiedName);
            return type;
        }

        [Test]
		public void ConstructFromType()
		{
            CanConstructFrom(typeof(FixtureWithTestFixtureAttribute));
		}

		[Test]
		public void ConstructFromNestedType()
		{
            CanConstructFrom(typeof(OuterClass.NestedTestFixture), "OuterClass+NestedTestFixture");
		}

		[Test]
		public void ConstructFromDoublyNestedType()
		{
            CanConstructFrom(typeof(OuterClass.NestedTestFixture.DoublyNestedTestFixture),
                "OuterClass+NestedTestFixture+DoublyNestedTestFixture");
		}

        public void ConstructFromTypeWithoutTestFixtureAttributeContainingTest()
        {
            CanConstructFrom(typeof(FixtureWithoutTestFixtureAttributeContainingTest));
        }
 
        [Test]
        public void ConstructFromTypeWithoutTestFixtureAttributeContainingTestCase()
        {
            CanConstructFrom(typeof(FixtureWithoutTestFixtureAttributeContainingTestCase));
        }
 
        [Test]
        public void ConstructFromTypeWithoutTestFixtureAttributeContainingTestCaseSource()
        {
            CanConstructFrom(typeof(FixtureWithoutTestFixtureAttributeContainingTestCaseSource));
        }
 
#if !NUNITLITE
        [Test]
        public void ConstructFromTypeWithoutTestFixtureAttributeContainingTheory()
        {
            CanConstructFrom(typeof(FixtureWithoutTestFixtureAttributeContainingTheory));
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
			Assert.AreEqual( "testing ignore a fixture", suite.Properties.Get(PropertyNames.SkipReason) );
		}

//		[Test]
//		public void CannotRunAbstractFixture()
//		{
//            TestAssert.IsNotRunnable(typeof(AbstractTestFixture));
//		}

        [Test]
        public void CanRunFixtureDerivedFromAbstractTestFixture()
        {
            TestAssert.IsRunnable(typeof(DerivedFromAbstractTestFixture));
        }

        [Test]
        public void CanRunFixtureDerivedFromAbstractDerivedTestFixture()
        {
            TestAssert.IsRunnable(typeof(DerivedFromAbstractDerivedTestFixture));
        }

//		[Test]
//		public void CannotRunAbstractDerivedFixture()
//		{
//            TestAssert.IsNotRunnable(typeof(AbstractDerivedTestFixture));
//		}

        [Test]
        public void FixtureInheritingTwoTestFixtureAttributesIsLoadedOnlyOnce()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(DoubleDerivedClassWithTwoInheritedAttributes));
            Assert.That(suite, Is.TypeOf(typeof(TestFixture)));
            Assert.That(suite.Tests.Count, Is.EqualTo(0));
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

        [Test]
        public void ConstructFromStaticTypeWithoutTestFixtureAttribute()
        {
            CanConstructFrom(typeof(StaticFixtureWithoutTestFixtureAttribute));
        }

        [Test]
        public void CanRunStaticFixture()
        {
            TestAssert.IsRunnable(typeof(StaticFixtureWithoutTestFixtureAttribute));
        }

#if !NETCF
        [Test]
        public void CanRunGenericFixtureWithProperArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(
                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithProperArgsProvided`1"));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(2));
        }

//        [Test]
//        public void CannotRunGenericFixtureWithNoTestFixtureAttribute()
//        {
//            TestSuite suite = TestBuilder.MakeFixture(
//                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithNoTestFixtureAttribute`1"));
//
//            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
//            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), 
//                Is.StringStarting("Fixture type contains generic parameters"));
//        }

        [Test]
        public void CannotRunGenericFixtureWithNoArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(
                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithNoArgsProvided`1"));

            Test fixture = (Test)suite.Tests[0];
            Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That((string)fixture.Properties.Get(PropertyNames.SkipReason), Is.StringStarting("Fixture type contains generic parameters"));
        }

        [Test]
        public void CannotRunGenericFixtureDerivedFromAbstractFixtureWithNoArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(
                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureDerivedFromAbstractFixtureWithNoArgsProvided`1"));
            TestAssert.IsNotRunnable((Test)suite.Tests[0]);
        }

        [Test]
        public void CanRunGenericFixtureDerivedFromAbstractFixtureWithArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(
                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureDerivedFromAbstractFixtureWithArgsProvided`1"));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(2));
        }
#endif
        
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
