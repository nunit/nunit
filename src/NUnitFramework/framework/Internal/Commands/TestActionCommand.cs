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
            Guard.ArgumentNotNull(action, nameof(action));

            BeforeTest = context =>
            {
                Action beforeTestMethod = context.ExecutionHooksEnabled ?
                    RunBeforeTestWithHooks : RunBeforeTest;

                beforeTestMethod();

                void RunBeforeTestWithHooks()
                {
                    try
                    {
                        context.ExecutionHooks.OnBeforeTestActionBeforeTest(context);

                        RunBeforeTest();
                    }
                    finally
                    {
                        context.ExecutionHooks.OnAfterTestActionBeforeTest(context);
                    }
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
                    try
                    {
                        context.ExecutionHooks.OnBeforeTestActionAfterTest(context);

                        RunAfterTest();
                    }
                    finally
                    {
                        context.ExecutionHooks.OnAfterTestActionAfterTest(context);
                    }
                }

                void RunAfterTest() => action.AfterTest(Test);
            };
        }
    }
}
