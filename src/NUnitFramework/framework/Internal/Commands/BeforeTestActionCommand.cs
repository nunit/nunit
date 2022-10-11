// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionBeforeCommand handles the BeforeTest method of a single
    /// TestActionItem, relying on the item to remember it has been run.
    /// </summary>
    public class BeforeTestActionCommand : BeforeTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeTestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="action">The TestActionItem to run before the inner command.</param>
        public BeforeTestActionCommand(TestCommand innerCommand, TestActionItem action)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "TestActionBeforeCommand may only apply to a TestSuite", nameof(innerCommand));
            Guard.ArgumentNotNull(action, nameof(action));

            BeforeTest = context =>
            {
                action.BeforeTest(Test);
            };
        }
    }
}
