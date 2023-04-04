// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Specifies that a test method should be rerun until failure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RetryUntilFailureAttribute : NUnitAttribute, IRepeatTest
    {

        #region IRepeatTest Members

        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        public TestCommand Wrap(TestCommand command)
        {
            return new RetryUntilFailure(command);
        }

        #endregion

        #region Nested RetryUntilFailure Class

        /// <summary>
        /// The test command for the <see cref="RetryUntilFailureAttribute"/>
        /// </summary>
        public class RetryUntilFailure : DelegatingTestCommand
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RetryUntilFailure"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            public RetryUntilFailure(TestCommand innerCommand) : base(innerCommand) { }

            /// <summary>
            /// Runs the test, saving failure in the supplied TestExecutionContext.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            public override TestResult Execute(TestExecutionContext context)
            {
                while (true)
                {
                    try
                    {
                        innerCommand.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        context.CurrentResult.RecordException(ex);
                        return context.CurrentResult;
                    }

                    if (context.CurrentResult.ResultState == ResultState.Failure)
                    {
                        return context.CurrentResult;
                    }
                }
            }
        }

        #endregion
    }
}
