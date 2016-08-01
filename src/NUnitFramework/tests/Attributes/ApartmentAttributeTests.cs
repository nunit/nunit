// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#if !SILVERLIGHT && !NETCF && !PORTABLE
using System;
using System.Threading;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class ApartmentAttributeTests : ThreadingTests
    {
        [Test]
        public void ApartmentStateUnknownThrowsException()
        {
            Assert.That(() => new ApartmentAttribute(ApartmentState.Unknown), Throws.ArgumentException);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void TestWithRequiresSTARunsInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            if (ParentThreadApartment == ApartmentState.STA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test]
        [Timeout(10000)]
        [Apartment(ApartmentState.STA)]
        public void TestWithTimeoutAndSTARunsInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
        }

        [TestFixture]
        [Timeout(10000)]
        [Apartment(ApartmentState.STA)]
        public class FixtureWithTimeoutRequiresSTA
        {
            [Test]
            public void RequiresSTACanBeSetOnTestFixtureWithTimeout()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            }
        }

        [TestFixture, Apartment(ApartmentState.STA)]
        public class FixtureRequiresSTA
        {
            [Test]
            public void RequiresSTACanBeSetOnTestFixture()
            {
                Assert.That( GetApartmentState( Thread.CurrentThread ), Is.EqualTo( ApartmentState.STA ) );
            }
        }

        [TestFixture]
        public class ChildFixtureRequiresSTA : FixtureRequiresSTA
        {
            // Issue #36 - Make RequiresThread, RequiresSTA, RequiresMTA inheritable
            // https://github.com/nunit/nunit-framework/issues/36
            [Test]
            public void RequiresSTAAtributeIsInheritable()
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), typeof(ApartmentAttribute), true);
                Assert.That(attributes, Has.Length.EqualTo(1),
                    "RequiresSTAAttribute was not inherited from the base class");
            }
        }

        [Test, Apartment(ApartmentState.MTA)]
        public void TestWithRequiresMTARunsInMTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));

            if (ParentThreadApartment == ApartmentState.MTA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [TestFixture, Apartment(ApartmentState.MTA)]
        public class FixtureRequiresMTA
        {
            [Test]
            public void RequiresMTACanBeSetOnTestFixture()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));
            }
        }

        [TestFixture]
        public class ChildFixtureRequiresMTA : FixtureRequiresMTA
        {
            // Issue #36 - Make RequiresThread, RequiresSTA, RequiresMTA inheritable
            // https://github.com/nunit/nunit-framework/issues/36
            [Test]
            public void RequiresMTAAtributeIsInheritable()
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), typeof(ApartmentAttribute), true);
                Assert.That(attributes, Has.Length.EqualTo(1),
                    "RequiresMTAAttribute was not inherited from the base class");
            }
        }
    }
}
#endif
