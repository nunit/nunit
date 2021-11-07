// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
#if !THREAD_ABORT
using System.Threading.Tasks;
#endif
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// <see cref="TimeoutCommand"/> creates a timer in order to cancel
    /// a test if it exceeds a specified time and adjusts
    /// the test result if it did time out.
    /// </summary>
    public class TimeoutCommand : BeforeAndAfterTestCommand
    {
        private readonly int _timeout;
        private readonly IDebugger _debugger;
#if THREAD_ABORT
        Timer _commandTimer;
        private bool _commandTimedOut;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance</param>
        internal TimeoutCommand(TestCommand innerCommand, int timeout, IDebugger debugger) : base(innerCommand)
        {
            _timeout = timeout;
            _debugger = debugger;

            Guard.ArgumentValid(innerCommand.Test is TestMethod, "TimeoutCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.ArgumentValid(timeout > 0, "Timeout value must be greater than zero", nameof(timeout));
            Guard.ArgumentNotNull(debugger, nameof(debugger));

#if THREAD_ABORT
            BeforeTest = (context) =>
            {
                var testThread = Thread.CurrentThread;
                var nativeThreadId = ThreadUtility.GetCurrentThreadNativeId();

                // Create a timer to cancel the current thread
                _commandTimer = new Timer(
                    (o) =>
                    {
                        if (_debugger.IsAttached)
                        {
                            return;
                        }

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
        public override Task<TestResult> Execute(TestExecutionContext context)
        {
            try
            {
                var testExecution = RunTestOnSeparateThread(context);
                if (Task.WaitAny(new Task[] { testExecution }, _timeout) != -1
                    || _debugger.IsAttached)
                {
                    context.CurrentResult = testExecution.GetAwaiter().GetResult();
                }
                else
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

            return Task.FromResult(context.CurrentResult);
        }

        private Task<TestResult> RunTestOnSeparateThread(TestExecutionContext context)
        {
            return Task.Run(() => innerCommand.Execute(context));
        }
#endif
    }
}
