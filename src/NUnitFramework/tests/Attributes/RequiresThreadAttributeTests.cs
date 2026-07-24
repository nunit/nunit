// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class RequiresThreadAttributeTests : ThreadingTests
    {
        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsInSeparateThread()
        {
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(ParentThread));
        }

        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        [Test]
        public void ApartmentStateUnknownIsNotRunnable()
        {
            var testSuite = TestBuilder.MakeFixture(typeof(ApartmentDataRequiresThreadAttribute));
            Assert.That(testSuite, Has.Property(nameof(TestSuite.RunState)).EqualTo(RunState.NotRunnable));
        }

#if NETCOREAPP
        [Platform(Includes = [PlatformNames.Win, PlatformNames.Mono])]
#endif
        [TestFixture]
        public class ApartmentStateRequiredTests : ThreadingTests
        {
            [Test, RequiresThread(ApartmentState.STA)]
            public void TestWithRequiresThreadWithSTAArgRunsOnSeparateThreadInSTA()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
                Assert.That(Thread.CurrentThread, Is.Not.EqualTo(ParentThread));
            }

            [Test, RequiresThread(ApartmentState.MTA)]
            public void TestWithRequiresThreadWithMTAArgRunsOnSeparateThreadInMTA()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));
                Assert.That(Thread.CurrentThread, Is.Not.EqualTo(ParentThread));
            }
        }
#if NETCOREAPP
        [Platform(Include = PlatformNames.UNIX)]
        [TestFixture]
        public class ApartmentStateRequiredToFailOnUnixNetCoreTests
        {
            [Test]
            public void TestWithRequiresThreadWithSTAArgRunsOnSeparateThreadInSTA()
            {
                var test = TestBuilder.MakeTestFromMethod(typeof(ApartmentStateRequiredTests), nameof(ApartmentStateRequiredTests.TestWithRequiresThreadWithSTAArgRunsOnSeparateThreadInSTA));
                var work = TestBuilder.CreateWorkItem(test);
                var result = TestBuilder.ExecuteWorkItem(work);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Skipped));
                Assert.That(result.Message, Is.EqualTo("Apartment state cannot be set on this platform."));
            }

            [Test]
            public void TestWithRequiresThreadWithMTAArgRunsOnSeparateThreadInMTA()
            {
                var test = TestBuilder.MakeTestFromMethod(typeof(ApartmentStateRequiredTests), nameof(ApartmentStateRequiredTests.TestWithRequiresThreadWithMTAArgRunsOnSeparateThreadInMTA));
                var work = TestBuilder.CreateWorkItem(test);
                var result = TestBuilder.ExecuteWorkItem(work);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Skipped));
                Assert.That(result.Message, Is.EqualTo("Apartment state cannot be set on this platform."));
            }
        }
#endif

        [TestFixture, RequiresThread]
        public class FixtureRequiresThread
        {
            private Thread _thread;

            [OneTimeSetUp]
            public void SetUp()
            {
                _thread = Thread.CurrentThread;
                Assert.That(Thread.CurrentThread.Name, Is.EqualTo("NUnit.Fw.WorkItemThread"));
                Assert.That(Environment.StackTrace, Contains.Substring("WorkItem.Start"));
            }

            [Test]
            public void RequiresThreadCanBeSetOnTestFixture1()
            {
                Assert.That(Thread.CurrentThread, Is.EqualTo(_thread));
            }

            [Test]
            public void RequiresThreadCanBeSetOnTestFixture2()
            {
                Assert.That(Thread.CurrentThread, Is.EqualTo(_thread));
            }
        }

        [TestFixture]
        public class ChildFixtureRequiresThread : FixtureRequiresThread
        {
            // Issue #36 - Make RequiresThread, RequiresSTA, RequiresMTA inheritable
            // https://github.com/nunit/nunit-framework/issues/36
            [Test]
            public void RequiresThreadAttributeIsInheritable()
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType(), typeof(RequiresThreadAttribute), true);
                Assert.That(attributes, Has.Length.EqualTo(1),
                    "RequiresThreadAttribute was not inherited from the base class");
            }
        }
    }
}
