// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownItem holds the setup and teardown methods
    /// for a single level of the inheritance hierarchy.
    /// </summary>
    public class SetUpTearDownItem
    {
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

            foreach (IMethodInfo setUpMethod in _setUpMethods)
            {
                RaiseOneTimeSetUpStarted(context);
                try
                {
                    RunSetUpOrTearDownMethod(context, setUpMethod);
                }
                finally
                {
                    RaiseOneTimeSetUpFinished(context);
                }
            }
        }

        private void RaiseOneTimeSetUpStarted(TestExecutionContext context)
        {
            if (context.CurrentTest is not null && context.CurrentTest.IsSuite)
                context.ListenerExt?.OneTimeSetUpStarted(context.CurrentTest);
        }

        private void RaiseOneTimeSetUpFinished(TestExecutionContext context)
        {
            if (context.CurrentTest is not null && context.CurrentTest.IsSuite)
                context.ListenerExt?.OneTimeSetUpFinished(context.CurrentTest);
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
                    // Count of assertion results so far
                    var oldCount = context.CurrentResult.AssertionResults.Count;

                    // Even though we are only running one level at a time, we
                    // run the teardowns in reverse order to provide consistency.
                    var index = _tearDownMethods.Count;
                    while (--index >= 0)
                    {
                        RaiseOneTimeTearDownStarted(context);
                        try
                        {
                            RunSetUpOrTearDownMethod(context, _tearDownMethods[index]);
                        }
                        finally
                        {
                            RaiseOneTimeTearDownFinished(context);
                        }
                    }

                    // If there are new assertion results here, they are warnings issued
                    // in teardown. Redo test completion so they are listed properly.
                    if (context.CurrentResult.AssertionResults.Count > oldCount)
                        context.CurrentResult.RecordTestCompletion();
                }
                catch (Exception ex)
                {
                    context.CurrentResult.RecordTearDownException(ex);
                }
            }
        }

        private void RaiseOneTimeTearDownStarted(TestExecutionContext context)
        {
            if (context.CurrentTest is not null && context.CurrentTest.IsSuite)
                context.ListenerExt?.OneTimeTearDownStarted(context.CurrentTest);
        }

        private void RaiseOneTimeTearDownFinished(TestExecutionContext context)
        {
            if (context.CurrentTest is not null && context.CurrentTest.IsSuite)
                context.ListenerExt?.OneTimeTearDownFinished(context.CurrentTest);
        }

        private void RunSetUpOrTearDownMethod(TestExecutionContext context, IMethodInfo method)
        {
            Guard.ArgumentNotAsyncVoid(method.MethodInfo, nameof(method));
            _methodValidator?.Validate(method.MethodInfo);

            var methodInfo = MethodInfoCache.Get(method);

            if (methodInfo.IsAsyncOperation)
                AsyncToSyncAdapter.Await(() => InvokeMethod(method, context));
            else
                InvokeMethod(method, context);
        }

        private static object InvokeMethod(IMethodInfo method, TestExecutionContext context)
        {
            return method.Invoke(method.IsStatic ? null : context.TestObject, null)!;
        }
    }
}
