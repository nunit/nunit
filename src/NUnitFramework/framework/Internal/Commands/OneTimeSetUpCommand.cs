// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeSetUpCommand runs any one-time setup methods for a suite,
    /// constructing the user test object if necessary.
    /// </summary>
    public class OneTimeSetUpCommand : BeforeTestCommand
    {
        /// <summary>
        /// Constructs a OneTimeSetUpCommand for a suite
        /// </summary>
        /// <param name="innerCommand">The inner command to which the command applies</param>
        /// <param name="setUpTearDown">A SetUpTearDownList for use by the command</param>
        public OneTimeSetUpCommand(TestCommand innerCommand, SetUpTearDownItem setUpTearDown)
            : base(innerCommand)
        {
            Guard.ArgumentValid(Test is TestSuite && Test.TypeInfo is not null,
                "OneTimeSetUpCommand must reference a TestFixture or SetUpFixture", nameof(innerCommand));

            BeforeTest = context =>
            {
                setUpTearDown.RunSetUp(context);
            };
        }
    }
}

