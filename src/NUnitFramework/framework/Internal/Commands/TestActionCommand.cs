// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionCommand handles a single ITestAction applied
    /// to a test. It runs the BeforeTest method, then runs the
    /// test and finally runs the AfterTest method.
    /// </summary>
    public class TestActionCommand : BeforeAndAfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="action">The TestAction with which to wrap the inner command.</param>
        public TestActionCommand(TestCommand innerCommand, ITestAction action)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "TestActionCommand may only apply to a TestMethod", nameof(innerCommand));
            ArgumentNullException.ThrowIfNull(action);

            BeforeTest = context =>
            {
                Action beforeTestMethod = context.ExecutionHooksEnabled ?
                    RunBeforeTestWithHooks : RunBeforeTest;

                beforeTestMethod();

                void RunBeforeTestWithHooks()
                {
                    var hookedMethodInfo = new MethodWrapper(action.GetType(), nameof(ITestAction.BeforeTest));
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

                void RunBeforeTest() => action.BeforeTest(Test);
            };

            AfterTest = context =>
            {
                Action afterTestMethod = context.ExecutionHooksEnabled ?
                    RunAfterTestWithHooks : RunAfterTest;

                afterTestMethod();

                void RunAfterTestWithHooks()
                {
                    var hookedMethodInfo = new MethodWrapper(action.GetType(), nameof(ITestAction.AfterTest));
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

                void RunAfterTest() => action.AfterTest(Test);
            };
        }
    }
}
