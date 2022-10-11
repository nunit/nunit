// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionAfterCommand handles the AfterTest method of a single 
    /// TestActionItem, provided the items BeforeTest has been run.
    /// </summary>
    public class AfterTestActionCommand : AfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterTestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="action">The TestActionItem to run before the inner command.</param>
        public AfterTestActionCommand(TestCommand innerCommand, TestActionItem action)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "BeforeTestActionCommand may only apply to a TestSuite", nameof(innerCommand));
            Guard.ArgumentNotNull(action, nameof(action));

            AfterTest = context =>
            {
                if (action.BeforeTestWasRun)
                {
                    var oldCount = context.CurrentResult.AssertionResults.Count;

                    action.AfterTest(Test);

                    // If there are new assertion results here, they are warnings issued
                    // in teardown. Redo test completion so they are listed properly.
                    if (context.CurrentResult.AssertionResults.Count > oldCount)
                        context.CurrentResult.RecordTestCompletion();
                }
            };
        }
    }
}
