// ***********************************************************************
// Copyright (c) 2012 Charlie Poole, Rob Prouse
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

#if PLATFORM_DETECTION
using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [NonParallelizable]
    public class TimeoutTests : ThreadingTests
    {
        [Test, Timeout(500)]
        public void TestWithTimeoutRunsOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(ParentThread));
        }

        [Test, Timeout(500)]
        public void TestWithTimeoutRunsSetUpAndTestOnSameThread()
        {
            Assert.That(Thread.CurrentThread, Is.EqualTo(SetupThread));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "Runner hangs at end when this is run")]
        public void TestTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("InfiniteLoopWith50msTimeout", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        [Test]
        public void SetUpTimesOutAndTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInSetUp();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("Test1", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "TearDown was not run");
        }

        /* TODO: Uncomment this test when issue #352 is fixed, ignoring causes build warnings
        [Test, Ignore("Issue #352 - Test with infinite loop in TearDown cannot be aborted")]
        public void TearDownTimesOutAndNoFurtherTearDownIsRun()
        {
            TimeoutFixture fixture = new TimeoutFixtureWithTimeoutInTearDown();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("Test1", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
            Assert.That(fixture.TearDownWasRun, "Base TearDown should not have been run but was");
        }
        */

        [Test]
        [Platform(Exclude = "Mono", Reason = "Runner hangs at end when this is run")]
        public void TimeoutCanBeSetOnTestFixture()
        {
            ITestResult suiteResult = TestBuilder.RunTestFixture(typeof(TimeoutFixtureWithTimeoutOnFixture));
            Assert.That(suiteResult.ResultState, Is.EqualTo(ResultState.ChildFailure));
            Assert.That(suiteResult.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(suiteResult.ResultState.Site, Is.EqualTo(FailureSite.Child));
            ITestResult result = TestFinder.Find("Test2WithInfiniteLoop", suiteResult, false);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("50ms"));
        }

        [Test]
        public void TestTimeoutNotElapsed()
        {
            TimeoutTestCaseFixture fixture = new TimeoutTestCaseFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("TestTimeOutNotElapsed", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void TestTimeoutElapsed()
        {
            TimeoutTestCaseFixture fixture = new TimeoutTestCaseFixture();
            TestSuite suite = TestBuilder.MakeFixture(fixture);
            TestMethod testMethod = (TestMethod)TestFinder.Find("TestTimeOutElapsed", suite, false);
            ITestResult result = TestBuilder.RunTest(testMethod, fixture);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Does.Contain("100ms"));
        }

        [Explicit("Tests that demonstrate Timeout failure")]
        public class ExplicitTests
        {
            [Test, Timeout(50)]
            public void TestTimesOut()
            {
                while (true) ;
            }

            [Test, Timeout(50), RequiresThread]
            public void TestTimesOutUsingRequiresThread()
            {
                while (true) ;
            }

            [Test, Timeout(50), Apartment(ApartmentState.STA)]
            public void TestTimesOutInSTA()
            {
                while (true) ;
            }

            // TODO: The test in TimeoutTestCaseFixture work as expected when run
            // directly by NUnit. It's only when run via TestBuilder as a second
            // level test that the result is incorrect. We need to fix this.
            [Test]
            public void TestTimeOutTestCaseWithOutElapsed()
            {
                TimeoutTestCaseFixture fixture = new TimeoutTestCaseFixture();
                TestSuite suite = TestBuilder.MakeFixture(fixture);
                ParameterizedMethodSuite methodSuite = (ParameterizedMethodSuite)TestFinder.Find("TestTimeOutTestCase", suite, false);
                ITestResult result = TestBuilder.RunTest(methodSuite, fixture);
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure), "Suite result");
                Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.Success), "First test");
                Assert.That(result.Children.ToArray()[1].ResultState, Is.EqualTo(ResultState.Failure), "Second test");
            }
        }

        [Test, Platform("Win")]
        public void TimeoutWithMessagePumpShouldAbort()
        {
            ITestResult result = TestBuilder.RunTest(
                TestBuilder.MakeTestFromMethod(typeof(TimeoutFixture), nameof(TimeoutFixture.TimeoutWithMessagePumpShouldAbort)),
                new TimeoutFixture());

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            Assert.That(result.Message, Is.EqualTo("Test exceeded Timeout value of 500ms"));
        }
    }
}
#endif
