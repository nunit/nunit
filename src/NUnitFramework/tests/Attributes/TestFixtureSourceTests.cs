// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
        public void CheckArgument(Type type)
        {
            var result = TestBuilder.RunTestFixture(type);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [TestCase(typeof(InstanceField_SameClass))]
        [TestCase(typeof(InstanceProperty_SameClass))]
        [TestCase(typeof(InstanceMethod_SameClass))]
        public void CheckNotRunnable(Type type)
        {
            var suite = TestBuilder.MakeFixture(type);
            Assert.That(suite.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(suite.Properties.Get(PropertyNames.SkipReason), Is.EqualTo(TestFixtureSourceAttribute.MUST_BE_STATIC));
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


        public static IEnumerable<TestCaseData> IndividualInstanceNameTestDataSource()
        {
            var suite = (ParameterizedFixtureSuite)TestBuilder.MakeFixture(typeof(IndividualInstanceNameTestDataFixture));

            foreach (var testFixture in suite.Tests)
            {
                var expectedName = (string)testFixture.Properties.Get("ExpectedFixtureName");

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
            Assert.That(suite, Is.TypeOf<TestFixture>());
            Assert.That(suite.TestCaseCount, Is.EqualTo(1));
            Assert.That(suite.Tests.Count, Is.EqualTo(1));
            Assert.That(suite.Tests[0], Is.TypeOf<TestMethod>());
        }

        [Test]
        public void CanRunGenericFixtureSourceWithProperArgsProvided()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(GenericFixtureSourceWithProperArgsProvided<>));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite is ParameterizedFixtureSuite);
            Assert.That(suite.Tests.Count, Is.EqualTo(GenericFixtureSource.Source.Length));
        }
    }
}
