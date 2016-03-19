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

#if !PORTABLE
using System.Threading;

namespace NUnit.Framework.Attributes
{
    [SingleThreaded]
    public class SingleThreadedFixtureTests : ThreadingTests
    {
        [Test, Timeout(1)]
        public void TestWithTimeout_RunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test, Timeout(1)]
        public void TestWithTimeout_TimeoutIsIgnored()
        {
            Thread.Sleep(100);
        }

#if !SILVERLIGHT
        [Test, RequiresThread]
        public void TestWithRequiresThread_RunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

#if !NETCF
        [Test, Apartment(ApartmentState.STA)]
        public void TestRequiringSTA_RunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test, Apartment(ApartmentState.STA)]
        public void TestRequiringSTA_RunsInParentApartment()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ParentThreadApartment));
        }
#endif
#endif
    }

#if !SILVERLIGHT && !NETCF
    [SingleThreaded, Apartment(ApartmentState.STA)]
    public class SingleThreadedFixtureRunInSTA : ThreadingTests
    {
        [Test]
        public void CanRunSingleThreadedFixtureInSTA()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
        }
    }
#endif
}
#endif
