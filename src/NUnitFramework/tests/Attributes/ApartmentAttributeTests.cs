// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [Platform(Include = "Win, Mono")]
    [TestFixture]
    public class ApartmentAttributeTests : ThreadingTests
    {
        [Test]
        public void ApartmentStateUnknownIsNotRunnable()
        {
            var testSuite = TestBuilder.MakeFixture(typeof(ApartmentDataApartmentAttribute));
            Assert.That(testSuite, Has.Property(nameof(TestSuite.RunState)).EqualTo(RunState.NotRunnable));
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [Test, Apartment(ApartmentState.STA)]
        public void TestWithRequiresSTARunsInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            if (ParentThreadApartment == ApartmentState.STA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test]
#if THREAD_ABORT
        [Timeout(10_000)]
#endif
#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [Apartment(ApartmentState.STA)]
        public void TestWithTimeoutAndSTARunsInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
        }

        [TestFixture]
#if THREAD_ABORT
        [Timeout(10_000)]
#endif
#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [Apartment(ApartmentState.STA)]
        public class FixtureWithTimeoutRequiresSTA
        {
            [Test]
            public void RequiresSTACanBeSetOnTestFixtureWithTimeout()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            }
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
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
            public void RequiresSTAAttributeIsInheritable()
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
            public void RequiresMTAAttributeIsInheritable()
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), typeof(ApartmentAttribute), true);
                Assert.That(attributes, Has.Length.EqualTo(1),
                    "RequiresMTAAttribute was not inherited from the base class");
            }
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [TestFixture]
        [Apartment(ApartmentState.STA)]
        [Parallelizable(ParallelScope.Children)]
        public class ParallelStaFixture
        {
            [Test]
            public void TestMethodsShouldInheritApartmentFromFixture()
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.STA));
            }

            [TestCase(1)]
            [TestCase(2)]
            public void TestCasesShouldInheritApartmentFromFixture(int n)
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.STA));
            }
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [TestFixture]
        [Apartment(ApartmentState.STA)]
        [Parallelizable(ParallelScope.Children)]
        public class ParallelStaFixtureWithMtaTests
        {
            [Test]
            [Apartment(ApartmentState.MTA)]
            public void TestMethodsShouldRespectTheirApartment()
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.MTA));
            }

            [TestCase(1)]
            [TestCase(2)]
            [Apartment(ApartmentState.MTA)]
            public void TestCasesShouldRespectTheirApartment(int n)
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.MTA));
            }
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [TestFixture]
        [Apartment(ApartmentState.STA)]
        [Parallelizable(ParallelScope.None)]
        public class NonParallelStaFixture
        {
            [Test]
            public void TestMethodsShouldInheritApartmentFromFixture()
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.STA));
            }

            [TestCase(1)]
            [TestCase(2)]
            public void TestCasesShouldInheritApartmentFromFixture(int n)
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.STA));
            }
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [TestFixture]
        [Apartment(ApartmentState.STA)]
        [Parallelizable(ParallelScope.None)]
        public class NonParallelStaFixtureWithMtaTests
        {
            [Test]
            [Apartment(ApartmentState.MTA)]
            public void TestMethodsShouldRespectTheirApartment()
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.MTA));
            }

            [TestCase(1)]
            [TestCase(2)]
            [Apartment(ApartmentState.MTA)]
            public void TestCasesShouldRespectTheirApartment(int n)
            {
                Assert.That(Thread.CurrentThread.GetApartmentState(), Is.EqualTo(ApartmentState.MTA));
            }
        }
    }
}
