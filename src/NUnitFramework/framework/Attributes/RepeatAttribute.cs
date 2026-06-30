// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// TODO: Rework this
// RepeatAttribute should either
//  1) Apply at load time to create the exact number of tests, or
//  2) Apply at run time, generating tests or results dynamically
//
// #1 is feasible but doesn't provide much benefit
// #2 requires infrastructure for dynamic test cases first

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework
{
    /// <summary>
    /// Specifies that a test should be run multiple times.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RepeatAttribute : PropertyAttribute, IRepeatTest
    {
        private readonly int _count;
        /// <summary>
        /// Whether to stop when a test is not successful or not
        /// </summary>
        public bool StopOnFailure { get; set; }

        /// <summary>
        /// The minimum percentage of runs that must pass for the test to succeed.
        /// Valid range is 1–100. Default is 100 (all runs must pass).
        /// When set below 100, <see cref="StopOnFailure"/> is ignored and all runs always execute.
        /// </summary>
        public int RequiredPassPercentage { get; set; } = 100;

        /// <summary>
        /// Construct a RepeatAttribute
        /// </summary>
        /// <param name="count">The number of times to run the test</param>
        public RepeatAttribute(int count) : this(count, true)
        {
        }

        /// <summary>
        /// Construct a RepeatAttribute
        /// </summary>
        /// <param name="count">The number of times to run the test</param>
        /// <param name="stopOnFailure">Whether to stop when a test is not successful or not</param>
        public RepeatAttribute(int count, bool stopOnFailure) : base(count)
        {
            _count = count;
            StopOnFailure = stopOnFailure;
        }

        #region IRepeatTest Members

        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        public TestCommand Wrap(TestCommand command)
        {
            return new RepeatedTestCommand(command, _count, StopOnFailure, RequiredPassPercentage);
        }

        #endregion

        #region Nested RepeatedTestCommand Class

        /// <summary>
        /// The test command for the RepeatAttribute
        /// </summary>
        public class RepeatedTestCommand : DelegatingTestCommand
        {
            private readonly int _repeatCount;
            private readonly bool _stopOnFailure;
            private readonly int _requiredPassPercentage;

            /// <summary>
            /// Initializes a new instance of the <see cref="RepeatedTestCommand"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="repeatCount">The number of repetitions</param>
            /// <param name="stopOnFailure">Whether to stop when a test is not successful or not</param>
            public RepeatedTestCommand(TestCommand innerCommand, int repeatCount, bool stopOnFailure)
                : this(innerCommand, repeatCount, stopOnFailure, 100)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="RepeatedTestCommand"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="repeatCount">The number of repetitions</param>
            /// <param name="stopOnFailure">Whether to stop when a test is not successful or not</param>
            /// <param name="requiredPassPercentage">Minimum percentage of runs that must pass (1–100)</param>
            public RepeatedTestCommand(TestCommand innerCommand, int repeatCount, bool stopOnFailure, int requiredPassPercentage)
                : base(innerCommand)
            {
                if (repeatCount < 1)
                    throw new ArgumentOutOfRangeException(nameof(repeatCount), repeatCount, "Must be at least 1.");

                if (requiredPassPercentage is < 1 or > 100)
                    throw new ArgumentOutOfRangeException(nameof(requiredPassPercentage), requiredPassPercentage, "Must be between 1 and 100.");

                _repeatCount = repeatCount;
                // When a pass threshold is set, all iterations must run regardless of StopOnFailure.
                _stopOnFailure = requiredPassPercentage == 100 && stopOnFailure;
                _requiredPassPercentage = requiredPassPercentage;
            }

            /// <summary>
            /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            public override TestResult Execute(TestExecutionContext context)
            {
                return _requiredPassPercentage < 100
                    ? ExecuteWithThreshold(context)
                    : ExecuteStandard(context);
            }

            private TestResult ExecuteStandard(TestExecutionContext context)
            {
                (TestResult overallResult, _) = RunIterations(context, (overall, iterationResult, count) =>
                {
                    if (iterationResult.ResultState != ResultState.Success || count == 0)
                        overall.SetResult(iterationResult.ResultState);

                    return iterationResult.ResultState != ResultState.Success && _stopOnFailure;
                });

                if (overallResult.AssertionResultCount > 0)
                    overallResult.RecordTestCompletion();
                context.CurrentResult = overallResult;

                return context.CurrentResult;
            }

            private TestResult ExecuteWithThreshold(TestExecutionContext context)
            {
                (TestResult overallResult, int successCount) = RunIterations(context, (_, _, _) => false);

                int passPercent = successCount * 100 / _repeatCount;

                if (passPercent >= _requiredPassPercentage)
                {
                    overallResult.SetResult(ResultState.Success);
                    if (successCount < _repeatCount)
                        overallResult.OutWriter.WriteLine($"{successCount} of {_repeatCount} runs passed ({passPercent}%), meeting the required {_requiredPassPercentage}% pass threshold.");
                }
                else
                {
                    overallResult.SetResult(ResultState.Failure,
                        $"{successCount} of {_repeatCount} runs passed ({passPercent}%), below the required {_requiredPassPercentage}% pass threshold.");
                }

                context.CurrentResult = overallResult;
                return context.CurrentResult;
            }

            // processIteration(overallResult, iterationResult, count) returns true to stop early.
            private (TestResult overallResult, int successCount) RunIterations(
                TestExecutionContext context, Func<TestResult, TestResult, int, bool> processIteration)
            {
                TestResult overallResult = context.CurrentTest.MakeTestResult();
                int successCount = 0;

                for (int count = 0; count < _repeatCount; count++)
                {
                    TestResult iterationResult = ExecuteOneIteration(context, count);

                    overallResult.AssertCount += iterationResult.AssertCount;
                    overallResult.OutWriter.Write(iterationResult.Output);

                    if (iterationResult.ResultState != ResultState.Success)
                    {
                        foreach (var assertionResult in iterationResult.AssertionResults)
                            overallResult.RecordAssertion(assertionResult);
                    }
                    else
                    {
                        successCount++;
                    }

                    if (processIteration(overallResult, iterationResult, count))
                        break;
                }

                return (overallResult, successCount);
            }

            private TestResult ExecuteOneIteration(TestExecutionContext context, int count)
            {
                if (count != 0)
                    context.CurrentRepeatCount++; // increment Retry count for next iteration. will only happen if we are guaranteed another iteration

                context.CurrentResult = context.CurrentTest.MakeTestResult();

                try
                {
                    context.CurrentResult = innerCommand.Execute(context);
                }
                // Commands are supposed to catch exceptions, but some don't
                // and we want to look at restructuring the API in the future.
                catch (Exception ex)
                {
                    context.CurrentResult.RecordException(ex);
                }

                return context.CurrentResult;
            }
        }
        #endregion
    }
}
