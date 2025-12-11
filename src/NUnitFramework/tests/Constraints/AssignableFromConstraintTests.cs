// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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

            private static readonly object[] SuccessData = [new TTo(), new TFrom(), new object()];
            private static readonly object[] FailureData = [new TestCaseData(new B(), "<" + typeof(B).FullName + ">")];
        }

        [TestCaseSource(nameof(SuccessCases))]
        public static void CanAssignFrom(object? actual, Type type)
        {
            Assert.That(actual, Is.AssignableFrom(type));
        }

        private static readonly TestCaseData[] SuccessCases =
        [
            new TestCaseData(new object(), null),
            new TestCaseData(42.0, typeof(int)),
            new TestCaseData(42.0, typeof(double)),
            new TestCaseData(42.0, typeof(float)),
            new TestCaseData(new D1(), typeof(D2)),
            new TestCaseData(new D1(), typeof(D3)),
        ];

        [TestCaseSource(nameof(FailureCases))]
        public static void CanNotAssignFrom(object? actual, Type type)
        {
            Assert.That(actual, Is.Not.AssignableFrom(type));
        }

        private static readonly TestCaseData[] FailureCases =
        [
            new TestCaseData(42.0f, typeof(double)),
            new TestCaseData(new D2(), typeof(D1)),
            new TestCaseData(new D3(), typeof(D1)),
        ];
    }
}
