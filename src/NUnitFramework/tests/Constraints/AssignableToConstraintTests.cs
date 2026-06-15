// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
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
            public ConstraintValidation(Constraint constraint) : base()
            {
                TheConstraint = constraint;
            }

            protected override Constraint TheConstraint { get; }

            [SetUp]
            public void SetUp()
            {
                ExpectedDescription = $"assignable to <{typeof(TTo)}>";
                StringRepresentation = $"<assignableto {typeof(TTo)}>";
            }
#pragma warning disable IDE0052 // Remove unread private members
            private static readonly object[] SuccessData = [new TTo(), new TFrom()];
            private static readonly object[] FailureData = [new TestCaseData(new object(), "<" + typeof(object).FullName + ">")];
#pragma warning restore IDE0052 // Remove unread private members
        }

        [TestCaseSource(nameof(SuccessCases))]
        public static void CanAssignTo(object? actual, Type type)
        {
            Assert.That(actual, Is.AssignableTo(type));
        }

        private static readonly TestCaseData[] SuccessCases =
        [
            new(null, typeof(object)),
            new(null, typeof(int?)),
            new(42, typeof(int?)),
            new(42, typeof(double)),
            new(42, typeof(double?)),
            new(42.0f, typeof(double)),
            new(new D2(), typeof(D1)),
            new(new D3(), typeof(D1)),
        ];

        [TestCaseSource(nameof(FailureCases))]
        public static void CanNotAssignTo(object? actual, Type type)
        {
            Assert.That(actual, Is.Not.AssignableTo(type));
        }

        [Test]
        public static void GenericConstraintUsesRuntimeTypeForActualValue()
        {
            D1 actual = new D2();

            Assert.That(actual, Is.AssignableTo<D2>());
        }

        [Test]
        public static void GenericConstraintReturnsFalseForNullActual()
        {
            object? actual = null;

            Assert.That(actual, Is.Not.AssignableTo<D1>());
        }

        private static readonly TestCaseData[] FailureCases =
        [
            new(null, typeof(int)),
            new(42.0, typeof(float)),
            new(new D1(), typeof(D2)),
            new(new D1(), typeof(D3)),
        ];

        private static IEnumerable<TestFixtureData> GetAssignableTestScenarios()
        {
            foreach (var assignment in AssignableTestScenarios.GetAssignableTestScenarios())
            {
                var from = assignment.from;
                var typeArgs = new Type[] { assignment.from, assignment.to };

                yield return new TestFixtureData(new AssignableToConstraint(from))
                {
                    TypeArgs = typeArgs
                }.SetArgDisplayNames("non-generic");

                var genericType = typeof(AssignableToConstraint<>).MakeGenericType(from);
                var genericConstraint = (Constraint)Activator.CreateInstance(genericType)!;
                yield return new TestFixtureData(genericConstraint)
                {
                    TypeArgs = typeArgs
                }.SetArgDisplayNames("generic");
            }
        }
    }
}
