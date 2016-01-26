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

// TODO: Get to work in Portable and Silverlight - will require building mock-assembly
#if !SILVERLIGHT && !PORTABLE

using System;
using System.Collections;
using System.IO;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;
using NUnit.Tests;
using NUnit.Tests.Assemblies;

namespace NUnit.Framework.Api
{
    // Functional tests of the TestAssemblyRunner and all subordinate classes
    public class TestAssemblyRunnerTests : ITestListener
    {
        private const string MOCK_ASSEMBLY = "mock-assembly.exe";
        private const string BAD_FILE = "mock-assembly.pdb";
        private const string SLOW_TESTS = "slow-nunit-tests.dll";
        private const string MISSING_FILE = "junk.dll";

        private IDictionary _settings = new Hashtable();
        private ITestAssemblyRunner _runner;
        private string _mockAssemblyPath;
        private string _slowTestsPath;

        private int _testStartedCount;
        private int _testFinishedCount;
        private int _successCount;
        private int _failCount;
        private int _skipCount;
        private int _inconclusiveCount;

        [SetUp]
        public void CreateRunner()
        {
            _mockAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY);
            _slowTestsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, SLOW_TESTS);
            _runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            _testStartedCount = 0;
            _testFinishedCount = 0;
            _successCount = 0;
            _failCount = 0;
            _skipCount = 0;
            _inconclusiveCount = 0;
        }

        #region Load
        [Test]
        public void Load_GoodFile_ReturnsRunnableSuite()
        {
            var result = _runner.Load(_mockAssemblyPath, _settings);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(MOCK_ASSEMBLY));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.Runnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
        }

        [Test]
        public void Load_FileNotFound_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(MISSING_FILE, _settings);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(MISSING_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.NotRunnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
#if NETCF
            Assert.That(result.Properties.Get(PropertyNames.SkipReason), Does.StartWith("File or assembly name").And.Contains(MISSING_FILE));
#else
            Assert.That(result.Properties.Get(PropertyNames.SkipReason), Does.StartWith("Could not load").And.Contains(MISSING_FILE));
#endif
        }

        [Test]
        public void Load_BadFile_ReturnsNonRunnableSuite()
        {
            var result = _runner.Load(BAD_FILE, _settings);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(BAD_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.NotRunnable));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
#if NETCF
            Assert.That(result.Properties.Get(PropertyNames.SkipReason), Does.StartWith("File or assembly name").And.Contains(BAD_FILE));
#else
            Assert.That(result.Properties.Get(PropertyNames.SkipReason), Does.StartWith("Could not load").And.Contains(BAD_FILE));
#endif
        }
        #endregion

        #region CountTestCases
        [Test]
        public void CountTestCases_AfterLoad_ReturnsCorrectCount()
        {
            _runner.Load(_mockAssemblyPath, _settings);
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
            _runner.Load(MISSING_FILE, _settings);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }

        [Test]
        public void CountTestCases_BadFile_ReturnsZero()
        {
            _runner.Load(BAD_FILE, _settings);
            Assert.That(_runner.CountTestCases(TestFilter.Empty), Is.EqualTo(0));
        }
        #endregion

        #region Run
        [Test]
        public void Run_AfterLoad_ReturnsRunnableSuite()
        {
            _runner.Load(_mockAssemblyPath, _settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.That(result.Test.IsSuite);
            Assert.That(result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(result.Test.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(result.Test.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.ChildFailure));
            Assert.That(result.PassCount, Is.EqualTo(MockAssembly.Success));
            Assert.That(result.FailCount, Is.EqualTo(MockAssembly.ErrorsAndFailures));
            Assert.That(result.SkipCount, Is.EqualTo(MockAssembly.Skipped));
            Assert.That(result.InconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
        }

        [Test]
        public void Run_AfterLoad_SendsExpectedEvents()
        {
            _runner.Load(_mockAssemblyPath, _settings);
            var result = _runner.Run(this, TestFilter.Empty);

            Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests));
            Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.Tests));

            Assert.That(_successCount, Is.EqualTo(MockAssembly.Success));
            Assert.That(_failCount, Is.EqualTo(MockAssembly.ErrorsAndFailures));
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
            _runner.Load(MISSING_FILE, _settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.That(result.Test.IsSuite);
            Assert.That(result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
#if NETCF
            Assert.That(result.Message, Does.StartWith("File or assembly name").And.Contains(MISSING_FILE));
#else
            Assert.That(result.Message, Does.StartWith("Could not load").And.Contains(MISSING_FILE));
#endif
        }

        [Test]
        public void Run_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BAD_FILE, _settings);
            var result = _runner.Run(TestListener.NULL, TestFilter.Empty);

            Assert.That(result.Test.IsSuite);
            Assert.That(result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
#if NETCF
            Assert.That(result.Message, Does.StartWith("File or assembly name").And.Contains(BAD_FILE));
#else
            Assert.That(result.Message, Does.StartWith("Could not load").And.Contains(BAD_FILE));
#endif
        }
        #endregion

        #region RunAsync
        [Test]
        public void RunAsync_AfterLoad_ReturnsRunnableSuite()
        {
            _runner.Load(_mockAssemblyPath, _settings);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.Runnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(MockAssembly.Tests));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.ChildFailure));
            Assert.That(_runner.Result.PassCount, Is.EqualTo(MockAssembly.Success));
            Assert.That(_runner.Result.FailCount, Is.EqualTo(MockAssembly.ErrorsAndFailures));
            Assert.That(_runner.Result.SkipCount, Is.EqualTo(MockAssembly.Skipped));
            Assert.That(_runner.Result.InconclusiveCount, Is.EqualTo(MockAssembly.Inconclusive));
        }

        [Test]
        public void RunAsync_AfterLoad_SendsExpectedEvents()
        {
            _runner.Load(_mockAssemblyPath, _settings);
            _runner.RunAsync(this, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.That(_testStartedCount, Is.EqualTo(MockAssembly.Tests - IgnoredFixture.Tests - BadFixture.Tests - ExplicitFixture.Tests));
            Assert.That(_testFinishedCount, Is.EqualTo(MockAssembly.Tests));

            Assert.That(_successCount, Is.EqualTo(MockAssembly.Success));
            Assert.That(_failCount, Is.EqualTo(MockAssembly.ErrorsAndFailures));
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
            _runner.Load(MISSING_FILE, _settings);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
#if NETCF
            Assert.That(_runner.Result.Message, Does.StartWith("File or assembly name").And.Contains(MISSING_FILE));
#else
            Assert.That(_runner.Result.Message, Does.StartWith("Could not load").And.Contains(MISSING_FILE));
#endif
        }

        [Test]
        public void RunAsync_BadFile_ReturnsNonRunnableSuite()
        {
            _runner.Load(BAD_FILE, _settings);
            _runner.RunAsync(TestListener.NULL, TestFilter.Empty);
            _runner.WaitForCompletion(Timeout.Infinite);

            Assert.NotNull(_runner.Result, "No result returned");
            Assert.That(_runner.Result.Test.IsSuite);
            Assert.That(_runner.Result.Test, Is.TypeOf<TestAssembly>());
            Assert.That(_runner.Result.Test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(_runner.Result.Test.TestCaseCount, Is.EqualTo(0));
            Assert.That(_runner.Result.ResultState, Is.EqualTo(ResultState.NotRunnable.WithSite(FailureSite.SetUp)));
#if NETCF
            Assert.That(_runner.Result.Message, Does.StartWith("File or assembly name").And.Contains(BAD_FILE));
#else
            Assert.That(_runner.Result.Message, Does.StartWith("Could not load").And.Contains(BAD_FILE));
#endif
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
            var tests = _runner.Load(_slowTestsPath, _settings);
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
            var tests = _runner.Load(_slowTestsPath, _settings);
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

        #endregion
    }
}
#endif
