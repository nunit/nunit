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
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.TestData.ThreadingFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    //[Platform(Exclude = "Mono", Reason = "Runner hangs at end when these are run")]
    public class ThreadingTests
    {
        Thread parentThread;
        ApartmentState parentThreadApartment;

        Thread setupThread;

        [TestFixtureSetUp]
        public void GetParentThreadInfo()
        {
			this.parentThread = Thread.CurrentThread;
            this.parentThreadApartment = GetApartmentState(parentThread);
        }

        private static ApartmentState GetApartmentState(Thread thread)
        { 
            return thread.GetApartmentState();
        }

        [SetUp]
        public void GetSetUpThreadInfo()
        {
            this.setupThread = Thread.CurrentThread;
        }

        [Test, Timeout(50)]
        public void TestWithTimeoutRunsOnSeparateThread()
        {
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, Timeout(50)]
        public void TestWithTimeoutRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(setupThread));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "Runner hangs at end when this is run")]
        [Platform(Exclude = "Net-1.1,Net-1.0", Reason = "Cancels the run when executed")]
        public void TestWithInfiniteLoopTimesOut()
        {
            ThreadingFixture fixture = new ThreadingFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            Test test = TestFinder.Find("InfiniteLoopWith50msTimeout", suite, false);
            ITestResult result = TestBuilder.RunTest(test, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Contains.Substring("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        //[Test, STAThread]
        //public void TestWithSTAThreadRunsInSTA()
        //{
        //    Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.STA));
        //    if (parentThreadApartment == ApartmentState.STA)
        //        Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        //}

        //[Test, MTAThread]
        //public void TestWithMTAThreadRunsInMTA()
        //{
        //    Assert.That(Thread.CurrentThread.ApartmentState, Is.EqualTo(ApartmentState.MTA));
        //    if (parentThreadApartment == ApartmentState.MTA)
        //        Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        //}

        [Test, RequiresSTA]
        public void TestWithRequiresSTARunsInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            if (parentThreadApartment == ApartmentState.STA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, RequiresMTA]
        public void TestWithRequiresMTARunsInMTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));
            if (parentThreadApartment == ApartmentState.MTA)
                Assert.That(Thread.CurrentThread, Is.EqualTo(parentThread));
        }

        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsInSeparateThread()
        {
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, RequiresThread]
        public void TestWithRequiresThreadRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(setupThread));
        }

        [Test, RequiresThread(ApartmentState.STA)]
        public void TestWithRequiresThreadWithSTAArgRunsOnSeparateThreadInSTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test, RequiresThread(ApartmentState.MTA)]
        public void TestWithRequiresThreadWithMTAArgRunsOnSeparateThreadInMTA()
        {
            Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));
            Assert.That(Thread.CurrentThread, Is.Not.EqualTo(parentThread));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "Runner hangs at end when this is run")]
        public void TimeoutCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(ThreadingFixtureWithTimeout));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.Failure));
            ITestResult result = TestFinder.Find("Test2WithInfiniteLoop", suiteResult, false);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Contains.Substring("50ms"));
        }

        [TestFixture, RequiresSTA]
        class FixtureRequiresSTA
        {
            [Test]
            public void RequiresSTACanBeSetOnTestFixture()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.STA));
            }
        }

        [TestFixture, RequiresMTA]
        class FixtureRequiresMTA
        {
            [Test]
            public void RequiresMTACanBeSetOnTestFixture()
            {
                Assert.That(GetApartmentState(Thread.CurrentThread), Is.EqualTo(ApartmentState.MTA));
            }
        }

        [TestFixture, RequiresThread]
        class FixtureRequiresThread
        {
            [Test]
            public void RequiresThreadCanBeSetOnTestFixture()
            {
                // TODO: Figure out how to test this
                //Assert.That(Environment.StackTrace, Contains.Substring("RunTestProc"));
            }
        }
    }
}
