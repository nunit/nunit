// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections;
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

        [Test]
        public void Issue1118()
        {
            TestSuite suite = TestBuilder.MakeFixture(typeof(Issue1118_Fixture));
            Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(suite.Tests.Count, Is.EqualTo(3));
            Assert.That(suite.TestCaseCount, Is.EqualTo(6));
        }
    }
}
