// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// ConstructFixtureCommand constructs the user test object if necessary.
    /// </summary>
    public class FixturePerTestCaseCommand : BeforeTestCommand
    {
        /// <summary>
        /// Handles the construction and disposement of a fixture per test case
        /// </summary>
        /// <param name="innerCommand">The inner command to which the command applies</param>
        public FixturePerTestCaseCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            TestFixture? testFixture = null;

            ITest? currentTest = Test;
            while (currentTest != null && testFixture == null)
            {
                testFixture = currentTest as TestFixture;
                currentTest = currentTest.Parent;
            }

            Guard.ArgumentValid(testFixture != null, "FixturePerTestCaseCommand must reference a TestFixture", nameof(innerCommand));

            ITypeInfo? typeInfo = testFixture.TypeInfo;

            BeforeTest = context =>
            {
                if (typeInfo is {IsStaticClass: false})
                {
                    context.TestObject = typeInfo.Construct(testFixture.Arguments);
                    Test.Fixture = context.TestObject;
                }
            };
        }
    }
}

