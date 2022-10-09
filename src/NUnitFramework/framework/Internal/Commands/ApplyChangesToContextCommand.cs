// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// ContextSettingsCommand applies specified changes to the
    /// TestExecutionContext prior to running a test. No special
    /// action is needed after the test runs, since the prior
    /// context will be restored automatically.
    /// </summary>
    class ApplyChangesToContextCommand : BeforeTestCommand
    {
        public ApplyChangesToContextCommand(TestCommand innerCommand, IApplyToContext change)
            : base(innerCommand)
        {
            BeforeTest = (context) =>
            {
                change.ApplyToContext(context);
            };
        }
    }
}
