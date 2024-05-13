// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [SingleThreaded]
    public class SingleThreadedFixtureTests : ThreadingTests
    {
        [Test]
        public void TestRunsOnSameThreadAsParent()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

#if THREAD_ABORT
        [Test, Timeout(100)]
        public void TestWithTimeoutIsValid()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }
#endif

        [Test]
        public void TestWithRequiresThreadIsInvalid()
        {
            CheckTestIsInvalid<SingleThreadedFixture_TestWithRequiresThread>("RequiresThreadAttribute may not be specified");
        }

        [Test]
        public void TestWithDifferentApartmentIsInvalid()
        {
            CheckTestIsInvalid<SingleThreadedFixture_TestWithDifferentApartment>("may not specify a different apartment");
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [SingleThreaded]
        public class SingleThreadedFixtureWithApartmentStateTests : ThreadingTests
        {
            [Test, Apartment(ApartmentState.MTA)]
            public void TestWithSameApartmentIsValid()
            {
                Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.MTA));
            }
        }

        private void CheckTestIsInvalid<TFixture>(string reason)
        {
            var result = TestBuilder.RunTestFixture(typeof(TFixture));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure.WithSite(FailureSite.Child)));
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
            Assert.That(result.Children.ToArray()[0].Message, Does.Contain(reason));
        }
    }

#if NETCOREAPP
    [Platform(Include = "Win")]
#endif
    [SingleThreaded, Apartment(ApartmentState.STA)]
    public class SingleThreadedFixtureRunInSTA : ThreadingTests
    {
        [Test]
        public void CanRunSingleThreadedFixtureInSTA()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
        }

        [Test, Apartment(ApartmentState.STA)]
        public void TestWithSameApartmentStateIsValid()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
        }
    }
}
