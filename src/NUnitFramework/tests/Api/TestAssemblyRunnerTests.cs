// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Tests;
using NUnit.Tests.Assemblies;
using NUnit.TestUtilities;

namespace NUnit.Framework.Api
{
    // Functional tests of the TestAssemblyRunner and all subordinate classes
    public class TestAssemblyRunnerTests : ITestListener
    {
#if NETSTANDARD1_6
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.dll";
        private const string COULD_NOT_LOAD_MSG = "The system cannot find the file specified.";
#else
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.exe";
        private const string COULD_NOT_LOAD_MSG = "Could not load";
#endif
        private const string BAD_FILE = "mock-assembly.pdb";
        private const string SLOW_TESTS_FILE = "slow-nunit-tests.dll";
        private const string MISSING_FILE = "junk.dll";

#if PORTABLE
        private static readonly string MOCK_ASSEMBLY_NAME = typeof(MockAssembly).GetTypeInfo().Assembly.FullName;
#endif
        private const string INVALID_FILTER_ELEMENT_MESSAGE = "Invalid filter element: {0}";

        private static readonly IDictionary<string, object> EMPTY_SETTINGS = new Dictionary<string, object>();

        private ITestAssemblyRunner _runner;

        private int _testStartedCount;
        private int _testFinishedCount;
        private int _testOutputCount;
        private int _successCount;
        private int _failCount;
        private int _skipCount;
        private int _inconclusiveCount;

        [SetUp]
        public void CreateRunner()
        {
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            _testStartedCount = 0;
            _testFinishedCount = 0;
            _testOutputCount = 0;
            _successCount = 0;
            _failCount = 0;
            _skipCount = 0;
            _inconclusiveCount = 0;
        }

        #region Load

        [Test]
        public void Load_GoodFile_ReturnsRunnableSuite()
        {
            var result = LoadMockAssembly();

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
#if PORTABLE
            Assert.That(result.Name, Is.EqualTo(MOCK_ASSEMBLY_NAME));
#else
            Assert.That(result.Name, Is.EqualTo(MOCK_ASSEMBLY_FILE));
#endif
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.Runnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void Load_FileNotFound_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(MISSING_FILE, EMPTY_SETTINGS);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(MISSING_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.NotRunnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.Properties.Get(PropertyNames.SkipReason),
                Does.StartWith(COULD_NOT_LOAD_MSG));
        }

        [Test]
        public void Load_BadFile_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(BAD_FILE, EMPTY_SETTINGS);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(BAD_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.NotRunnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.Properties.Get(PropertyNames.SkipReason),
                Does.StartWith("Could not load").And.Contains(BAD_FILE));
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
        public void CountTestCases_WithoutLoad_ThrowsInvalidOperation()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.CountTestCases(TestFilter.Empty));
            Assert.That(ex.Message, Is.EqualTo("The CountTestCases method was called but no test has been loaded"));
        }

        [Test]
        public void CountTestCases_FileNotFound_ReturnsZero()
        {
            _runner.Load(MISSING_FILE, EMPTY_SETTINGS);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        [Test]
        public void CountTestCases_BadFile_ReturnsZero()
        {
            _runner.Load(BAD_FILE, EMPTY_SETTINGS);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        #endregion

        #region Run

        [Test]
        public void Run_AfterLoad_ReturnsRunnableSuite()
        {
            LoadMockAssembly();
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

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
        }

        [Test]
        public void Run_AfterLoad_SendsExpectedEvents()
        {
            LoadMockAssembly();
            _runner.Run(this, TestFilter.Empty);

            Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.TestStartedEvents));
            Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.TestFinishedEvents));
#if !PORTABLE
            Assert.That(_testOutputCount, Is.EqualTo(MockAssembly.TestOutputEvents));
#endif

            Assert.That(_successCount, Is.EqualTo(MockAssembly.Passed));
            Assert.That(_failCount, Is.EqualTo(MockAssembly.Failed));
            Assert.That(_skipCount, Is.EqualTo(MockAssembly.Skipped));
            Assert.That(_inconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
        }

        [Test]
        public void Run_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.Run(TestListener.NULL, TestFilter.Empty));
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

        [Test]
        public void Run_FileNotFound_ReturnsNonRunnableSuite()
        {
            _runner.Load(MISSING_FILE, EMPTY_SETTINGS);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.That(result.Test.IsSuite);
            Assert.That(result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
            Assert.That(result.Message,
                Does.StartWith(COULD_NOT_LOAD_MSG));
        }

        [Test]
        public void RunTestsAction_WithInvalidFilterElement_ThrowsArgumentException()
        {
            LoadMockAssembly();

            var ex = Assert.Catch(() =>
                {
                    var invalidFilter = TestFilter.FromXml("<filter><invalidElement>foo</invalidElement></filter>");
                    _runner.Run(this, invalidFilter);
                });

            Assert.That(ex, Is.TypeOf<ArgumentException>());
            Assert.That(ex.Message, Does.StartWith(string.Format(INVALID_FILTER_ELEMENT_MESSAGE, "invalidElement")));
        }

#if !PORTABLE
        [Test]
        public void Run_WithParameters()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("X", "5");
            dict.Add("Y", "7");

            var settings = new Dictionary<string, object>();
            settings.Add("TestParametersDictionary", dict);
            LoadMockAssembly(settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);
            CheckParameterOutput(result);
        }

        [Test]
        public void Run_WithLegacyParameters()
        {
            var settings = new Dictionary<string, object>();
            settings.Add("TestParameters", "X=5;Y=7");
            LoadMockAssembly(settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);
            CheckParameterOutput(result);
        }
#endif

        [Test]
        public void Run_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BAD_FILE, EMPTY_SETTINGS);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.That(result.Test.IsSuite);
            Assert.That(result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
            Assert.That(result.Message,
                Does.StartWith("Could not load"));
        }

        #endregion

        #region RunAsync

        [Test]
        public void RunAsync_AfterLoad_ReturnsRunnableSuite()
        {
            LoadMockAssembly();
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.ChildFailure));
            Assert.That(_runner.Result.PassCount, Is.EqualTo(MockAssembly.Passed));
            Assert.That(_runner.Result.FailCount, Is.EqualTo(MockAssembly.Failed));
            Assert.That(_runner.Result.SkipCount, Is.EqualTo(MockAssembly.Skipped));
            Assert.That(_runner.Result.InconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
        }

        [Test]
        public void RunAsync_AfterLoad_SendsExpectedEvents()
        {
            LoadMockAssembly();
            _runner.RunAsync(this, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests));
            Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.Tests));

            Assert.That(_successCount, Is.EqualTo(MockAssembly.Passed));
            Assert.That(_failCount, Is.EqualTo(MockAssembly.Failed));
            Assert.That(_skipCount, Is.EqualTo(MockAssembly.Skipped));
            Assert.That(_inconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
        }

        [Test]
        public void RunAsync_WithoutLoad_ReturnsError()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                    () => _runner.RunAsync(TestListener.NULL, TestFilter.Empty));
            Assert.That(ex.Message, Is.EqualTo("The Run method was called but no test has been loaded"));
        }

        [Test]
        public void RunAsync_FileNotFound_ReturnsNonRunnableSuite()
        {
            _runner.Load(MISSING_FILE, EMPTY_SETTINGS);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
            Assert.That(_runner.Result.Message,
                Does.StartWith(COULD_NOT_LOAD_MSG));
        }

        [Test]
        public void RunAsync_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BAD_FILE, EMPTY_SETTINGS);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
            Assert.That(_runner.Result.Message,
                Does.StartWith("Could not load"));
        }

        #endregion

        #region StopRun

        [Test]
        public void StopRun_WhenNoTestIsRunning_Succeeds()
        {
            _runner.StopRun(false);
        }

        [Test]
        public void StopRun_WhenTestIsRunning_StopsTest()
        {
            var tests = LoadSlowTests();
            var count = tests.TestCaseCount;
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.StopRun(false);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.True(_runner.IsTestComplete, "Test is not complete");

            if (_runner.Result.ResultState != ResultState.Success) // Test may have finished before we stopped it
            {
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.Cancelled));
                Assert.That(_runner.Result.PassCount, Is.LessThan(count));
            }
        }

        #endregion

        #region Cancel Run

        [Test]
        public void CancelRun_WhenNoTestIsRunning_Succeeds()
        {
            _runner.StopRun(true);
        }

        [Test]
        public void CancelRun_WhenTestIsRunning_StopsTest()
        {
            var tests = LoadSlowTests();
            var count = tests.TestCaseCount;
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.StopRun(true);

            // When cancelling, the completion event may not be signalled,
            // so we only wait a short time before checking.
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.True(_runner.IsTestComplete, "Test is not complete");

            if (_runner.Result.ResultState != ResultState.Success)
            {
                Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.Cancelled));
                Assert.That(_runner.Result.PassCount, Is.LessThan(count));
            }
        }

        #endregion

        #region ITestListener Implementation

        void ITestListener.TestStarted(ITest test)
        {
            if (!test.IsSuite)
                _testStartedCount++;
        }

        void ITestListener.TestFinished(ITestResult result)
        {
            if (!result.Test.IsSuite)
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
        public void TestOutput(TestOutput output)
        {
            _testOutputCount++;
        }

        #endregion

        #region Helper Methods

        private ITest LoadMockAssembly()
        {
            return LoadMockAssembly(EMPTY_SETTINGS);
        }

        private ITest LoadMockAssembly(IDictionary<string, object> settings)
        {
#if PORTABLE
            return _runner.Load(
                typeof(MockAssembly).GetTypeInfo().Assembly, 
                settings);
#else
            return _runner.Load(
                Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY_FILE),
                settings);
#endif
        }

        private ITest LoadSlowTests()
        {
#if PORTABLE
            return _runner.Load(typeof(SlowTests).GetTypeInfo().Assembly, EMPTY_SETTINGS);
#else
            return _runner.Load(Path.Combine(TestContext.CurrentContext.TestDirectory, SLOW_TESTS_FILE), EMPTY_SETTINGS);
#endif
        }

        private void CheckParameterOutput(ITestResult result)
        {
            var childResult = TestFinder.Find(
                "DisplayRunParameters", result, true);

            Assert.That(childResult.Output, Is.EqualTo(
                "Parameter X = 5" + Environment.NewLine +
                "Parameter Y = 7" + Environment.NewLine));
        }

        #endregion
    }
}
