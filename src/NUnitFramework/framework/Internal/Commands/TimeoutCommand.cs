// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if THREAD_ABORT
using System.Threading;
#else
using System;
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
        private Timer? _commandTimer;
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
            BeforeTest = _ =>
            {
                var testThread = Thread.CurrentThread;
                var nativeThreadId = ThreadUtility.GetCurrentThreadNativeId();

                // Create a timer to cancel the current thread
                _commandTimer = new Timer(
                    o =>
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
                _commandTimer?.Dispose();

                // If the timer cancelled the current thread, change the result
                if (_commandTimedOut)
                {
                    var message = $"Test exceeded Timeout value of {timeout}ms";

                    context.CurrentResult.SetResult(
                        ResultState.Failure,
                        message);
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
                var separateContext = new TestExecutionContext(context)
                {
                    CurrentResult = context.CurrentTest.MakeTestResult()
                };
                var testExecution = Task.Run(() => innerCommand.Execute(separateContext));
                var timedOut = Task.WaitAny(new Task[] { testExecution }, _timeout) == -1;

                if (timedOut && !_debugger.IsAttached)
                {
                    context.CurrentResult.SetResult(
                        ResultState.Failure,
                        $"Test exceeded Timeout value of {_timeout}ms");
                }
                else
                {
                    context.CurrentResult.CopyOutputTo(separateContext.CurrentResult);
                    context.CurrentResult = testExecution.GetAwaiter().GetResult();
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
