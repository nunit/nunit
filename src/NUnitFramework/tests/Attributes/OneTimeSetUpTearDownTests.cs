// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.OneTimeSetUpTearDownData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class OneTimeSetupTearDownTest
    {
        [Test]
        public void MakeSureSetUpAndTearDownAreCalled()
        {
            SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "SetUp");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreCalledOnFixtureWithTestCases()
        {
            var fixture = new SetUpAndTearDownFixtureWithTestCases();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "SetUp");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreCalledOnFixtureWithTheories()
        {
            var fixture = new SetUpAndTearDownFixtureWithTheories();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "SetUp");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "TearDown");
        }

        [Test]
        public void MakeSureSetUpAndTearDownAreNotCalledOnExplicitFixture()
        {
            ExplicitSetUpAndTearDownFixture fixture = new ExplicitSetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(0), "SetUp");
            Assert.That(fixture.TearDownCount, Is.EqualTo(0), "TearDown");
        }

        [Test]
        public void CheckInheritedSetUpAndTearDownAreCalled()
        {
            InheritSetUpAndTearDown fixture = new InheritSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1));
            Assert.That(fixture.TearDownCount, Is.EqualTo(1));
        }

        [Test]
        public static void StaticSetUpAndTearDownAreCalled()
        {
            StaticSetUpAndTearDownFixture.SetUpCount = 0;
            StaticSetUpAndTearDownFixture.TearDownCount = 0;
            TestBuilder.RunTestFixture(typeof(StaticSetUpAndTearDownFixture));

            Assert.That(StaticSetUpAndTearDownFixture.SetUpCount, Is.EqualTo(1));
            Assert.That(StaticSetUpAndTearDownFixture.TearDownCount, Is.EqualTo(1));
        }

        [Test]
        public static void StaticClassSetUpAndTearDownAreCalled()
        {
            StaticClassSetUpAndTearDownFixture.SetUpCount = 0;
            StaticClassSetUpAndTearDownFixture.TearDownCount = 0;

            TestBuilder.RunTestFixture(typeof(StaticClassSetUpAndTearDownFixture));

            Assert.That(StaticClassSetUpAndTearDownFixture.SetUpCount, Is.EqualTo(1));
            Assert.That(StaticClassSetUpAndTearDownFixture.TearDownCount, Is.EqualTo(1));
        }

        [Test]
        public void OverriddenSetUpAndTearDownAreNotCalled()
        {
            OverrideSetUpAndTearDown fixture = new OverrideSetUpAndTearDown();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(0));
            Assert.That(fixture.TearDownCount, Is.EqualTo(0));
            Assert.That(fixture.DerivedSetUpCount, Is.EqualTo(1));
            Assert.That(fixture.DerivedTearDownCount, Is.EqualTo(1));
        }

        [Test]
        public void BaseSetUpCalledFirstAndTearDownCalledLast()
        {
            DerivedSetUpAndTearDownFixture fixture = new DerivedSetUpAndTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1));
            Assert.That(fixture.TearDownCount, Is.EqualTo(1));
            Assert.That(fixture.DerivedSetUpCount, Is.EqualTo(1));
            Assert.That(fixture.DerivedTearDownCount, Is.EqualTo(1));
            Assert.That(fixture.BaseSetUpCalledFirst, "Base SetUp called first");
            Assert.That(fixture.BaseTearDownCalledLast, "Base TearDown called last");
        }

        [Test]
        public void FailedBaseSetUpCausesDerivedSetUpAndTeardownToBeSkipped()
        {
            DerivedSetUpAndTearDownFixture fixture = new DerivedSetUpAndTearDownFixture();
            fixture.ThrowInBaseSetUp = true;
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1));
            Assert.That(fixture.TearDownCount, Is.EqualTo(1));
            Assert.That(fixture.DerivedSetUpCount, Is.EqualTo(0));
            Assert.That(fixture.DerivedTearDownCount, Is.EqualTo(0));
        }

        [Test]
        public void FailedSetUpStacktracePropogatesToTestResult()
        {
            SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
            fixture.ThrowInBaseSetUp = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.That(result.ResultState.Site, Is.EqualTo(FailureSite.SetUp));
            Assert.That(result.StackTrace, Is.Not.Null);
            Assert.That(result.StackTrace, Does.Contain($"{nameof(SetUpAndTearDownFixture)}.{nameof(SetUpAndTearDownFixture.Init)}"));

            Assert.That(result.HasChildren, Is.True);
            foreach (var childResult in result.Children)
            {
                Assert.That(childResult.ResultState.Site, Is.EqualTo(FailureSite.Parent));
                Assert.That(childResult.StackTrace, Is.EqualTo(result.StackTrace));
            }

            Assert.That(fixture.SetUpCount, Is.EqualTo(1));
            Assert.That(fixture.TearDownCount, Is.EqualTo(1));
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

            Assert.That(DerivedStaticSetUpAndTearDownFixture.SetUpCount, Is.EqualTo(1));
            Assert.That(DerivedStaticSetUpAndTearDownFixture.TearDownCount, Is.EqualTo(1));
            Assert.That(DerivedStaticSetUpAndTearDownFixture.DerivedSetUpCount, Is.EqualTo(1));
            Assert.That(DerivedStaticSetUpAndTearDownFixture.DerivedTearDownCount, Is.EqualTo(1));
            Assert.That(DerivedStaticSetUpAndTearDownFixture.BaseSetUpCalledFirst, "Base SetUp called first");
            Assert.That(DerivedStaticSetUpAndTearDownFixture.BaseTearDownCalledLast, "Base TearDown called last");
        }

        [Test]
        public void HandleErrorInFixtureSetup()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInSetUp = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.SetUpError));
            Assert.That(result.Message, Is.EqualTo("System.Exception : This was thrown from fixture setup"), "TestSuite Message");
            Assert.That(result.StackTrace, Is.Not.Null, "TestSuite StackTrace should not be null");

            Assert.That(result.Children.Count(), Is.EqualTo(1), "Child result count");
            Assert.That(result.FailCount, Is.EqualTo(1), "Failure count");
        }

        [Test]
        public void RerunFixtureAfterSetUpFixed()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInSetUp = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.SetUpError));

            //fix the blow up in setup
            fixture.Reinitialize();
            result = TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void HandleIgnoreInFixtureSetup()
        {
            IgnoreInFixtureSetUp fixture = new IgnoreInFixtureSetUp();
            ITestResult result = TestBuilder.RunTestFixture(fixture);

            // should have one suite and one fixture
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Ignored.WithSite(FailureSite.SetUp)), "Suite should be ignored");
            Assert.That(result.Message, Is.EqualTo("OneTimeSetUp called Ignore"));
            Assert.That(result.StackTrace, Is.Not.Null, "StackTrace should not be null");

            Assert.That(result.Children.Count(), Is.EqualTo(1), "Child result count");
            Assert.That(result.SkipCount, Is.EqualTo(1), "SkipCount");
        }

        [Test]
        public void HandleErrorInFixtureTearDown()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.That(result.Children.Count(), Is.EqualTo(1));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.TearDownError));

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");

            Assert.That(result.Message, Is.EqualTo("TearDown : System.Exception : This was thrown from fixture teardown"));
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));
        }

        [Test]
        public void HandleErrorInFixtureTearDownAfterErrorInTest()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTest = true;
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.That(result.Children.Count(), Is.EqualTo(1));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.TearDownError));

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");

            Assert.That(result.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE + Environment.NewLine + "TearDown : System.Exception : This was thrown from fixture teardown"));
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
            Assert.That(result.Children.Count(), Is.EqualTo(1));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.TearDownError));

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");

            Assert.That(result.Message, Is.EqualTo("System.Exception : This was thrown from fixture setup" + Environment.NewLine +
                "TearDown : System.Exception : This was thrown from fixture teardown"));
            Assert.That(result.StackTrace, Does.Contain("--TearDown"));
        }

        [Test]
        public void HandleExceptionInFixtureConstructor()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(ExceptionInConstructor));

            Assert.That(result.ResultState, Is.EqualTo(ResultState.SetUpError));
            Assert.That(result.Message, Is.EqualTo("System.Exception : This was thrown in constructor"), "TestSuite Message");
            Assert.That(result.StackTrace, Is.Not.Null, "TestSuite StackTrace should not be null");

            Assert.That(result.Children.Count(), Is.EqualTo(1), "Child result count");
            Assert.That(result.FailCount, Is.EqualTo(1), "Failure count");
        }

        [Test]
        public void RerunFixtureAfterTearDownFixed()
        {
            MisbehavingFixture fixture = new MisbehavingFixture();
            fixture.BlowUpInTearDown = true;
            ITestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.That(result.Children.Count(), Is.EqualTo(1));

            fixture.Reinitialize();
            result = TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1), "setUpCount");
            Assert.That(fixture.TearDownCount, Is.EqualTo(1), "tearDownCount");
        }

        [Test]
        public void HandleSetUpAndTearDownWithTestInName()
        {
            SetUpAndTearDownWithTestInName fixture = new SetUpAndTearDownWithTestInName();
            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetUpCount, Is.EqualTo(1));
            Assert.That(fixture.TearDownCount, Is.EqualTo(1));
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
            TestSuite fixtureSuite = TestBuilder.MakeFixture(fixture.GetType());
            TestMethod testMethod = (TestMethod)fixtureSuite.Tests[0];
            suite.Add(fixtureSuite);

            TestBuilder.RunTest(fixtureSuite, fixture);
            Assert.That(fixture.SetupCalled, Is.False, "OneTimeSetUp called running fixture");
            Assert.That(fixture.TeardownCalled, Is.False, "OneTimeSetUp called running fixture");

            TestBuilder.RunTest(suite, fixture);
            Assert.That(fixture.SetupCalled, Is.False, "OneTimeSetUp called running enclosing suite");
            Assert.That(fixture.TeardownCalled, Is.False, "OneTimeTearDown called running enclosing suite");

            TestBuilder.RunTest(testMethod, fixture);
            Assert.That(fixture.SetupCalled, Is.False, "OneTimeSetUp called running a test case");
            Assert.That(fixture.TeardownCalled, Is.False, "OneTimeTearDown called running a test case");
        }

        [Test]
        public void FixtureWithNoTestsShouldNotCallFixtureSetUpOrTearDown()
        {
            FixtureWithNoTests fixture = new FixtureWithNoTests();

            TestBuilder.RunTestFixture(fixture);

            Assert.That(fixture.SetupCalled, Is.False, "OneTimeSetUp should not be called for a fixture with no tests");
            Assert.That(fixture.TeardownCalled, Is.False, "OneTimeTearDown should not be called for a fixture with no tests");
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

        [Test]
        public void AsyncDisposeCalledOnceWhenFixtureImplementsIAsyncDisposable()
        {
            var fixture = new AsyncDisposableFixture();
            TestBuilder.RunTestFixture(fixture);

            var expected = new[] { nameof(AsyncDisposableFixture.OneTimeSetUp), nameof(AsyncDisposableFixture.OneTimeTearDown), nameof(AsyncDisposableFixture.DisposeAsync) };

            Assert.That(fixture.DisposeCalled, Is.EqualTo(1));
            Assert.That(fixture.Actions, Is.EqualTo(expected));
        }

        [Test]
        public void AsyncDisposeCalledOnceWhenFixtureImplementsIAsyncDisposableThroughInheritance()
        {
            var fixture = new InheritedAsyncDisposableFixture();
            TestBuilder.RunTestFixture(fixture);

            var expected = new[] { nameof(AsyncDisposableFixture.OneTimeSetUp), nameof(AsyncDisposableFixture.OneTimeTearDown), nameof(AsyncDisposableFixture.DisposeAsync) };

            Assert.That(fixture.DisposeCalled, Is.EqualTo(1));
            Assert.That(fixture.Actions, Is.EqualTo(expected));
        }

        [Test]
        public void AsyncDisposePrioritizedWhenSyncAndAsyncDispose()
        {
            var fixture = new AsyncAndSyncDisposableFixture();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.DisposeCalled, Is.EqualTo(1));
            Assert.That(fixture.Actions, Does.Contain(nameof(IAsyncDisposable.DisposeAsync)));
            Assert.That(fixture.Actions, Does.Not.Contain(nameof(IDisposable.Dispose)));
        }
    }

    [TestFixture]
    internal class ChangesMadeInFixtureSetUp
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: This test requires fixture setup and all tests to run on same thread
            GenericIdentity identity = new GenericIdentity("foo");
            Thread.CurrentPrincipal = new GenericPrincipal(identity, Array.Empty<string>());

            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [Test]
        public void TestThatChangesPersistUsingSameThread()
        {
            Assert.That(Thread.CurrentPrincipal?.Identity?.Name, Is.EqualTo("foo"));
            Assert.That(Thread.CurrentThread.CurrentCulture.Name, Is.EqualTo("en-GB"));
            Assert.That(Thread.CurrentThread.CurrentUICulture.Name, Is.EqualTo("en-GB"));
        }

        [Test, RequiresThread]
        public void TestThatChangesPersistUsingSeparateThread()
        {
            Assert.That(Thread.CurrentPrincipal?.Identity?.Name, Is.EqualTo("foo"));
            Assert.That(Thread.CurrentThread.CurrentCulture.Name, Is.EqualTo("en-GB"), "#CurrentCulture");
            Assert.That(Thread.CurrentThread.CurrentUICulture.Name, Is.EqualTo("en-GB"), "#CurrentUICulture");
        }
    }
}
