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
        private readonly int? _maxRetries;

        /// <summary>
        /// Construct a <see cref="RetryUntilFailureAttribute" />
        /// </summary>
        /// <param name="maxRetries">The maximum number of times the test should incase it doesn't fail</param>
        public RetryUntilFailureAttribute(int maxRetries)
        {
            _maxRetries = maxRetries;
        }

        /// <summary>
        /// Construct a <see cref="RetryUntilFailureAttribute" />
        /// </summary>
        public RetryUntilFailureAttribute()
        {
            _maxRetries = null;
        }

        #region IRepeatTest Members

        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        public TestCommand Wrap(TestCommand command)
        {
            return new RetryUntilFailure(command, _maxRetries);
        }

        #endregion

        #region Nested RetryUntilFailure Class

        /// <summary>
        /// The test command for the <see cref="RetryUntilFailureAttribute"/>
        /// </summary>
        public class RetryUntilFailure : DelegatingTestCommand
        {
            private int? _maxRetries;

            /// <summary>
            /// Initializes a new instance of the <see cref="RetryUntilFailure"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="maxRetries">The maximum number of times the test should incase it doesn't fail.</param>
            public RetryUntilFailure(TestCommand innerCommand, int? maxRetries = null) : base(innerCommand)
            {
                _maxRetries = maxRetries;
            }

            /// <summary>
            /// Decide if test should run forever or limited amount of run.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            public override TestResult Execute(TestExecutionContext context)
            {
                return _maxRetries == null ? RunForever(context) : RunLimited(context);
            }


            /// <summary>
            /// Runs the test, saving failure in the supplied TestExecutionContext.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            private TestResult RunForever(TestExecutionContext context)
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

            /// <summary>
            /// Run test until it fails or done limited amount of runs specified.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            private TestResult RunLimited(TestExecutionContext context)
            {
                while (_maxRetries-- != 0)
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

                return context.CurrentResult;
            }
        }

        #endregion
    }
}
