// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// ConstructFixtureCommand constructs the user test object if necessary.
    /// </summary>
    public class ConstructFixtureCommand : BeforeTestCommand
    {
        /// <summary>
        /// Constructs a OneTimeSetUpCommand for a suite
        /// </summary>
        /// <param name="innerCommand">The inner command to which the command applies</param>
        public ConstructFixtureCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            Guard.ArgumentValid(Test is TestSuite, "ConstructFixtureCommand must reference a TestSuite", nameof(innerCommand));

            BeforeTest = (context) =>
            {
                ITypeInfo typeInfo = Test.TypeInfo;

                if (typeInfo != null)
                {
                    // Use preconstructed fixture if available, otherwise construct it
                    if (!typeInfo.IsStaticClass)
                    {
                        context.TestObject = Test.Fixture ?? typeInfo.Construct(((TestSuite)Test).Arguments);
                        if (Test.Fixture == null)
                            Test.Fixture = context.TestObject;
                    }
                }
            };
        }
    }
}

