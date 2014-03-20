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

#if !SILVERLIGHT && !NETCF
using System;
using System.Threading;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class RequiresMTAAttributeTests : ThreadingTests
    {
        [Test, RequiresMTA]
        public void TestWithRequiresMTARunsInMTA()
        {
            Assert.That( GetApartmentState( Thread.CurrentThread ), Is.EqualTo( ApartmentState.MTA ) );
            if ( ParentThreadApartment == ApartmentState.MTA )
                Assert.That( Thread.CurrentThread, Is.EqualTo( ParentThread ) );
        }

        [TestFixture, RequiresMTA]
        public class FixtureRequiresMTA
        {
            [Test]
            public void RequiresMTACanBeSetOnTestFixture()
            {
                Assert.That( GetApartmentState( Thread.CurrentThread ), Is.EqualTo( ApartmentState.MTA ) );
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
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), typeof (RequiresMTAAttribute), true);
                Assert.That(attributes, Has.Length.EqualTo(1),
                    "RequiresMTAAttribute was not inherited from the base class");
            }
        }
    }
}
#endif