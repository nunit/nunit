// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
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
        [Platform(Include = "Win, Mono")]
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
        [Platform(Include = "Unix")]
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
            [Test]
            public void RequiresThreadCanBeSetOnTestFixture()
            {
                // TODO: Figure out how to test this
                // Assert.That(Environment.StackTrace, Contains.Substring("RunTestProc"));
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
