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
using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [SingleThreaded]
    public class SingleThreadedFixtureTests : ThreadingTests
    {
        [Test]
        public void TestRunsOnSameThreadAsParent()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test]
        public void TestWithTimeoutIsInvalid()
        {
            CheckTestIsInvalid<SingleThreadedFixture_TestWithTimeout>("Timeout");
        }

#if !SILVERLIGHT
        [Test]
        public void TestWithRequiresThreadIsInvalid()
        {
            CheckTestIsInvalid<SingleThreadedFixture_TestWithRequiresThread>("RequiresThread");
        }

#if !NETCF
        [Test]
        public void TestRequiringSTAIsInvalid()
        {
            CheckTestIsInvalid<SingleThreadedFixture_TestWithDifferentApartment>("DifferentApartment");
        }
#endif
#endif

        private void CheckTestIsInvalid<TFixture>(string reason)
        {
            var result = TestBuilder.RunTestFixture(typeof(TFixture));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure.WithSite(FailureSite.Child)));
            Assert.That(result.Children[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
            Assert.That(result.Children[0].Message, Does.Contain(reason));
        }
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
