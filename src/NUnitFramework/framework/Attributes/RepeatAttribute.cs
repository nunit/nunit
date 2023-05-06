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
        /// Construct a RepeatAttribute
        /// </summary>
        /// <param name="count">The number of times to run the test</param>
        public RepeatAttribute(int count) : base(count)
        {
            _count = count;
        }

        #region IRepeatTest Members

        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        public TestCommand Wrap(TestCommand command)
        {
            return new RepeatedTestCommand(command, _count);
        }

        #endregion

        #region Nested RepeatedTestCommand Class

        /// <summary>
        /// The test command for the RepeatAttribute
        /// </summary>
        public class RepeatedTestCommand : DelegatingTestCommand
        {
            private readonly int _repeatCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="RepeatedTestCommand"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="repeatCount">The number of repetitions</param>
            public RepeatedTestCommand(TestCommand innerCommand, int repeatCount)
                : base(innerCommand)
            {
                _repeatCount = repeatCount;
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
                    context.CurrentResult = innerCommand.Execute(context);

                    // TODO: We may want to change this so that all iterations are run
                    if (context.CurrentResult.ResultState != ResultState.Success)
                        break;

                    context.CurrentRepeatCount++;
                    context.CurrentTest.SetNextId();
                }

                return context.CurrentResult;
            }
        }

        #endregion
    }
}
