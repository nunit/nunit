// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownCommand runs SetUp methods for a suite,
    /// runs the test and then runs TearDown methods.
    /// </summary>
    public class SetUpTearDownCommand : BeforeAndAfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="setUpTearDown">List of setup/teardown items</param>
        public SetUpTearDownCommand(TestCommand innerCommand, SetUpTearDownItem setUpTearDown)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "SetUpTearDownCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.OperationValid(Test.IsFixture, "TestSuite must be a TestFixture or a SetupFixture");
            ArgumentNullException.ThrowIfNull(setUpTearDown);

            BeforeTest = setUpTearDown.RunSetUp;

            AfterTest = setUpTearDown.RunTearDown;
        }
    }
}
