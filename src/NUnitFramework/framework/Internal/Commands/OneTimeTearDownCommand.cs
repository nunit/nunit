// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeTearDownCommand performs any teardown actions
    /// specified for a suite and calls Dispose on the user
    /// test object, if any.
    /// </summary>
    public class OneTimeTearDownCommand : AfterTestCommand
    {
        /// <summary>
        /// Construct a OneTimeTearDownCommand
        /// </summary>
        /// <param name="innerCommand">The command wrapped by this command</param>
        /// <param name="setUpTearDownItem">A SetUpTearDownList for use by the command</param>
        public OneTimeTearDownCommand(TestCommand innerCommand, SetUpTearDownItem setUpTearDownItem)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "OneTimeTearDownCommand may only apply to a TestSuite", nameof(innerCommand));
            ArgumentNullException.ThrowIfNull(setUpTearDownItem);

            AfterTest = setUpTearDownItem.RunTearDown;
        }
    }
}
