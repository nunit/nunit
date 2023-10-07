// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// ContextSettingsCommand applies specified changes to the
    /// TestExecutionContext prior to running a test. No special
    /// action is needed after the test runs, since the prior
    /// context will be restored automatically.
    /// </summary>
    internal class ApplyChangesToContextCommand : BeforeTestCommand
    {
        public ApplyChangesToContextCommand(TestCommand innerCommand, IApplyToContext change)
            : base(innerCommand)
        {
            BeforeTest = change.ApplyToContext;
        }
    }
}
