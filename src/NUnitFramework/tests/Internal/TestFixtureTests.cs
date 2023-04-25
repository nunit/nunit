// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.TestData.OneTimeSetUpTearDownData;
using NUnit.TestUtilities;
using NUnit.TestData.TestFixtureTests;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Tests of the NUnitTestFixture class
    /// </summary>
    [TestFixture]
    public class TestFixtureTests
    {
        private static readonly string dataAssembly = "nunit.testdata";

        private static void CanConstructFrom(Type fixtureType)
        {
            CanConstructFrom(fixtureType, fixtureType.Name);
        }

        private static void CanConstructFrom(Type fixtureType, string expectedName)
        {
            TestSuite fixture = TestBuilder.MakeFixture(fixtureType);
            Assert.That(fixture.Name, Is.EqualTo(expectedName));
            Assert.That(fixture.FullName, Is.EqualTo(fixtureType.FullName));
        }

        private static Type GetTestDataType(string typeName)
        {
            string qualifiedName = $"{typeName},{dataAssembly}";
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
            CanConstructFrom(typeof(OuterClass.NestedTestFixture.DoublyNestedTestFixture), "OuterClass+NestedTestFixture+DoublyNestedTestFixture");
        }

        [Test]
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

        [Test]
        public void ConstructFromTypeWithoutTestFixtureAttributeContainingTheory()
        {
            CanConstructFrom(typeof(FixtureWithoutTestFixtureAttributeContainingTheory));
        }

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
        public void CapturesArgumentsForConstructorWithArgsSupplied()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithArgsSupplied));
            Assert.That(fixture.Arguments, Is.EqualTo(new[] { 7, 3 }));
        }

        [Test]
        public void CapturesNoArgumentsForConstructorWithoutArgsSupplied()
        {
            var fixture = TestBuilder.MakeFixture(typeof(RegularFixtureWithOneTest));
            Assert.That(fixture.Arguments, Is.EqualTo(Array.Empty<object>()));
        }

        [Test]
        public void CapturesArgumentsForConstructorWithMultipleArgsSupplied()
        {
            var fixture = TestBuilder.MakeFixture(typeof(FixtureWithMultipleArgsSupplied));
            Assert.That(fixture.HasChildren, Is.True);

            var expectedArgumentSeries = new[]
            {
                new object[] {8, 4},
                new object[] {7, 3}
            };

            var actualArgumentSeries = fixture.Tests.Select(x => x.Arguments).ToArray();

            Assert.That(actualArgumentSeries, Is.EquivalentTo(expectedArgumentSeries));
        }

        [Test]
        public void BadConstructorRunsWithSetUpError()
        {
            var result = TestBuilder.RunTestFixture(typeof(BadCtorFixture));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.SetUpError));
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
        public void FixtureUsingIgnoreAttributeIsIgnored()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(FixtureUsingIgnoreAttribute));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("testing ignore a fixture"));
        }

        [Test]
        public void FixtureWithNestedIgnoreAttributeIsIgnored() {
            TestSuite suite = TestBuilder.MakeFixture(typeof(FixtureUsingIgnoreAttribute.SubFixture));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("testing ignore a fixture"));
        }

        [Test]
        public void FixtureUsingIgnorePropertyIsIgnored()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(FixtureUsingIgnoreProperty));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("testing ignore a fixture"));
        }

        [Test]
        public void FixtureUsingIgnoreReasonPropertyIsIgnored()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(FixtureUsingIgnoreReasonProperty));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("testing ignore a fixture"));
        }

        [Test]
        public void FixtureWithParallelizableOnOneTimeSetUpIsInvalid()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(FixtureWithParallelizableOnOneTimeSetUp));
            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("ParallelizableAttribute is only allowed on test methods and fixtures"));
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
            Assert.That(suite.Tests, Is.Empty);
        }

        [Test]
        public void CanRunMultipleOneTimeSetUp()
        {
            TestAssert.IsRunnable(typeof(MultipleFixtureSetUpAttributes));
        }

        [Test]
        public void CanRunMultipleOneTimeTearDown()
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

        [Test]
        public void CanRunGenericFixtureWithProperArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureWithProperArgsProvided<>));
            // GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithProperArgsProvided`1"));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests, Has.Count.EqualTo(2));
        }

//        [Test]
//        public void CannotRunGenericFixtureWithNoTestFixtureAttribute()
//        {
//            TestSuite suite = TestBuilder.MakeFixture(
//                GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithNoTestFixtureAttribute`1"));
//
//            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
//            Assert.That(suite.Properties.Get(PropertyNames.SkipReason),
//                Does.StartWith("Fixture type contains generic parameters"));
//        }

        [Test]
        public void CannotRunGenericFixtureWithNoArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureWithNoArgsProvided<>));
            // GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureWithNoArgsProvided`1"));

            Test fixture = (Test)suite.Tests[0];
            Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That((string)fixture.Properties.Get(PropertyNames.SkipReason), Does.StartWith("Fixture type contains generic parameters"));
        }

        [Test]
        public void CannotRunGenericFixtureDerivedFromAbstractFixtureWithNoArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureDerivedFromAbstractFixtureWithNoArgsProvided<>));
            // GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureDerivedFromAbstractFixtureWithNoArgsProvided`1"));
            TestAssert.IsNotRunnable((Test)suite.Tests[0]);
        }

        [Test]
        public void CanRunGenericFixtureDerivedFromAbstractFixtureWithArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureDerivedFromAbstractFixtureWithArgsProvided<>));
            // GetTestDataType("NUnit.TestData.TestFixtureData.GenericFixtureDerivedFromAbstractFixtureWithArgsProvided`1"));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests, Has.Count.EqualTo(2));
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

        #region OneTimeSetUp Signature

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

        #region OneTimeTearDown Signature

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

        #region Issue 318 - Test Fixture is null on ITest objects

        public class FixtureNotNullTestAttribute : TestActionAttribute
        {
            private readonly string _location;

            public FixtureNotNullTestAttribute(string location)
            {
                _location = location;
            }

            public override void BeforeTest(ITest test)
            {
                Assert.That(test, Is.Not.Null, "ITest is null on a " + _location);
                Assert.That(test.Fixture, Is.Not.Null, "ITest.Fixture is null on a " + _location);
                Assert.That(test.Fixture.GetType(), Is.EqualTo(test.TypeInfo.Type), "ITest.Fixture is not the correct type on a " + _location);
            }
        }

        [FixtureNotNullTest("TestFixture class")]
        [TestFixture]
        public class FixtureIsNotNullForTests
        {
            [FixtureNotNullTest("Test method")]
            [Test]
            public void TestMethod()
            {
            }

            [FixtureNotNullTest("TestCase method")]
            [TestCase(1)]
            public void TestCaseMethod(int i)
            {
            }
        }

        #endregion
    }
}
