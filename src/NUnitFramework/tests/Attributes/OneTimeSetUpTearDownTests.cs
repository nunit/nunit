// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using System.Linq;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.TestData.OneTimeSetUpTearDownData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class OneTimeSetupTearDownTest
    {
        [Test]
        public void MakeSureSetUpAndTearDownAreCalled()
        {
            SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount, "SetUp");
            Assert.AreEqual(1, fixture.TearDownCount, "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreCalledOnFixtureWithTestCases()
        {
            var fixture = new SetUpAndTearDownFixtureWithTestCases();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount, "SetUp");
            Assert.AreEqual(1, fixture.TearDownCount, "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreCalledOnFixtureWithTheories()
        {
            var fixture = new SetUpAndTearDownFixtureWithTheories();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount, "SetUp");
            Assert.AreEqual(1, fixture.TearDownCount, "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreNotCalledOnExplicitFixture()
        {
            ExplicitSetUpAndTearDownFixture fixture = new ExplicitSetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(0, fixture.SetUpCount, "SetUp");
            Assert.AreEqual(0, fixture.TearDownCount, "TearDown");
        }

        [Test]
        public void CheckInheritedSetUpAndTearDownAreCalled()
        {
            InheritSetUpAndTearDown fixture = new InheritSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount);
            Assert.AreEqual(1, fixture.TearDownCount);
        }

        [Test]
        public static void StaticSetUpAndTearDownAreCalled()
        {
            StaticSetUpAndTearDownFixture.SetUpCount = 0;
            StaticSetUpAndTearDownFixture.TearDownCount = 0;
            TestBuilder.RunTestFixture(typeof(StaticSetUpAndTearDownFixture));

            Assert.AreEqual(1, StaticSetUpAndTearDownFixture.SetUpCount);
            Assert.AreEqual(1, StaticSetUpAndTearDownFixture.TearDownCount);
        }

        [Test]
        public static void StaticClassSetUpAndTearDownAreCalled()
        {
            StaticClassSetUpAndTearDownFixture.SetUpCount = 0;
            StaticClassSetUpAndTearDownFixture.TearDownCount = 0;

            TestBuilder.RunTestFixture(typeof(StaticClassSetUpAndTearDownFixture));

            Assert.AreEqual(1, StaticClassSetUpAndTearDownFixture.SetUpCount);
            Assert.AreEqual(1, StaticClassSetUpAndTearDownFixture.TearDownCount);
        }

        [Test]
        public void OverriddenSetUpAndTearDownAreNotCalled()
        {
            OverrideSetUpAndTearDown fixture = new OverrideSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(0, fixture.SetUpCount);
            Assert.AreEqual(0, fixture.TearDownCount);
            Assert.AreEqual(1, fixture.DerivedSetUpCount);
            Assert.AreEqual(1, fixture.DerivedTearDownCount);
        }

        [Test]
        public void BaseSetUpCalledFirstAndTearDownCalledLast()
        {
            DerivedSetUpAndTearDownFixture fixture = new DerivedSetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount);
            Assert.AreEqual(1, fixture.TearDownCount);
            Assert.AreEqual(1, fixture.DerivedSetUpCount);
            Assert.AreEqual(1, fixture.DerivedTearDownCount);
            Assert.That(fixture.BaseSetUpCalledFirst, "Base SetUp called first");
            Assert.That(fixture.BaseTearDownCalledLast, "Base TearDown called last");
        }

        [Test]
        public void FailedBaseSetUpCausesDerivedSetUpAndTeardownToBeSkipped()
        {
            DerivedSetUpAndTearDownFixture fixture = new DerivedSetUpAndTearDownFixture();
            fixture.ThrowInBaseSetUp = true;
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount);
            Assert.AreEqual(1, fixture.TearDownCount);
            Assert.AreEqual(0, fixture.DerivedSetUpCount);
            Assert.AreEqual(0, fixture.DerivedTearDownCount);
        }

        [Test]
        public void StaticBaseSetUpCalledFirstAndTearDownCalledLast()
        {
            StaticSetUpAndTearDownFixture.SetUpCount = 0;
            StaticSetUpAndTearDownFixture.TearDownCount = 0;
            DerivedStaticSetUpAndTearDownFixture.DerivedSetUpCount = 0;
            DerivedStaticSetUpAndTearDownFixture.DerivedTearDownCount = 0;

            DerivedStaticSetUpAndTearDownFixture fixture = new DerivedStaticSetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, DerivedStaticSetUpAndTearDownFixture.SetUpCount);
            Assert.AreEqual(1, DerivedStaticSetUpAndTearDownFixture.TearDownCount);
            Assert.AreEqual(1, DerivedStaticSetUpAndTearDownFixture.DerivedSetUpCount);
            Assert.AreEqual(1, DerivedStaticSetUpAndTearDownFixture.DerivedTearDownCount);
            Assert.That(DerivedStaticSetUpAndTearDownFixture.BaseSetUpCalledFirst, "Base SetUp called first");
            Assert.That(DerivedStaticSetUpAndTearDownFixture.BaseTearDownCalledLast, "Base TearDown called last");
        }

        [Test]
        public void HandleErrorInFixtureSetup()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInSetUp = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual( 1, fixture.SetUpCount, "setUpCount" );
            Assert.AreEqual( 1, fixture.TearDownCount, "tearDownCount" );

            Assert.AreEqual(ResultState.SetUpError, result.ResultState);
            Assert.AreEqual("System.Exception : This was thrown from fixture setup", result.Message, "TestSuite Message");
            Assert.IsNotNull(result.StackTrace, "TestSuite StackTrace should not be null");

            Assert.AreEqual(1, result.Children.Count(), "Child result count");
            Assert.AreEqual(1, result.FailCount, "Failure count");
        }

        [Test]
        public void RerunFixtureAfterSetUpFixed()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInSetUp = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(ResultState.SetUpError, result.ResultState);

            //fix the blow up in setup
            fixture.Reinitialize();
            result = TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual( 1, fixture.SetUpCount, "setUpCount" );
            Assert.AreEqual( 1, fixture.TearDownCount, "tearDownCount" );

            Assert.AreEqual(ResultState.Success, result.ResultState);
        }

        [Test]
        public void HandleIgnoreInFixtureSetup()
        {
            IgnoreInFixtureSetUp fixture = new IgnoreInFixtureSetUp();
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            // should have one suite and one fixture
            Assert.AreEqual(ResultState.Ignored.WithSite(FailureSite.SetUp), result.ResultState, "Suite should be ignored");
            Assert.AreEqual("OneTimeSetUp called Ignore", result.Message);
            Assert.IsNotNull(result.StackTrace, "StackTrace should not be null");

            Assert.AreEqual(1, result.Children.Count(), "Child result count");
            Assert.AreEqual(1, result.SkipCount, "SkipCount");
        }

        [Test]
        public void HandleErrorInFixtureTearDown()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.AreEqual(1, result.Children.Count());
            Assert.AreEqual(ResultState.TearDownError, result.ResultState);

            Assert.AreEqual(1, fixture.SetUpCount, "setUpCount");
            Assert.AreEqual(1, fixture.TearDownCount, "tearDownCount");

            Assert.AreEqual("TearDown : System.Exception : This was thrown from fixture teardown", result.Message);
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));
        }

        [Test]
        public void HandleErrorInFixtureTearDownAfterErrorInTest()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTest = true;
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.AreEqual(1, result.Children.Count());
            Assert.AreEqual(ResultState.TearDownError, result.ResultState);

            Assert.AreEqual(1, fixture.SetUpCount, "setUpCount");
            Assert.AreEqual(1, fixture.TearDownCount, "tearDownCount");

            Assert.AreEqual(TestResult.CHILD_ERRORS_MESSAGE + Environment.NewLine + "TearDown : System.Exception : This was thrown from fixture teardown", result.Message);
            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.TearDown));
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));
        }

        [Test]
        public void HandleErrorInFixtureTearDownAfterErrorInFixtureSetUp()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInSetUp = true;
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.AreEqual(1, result.Children.Count());
            Assert.AreEqual(ResultState.TearDownError, result.ResultState);

            Assert.AreEqual(1, fixture.SetUpCount, "setUpCount");
            Assert.AreEqual(1, fixture.TearDownCount, "tearDownCount");

            Assert.AreEqual("System.Exception : This was thrown from fixture setup" + Environment.NewLine +
                "TearDown : System.Exception : This was thrown from fixture teardown", result.Message);
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));
        }

        [Test]
        public void HandleExceptionInFixtureConstructor()
        {
            ITestResult result = TestBuilder.RunTestFixture( typeof( ExceptionInConstructor ) );

            Assert.AreEqual(ResultState.SetUpError, result.ResultState);
            Assert.AreEqual("System.Exception : This was thrown in constructor", result.Message, "TestSuite Message");
            Assert.IsNotNull(result.StackTrace, "TestSuite StackTrace should not be null");

            Assert.AreEqual(1, result.Children.Count(), "Child result count");
            Assert.AreEqual(1, result.FailCount, "Failure count");
        }

        [Test]
        public void RerunFixtureAfterTearDownFixed()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.AreEqual(1, result.Children.Count());

            fixture.Reinitialize();
            result = TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual( 1, fixture.SetUpCount, "setUpCount" );
            Assert.AreEqual( 1, fixture.TearDownCount, "tearDownCount" );
        }

        [Test]
        public void HandleSetUpAndTearDownWithTestInName()
        {
            SetUpAndTearDownWithTestInName fixture = new SetUpAndTearDownWithTestInName();
            TestBuilder.RunTestFixture(fixture);

            Assert.AreEqual(1, fixture.SetUpCount);
            Assert.AreEqual(1, fixture.TearDownCount);
        }

        //[Test]
        //public void RunningSingleMethodCallsSetUpAndTearDown()
        //{
        //    SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
        //    TestSuite suite = TestBuilder.MakeFixture(fixture.GetType());
        //    suite.Fixture = fixture;
        //    Test test = (Test)suite.Tests[0];

        //    suite.Run(TestListener.NULL, new NameFilter(test.TestName));

        //    Assert.AreEqual(1, fixture.SetUpCount);
        //    Assert.AreEqual(1, fixture.TearDownCount);
        //}

        [Test]
        public void IgnoredFixtureShouldNotCallFixtureSetUpOrTearDown()
        {
            IgnoredFixture fixture = new IgnoredFixture();
            TestSuite suite = new TestSuite("IgnoredFixtureSuite");
            TestSuite fixtureSuite = TestBuilder.MakeFixture( fixture.GetType() );
            TestMethod testMethod = (TestMethod)fixtureSuite.Tests[0];
            suite.Add( fixtureSuite );

            TestBuilder.RunTest(fixtureSuite, fixture);
            Assert.IsFalse( fixture.SetupCalled, "OneTimeSetUp called running fixture");
            Assert.IsFalse( fixture.TeardownCalled, "OneTimeSetUp called running fixture");

            TestBuilder.RunTest(suite, fixture);
            Assert.IsFalse( fixture.SetupCalled, "OneTimeSetUp called running enclosing suite");
            Assert.IsFalse( fixture.TeardownCalled, "OneTimeTearDown called running enclosing suite");

            TestBuilder.RunTest(testMethod, fixture);
            Assert.IsFalse( fixture.SetupCalled, "OneTimeSetUp called running a test case");
            Assert.IsFalse( fixture.TeardownCalled, "OneTimeTearDown called running a test case");
        }

        [Test]
        public void FixtureWithNoTestsShouldNotCallFixtureSetUpOrTearDown()
        {
            FixtureWithNoTests fixture = new FixtureWithNoTests();

            TestBuilder.RunTestFixture(fixture);

            Assert.That( fixture.SetupCalled, Is.False, "OneTimeSetUp should not be called for a fixture with no tests" );
            Assert.That( fixture.TeardownCalled, Is.False, "OneTimeTearDown should not be called for a fixture with no tests" );
        }

        [Test]
        public void DisposeCalledOnceWhenFixtureImplementsIDisposable()
        {
            var fixture = new DisposableFixture();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.DisposeCalled, Is.EqualTo(1));
            Assert.That(fixture.Actions, Is.EqualTo(new object[] { "OneTimeSetUp", "OneTimeTearDown", "Dispose" }));
        }

        [Test]
        public void DisposeCalledOnceWhenFixtureImplementsIDisposableAndHasTestCases()
        {
            var fixture = new DisposableFixtureWithTestCases();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.DisposeCalled, Is.EqualTo(1));
        }
    }

#if !NETCOREAPP1_1
    [TestFixture]
    class ChangesMadeInFixtureSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: This test requires fixture setup and all tests to run on same thread
            GenericIdentity identity = new GenericIdentity("foo");
            Thread.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);

            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [Test]
        public void TestThatChangesPersistUsingSameThread()
        {
            Assert.AreEqual("foo", Thread.CurrentPrincipal.Identity.Name);
            Assert.AreEqual("en-GB", Thread.CurrentThread.CurrentCulture.Name);
            Assert.AreEqual("en-GB", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [Test, RequiresThread]
        public void TestThatChangesPersistUsingSeparateThread()
        {
            Assert.AreEqual("foo", Thread.CurrentPrincipal.Identity.Name);
            Assert.AreEqual("en-GB", Thread.CurrentThread.CurrentCulture.Name, "#CurrentCulture");
            Assert.AreEqual("en-GB", Thread.CurrentThread.CurrentUICulture.Name, "#CurrentUICulture");
        }
    }
#endif
}
