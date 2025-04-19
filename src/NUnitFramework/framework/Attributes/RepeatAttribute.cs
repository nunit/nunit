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
            return new RepeatedTestCommand(command, _count, StopOnFailure);
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

            /// <summary>
            /// Initializes a new instance of the <see cref="RepeatedTestCommand"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="repeatCount">The number of repetitions</param>
            /// <param name="stopOnFailure">Whether to stop when a test is not successful or not</param>
            public RepeatedTestCommand(TestCommand innerCommand, int repeatCount, bool stopOnFailure)
                : base(innerCommand)
            {
                _repeatCount = repeatCount;
                _stopOnFailure = stopOnFailure;
            }

            /// <summary>
            /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            public override TestResult Execute(TestExecutionContext context)
            {
                int count = _repeatCount;

                while (count-- > 0)
                {
                    try
                    {
                        context.CurrentResult = innerCommand.Execute(context);
                    }
                    // Commands are supposed to catch exceptions, but some don't
                    // and we want to look at restructuring the API in the future.
                    catch (Exception ex)
                    {
                        if (context.CurrentResult is null)
                            context.CurrentResult = context.CurrentTest.MakeTestResult();
                        context.CurrentResult.RecordException(ex);
                    }

                    if (_stopOnFailure && context.CurrentResult.ResultState != ResultState.Success)
                        break;

                    // Clear result for repeat
                    if (count > 0)
                    {
                        context.CurrentResult = context.CurrentTest.MakeTestResult();
                        context.CurrentRepeatCount++; // increment Retry count for next iteration. will only happen if we are guaranteed another iteration
                    }
                }

                return context.CurrentResult;
            }
        }
        #endregion
    }
}
