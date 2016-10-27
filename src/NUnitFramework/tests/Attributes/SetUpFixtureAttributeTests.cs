// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
        [TestCase(typeof(Class1))]
        [TestCase(typeof(Class2))]
        [TestCase(typeof(Class3))]
        [TestCase(typeof(Class4))]
        public void CertainAttributesAreNotAllowed(Type type)
        {
            var fixtures = new SetUpFixtureAttribute().BuildFrom(new TypeWrapper(type));
            foreach (var fixture in fixtures)
                Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
        }

#pragma warning disable 618 // Obsolete Attributes
        private class Class1
        {
            [TestFixtureSetUp]
            public void SomeMethod() { }
        }

        private class Class2
        {
            [TestFixtureTearDown]
            public void SomeMethod() { }
        }
#pragma warning restore

        private class Class3
        {
            [SetUp]
            public void SomeMethod() { }
        }

        private class Class4
        {
            [TearDown]
            public void SomeMethod() { }
        }
    }
}
