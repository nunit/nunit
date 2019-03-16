// ***********************************************************************
// Copyright (c) 2017-2018 Charlie Poole, Rob Prouse
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
using System.Threading;
#if !THREAD_ABORT
using System.Threading.Tasks;
#endif
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TimeoutCommand creates a timer in order to cancel
    /// a test if it exceeds a specified time and adjusts
    /// the test result if it did time out.
    /// </summary>
    public class TimeoutCommand : BeforeAndAfterTestCommand
    {
        private readonly int _timeout;
#if THREAD_ABORT
        Timer _commandTimer;
        private bool _commandTimedOut;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="timeout">Timeout value</param>
        public TimeoutCommand(TestCommand innerCommand, int timeout) : base(innerCommand)
        {
            _timeout = timeout;
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "TimeoutCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.ArgumentValid(timeout > 0, "Timeout value must be greater than zero", nameof(timeout));

#if THREAD_ABORT
            BeforeTest = (context) =>
            {
                var testThread = Thread.CurrentThread;
                var nativeThreadId = ThreadUtility.GetCurrentThreadNativeId();

                // Create a timer to cancel the current thread
                _commandTimer = new Timer(
                    (o) =>
                    {
                        _commandTimedOut = true;
                        ThreadUtility.Abort(testThread, nativeThreadId);
                        // No join here, since the thread doesn't really terminate
                    },
                    null,
                    timeout,
                    Timeout.Infinite);
            };

            AfterTest = (context) =>
            {
                _commandTimer.Dispose();

                // If the timer cancelled the current thread, change the result
                if (_commandTimedOut)
                {
                    context.CurrentResult.SetResult(ResultState.Failure, $"Test exceeded Timeout value of {timeout}ms");
                }
            };
#else
            BeforeTest = _ => { };
            AfterTest = _ => { };
#endif
        }

#if !THREAD_ABORT
        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            try
            {
                if (!Task.Run(() => context.CurrentResult = innerCommand.Execute(context)).Wait(_timeout))
                {
                    context.CurrentResult.SetResult(new ResultState(
                        TestStatus.Failed,
                        $"Test exceeded Timeout value {_timeout}ms.",
                        FailureSite.Test));
                }
            }
            catch (Exception exception)
            {
                context.CurrentResult.RecordException(exception, FailureSite.Test);
            }

            return context.CurrentResult;
        }
#endif
    }
}
