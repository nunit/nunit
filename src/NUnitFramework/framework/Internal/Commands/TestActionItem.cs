// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionItem wraps a single execution of an ITestAction.
    /// Its primary purpose is to track whether the BeforeTest
    /// method has been called and suppress calling the
    /// AfterTest method if it has not. This is necessary when
    /// ITestActions are used before and after a CompositeWorkItem,
    /// since the OneTimeSetUpCommand and OneTimeTearDownCommand
    /// are separate command chains. By sharing a TestActionItem
    /// between the setup and teardown chains, the two calls can
    /// be coordinated.
    /// </summary>
    public class TestActionItem
    {
        private readonly ITestAction _action;

        /// <summary>
        /// Construct a TestActionItem
        /// </summary>
        /// <param name="action">The ITestAction to be included</param>
        public TestActionItem(ITestAction action)
        {
            _action = action;
        }

        /// <summary>
        /// Get flag indicating if the BeforeTest entry was already called.
        /// </summary>
        public bool BeforeTestWasRun { get; private set; }

        /// <summary>
        /// Run the BeforeTest method of the action and remember that it has been run.
        /// </summary>
        /// <param name="test">The test to which the action applies</param>
        public void BeforeTest(Interfaces.ITest test)
        {
            var context = TestExecutionContext.CurrentContext;

            Action beforeTestMethod = context.ExecutionHooksEnabled ?
                RunBeforeTestWithHooks : RunBeforeTest;

            beforeTestMethod();

            void RunBeforeTestWithHooks()
            {
                var hookedMethodInfo = new MethodWrapper(_action.GetType(), nameof(ITestAction.BeforeTest));
                try
                {
                    context.ExecutionHooks.OnBeforeTestActionBeforeTest(context, hookedMethodInfo);

                    RunBeforeTest();
                }
                catch (Exception ex)
                {
                    context.ExecutionHooks.OnAfterTestActionBeforeTest(context, hookedMethodInfo, ex);
                    throw;
                }
                context.ExecutionHooks.OnAfterTestActionBeforeTest(context, hookedMethodInfo);
            }

            void RunBeforeTest()
            {
                BeforeTestWasRun = true;
                _action.BeforeTest(test);
            }
        }

        /// <summary>
        /// Run the AfterTest action, but only if the BeforeTest
        /// action was actually run.
        /// </summary>
        /// <param name="test">The test to which the action applies</param>
        public void AfterTest(Interfaces.ITest test)
        {
            var context = TestExecutionContext.CurrentContext;
            Action afterTestMethod = context.ExecutionHooksEnabled ?
                RunAfterTestWithHooks : RunAfterTest;

            afterTestMethod();

            void RunAfterTest()
            {
                if (BeforeTestWasRun)
                    _action.AfterTest(test);
            }

            void RunAfterTestWithHooks()
            {
                var hookedMethodInfo = new MethodWrapper(_action.GetType(), nameof(ITestAction.AfterTest));
                try
                {
                    context.ExecutionHooks.OnBeforeTestActionAfterTest(context, hookedMethodInfo);

                    RunAfterTest();
                }
                catch (Exception ex)
                {
                    context.ExecutionHooks.OnAfterTestActionAfterTest(context, hookedMethodInfo, ex);
                    throw;
                }
                context.ExecutionHooks.OnAfterTestActionAfterTest(context, hookedMethodInfo);
            }
        }
    }
}
