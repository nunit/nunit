// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
    /// RepeatAttribute may be applied to test case in order
    /// to run it multiple times.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RetryAttribute : PropertyAttribute, IWrapSetUpTearDown
    {
        private int _count;

        /// <summary>
        /// Construct a RepeatAttribute
        /// </summary>
        /// <param name="count">The number of times to run the test</param>
        public RetryAttribute(int count) : base(count)
        {
            _count = count;
        }

        #region IWrapSetUpTearDown Members

        /// <summary>
        /// Wrap a command and return the result.
        /// </summary>
        /// <param name="command">The command to be wrapped</param>
        /// <returns>The wrapped command</returns>
        public TestCommand Wrap(TestCommand command)
        {
            return new RetryCommand(command, _count);
        }

        #endregion

        #region Nested RetryCommand Class

        /// <summary>
        /// The test command for the RetryAttribute
        /// </summary>
        public class RetryCommand : DelegatingTestCommand
        {
            private int _retryCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="RetryCommand"/> class.
            /// </summary>
            /// <param name="innerCommand">The inner command.</param>
            /// <param name="retryCount">The number of repetitions</param>
            public RetryCommand(TestCommand innerCommand, int retryCount)
                : base(innerCommand)
            {
                _retryCount = retryCount;
            }

            /// <summary>
            /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
            /// </summary>
            /// <param name="context">The context in which the test should run.</param>
            /// <returns>A TestResult</returns>
            public override TestResult Execute(TestExecutionContext context)
            {
                int count = _retryCount;

                while (count-- > 0)
                {
                    context.CurrentResult = innerCommand.Execute(context);

                    if (context.CurrentResult.ResultState != ResultState.Failure)
                    {
                        break;
                    }
                }

                return context.CurrentResult;
            }
        }

        #endregion
    }
}
