// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;
using static NUnit.Framework.Tests.Constraints.AssignableTestScenarios;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public static class AssignableFromConstraintTests
    {
        [TestFixtureSource(typeof(AssignableTestScenarios), nameof(GetAssignableTestScenarios))]
        public class ConstraintValidation<TFrom, TTo> : ConstraintTestBase
            where TFrom : new()
            where TTo : new()
        {
            protected override Constraint TheConstraint { get; } = new AssignableFromConstraint(typeof(TFrom));

            [SetUp]
            public void SetUp()
            {
                ExpectedDescription = $"assignable from <{typeof(TFrom)}>";
                StringRepresentation = $"<assignablefrom {typeof(TFrom)}>";
            }

            private static readonly object[] SuccessData = [new TTo(), new object()];
            private static readonly object[] FailureData = [new TestCaseData(new B(), "<" + typeof(B).FullName + ">")];
        }
    }
}
