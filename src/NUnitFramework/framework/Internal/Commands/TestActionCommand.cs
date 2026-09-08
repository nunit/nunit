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

            BeforeTest = context => TestActionHookRunner.Run(
                context,
                action.GetType(),
                nameof(ITestAction.BeforeTest),
                (ctx, m) => ctx.ExecutionHooks.OnBeforeTestActionBeforeTest(ctx, m),
                (ctx, m, ex) => ctx.ExecutionHooks.OnAfterTestActionBeforeTest(ctx, m, ex),
                () => action.BeforeTest(Test));

            AfterTest = context => TestActionHookRunner.Run(
                context,
                action.GetType(),
                nameof(ITestAction.AfterTest),
                (ctx, m) => ctx.ExecutionHooks.OnBeforeTestActionAfterTest(ctx, m),
                (ctx, m, ex) => ctx.ExecutionHooks.OnAfterTestActionAfterTest(ctx, m, ex),
                () => action.AfterTest(Test));
        }
    }
}
