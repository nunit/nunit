// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    [TestFixture]
    public class CalculateDeltaWithPreviousTests
    {
        private TestCaseResult _previousResult;
        private TestCaseResult _currentResult;

        private TestSuiteResult _previousSuiteResult;
        private TestSuiteResult _currentSuiteResult;

        [SetUp]
        public void SetUp()
        {
            var test = new TestMethod(new MethodWrapper(typeof(CalculateDeltaWithPreviousTests), nameof(SetUp)));
            _previousResult = (TestCaseResult)test.MakeTestResult();
            _currentResult = (TestCaseResult)test.MakeTestResult();

            _previousSuiteResult = new TestSuiteResult(new TestSuite("PreviousSuite"));
            _currentSuiteResult = new TestSuiteResult(new TestSuite("CurrentSuite"));
        }

        [Test]
        public void CalculateDeltaWithPrevious_ResultStateUnchanged_ReturnsSuccessState()
        {
            _previousResult.SetResult(ResultState.Success);
            _currentResult.SetResult(ResultState.Success);

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.That(delta.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(delta.ResultState.Label, Is.Empty);
        }

        [Test]
        public void CalculateDeltaWithPrevious_ResultStateChanged_ReturnsNewState()
        {
            _previousResult.SetResult(ResultState.Success);
            _currentResult.SetResult(ResultState.Failure, "Test failed");

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(delta.ResultState.Label, Is.Empty);
                Assert.That(delta.Message, Is.EqualTo("Test failed"));
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_EnsureStatesHaveNotChanged_AlarmOnChange()
        {
            List<TestStatus> allStates =
                Enum.GetValues(typeof(TestStatus))
                .Cast<TestStatus>()
                .ToList();

            Assert.That(allStates, Has.Count.EqualTo(5),
                "TestStatus enum seems to have unexpected number of values." +
                "Please ensure that this change is reflected in the test cases: " +
                $"{nameof(CalculateDeltaWithPrevious_DifferentResultStates_ReturnsCurrentState)} and " +
                $"{nameof(CalculateDeltaWithPrevious_SameResultStates_ReturnsPassed)}.");
        }

        [Test]
        public void CalculateDeltaWithPrevious_DifferentResultStates_ReturnsCurrentState()
        {
            List<TestStatus> allStates =
                Enum.GetValues(typeof(TestStatus))
                .Cast<TestStatus>()
                .ToList();

            foreach (var previousState in allStates)
            {
                foreach (var currentState in allStates)
                {
                    if (previousState != currentState)
                    {
                        _previousResult.SetResult(new ResultState(previousState));
                        _currentResult.SetResult(new ResultState(currentState));
                        var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

                        Assert.Multiple(() =>
                        {
                            Assert.That(delta.ResultState.Status, Is.EqualTo(currentState));
                            Assert.That(delta.ResultState.Label, Is.EqualTo(_currentResult.ResultState.Label));
                        });
                    }
                }
            }
        }

        [Test]
        public void CalculateDeltaWithPrevious_SameResultStates_ReturnsPassed()
        {
            List<TestStatus> allStates =
                Enum.GetValues(typeof(TestStatus))
                .Cast<TestStatus>()
                .ToList();

            foreach (var state in allStates)
            {
                _previousResult.SetResult(new ResultState(state));
                _currentResult.SetResult(new ResultState(state));
                var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

                Assert.Multiple(() =>
                {
                    Assert.That(delta.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                    Assert.That(delta.ResultState.Label, Is.Empty);
                });
            }
        }

        [Test]
        public void CalculateDeltaWithPrevious_AssertCountChanged_ReturnsDelta()
        {
            _previousResult.AssertCount = 2;
            _currentResult.AssertCount = 5;

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.AssertCount, Is.EqualTo(3), "Should show only the new assertions");
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_OutputIncreased_ReturnsIncreasedContent()
        {
            _previousResult.OutWriter.Write("Previous output");
            _currentResult.OutWriter.Write("Previous output");
            _currentResult.OutWriter.Write("New output");

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.Output, Is.EqualTo("Previous output" + "New output"));
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_OutputUnchanged_ReturnsOldOutput()
        {
            _previousResult.OutWriter.Write("Previous output");
            _currentResult.OutWriter.Write("Previous output");

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.Output, Is.EqualTo("Previous output"));
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_AssertionResultsChanged_ReturnsOnlyNewAssertions()
        {
            var assertion1 = new AssertionResult(AssertionStatus.Passed, "First assertion", null);
            var assertion2 = new AssertionResult(AssertionStatus.Failed, "Second assertion", null);

            _previousResult.RecordAssertion(assertion1);
            _currentResult.RecordAssertion(assertion1);
            _currentResult.RecordAssertion(assertion2);

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.AssertionResults, Has.Count.EqualTo(1));
                Assert.That(delta.AssertionResults[0].Message, Is.EqualTo(assertion2.Message));
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_ExceptionContextProvided_RecordsException()
        {
            var exception = new InvalidOperationException("Test exception");

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult, exception);

            Assert.Multiple(() =>
            {
                Assert.That(delta.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(delta.ResultState.Label, Is.EqualTo(ResultState.Error.Label));
                Assert.That(delta.Message, Does.Contain(exception.Message));
            });
        }

        [Test]
        public void CalculateDeltaWithPrevious_WarningsAdded_UpdatesResultState()
        {
            _currentResult.RecordAssertion(AssertionStatus.Warning, "Warning message");

            var delta = _currentResult.CalculateDeltaWithPrevious(_previousResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.ResultState.Status, Is.EqualTo(TestStatus.Warning));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }

        private class FakeTestSuiteResult(
            int passedCount,
            int failedCount,
            int warningCount,
            int skipCount,
            int inconclusiveCount)
            : TestResult(new TestMethod(new MethodWrapper(typeof(CalculateDeltaWithPreviousTests), nameof(SetUp))))
        {
            public override int TotalCount => FailCount + WarningCount + PassCount + SkipCount + InconclusiveCount;
            public override int FailCount => failedCount;
            public override int WarningCount => warningCount;
            public override int PassCount => passedCount;
            public override int SkipCount => skipCount;
            public override int InconclusiveCount => inconclusiveCount;

            public override bool HasChildren => false;
            public override IEnumerable<ITestResult> Children => [];

            public override TestResult CalculateDeltaWithPrevious(TestResult previous, Exception? exception = null)
                => throw new NotImplementedException();

            public override TestResult Clone()
                => throw new NotImplementedException();
        }

        [Test]
        public void CalculateDeltaWithPrevious_SuiteResultDelta_ReturnsCorrectCounts()
        {
            _previousSuiteResult.SetResult(new ResultState(TestStatus.Passed));
            _previousSuiteResult.AddResult(new FakeTestSuiteResult(
                passedCount: 1,
                failedCount: 2,
                warningCount: 3,
                skipCount: 4,
                inconclusiveCount: 5));

            _currentSuiteResult.SetResult(new ResultState(TestStatus.Failed));
            _currentSuiteResult.AddResult(new FakeTestSuiteResult(
                passedCount: 10,
                failedCount: 10,
                warningCount: 10,
                skipCount: 10,
                inconclusiveCount: 10));

            var delta = _currentSuiteResult.CalculateDeltaWithPrevious(_previousSuiteResult);

            Assert.Multiple(() =>
            {
                Assert.That(delta.TotalCount, Is.EqualTo(35));
                Assert.That(delta.PassCount, Is.EqualTo(9));
                Assert.That(delta.FailCount, Is.EqualTo(8));
                Assert.That(delta.WarningCount, Is.EqualTo(7));
                Assert.That(delta.SkipCount, Is.EqualTo(6));
                Assert.That(delta.InconclusiveCount, Is.EqualTo(5));
                Assert.That(delta.ResultState, Is.EqualTo(ResultState.Failure));
                Assert.That(delta.ResultState.Label, Is.Empty);
            });
        }
    }
}
