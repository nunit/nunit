// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;
using static NUnit.Framework.Tests.Constraints.AssignableTestScenarios;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public static class AssignableToConstraintTests
    {
        [TestFixtureSource(typeof(AssignableTestScenarios), nameof(GetAssignableTestScenarios))]
        public class ConstraintValidation<TFrom, TTo> : ConstraintTestBase
        where TFrom : new()
        where TTo : new()
        {
            protected override Constraint TheConstraint { get; } = new AssignableToConstraint(typeof(TTo));

            [SetUp]
            public void SetUp()
            {
                ExpectedDescription = $"assignable to <{typeof(TTo)}>";
                StringRepresentation = $"<assignableto {typeof(TTo)}>";
            }

            private static readonly object[] SuccessData = [new TTo(), new TFrom()];
            private static readonly object[] FailureData = [new TestCaseData(new object(), "<" + typeof(object).FullName + ">")];
        }
    }
}
