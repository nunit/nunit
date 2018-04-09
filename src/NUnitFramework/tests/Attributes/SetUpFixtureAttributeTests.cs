// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class SetUpFixtureAttributeTests
    {
        [Test]
        public void SetUpFixtureCanBeIgnored()
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(typeof(IgnoredSetUpFixture));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.Ignored));
        }

        [Ignore("Just Because")]
        private class IgnoredSetUpFixture
        {
        }

        [Test]
        public void SetUpFixtureMayBeParallelizable()
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(typeof(ParallelizableSetUpFixture));
            foreach (var fixture in fixtures)
                Assert.That(fixture.Properties.Get(PropertyNames.ParallelScope), Is.EqualTo(ParallelScope.Self));
        }

        [Parallelizable]
        private class ParallelizableSetUpFixture
        {
        }

        [TestCase(typeof(TestSetupClass))]
        [TestCase(typeof(TestTearDownClass))]
        public void CertainAttributesAreNotAllowed(Type type)
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(type);
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        private class TestSetupClass
        {
            [SetUp]
            public void SomeMethod() { }
        }

        private class TestTearDownClass
        {
            [TearDown]
            public void SomeMethod() { }
        }
    }
}
