// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownItem holds the setup and teardown methods
    /// for a single level of the inheritance hierarchy.
    /// </summary>
    public class SetUpTearDownItem
    {
        private static readonly IDebugger Debugger = new DebuggerProxy();

        private readonly IMethodValidator? _methodValidator;
        private readonly IList<IMethodInfo> _setUpMethods;
        private readonly IList<IMethodInfo> _tearDownMethods;
        private bool _setUpWasRun;

        /// <summary>
        /// Construct a SetUpTearDownNode
        /// </summary>
        /// <param name="setUpMethods">A list of setup methods for this level</param>
        /// <param name="tearDownMethods">A list teardown methods for this level</param>
        /// <param name="methodValidator">A method validator to validate each method before calling.</param>
        public SetUpTearDownItem(
            IList<IMethodInfo> setUpMethods,
            IList<IMethodInfo> tearDownMethods,
            IMethodValidator? methodValidator = null)
        {
            _setUpMethods = setUpMethods;
            _tearDownMethods = tearDownMethods;
            _methodValidator = methodValidator;
        }

        /// <summary>
        ///  Returns true if this level has any methods at all.
        ///  This flag is used to discard levels that do nothing.
        /// </summary>
        public bool HasMethods => _setUpMethods.Count > 0 || _tearDownMethods.Count > 0;

        /// <summary>
        /// Run SetUp on this level.
        /// </summary>
        /// <param name="context">The execution context to use for running.</param>
        public void RunSetUp(TestExecutionContext context)
        {
            _setUpWasRun = true;

            Action<TestExecutionContext, IMethodInfo> runMethod = context.ExecutionHooksEnabled
                ? RunSetUpMethodWithHooks
                : RunSetUpOrTearDownMethod;

            foreach (IMethodInfo setUpMethod in _setUpMethods)
            {
                if (!TryRunMethod(context, setUpMethod, runMethod, FailureSite.SetUp))
                    return;
            }
        }

        /// <summary>
        /// Run TearDown for this level.
        /// </summary>
        /// <param name="context"></param>
        public void RunTearDown(TestExecutionContext context)
        {
            // As of NUnit 3.0, we will only run teardown at a given
            // inheritance level if we actually ran setup at that level.
            if (_setUpWasRun)
            {
                try
                {
                    Action<TestExecutionContext, IMethodInfo> runMethod = context.ExecutionHooksEnabled
                        ? RunTearDownMethodWithHooks
                        : RunSetUpOrTearDownMethod;

                    // Count of assertion results so far
                    var oldCount = context.CurrentResult.AssertionResultCount;

                    // Even though we are only running one level at a time, we
                    // run the teardowns in reverse order to provide consistency.
                    var index = _tearDownMethods.Count;

                    while (--index >= 0)
                    {
                        if (!TryRunMethod(context, _tearDownMethods[index], runMethod, FailureSite.TearDown))
                            return;
                    }

                    // If there are new assertion results here, they are warnings issued
                    // in teardown. Redo test completion so they are listed properly.
                    if (context.CurrentResult.AssertionResultCount > oldCount)
                        context.CurrentResult.RecordTestCompletion();
                }
                catch (Exception ex)
                {
                    context.CurrentResult.RecordException(ex, FailureSite.TearDown);
                }
            }
        }

        /// <summary>
        /// Runs a setup or teardown method, recording any exception it throws.
        /// </summary>
        /// <returns><see langword="true"/> if the method succeeded, <see langword="false"/> if an exception was recorded.</returns>
        private static bool TryRunMethod(
            TestExecutionContext context,
            IMethodInfo method,
            Action<TestExecutionContext, IMethodInfo> runMethod,
            FailureSite failureSite)
        {
            int cancelAfterTimeout = MethodInfoCache.Get(method).CancelAfterTimeout;
            if (cancelAfterTimeout > 0)
                return TryRunMethodWithCancelAfter(context, method, runMethod, failureSite, cancelAfterTimeout);

            try
            {
                runMethod(context, method);
                return true;
            }
            catch (Exception ex)
            {
                context.CurrentResult.RecordException(ex, failureSite);
                return false;
            }
        }

        /// <summary>
        /// Runs a setup or teardown method that has a <see cref="CancelAfterAttribute"/>.
        /// The method gets its own cancellation token, which is also cancelled when the enclosing token is.
        /// </summary>
        private static bool TryRunMethodWithCancelAfter(
            TestExecutionContext context,
            IMethodInfo method,
            Action<TestExecutionContext, IMethodInfo> runMethod,
            FailureSite failureSite,
            int timeout)
        {
            // Like CancelAfterCommand, only cancel if no debugger is attached when the timer expires.
            using var methodCancellationTokenSource = new CancellationTokenSource();
            using var timedCancellationTokenSource = new CancellationTokenSource(timeout);
            using var registration = timedCancellationTokenSource.Token.Register(() =>
            {
                if (!Debugger.IsAttached)
                    methodCancellationTokenSource.Cancel();
            });

            CancellationToken enclosingCancellationToken = context.CancellationToken;
            using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                enclosingCancellationToken,
                methodCancellationTokenSource.Token);
            context.CancellationToken = combinedCancellationTokenSource.Token;

            try
            {
                runMethod(context, method);
                return true;
            }
            catch (Exception ex)
            {
                // Record it before the token is reset, so it is reported as exceeding CancelAfter.
                if (methodCancellationTokenSource.IsCancellationRequested)
                    RecordCancelAfterException(context, ex, failureSite, timeout);
                else
                    context.CurrentResult.RecordException(ex, failureSite);

                return false;
            }
            finally
            {
                context.CancellationToken = enclosingCancellationToken;
            }
        }

        /// <summary>
        /// Records an exception of a setup or teardown method whose own <see cref="CancelAfterAttribute"/> expired,
        /// reporting its phase and timeout instead of those of an enclosing <see cref="CancelAfterAttribute"/>.
        /// </summary>
        private static void RecordCancelAfterException(TestExecutionContext context, Exception ex, FailureSite failureSite, int timeout)
        {
            string phase = failureSite == FailureSite.SetUp ? "SetUp" : "TearDown";
            if (context.CurrentTest.IsSuite)
                phase = "OneTime" + phase;

            string enclosingPhase = context.CancelAfterPhase;
            int enclosingTimeout = context.TestCaseTimeout;
            context.CancelAfterPhase = phase;
            context.TestCaseTimeout = timeout;

            try
            {
                context.CurrentResult.RecordException(ex, failureSite);
            }
            finally
            {
                context.CancelAfterPhase = enclosingPhase;
                context.TestCaseTimeout = enclosingTimeout;
            }
        }

        private void RunSetUpMethodWithHooks(TestExecutionContext context, IMethodInfo setUpMethod)
        {
            try
            {
                context.ExecutionHooks.OnBeforeEverySetUp(context, setUpMethod);
                RunSetUpOrTearDownMethod(context, setUpMethod);
            }
            catch (Exception ex)
            {
                context.ExecutionHooks.OnAfterEverySetUp(context, setUpMethod, ex);
                throw;
            }
            context.ExecutionHooks.OnAfterEverySetUp(context, setUpMethod);
        }

        private void RunTearDownMethodWithHooks(TestExecutionContext context, IMethodInfo tearDownMethod)
        {
            try
            {
                context.ExecutionHooks.OnBeforeEveryTearDown(context, tearDownMethod);
                RunSetUpOrTearDownMethod(context, tearDownMethod);
            }
            catch (Exception ex)
            {
                context.ExecutionHooks.OnAfterEveryTearDown(context, tearDownMethod, ex);
                throw;
            }
            context.ExecutionHooks.OnAfterEveryTearDown(context, tearDownMethod);
        }

        private void RunSetUpOrTearDownMethod(TestExecutionContext context, IMethodInfo method)
        {
            Guard.ArgumentNotAsyncVoid(method.MethodInfo, nameof(method));
            _methodValidator?.Validate(method.MethodInfo);

            var methodInfo = MethodInfoCache.Get(method);

            bool acceptsCancellationToken = methodInfo.Parameters.LastParameterAcceptsCancellationToken();

            if (methodInfo.IsAsyncOperation)
                AsyncToSyncAdapter.Await(context, () => InvokeMethod(method, context, acceptsCancellationToken));
            else
                InvokeMethod(method, context, acceptsCancellationToken);
        }

        private static object InvokeMethod(IMethodInfo method, TestExecutionContext context, bool acceptsCancellationToken)
        {
            object?[]? arguments = acceptsCancellationToken ? [context.CancellationToken] : null;
            return method.Invoke(method.IsStatic ? null : context.TestObject, arguments)!;
        }
    }
}
