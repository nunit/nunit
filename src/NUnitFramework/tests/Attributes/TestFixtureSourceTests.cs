// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.TestFixtureSourceData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    public class TestFixtureSourceTests
    {
        [TestCase(typeof(StaticField_SameClass))]
        [TestCase(typeof(StaticProperty_SameClass))]
        [TestCase(typeof(StaticMethod_SameClass))]
        [TestCase(typeof(StaticField_DifferentClass))]
        [TestCase(typeof(StaticProperty_DifferentClass))]
        [TestCase(typeof(StaticMethod_DifferentClass))]
        [TestCase(typeof(StaticProperty_InheritedClass))]
        [TestCase(typeof(IEnumerableSource))]
        [TestCase(typeof(SourceReturnsObjectArray))]
        [TestCase(typeof(SourceReturnsFixtureParameters))]
        [TestCase(typeof(ExtraTestFixtureAttributeIsIgnored))]
        [TestCase(typeof(TestFixtureMayUseMultipleSourceAttributes))]
        [TestCase(typeof(DerivedClassUsingBaseClassDataSource))]
        [TestCase(typeof(BaseClassUsingDerivedClassDataSource))]
        public void CheckArgument(Type type)
        {
            var result = TestBuilder.RunTestFixture(type);
            Assert.That(result.HasChildren, Is.True);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [TestCase(typeof(InstanceField_SameClass))]
        [TestCase(typeof(InstanceProperty_SameClass))]
        [TestCase(typeof(InstanceMethod_SameClass))]
        public void CheckNotRunnable(Type type)
        {
            var suite = TestBuilder.MakeFixture(type);
            Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(suite.Tests[0].Properties.Get(PropertyNames.SkipReason), Is.EqualTo(TestFixtureSourceAttribute.MUST_BE_STATIC));
            //var result = TestBuilder.RunTestSuite(suite, null);
            //Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        [Test]
        public void CanIgnoreIndividualFixtures()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(IndividualInstancesMayBeIgnored));

            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[1].RunState, Is.EqualTo(RunState.Ignored));
            Assert.That(suite.Tests[1].Properties.Get(PropertyNames.SkipReason), Is.EqualTo("There must be a reason"));
            Assert.That(suite.Tests[2].RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void CanMarkIndividualFixturesExplicit()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(IndividualInstancesMayBeExplicit));

            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[0].RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests[1].RunState, Is.EqualTo(RunState.Explicit));
            Assert.That(suite.Tests[1].Properties.Get(PropertyNames.SkipReason), Is.EqualTo("Runs long"));
            Assert.That(suite.Tests[2].RunState, Is.EqualTo(RunState.Explicit));
        }

        #region Test name tests

        public static IEnumerable<TestCaseData> IndividualInstanceNameTestDataSource()
        {
            var suite = (ParameterizedFixtureSuite)TestBuilder.MakeFixture(typeof(IndividualInstanceNameTestDataFixture));

            foreach (var testFixture in suite.Tests)
            {
                var expectedName = (string)testFixture.Properties.Get("ExpectedTestName");

                yield return new TestCaseData((TestFixture)testFixture, expectedName)
                    .SetArgDisplayNames(expectedName); // SetArgDisplayNames (here) is purely cosmetic for the purposes of these tests
            }
        }

        [TestCaseSource(nameof(IndividualInstanceNameTestDataSource))]
        public static void IndividualInstanceName(TestFixture testFixture, string expectedName)
        {
            Assert.That(testFixture.Name, Is.EqualTo(expectedName));
        }

        [TestCaseSource(nameof(IndividualInstanceNameTestDataSource))]
        public static void IndividualInstanceFullName(TestFixture testFixture, string expectedName)
        {
            var expectedFullName = typeof(IndividualInstanceNameTestDataFixture).Namespace + "." + expectedName;
            Assert.That(testFixture.FullName, Is.EqualTo(expectedFullName));
        }

        #endregion

        [Test]
        public void Issue1118()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(Issue1118_Fixture));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests.Count, Is.EqualTo(3));
            Assert.That(suite.TestCaseCount, Is.EqualTo(6));
        }

        [Test]
        public void NoNamespaceSourceWithTwoValues()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(NoNamespaceTestFixtureSourceWithTwoValues));
            Assert.That(suite, Is.TypeOf<ParameterizedFixtureSuite>());
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.TestCaseCount, Is.EqualTo(2));
            Assert.That(suite.Tests.Count, Is.EqualTo(2));
            Assert.That(suite.Tests, Is.All.TypeOf<TestFixture>());
        }

        [Test]
        public void NoNamespaceSourceWithSingleValue()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(NoNamespaceTestFixtureSourceWithSingleValue));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite, Is.TypeOf<ParameterizedFixtureSuite>());
            Assert.That(suite.TestCaseCount, Is.EqualTo(1));
            Assert.That(suite.Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0], Is.TypeOf<TestFixture>());
            Assert.That(suite.Tests[0].TestCaseCount, Is.EqualTo(1));
            Assert.That(suite.Tests[0].Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0].Tests[0], Is.TypeOf<TestMethod>());
        }

        [Test]
        public void CanRunGenericFixtureSourceWithProperTypeArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureSourceWithProperArgsProvided<>));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0] is ParameterizedFixtureSuite);
            Assert.That(suite.Tests[0].Tests.Count, Is.EqualTo(GenericFixtureSource.Source.Length));
        }

        [Test]
        public void CanRunGenericFixtureSourceWithProperTypeAndConstructorArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureSourceWithTypeAndConstructorArgs<>));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0] is ParameterizedFixtureSuite);
            Assert.That(suite.Tests[0].Tests.Count, Is.EqualTo(GenericFixtureWithTypeAndConstructorArgsSource.Source.Length));
        }

        [Test]
        public void CanRunGenericFixtureSourceWithConstructorArgsProvidedAndTypeArgsDeduced()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureSourceWithConstructorArgs<>));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0] is ParameterizedFixtureSuite);
            Assert.That(suite.Tests[0].Tests.Count, Is.EqualTo(GenericFixtureWithConstructorArgsSource.Source.Length));
        }



        [Test]
        public void ParallelizableAttributeOnFixtureSourceIsAppliedToTests()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(TextFixtureSourceWithParallelizableAttribute));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite, Is.TypeOf<ParameterizedFixtureSuite>());
            Assert.That(suite.TestCaseCount, Is.EqualTo(3));
            Assert.That(suite.Tests.Count, Is.EqualTo(3));

            Assert.That(suite.Tests[0].Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.All));
            Assert.That(suite.Tests[1].Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.All));
            Assert.That(suite.Tests[2].Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.All));
        }
    }
}
