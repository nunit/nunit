// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.Tests;
using NUnit.Tests.Assemblies;

#if THREAD_ABORT
using System.Text;
#endif

namespace NUnit.Framework.Tests.Api
{
    // Functional tests of the TestAssemblyRunner and all subordinate classes
    [NonParallelizable]
    public class TestAssemblyRunnerTests : ITestListener
    {
        private const string MockAssemblyFile = "mock-assembly.dll";
        private const string CouldNotLoadMsg = "Could not load";
        private const string BadFile = "mock-assembly.pdb";
        private const string SlowTestsFile = "slow-nunit-tests.dll";
        private const string MissingFile = "junk.dll";

        private const string InvalidFilterElementMessage = "Invalid filter element: {0}";

        private static readonly IDictionary<string, object> EmptySettings = new Dictionary<string, object>();

        private ITestAssemblyRunner _runner;

        private int _suiteStartedCount;
        private int _suiteFinishedCount;
        private int _testStartedCount;
        private int _testFinishedCount;
        private int _testOutputCount;
        private int _successCount;
        private int _failCount;
        private int _skipCount;
        private int _inconclusiveCount;

        private Dictionary<string, bool> _activeTests;

        [SetUp]
        public void CreateRunner()
        {
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            _suiteStartedCount = 0;
            _suiteFinishedCount = 0;
            _testStartedCount = 0;
            _testFinishedCount = 0;
            _testOutputCount = 0;
            _successCount = 0;
            _failCount = 0;
            _skipCount = 0;
            _inconclusiveCount = 0;

            _activeTests = new Dictionary<string, bool>();
        }

        #region Load

        [Test]
        public void Load_GoodFile_ReturnsRunnableSuite()
        {
            var result = LoadMockAssembly();

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuite);
                Assert.That(result, Is.TypeOf<TestAssembly>());
                Assert.That(result.Name, Is.EqualTo(MockAssemblyFile));
                Assert.That(result.RunState, Is.EqualTo(Framework.Interfaces.RunState.Runnable));
                Assert.That(result.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
            });
        }

        [Test, SetUICulture("en-US")]
        public void Load_FileNotFound_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(MissingFile, EmptySettings);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuite);
                Assert.That(result, Is.TypeOf<TestAssembly>());
                Assert.That(result.Name, Is.EqualTo(MissingFile));
                Assert.That(result.RunState, Is.EqualTo(Framework.Interfaces.RunState.NotRunnable));
                Assert.That(result.TestCaseCount, Is.EqualTo(0));
                Assert.That(result.Properties.Get(PropertyNames.SkipReason),
                    Does.StartWith(CouldNotLoadMsg));
            });
        }

        [Test, SetUICulture("en-US")]
        public void Load_BadFile_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(BadFile, EmptySettings);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuite);
                Assert.That(result, Is.TypeOf<TestAssembly>());
                Assert.That(result.Name, Is.EqualTo(BadFile));
                Assert.That(result.RunState, Is.EqualTo(Framework.Interfaces.RunState.NotRunnable));
                Assert.That(result.TestCaseCount, Is.EqualTo(0));
                Assert.That(result.Properties.Get(PropertyNames.SkipReason),
                    Does.StartWith(CouldNotLoadMsg).And.Contains(BadFile));
            });
        }

        #endregion

        #region CountTestCases

        [Test]
        public void CountTestCases_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void CountTestCases_FullNameRegexFilterForFixture_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            Assert.That(_runner.CountTestCases(new FullNameFilter(".*BadFixture.*", isRegex: true)),
                Is.EqualTo(BadFixture.Tests));
        }

        [Test]
        public void CountTestCases_FullNameRegexFilterForAssembly_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            Assert.That(_runner.CountTestCases(new FullNameFilter(".*mock-assembly.dll", isRegex: true)),
                Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void CountTestCases_FullNameRegexFilterForTest_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            Assert.That(_runner.CountTestCases(new OrFilter(
                new FullNameFilter("NUnit.Tests.ExplicitFixture.Test1"),
                new FullNameFilter("NUnit.Tests.ExplicitFixture.Test2"))),
                Is.EqualTo(2));
        }

        [Test]
        public void CountTestCases_CategoryFilter_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            Assert.That(_runner.CountTestCases(new CategoryFilter("FixtureCategory")), Is.EqualTo(MockTestFixture.Tests));
        }

        [Test]
        public void CountTestCases_WithoutLoad_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.CountTestCases(TestFilter.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Tests must be loaded before counting test cases."));
        }

        [Test]
        public void CountTestCases_FileNotFound_ReturnsZero()
        {
            _runner.Load(MissingFile, EmptySettings);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        [Test]
        public void CountTestCases_BadFile_ReturnsZero()
        {
            _runner.Load(BadFile, EmptySettings);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        #endregion

        #region ExploreTests
        [Test]
        public void ExploreTests_WithoutLoad_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.ExploreTests(TestFilter.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Tests must be loaded before exploring them."));
        }

        [Test]
        public void ExploreTests_FileNotFound_ReturnsZeroTests()
        {
            _runner.Load(MissingFile, EmptySettings);
            var explorer = _runner.ExploreTests(TestFilter.Empty);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(0));
        }

        [Test]
        public void ExploreTests_BadFile_ReturnsZeroTests()
        {
            _runner.Load(BadFile, EmptySettings);
            var explorer = _runner.ExploreTests(TestFilter.Empty);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(0));
        }

        [Test]
        public void ExploreTests_AfterLoad_ReturnsCorrectCount()
        {
            LoadMockAssembly();
            var explorer = _runner.ExploreTests(TestFilter.Empty);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void ExploreTest_AfterLoad_ReturnsSameTestCount()
        {
            LoadMockAssembly();
            var explorer = _runner.ExploreTests(TestFilter.Empty);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(_runner.CountTestCases(TestFilter.Empty)));
        }

        [Test]
        public void ExploreTest_AfterLoad_AllIdsAreUnique()
        {
            LoadMockAssembly();
            var explorer = _runner.ExploreTests(TestFilter.Empty);

            var dict = new Dictionary<string, bool>();
            CheckForDuplicates(explorer, dict);
        }

        private void CheckForDuplicates(ITest test, Dictionary<string, bool> dict)
        {
            Assert.That(dict.ContainsKey(test.Id), Is.False, $"Duplicate key: {test.Id}");
            dict.Add(test.Id, true);

            foreach (var child in test.Tests)
                CheckForDuplicates(child, dict);
        }

        [Test]
        public void ExploreTests_AfterLoad_WithFilter_ReturnCorrectCount()
        {
            LoadMockAssembly();
            ITestFilter filter = new CategoryFilter("FixtureCategory");

            var explorer = _runner.ExploreTests(filter);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(MockTestFixture.Tests));
        }

        [Test]
        public void ExploreTests_AfterLoad_WithFilter_ReturnSameTestCount()
        {
            LoadMockAssembly();
            ITestFilter filter = new CategoryFilter("FixtureCategory");

            var explorer = _runner.ExploreTests(filter);
            Assert.That(explorer.TestCaseCount, Is.EqualTo(_runner.CountTestCases(filter)));
        }

        [Test]
        public void ExploreTests_AfterLoad_WithFilter_TestSuitesRetainProperties()
        {
            LoadMockAssembly();
            ITestFilter filter = new CategoryFilter("FixtureCategory");

            var explorer = _runner.ExploreTests(filter);

            Assert.That(_runner.LoadedTest, Is.Not.Null);
            var runnerFixture = _runner.LoadedTest.Tests[0].Tests[0].Tests[0].Tests[0];
            var explorerFixture = explorer.Tests[0].Tests[0].Tests[0].Tests[0];

            Assert.Multiple(() =>
            {
                Assert.That(explorerFixture.Properties.Keys, Has.Count.EqualTo(runnerFixture.Properties.Keys.Count));
                Assert.That(explorerFixture.Properties.Get(PropertyNames.Category),
                    Is.EqualTo(runnerFixture.Properties.Get(PropertyNames.Category)));
                Assert.That(explorerFixture.Properties.Get(PropertyNames.Description),
                    Is.EqualTo(runnerFixture.Properties.Get(PropertyNames.Description)));
            });
        }
        #endregion

        #region Run

        [Test]
        public void Run_AfterLoad_ReturnsRunnableSuite()
        {
            LoadMockAssembly();
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(result.Test.IsSuite);
                Assert.That(result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(result.Test.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(result.Test.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
                Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(result.PassCount, Is.EqualTo(MockAssembly.Passed));
                Assert.That(result.FailCount, Is.EqualTo(MockAssembly.Failed));
                Assert.That(result.WarningCount, Is.EqualTo(MockAssembly.Warnings));
                Assert.That(result.SkipCount, Is.EqualTo(MockAssembly.Skipped));
                Assert.That(result.InconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
            });
        }

        [Test]
        public void Run_AfterLoad_SendsExpectedEvents()
        {
            LoadMockAssembly();
            _runner.Run(this, TestFilter.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(_suiteStartedCount, Is.EqualTo(MockAssembly.Suites));
                Assert.That(_suiteFinishedCount, Is.EqualTo(MockAssembly.Suites));
                Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.TestStartedEvents));
                Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.TestFinishedEvents));
                Assert.That(_testOutputCount, Is.EqualTo(MockAssembly.TestOutputEvents));

                Assert.That(_successCount, Is.EqualTo(MockAssembly.Passed));
                Assert.That(_failCount, Is.EqualTo(MockAssembly.Failed));
                Assert.That(_skipCount, Is.EqualTo(MockAssembly.Skipped));
                Assert.That(_inconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
            });
        }

        [Test]
        public void Run_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.Run(TestListener.NULL, TestFilter.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Tests must be loaded before running them."));
        }

        [Test, SetUICulture("en-US")]
        public void Run_FileNotFound_ReturnsNonRunnableSuite()
        {
            _runner.Load(MissingFile, EmptySettings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(result.Test.IsSuite);
                Assert.That(result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
                Assert.That(result.Message, Does.StartWith(CouldNotLoadMsg));
            });
        }

        [Test]
        public void RunTestsAction_WithInvalidFilterElement_ThrowsArgumentException()
        {
            LoadMockAssembly();

            var ex = Assert.Throws<ArgumentException>(() =>
                TestFilter.FromXml("<filter><invalidElement>foo</invalidElement></filter>"));

            Assert.That(ex?.Message, Does.StartWith(string.Format(InvalidFilterElementMessage, "invalidElement")));
        }

        [Test]
        public void Run_WithParameters()
        {
            var dict = new Dictionary<string, string>
            {
                { "X", "5" },
                { "Y", "7" }
            };

            var settings = new Dictionary<string, object> { { "TestParametersDictionary", dict } };
            LoadMockAssembly(settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);
            CheckParameterOutput(result);
        }

        [Test]
        public void Run_WithLegacyParameters()
        {
            var settings = new Dictionary<string, object> { { "TestParameters", "X=5;Y=7" } };
            LoadMockAssembly(settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);
            CheckParameterOutput(result);
        }

        [Test, SetUICulture("en-US")]
        public void Run_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BadFile, EmptySettings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(result.Test.IsSuite);
                Assert.That(result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
                Assert.That(result.Message, Does.StartWith(CouldNotLoadMsg));
            });
        }

        #endregion

        #region RunAsync

        [Test]
        public void RunAsync_AfterLoad_ReturnsRunnableSuite()
        {
            LoadMockAssembly();
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.That(_runner.Result, Is.Not.Null, "No result returned");
            Assert.Multiple(() =>
            {
                Assert.That(_runner.Result.Test.IsSuite);
                Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.ChildFailure));
                Assert.That(_runner.Result.PassCount, Is.EqualTo(MockAssembly.Passed));
                Assert.That(_runner.Result.FailCount, Is.EqualTo(MockAssembly.Failed));
                Assert.That(_runner.Result.SkipCount, Is.EqualTo(MockAssembly.Skipped));
                Assert.That(_runner.Result.InconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
            });
        }

        [Test]
        public void RunAsync_AfterLoad_SendsExpectedEvents()
        {
            LoadMockAssembly();
            _runner.RunAsync(this, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.Multiple(() =>
            {
                Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests));
                Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.Tests));

                Assert.That(_successCount, Is.EqualTo(MockAssembly.Passed));
                Assert.That(_failCount, Is.EqualTo(MockAssembly.Failed));
                Assert.That(_skipCount, Is.EqualTo(MockAssembly.Skipped));
                Assert.That(_inconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
            });
        }

        [Test]
        public void RunAsync_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.RunAsync(TestListener.NULL, TestFilter.Empty));
            Assert.That(ex?.Message, Is.EqualTo("Tests must be loaded before running them."));
        }

        [Test, SetUICulture("en-US")]
        public void RunAsync_FileNotFound_ReturnsNonRunnableSuite()
        {
            _runner.Load(MissingFile, EmptySettings);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.That(_runner.Result, Is.Not.Null, "No result returned");
            Assert.Multiple(() =>
            {
                Assert.That(_runner.Result.Test.IsSuite);
                Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
                Assert.That(_runner.Result.Message, Does.StartWith(CouldNotLoadMsg));
            });
        }

        [Test, SetUICulture("en-US")]
        public void RunAsync_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BadFile, EmptySettings);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.That(_runner.Result, Is.Not.Null, "No result returned");
            Assert.Multiple(() =>
            {
                Assert.That(_runner.Result.Test.IsSuite);
                Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
                Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
                Assert.That(_runner.Result.Message, Does.StartWith(CouldNotLoadMsg));
            });
        }

        #endregion

        #region StopRun

#if THREAD_ABORT // Can't stop run on platforms without ability to abort thread

        // Arbitrary delay for cancellation based on the time to run each case in SlowTests
        private const int CancelTestDelay = SlowTests.SINGLE_TEST_DELAY * 2;

        [Test]
        public void StopRun_WhenNoTestIsRunning_DoesNotThrow([Values] bool force)
        {
            Assert.DoesNotThrow(() => _runner.StopRun(force));
        }

        private static readonly TestCaseData[] StopRunCases = new TestCaseData[]
        {
            new TestCaseData(0, false).SetName("{m}(Simple dispatcher, cooperative stop)"),
            new TestCaseData(0, true).SetName("{m}(Simple dispatcher, forced stop)"),
            new TestCaseData(2, false).SetName("{m}(Parallel dispatcher, cooperative stop)"),
            new TestCaseData(2, true).SetName("{m}(Parallel dispatcher, forced stop)")
        };

        [TestCaseSource(nameof(StopRunCases))]
        public void StopRun_WhenTestIsRunning_StopsTest(int workers, bool force)
        {
            var tests = LoadSlowTests(workers);
            var count = tests.TestCaseCount;
            var stopType = force ? "forced stop" : "cooperative stop";

            _runner.RunAsync(this, TestFilter.Empty);

            // Ensure that at least one test started, otherwise we aren't testing anything!
            SpinWait.SpinUntil(() => _testStartedCount > 0, CancelTestDelay);
            
            _runner.StopRun(force);

            var completionWasSignaled = _runner.WaitForCompletion(CancelTestDelay);

            // Write out status for debugging
            TestContext.Out.WriteLine($"No of started suites  {_suiteStartedCount}:");
            TestContext.Out.WriteLine($"No of finished suites {_suiteFinishedCount}:");
            TestContext.Out.WriteLine($"No of started tests   {_testStartedCount}:");
            TestContext.Out.WriteLine($"No of finished tests  {_testFinishedCount}:");

            // Use Assert.Multiple so we can see everything that went wrong at one time
            Assert.Multiple(() =>
            {
                Assert.That(completionWasSignaled, Is.True, "Runner never signaled completion");
                Assert.That(_runner.IsTestComplete, Is.True, "Test is not recorded as complete");

                if (_activeTests.Count > 0)
                {
                    var sb = new StringBuilder("The following tests never terminated:" + Environment.NewLine);
                    foreach (var name in _activeTests.Keys)
                        sb.AppendLine($" * {name}");
                    Assert.Fail(sb.ToString());
                }

                Assert.That(_suiteStartedCount, Is.GreaterThan(0), "No suites started");
                Assert.That(_testStartedCount, Is.GreaterThan(0), "No test cases started");
                Assert.That(_suiteFinishedCount, Is.EqualTo(_suiteStartedCount), $"Not all suites terminated after {stopType}");
                Assert.That(_testFinishedCount, Is.EqualTo(_testStartedCount), $"Not all test cases terminated after {stopType}");
            });

            Assert.That(_runner.Result, Is.Not.Null, "No result returned.");
            Assert.Multiple(() =>
            {
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.Cancelled), $"Invalid ResultState after {stopType}");
                Assert.That(_runner.Result.PassCount, Is.LessThan(count), $"All tests passed in spite of {stopType}");
            });
        }
#endif

        #endregion

        #region ITestListener Implementation

        void ITestListener.TestStarted(ITest test)
        {
            _activeTests.Add(test.Name, true);
            TestContext.Out.WriteLine($"Added {(test.IsSuite ? "Suite" : "Test")} {test.Name} {test.FullName}");
            if (test.IsSuite)
                _suiteStartedCount++;
            else
                _testStartedCount++;
        }

        void ITestListener.TestFinished(ITestResult result)
        {
            _activeTests.Remove(result.Test.Name);
            TestContext.Out.WriteLine($"Removed {(result.Test.IsSuite ? "Suite" : "Test")} {result.Test.Name} {result.Test.FullName}");
            if (result.Test.IsSuite)
            {
                _suiteFinishedCount++;
            }
            else
            {
                _testFinishedCount++;

                switch (result.ResultState.Status)
                {
                    case TestStatus.Passed:
                        _successCount++;
                        break;
                    case TestStatus.Failed:
                        _failCount++;
                        break;
                    case TestStatus.Skipped:
                        _skipCount++;
                        break;
                    case TestStatus.Inconclusive:
                        _inconclusiveCount++;
                        break;
                }
            }
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        void ITestListener.TestOutput(TestOutput output)
        {
            _testOutputCount++;
        }

        /// <summary>
        /// Called when a test produces message to be sent to listeners
        /// </summary>
        /// <param name="message">A TestMessage object containing the text to send</param>
        void ITestListener.SendMessage(TestMessage message)
        {
        }

        #endregion

        #region Helper Methods

        private ITest LoadMockAssembly()
        {
            return LoadMockAssembly(EmptySettings);
        }

        private ITest LoadMockAssembly(IDictionary<string, object> settings)
        {
            return _runner.Load(
                Path.Combine(TestContext.CurrentContext.TestDirectory, MockAssemblyFile),
                settings);
        }

        private ITest LoadSlowTests(int workers)
        {
            var settings = new Dictionary<string, object> { { FrameworkPackageSettings.NumberOfTestWorkers, workers } };

            return _runner.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, SlowTestsFile), settings);
        }

        private void CheckParameterOutput(ITestResult result)
        {
            var childResult = TestFinder.Find(
                "DisplayRunParameters", result, true);

            Assert.That(childResult, Is.Not.Null);
            Assert.That(childResult!.Output, Is.EqualTo(
                "Parameter X = 5" + Environment.NewLine +
                "Parameter Y = 7" + Environment.NewLine));
        }

        #endregion
    }
}
